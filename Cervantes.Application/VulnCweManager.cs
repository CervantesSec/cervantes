using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class VulnCweManager : GenericManager<VulnCwe>, IVulnCweManager
{
    /// <summary>
    /// vuln Cwe Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public VulnCweManager(IApplicationDbContext context) : base(context)
    {
    }
}