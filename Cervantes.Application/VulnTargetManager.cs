using Cervantes.Contracts;
using Cervantes.CORE;

namespace Cervantes.Application;

public class VulnTargetManager: GenericManager<VulnTargets>, IVulnTargetManager
{
    /// <summary>
    /// VulnTarget Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public VulnTargetManager(IApplicationDbContext context) : base(context)
    {
    }
}