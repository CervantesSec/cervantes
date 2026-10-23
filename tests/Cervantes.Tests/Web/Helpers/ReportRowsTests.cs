using Cervantes.CORE.Entities;
using Cervantes.Web.Helpers;
using Task = Cervantes.CORE.Entities.Task;
using TaskStatus = Cervantes.CORE.Entities.TaskStatus;

namespace Cervantes.Tests.Web.Helpers;

public class ReportRowsTests
{
    private static Task NewTask(ApplicationUser? assignee) => new()
    {
        Name = "Recon and mapping",
        Description = "Map the attack surface",
        Status = TaskStatus.Done,
        StartDate = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc),
        EndDate = new DateTime(2026, 10, 5, 0, 0, 0, DateTimeKind.Utc),
        AsignedUser = assignee,
        CreatedUser = new ApplicationUser { FullName = "Marta Manager" }
    };

    // TaskAssignedTo was only ever set for unassigned tasks (to ""), never to the assignee's name.
    [Fact]
    public void Task_CarriesTheAssigneeName()
    {
        var row = ReportRows.Task(NewTask(new ApplicationUser { FullName = "Pablo Pentester" }));

        Assert.Equal("Pablo Pentester", row["TaskAssignedTo"]);
        Assert.Equal("Recon and mapping", row["TaskName"]);
        Assert.Equal("Done", row["TaskStatus"]);
        Assert.Equal("Marta Manager", row["TaskCreatedBy"]);
    }

    [Fact]
    public void Task_UnassignedIsEmpty()
    {
        Assert.Equal("", ReportRows.Task(NewTask(null))["TaskAssignedTo"]);
    }
}
