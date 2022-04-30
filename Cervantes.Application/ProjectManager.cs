using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application
{
    public class ProjectManager: GenericManager<Project>, IProjectManager
    {
        /// <summary>
        /// Project Manager Constructor
        /// </summary>
        /// <param name="context"></param>
        public ProjectManager(IApplicationDbContext context) : base(context)
        {

        }
    }
}
