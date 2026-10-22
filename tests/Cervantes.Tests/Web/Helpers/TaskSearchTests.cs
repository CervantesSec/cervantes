using Cervantes.CORE.Entities;
using Cervantes.CORE.Helpers;
using Cervantes.Web.Helpers;
using Task = Cervantes.CORE.Entities.Task;
using TaskStatus = Cervantes.CORE.Entities.TaskStatus;

namespace Cervantes.Tests.Web.Helpers;

public class TaskSearchTests
{
    private static readonly DateTime Start = new(2026, 9, 30, 22, 0, 0, DateTimeKind.Utc);

    private static Task Unassigned() => new()
    {
        Name = "Recon", Status = TaskStatus.ToDo, StartDate = Start, EndDate = Start.AddDays(4), AsignedUser = null
    };

    [Fact]
    public void Matches_UnassignedTaskDoesNotThrow()
    {
        var task = Unassigned();
        var day = Start.ToLocalDate().ToString("d");

        Assert.False(TaskSearch.Matches(task, "nobody"));
        Assert.True(TaskSearch.Matches(task, day));
    }

    [Fact]
    public void Matches_ByAssigneeName()
    {
        var task = Unassigned();
        task.AsignedUser = new ApplicationUser { FullName = "Pablo Pentester" };

        Assert.True(TaskSearch.Matches(task, "pablo"));
    }

    [Fact]
    public void Matches_BlankSearch()
    {
        Assert.True(TaskSearch.Matches(Unassigned(), null));
        Assert.True(TaskSearch.Matches(Unassigned(), "  "));
    }
}
