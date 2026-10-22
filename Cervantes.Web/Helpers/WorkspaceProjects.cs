using System.Linq.Expressions;
using Cervantes.CORE.Entities;

namespace Cervantes.Web.Helpers;

public static class WorkspaceProjects
{
    /// <summary>
    /// The projects /workspaces lists: those the user is a member of, while Active or PendingReview. The status
    /// test is grouped, so a PendingReview project of someone else is not listed.
    /// </summary>
    public static Expression<Func<Project, bool>> ListedFor(ICollection<Guid> memberProjectIds)
    {
        return x => memberProjectIds.Contains(x.Id) && (x.Status == ProjectStatus.Active || x.Status == ProjectStatus.PendingReview);
    }
}
