namespace Cervantes.Web.Helpers;

public static class TaskFilter
{
    /// <summary>
    /// The task boards' user and project filters, applied to a freshly loaded list so filtering twice never narrows
    /// an already filtered list. An empty user or <see cref="Guid.Empty"/> project leaves that filter off.
    /// </summary>
    public static List<CORE.Entities.Task> Apply(IEnumerable<CORE.Entities.Task> tasks, string? userId, Guid projectId)
    {
        var result = tasks;
        if (!string.IsNullOrEmpty(userId))
        {
            result = result.Where(x => x.AsignedUserId == userId);
        }
        if (projectId != Guid.Empty)
        {
            result = result.Where(x => x.ProjectId == projectId);
        }
        return result.ToList();
    }
}
