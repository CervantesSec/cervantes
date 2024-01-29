using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class TaskTargetManager: GenericManager<TaskTargets>, ITaskTargetManager
{
    /// <summary>
    /// TaskTarget Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public TaskTargetManager(IApplicationDbContext context) : base(context)
    {
        
    }
}