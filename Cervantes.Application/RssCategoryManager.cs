using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class RssCategoryManager: GenericManager<RssCategory>, IRssCategoryManager
{
    public RssCategoryManager(IApplicationDbContext context) : base(context)
    {
    }
}