using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class VulnCustomFieldValueManager : GenericManager<VulnCustomFieldValue>, IVulnCustomFieldValueManager
{
    public VulnCustomFieldValueManager(IApplicationDbContext context) : base(context)
    {
    }
}