using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

public interface IChecklistExecutionManager : IGenericManager<ChecklistExecution>
{
    /// <summary>
    /// Get executions by checklist
    /// </summary>
    /// <param name="checklistId"></param>
    /// <returns></returns>
    IQueryable<ChecklistExecution> GetByChecklist(Guid checklistId);

    /// <summary>
    /// Get executions by item
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    IQueryable<ChecklistExecution> GetByItem(Guid itemId);

    /// <summary>
    /// Get executions by user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    IQueryable<ChecklistExecution> GetByUser(string userId);

    /// <summary>
    /// Bulk update execution status
    /// </summary>
    /// <param name="executionIds"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    Task<int> BulkUpdateStatus(List<Guid> executionIds, ChecklistItemStatus status);
}