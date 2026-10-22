namespace Cervantes.Web.Helpers;

public static class TaskSearch
{
    /// <summary>
    /// The task grids' quick search: name, assignee, status and the start and end days as the grid shows them.
    /// An unassigned task has no assignee to match, and a blank search matches every task.
    /// </summary>
    public static bool Matches(CORE.Entities.Task task, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return true;
        if (task.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
            return true;
        if (task.AsignedUser?.FullName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
            return true;
        if (task.Status.ToString().Contains(search, StringComparison.OrdinalIgnoreCase))
            return true;
        return DateSearch.MatchesCalendarDate(task.StartDate, "d", search)
               || DateSearch.MatchesCalendarDate(task.EndDate, "d", search);
    }
}
