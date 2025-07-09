using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

public interface IChecklistManager : IGenericManager<Checklist>
{
    /// <summary>
    /// Get checklists by project
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    IQueryable<Checklist> GetByProject(Guid projectId);

    /// <summary>
    /// Get checklists by target
    /// </summary>
    /// <param name="targetId"></param>
    /// <returns></returns>
    IQueryable<Checklist> GetByTarget(Guid targetId);

    /// <summary>
    /// Get checklists by user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    IQueryable<Checklist> GetByUser(string userId);

    /// <summary>
    /// Calculate completion percentage for a checklist
    /// </summary>
    /// <param name="checklistId"></param>
    /// <returns></returns>
    decimal CalculateCompletionPercentage(Guid checklistId);
}