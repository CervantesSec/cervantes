using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Application;

public class ChecklistTemplateManager : GenericManager<ChecklistTemplate>, IChecklistTemplateManager
{
    /// <summary>
    /// ChecklistTemplate Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public ChecklistTemplateManager(IApplicationDbContext context) : base(context)
    {
    }

    public IQueryable<ChecklistTemplate> GetByOrganization(int? organizationId)
    {
        return GetAll()
            .Include(ct => ct.Categories)
                .ThenInclude(c => c.Items)
            .Include(ct => ct.User)
            .Include(ct => ct.Organization)
            .Where(ct => ct.OrganizationId == organizationId || ct.OrganizationId == null);
    }

    public IQueryable<ChecklistTemplate> GetSystemTemplates()
    {
        return GetAll()
            .Include(ct => ct.Categories)
                .ThenInclude(c => c.Items)
            .Include(ct => ct.User)
            .Where(ct => ct.IsSystemTemplate == true);
    }

    public IQueryable<ChecklistTemplate> GetUserTemplates(string userId)
    {
        return GetAll()
            .Include(ct => ct.Categories)
                .ThenInclude(c => c.Items)
            .Include(ct => ct.User)
            .Include(ct => ct.Organization)
            .Where(ct => ct.UserId == userId && ct.IsSystemTemplate == false);
    }
}