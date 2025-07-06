using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class VulnCustomFieldManager : GenericManager<VulnCustomField>, IVulnCustomFieldManager
{
    public VulnCustomFieldManager(IApplicationDbContext context) : base(context)
    {
    }
}