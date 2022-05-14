using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application;

public class TaskNoteManager : GenericManager<TaskNote>, ITaskNoteManager
{
    /// <summary>
    /// TaskNote Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public TaskNoteManager(IApplicationDbContext context) : base(context)
    {
    }
}