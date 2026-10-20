using Cervantes.Contracts;
using Microsoft.Extensions.Logging;

namespace Cervantes.Web.Helpers;

/// <summary>The rows a project delete removed whose attachment folders must go once the delete is saved.</summary>
public sealed record ProjectTree(IReadOnlyList<Guid> ProjectIds, IReadOnlyList<Guid> VulnIds, IReadOnlyList<Guid> TaskIds);

/// <summary>
/// Deletes projects together with their vulns, tasks and targets. Those three point at the project with a NO ACTION
/// foreign key, so removing only the project fails in the database, or, when the dependents are already tracked (a
/// Blazor circuit that showed the project), EF sets their ProjectId to null and leaves them without a project.
/// Everything else under a project, a vuln, a task or a target is removed by cascade.
/// </summary>
public class ProjectTreeDeleter
{
    private readonly IProjectManager projectManager;
    private readonly IVulnManager vulnManager;
    private readonly ITaskManager taskManager;
    private readonly ITargetManager targetManager;
    private readonly IWebHostEnvironment env;
    private readonly ILogger<ProjectTreeDeleter> logger;

    public ProjectTreeDeleter(IProjectManager projectManager, IVulnManager vulnManager, ITaskManager taskManager,
        ITargetManager targetManager, IWebHostEnvironment env, ILogger<ProjectTreeDeleter> logger)
    {
        this.projectManager = projectManager;
        this.vulnManager = vulnManager;
        this.taskManager = taskManager;
        this.targetManager = targetManager;
        this.env = env;
        this.logger = logger;
    }

    /// <summary>
    /// Marks the projects and their vulns, tasks and targets as removed in the shared context, without saving. The
    /// caller saves once, so the whole delete commits or fails together.
    /// </summary>
    public ProjectTree Stage(IReadOnlyCollection<Guid> projectIds)
    {
        var ids = projectIds.ToList();
        var vulns = vulnManager.GetAll().Where(x => x.ProjectId != null && ids.Contains(x.ProjectId.Value)).ToList();
        var tasks = taskManager.GetAll().Where(x => x.ProjectId != null && ids.Contains(x.ProjectId.Value)).ToList();
        var targets = targetManager.GetAll().Where(x => x.ProjectId != null && ids.Contains(x.ProjectId.Value)).ToList();

        foreach (var vuln in vulns)
        {
            vulnManager.Remove(vuln);
        }

        foreach (var task in tasks)
        {
            taskManager.Remove(task);
        }

        foreach (var target in targets)
        {
            targetManager.Remove(target);
        }

        var removed = new List<Guid>();
        foreach (var id in ids)
        {
            var project = projectManager.GetById(id);
            if (project != null)
            {
                projectManager.Remove(project);
                removed.Add(id);
            }
        }

        return new ProjectTree(removed, vulns.Select(x => x.Id).ToList(), tasks.Select(x => x.Id).ToList());
    }

    /// <summary>Removes the attachment folders of a saved project delete. A folder that cannot go is only logged.</summary>
    public void DeleteFolders(ProjectTree tree, string userId)
    {
        foreach (var id in tree.VulnIds)
        {
            AttachmentFiles.DeleteOwnerFolder(env.WebRootPath, "Vuln", id, logger, "Vuln", userId);
        }

        foreach (var id in tree.TaskIds)
        {
            AttachmentFiles.DeleteOwnerFolder(env.WebRootPath, "Task", id, logger, "Task", userId);
        }

        foreach (var id in tree.ProjectIds)
        {
            AttachmentFiles.DeleteOwnerFolder(env.WebRootPath, "Projects", id, logger, "Project", userId);
        }
    }
}
