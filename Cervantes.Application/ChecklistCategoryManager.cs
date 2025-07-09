using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ChecklistCategoryManager : GenericManager<ChecklistCategory>, IChecklistCategoryManager
{
    /// <summary>
    /// ChecklistCategory Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public ChecklistCategoryManager(IApplicationDbContext context) : base(context)
    {
    }
}