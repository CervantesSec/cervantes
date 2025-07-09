using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

public interface IChecklistTemplateManager : IGenericManager<ChecklistTemplate>
{
    /// <summary>
    /// Get checklist templates by organization
    /// </summary>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    IQueryable<ChecklistTemplate> GetByOrganization(int? organizationId);

    /// <summary>
    /// Get system templates (WSTG, MASTG, etc.)
    /// </summary>
    /// <returns></returns>
    IQueryable<ChecklistTemplate> GetSystemTemplates();

    /// <summary>
    /// Get user-created templates
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    IQueryable<ChecklistTemplate> GetUserTemplates(string userId);
}