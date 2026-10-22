using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;

namespace Cervantes.Web.Helpers;

public sealed record ChecklistTemplateSyncResult(
    IReadOnlyList<ChecklistCategory> AddedCategories,
    IReadOnlyList<ChecklistItem> AddedItems,
    IReadOnlyList<ChecklistCategory> RemovedCategories,
    IReadOnlyList<ChecklistItem> RemovedItems,
    IReadOnlyList<Guid> BlockedItemIds);

public static class ChecklistTemplateSync
{
    /// <summary>
    /// Applies an edited custom checklist template to the stored one in place. Categories and items whose id the
    /// model carries are updated, so the checklist executions that point to them survive; new ones get fresh ids;
    /// stored ones missing from the model are removed. Removing an item that has executions is refused: the result
    /// lists those item ids and nothing is changed, so a refused update leaves the tracked entities untouched.
    /// </summary>
    public static ChecklistTemplateSyncResult Apply(ChecklistTemplate template, ChecklistTemplateCreateViewModel model,
        ISet<Guid> itemIdsWithExecutions, Func<string?, string?> sanitize)
    {
        var categories = template.Categories ??= new List<ChecklistCategory>();
        var modelCategoryIds = model.Categories.Where(c => c.Id.HasValue).Select(c => c.Id!.Value).ToHashSet();
        var modelItemIds = model.Categories.SelectMany(c => c.Items).Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToHashSet();

        var removedCategories = categories.Where(c => !modelCategoryIds.Contains(c.Id)).ToList();
        var removedItems = categories.Except(removedCategories)
            .SelectMany(c => c.Items).Where(i => !modelItemIds.Contains(i.Id)).ToList();
        var blocked = removedCategories.SelectMany(c => c.Items).Concat(removedItems)
            .Where(i => itemIdsWithExecutions.Contains(i.Id)).Select(i => i.Id).ToList();
        if (blocked.Count > 0)
        {
            return new ChecklistTemplateSyncResult(Array.Empty<ChecklistCategory>(), Array.Empty<ChecklistItem>(),
                Array.Empty<ChecklistCategory>(), Array.Empty<ChecklistItem>(), blocked);
        }

        var addedCategories = new List<ChecklistCategory>();
        var addedItems = new List<ChecklistItem>();
        foreach (var categoryModel in model.Categories)
        {
            var category = categoryModel.Id is { } id ? categories.FirstOrDefault(c => c.Id == id) : null;
            if (category == null)
            {
                category = new ChecklistCategory { Id = Guid.NewGuid(), ChecklistTemplateId = template.Id, Items = new List<ChecklistItem>() };
                addedCategories.Add(category);
            }
            category.Name = sanitize(categoryModel.Name) ?? "";
            category.Description = sanitize(categoryModel.Description);
            category.Order = categoryModel.Order;

            foreach (var itemModel in categoryModel.Items)
            {
                var item = itemModel.Id is { } itemId ? category.Items.FirstOrDefault(i => i.Id == itemId) : null;
                if (item == null)
                {
                    item = new ChecklistItem { Id = Guid.NewGuid(), ChecklistCategoryId = category.Id };
                    category.Items.Add(item);
                    if (!addedCategories.Contains(category))
                    {
                        addedItems.Add(item);
                    }
                }
                item.Code = sanitize(itemModel.Code) ?? "";
                item.Name = sanitize(itemModel.Name) ?? "";
                item.Description = sanitize(itemModel.Description);
                item.Objectives = sanitize(itemModel.Objectives);
                item.TestProcedure = sanitize(itemModel.TestProcedure);
                item.PassCriteria = sanitize(itemModel.PassCriteria);
                item.Order = itemModel.Order;
                item.IsRequired = itemModel.IsRequired;
                item.Severity = itemModel.Severity;
                item.References = sanitize(itemModel.References);
            }
        }

        foreach (var item in removedItems)
        {
            categories.First(c => c.Id == item.ChecklistCategoryId).Items.Remove(item);
        }
        foreach (var category in removedCategories)
        {
            categories.Remove(category);
        }
        foreach (var category in addedCategories)
        {
            categories.Add(category);
        }

        return new ChecklistTemplateSyncResult(addedCategories, addedItems, removedCategories, removedItems, Array.Empty<Guid>());
    }
}
