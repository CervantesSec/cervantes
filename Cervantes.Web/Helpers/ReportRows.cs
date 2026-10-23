using Cervantes.CORE.Helpers;

namespace Cervantes.Web.Helpers;

/// <summary>Rows of the collections that report templates read with Scriban.</summary>
public static class ReportRows
{
    /// <summary>A task as the "Tasks" collection exposes it; an unassigned task has an empty TaskAssignedTo.</summary>
    public static Dictionary<string, string> Task(CORE.Entities.Task task)
    {
        return new Dictionary<string, string>
        {
            {"TaskName", task.Name},
            {"TaskDescription", task.Description},
            {"TaskStatus", task.Status.ToString()},
            {"TaskStartDate", task.StartDate.ToLocalDate().ToString("dd/MM/yyyy")},
            {"TaskEndDate", task.EndDate.ToLocalDate().ToString("dd/MM/yyyy")},
            {"TaskAssignedTo", task.AsignedUser?.FullName ?? ""},
            {"TaskCreatedBy", task.CreatedUser?.FullName ?? ""},
        };
    }
}
