using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application
{
    public class OrganizationManager: GenericManager<Organization>, IOrganizationManager
    {
        /// <summary>
        /// Organization Manager Constructor
        /// </summary>
        /// <param name="context"></param>
        public OrganizationManager(IApplicationDbContext context) : base(context)
        {
        }
    }
}
