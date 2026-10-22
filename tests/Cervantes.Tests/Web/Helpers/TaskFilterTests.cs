using Cervantes.Web.Helpers;
using Task = Cervantes.CORE.Entities.Task;

namespace Cervantes.Tests.Web.Helpers;

public class TaskFilterTests
{
    private static readonly Guid Web = Guid.NewGuid();
    private static readonly Guid Mobile = Guid.NewGuid();

    private static readonly List<Task> All = new()
    {
        new Task { Name = "a-web", AsignedUserId = "a", ProjectId = Web },
        new Task { Name = "a-mobile", AsignedUserId = "a", ProjectId = Mobile },
        new Task { Name = "b-web", AsignedUserId = "b", ProjectId = Web },
        new Task { Name = "none", AsignedUserId = null, ProjectId = null },
    };

    [Fact]
    public void Apply_FiltersByUserAndProject()
    {
        var result = TaskFilter.Apply(All, "a", Web);

        Assert.Equal(new[] { "a-web" }, result.Select(t => t.Name));
    }

    [Fact]
    public void Apply_EmptyFiltersKeepAll()
    {
        Assert.Equal(4, TaskFilter.Apply(All, "", Guid.Empty).Count);
        Assert.Equal(4, TaskFilter.Apply(All, null, Guid.Empty).Count);
    }

    [Fact]
    public void Apply_IsNotCumulative()
    {
        TaskFilter.Apply(All, "a", Guid.Empty);
        var result = TaskFilter.Apply(All, "b", Guid.Empty);

        Assert.Equal(new[] { "b-web" }, result.Select(t => t.Name));
    }
}
