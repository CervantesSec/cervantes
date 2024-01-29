using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ProjectAttachmentManager : GenericManager<ProjectAttachment>, IProjectAttachmentManager
{
    /// <summary>
    /// ProjectAttachment Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public ProjectAttachmentManager(IApplicationDbContext context) : base(context)
    {
    }
}