using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application;

public class TaskAttachmentManager : GenericManager<TaskAttachment>, ITaskAttachmentManager
{
    /// <summary>
    /// TaskAttachment Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public TaskAttachmentManager(IApplicationDbContext context) : base(context)
    {
    }
}