using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Application;

public class ChecklistManager : GenericManager<Checklist>, IChecklistManager
{
    /// <summary>
    /// Checklist Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public ChecklistManager(IApplicationDbContext context) : base(context)
    {
    }

    public IQueryable<Checklist> GetByProject(Guid projectId)
    {
        return GetAll()
            .Include(c => c.ChecklistTemplate)
            .Include(c => c.Target)
            .Include(c => c.User)
            .Include(c => c.Executions)
            .Where(c => c.ProjectId == projectId);
    }

    public IQueryable<Checklist> GetByTarget(Guid targetId)
    {
        return GetAll()
            .Include(c => c.ChecklistTemplate)
            .Include(c => c.Project)
            .Include(c => c.User)
            .Include(c => c.Executions)
            .Where(c => c.TargetId == targetId);
    }

    public IQueryable<Checklist> GetByUser(string userId)
    {
        return GetAll()
            .Include(c => c.ChecklistTemplate)
            .Include(c => c.Project)
            .Include(c => c.Target)
            .Include(c => c.Executions)
            .Where(c => c.UserId == userId);
    }

    public decimal CalculateCompletionPercentage(Guid checklistId)
    {
        var executions = Context.Set<ChecklistExecution>()
            .Where(e => e.ChecklistId == checklistId)
            .ToList();

        if (!executions.Any())
            return 0;

        var completedCount = executions.Count(e => 
            e.Status == ChecklistItemStatus.Passed || 
            e.Status == ChecklistItemStatus.Failed || 
            e.Status == ChecklistItemStatus.NotApplicable);

        return Math.Round((decimal)completedCount / executions.Count * 100, 2);
    }
}