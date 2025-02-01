using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class RssNewsManager: GenericManager<RssNews>, IRssNewsManager
{
    public RssNewsManager(IApplicationDbContext context) : base(context)
    {
    }
}