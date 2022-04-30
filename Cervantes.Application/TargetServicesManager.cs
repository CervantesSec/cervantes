using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application
{
    public class TargetServicesManager: GenericManager<TargetServices>, ITargetServicesManager
    {
        /// <summary>
        /// Client Manager Constructor
        /// </summary>
        /// <param name="context"></param>
        public TargetServicesManager(IApplicationDbContext context) : base(context)
        {
        }
    }
}
