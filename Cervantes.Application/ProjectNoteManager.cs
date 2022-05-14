using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application;

public class ProjectNoteManager : GenericManager<ProjectNote>, IProjectNoteManager
{
    /// <summary>
    /// ProjectNote Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public ProjectNoteManager(IApplicationDbContext context) : base(context)
    {
    }
}