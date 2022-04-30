using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application
{
    public class VulnNoteManager: GenericManager<VulnNote>, IVulnNoteManager
    {
        /// <summary>
        /// VulnNote Manager Constructor
        /// </summary>
        /// <param name="context"></param>
        public VulnNoteManager(IApplicationDbContext context) : base(context)
        {
        }
    }
}
