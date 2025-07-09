using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ChecklistItemManager : GenericManager<ChecklistItem>, IChecklistItemManager
{
    /// <summary>
    /// ChecklistItem Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public ChecklistItemManager(IApplicationDbContext context) : base(context)
    {
    }
}