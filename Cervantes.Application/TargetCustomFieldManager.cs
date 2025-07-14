using Cervantes.CORE;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class TargetCustomFieldManager : GenericManager<TargetCustomField>, ITargetCustomFieldManager
{
    /// <summary>
    /// Target Custom Field Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public TargetCustomFieldManager(IApplicationDbContext context) : base(context)
    {
    }
}