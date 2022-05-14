using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application;

public class TaskManager : GenericManager<Cervantes.CORE.Task>, ITaskManager
{
    /// <summary>
    /// Task Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public TaskManager(IApplicationDbContext context) : base(context)
    {
    }
}