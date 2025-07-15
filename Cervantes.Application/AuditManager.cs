using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class AuditManager: GenericManager<Audit>, IAuditManager
{
    public AuditManager(IApplicationDbContext context) : base(context)
    {
    }
}