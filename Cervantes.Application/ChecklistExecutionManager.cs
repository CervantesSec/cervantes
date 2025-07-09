using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Application;

public class ChecklistExecutionManager : GenericManager<ChecklistExecution>, IChecklistExecutionManager
{
    /// <summary>
    /// ChecklistExecution Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public ChecklistExecutionManager(IApplicationDbContext context) : base(context)
    {
    }

    public IQueryable<ChecklistExecution> GetByChecklist(Guid checklistId)
    {
        return GetAll()
            .Include(e => e.ChecklistItem)
                .ThenInclude(i => i.ChecklistCategory)
            .Include(e => e.TestedByUser)
            .Include(e => e.Vulnerability)
            .Where(e => e.ChecklistId == checklistId);
    }

    public IQueryable<ChecklistExecution> GetByItem(Guid itemId)
    {
        return GetAll()
            .Include(e => e.Checklist)
                .ThenInclude(c => c.Project)
            .Include(e => e.TestedByUser)
            .Include(e => e.Vulnerability)
            .Where(e => e.ChecklistItemId == itemId);
    }

    public IQueryable<ChecklistExecution> GetByUser(string userId)
    {
        return GetAll()
            .Include(e => e.Checklist)
                .ThenInclude(c => c.Project)
            .Include(e => e.ChecklistItem)
                .ThenInclude(i => i.ChecklistCategory)
            .Include(e => e.Vulnerability)
            .Where(e => e.TestedByUserId == userId);
    }

    public async Task<int> BulkUpdateStatus(List<Guid> executionIds, ChecklistItemStatus status)
    {
        var executions = await Context.Set<ChecklistExecution>()
            .Where(e => executionIds.Contains(e.Id))
            .ToListAsync();

        var affectedChecklists = executions.Select(e => e.ChecklistId).Distinct().ToList();

        foreach (var execution in executions)
        {
            execution.Status = status;
            if (status != ChecklistItemStatus.NotTested)
            {
                execution.TestedDate = DateTime.UtcNow;
            }
        }

        await Context.SaveChangesAsync();

        // Update completion percentage for all affected checklists
        foreach (var checklistId in affectedChecklists)
        {
            UpdateChecklistCompletion(checklistId);
        }

        return executions.Count;
    }

    private void UpdateChecklistCompletion(Guid checklistId)
    {
        var checklist = Context.Set<Checklist>().FirstOrDefault(c => c.Id == checklistId);
        if (checklist == null) return;

        var executions = Context.Set<ChecklistExecution>()
            .Where(e => e.ChecklistId == checklistId)
            .ToList();

        if (!executions.Any())
        {
            checklist.CompletionPercentage = 0;
        }
        else
        {
            var completedCount = executions.Count(e => 
                e.Status == ChecklistItemStatus.Passed || 
                e.Status == ChecklistItemStatus.Failed || 
                e.Status == ChecklistItemStatus.NotApplicable);

            checklist.CompletionPercentage = Math.Round((decimal)completedCount / executions.Count * 100, 2);
        }

        // Update checklist status based on completion
        if (checklist.CompletionPercentage == 100)
        {
            checklist.Status = ChecklistStatus.Completed;
            checklist.CompletedDate = DateTime.UtcNow;
        }
        else if (checklist.CompletionPercentage > 0)
        {
            checklist.Status = ChecklistStatus.InProgress;
        }
        else
        {
            checklist.Status = ChecklistStatus.NotStarted;
        }

        checklist.ModifiedDate = DateTime.UtcNow;
        Context.SaveChangesAsync();
    }
}