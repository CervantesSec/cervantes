using Cervantes.CORE;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class TargetCustomFieldValueManager : GenericManager<TargetCustomFieldValue>, ITargetCustomFieldValueManager
{
    /// <summary>
    /// Target Custom Field Value Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public TargetCustomFieldValueManager(IApplicationDbContext context) : base(context)
    {
    }
}