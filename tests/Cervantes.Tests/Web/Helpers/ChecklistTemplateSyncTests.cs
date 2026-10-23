using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class ChecklistTemplateSyncTests
{
    private static readonly Func<string?, string?> Same = s => s;

    private static (ChecklistTemplate Template, ChecklistCategory Category, ChecklistItem Kept, ChecklistItem Other) Existing()
    {
        var template = new ChecklistTemplate { Id = Guid.NewGuid(), Name = "t" };
        var category = new ChecklistCategory { Id = Guid.NewGuid(), Name = "Auth", ChecklistTemplateId = template.Id };
        var kept = new ChecklistItem { Id = Guid.NewGuid(), Code = "A-1", Name = "Login", ChecklistCategoryId = category.Id };
        var other = new ChecklistItem { Id = Guid.NewGuid(), Code = "A-2", Name = "Logout", ChecklistCategoryId = category.Id };
        category.Items = new List<ChecklistItem> { kept, other };
        template.Categories = new List<ChecklistCategory> { category };
        return (template, category, kept, other);
    }

    private static ChecklistTemplateCreateViewModel Model(params ChecklistCategoryCreateViewModel[] categories) =>
        new() { Name = "t2", Categories = categories.ToList() };

    private static ChecklistItemCreateViewModel Item(Guid? id, string code, string name) => new() { Id = id, Code = code, Name = name };

    [Fact]
    public void Sync_KeepsExistingItemIds()
    {
        var (template, category, kept, other) = Existing();
        var model = Model(new ChecklistCategoryCreateViewModel
        {
            Id = category.Id, Name = "Authentication",
            Items = { Item(kept.Id, "A-1", "Login form"), Item(other.Id, "A-2", "Logout") }
        });

        var result = ChecklistTemplateSync.Apply(template, model, new HashSet<Guid>(), Same);

        Assert.Empty(result.BlockedItemIds);
        Assert.Same(kept, category.Items.Single(i => i.Code == "A-1"));
        Assert.Equal("Login form", kept.Name);
        Assert.Equal("Authentication", category.Name);
        Assert.Empty(result.AddedCategories);
        Assert.Empty(result.AddedItems);
    }

    [Fact]
    public void Sync_AddsNewItems()
    {
        var (template, category, kept, other) = Existing();
        var model = Model(
            new ChecklistCategoryCreateViewModel
            {
                Id = category.Id, Name = "Auth",
                Items = { Item(kept.Id, "A-1", "Login"), Item(other.Id, "A-2", "Logout"), Item(null, "A-3", "MFA") }
            },
            new ChecklistCategoryCreateViewModel { Name = "Session", Items = { Item(null, "S-1", "Timeout") } });

        var result = ChecklistTemplateSync.Apply(template, model, new HashSet<Guid>(), Same);

        var added = Assert.Single(result.AddedItems);
        Assert.Equal("A-3", added.Code);
        Assert.Equal(category.Id, added.ChecklistCategoryId);
        Assert.NotEqual(Guid.Empty, added.Id);
        var newCategory = Assert.Single(result.AddedCategories);
        Assert.Equal(template.Id, newCategory.ChecklistTemplateId);
        Assert.Equal("S-1", Assert.Single(newCategory.Items).Code);
    }

    [Fact]
    public void Sync_RemovesUnusedItems()
    {
        var (template, category, kept, other) = Existing();
        var model = Model(new ChecklistCategoryCreateViewModel { Id = category.Id, Name = "Auth", Items = { Item(kept.Id, "A-1", "Login") } });

        var result = ChecklistTemplateSync.Apply(template, model, new HashSet<Guid>(), Same);

        Assert.Same(other, Assert.Single(result.RemovedItems));
        Assert.DoesNotContain(other, category.Items);
    }

    [Fact]
    public void Sync_RefusesRemovingItemsWithExecutions()
    {
        var (template, category, kept, other) = Existing();
        var model = Model(new ChecklistCategoryCreateViewModel { Id = category.Id, Name = "Renamed", Items = { Item(kept.Id, "A-1", "Login") } });

        var result = ChecklistTemplateSync.Apply(template, model, new HashSet<Guid> { other.Id }, Same);

        Assert.Equal(new[] { other.Id }, result.BlockedItemIds);
        Assert.Empty(result.RemovedItems);
        Assert.Contains(other, category.Items);
        Assert.Equal("Auth", category.Name); // nothing is touched when the update is refused
    }

    [Fact]
    public void Sync_RefusesRemovingACategoryWithExecutions()
    {
        var (template, _, kept, _) = Existing();

        var result = ChecklistTemplateSync.Apply(template, Model(), new HashSet<Guid> { kept.Id }, Same);

        Assert.Equal(new[] { kept.Id }, result.BlockedItemIds);
        Assert.Empty(result.RemovedCategories);
    }

    private static ChecklistCategory AddSession(ChecklistTemplate template)
    {
        var session = new ChecklistCategory { Id = Guid.NewGuid(), Name = "Session", ChecklistTemplateId = template.Id, Items = new List<ChecklistItem>() };
        template.Categories.Add(session);
        return session;
    }

    // The item keeps its id, so the executions that point to it survive the move.
    [Fact]
    public void Apply_MovesItemSentUnderAnotherCategory()
    {
        var (template, category, kept, other) = Existing();
        var session = AddSession(template);
        var model = Model(
            new ChecklistCategoryCreateViewModel { Id = category.Id, Name = "Auth", Items = { Item(other.Id, "A-2", "Logout") } },
            new ChecklistCategoryCreateViewModel { Id = session.Id, Name = "Session", Items = { Item(kept.Id, "S-1", "Login") } });

        var result = ChecklistTemplateSync.Apply(template, model, new HashSet<Guid> { kept.Id }, Same);

        Assert.Empty(result.BlockedItemIds);
        Assert.Same(kept, Assert.Single(session.Items));
        Assert.Equal(session.Id, kept.ChecklistCategoryId);
        Assert.Equal("S-1", kept.Code);
        Assert.DoesNotContain(kept, category.Items);
        Assert.Empty(result.AddedItems);
        Assert.Empty(result.RemovedItems);
        Assert.Equal(2, template.Categories.SelectMany(c => c.Items).Count());
    }

    [Fact]
    public void Apply_MovingItemOutOfRemovedCategoryIsNotBlocked()
    {
        var (template, category, kept, other) = Existing();
        var session = AddSession(template);
        var model = Model(new ChecklistCategoryCreateViewModel { Id = session.Id, Name = "Session", Items = { Item(kept.Id, "A-1", "Login") } });

        var result = ChecklistTemplateSync.Apply(template, model, new HashSet<Guid> { kept.Id }, Same);

        Assert.Empty(result.BlockedItemIds);
        Assert.Same(kept, Assert.Single(session.Items));
        Assert.Equal(session.Id, kept.ChecklistCategoryId);
        Assert.Same(category, Assert.Single(result.RemovedCategories));
        Assert.DoesNotContain(kept, category.Items);
    }
}
