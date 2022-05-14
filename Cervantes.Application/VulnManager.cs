using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application;

public class VulnManager : GenericManager<Vuln>, IVulnManager
{
    /// <summary>
    /// Vuln Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public VulnManager(IApplicationDbContext context) : base(context)
    {
    }
}