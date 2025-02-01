using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class RssSourceManager: GenericManager<RssSource>, IRssSourceManager
{
    public RssSourceManager(IApplicationDbContext context) : base(context)
    {
    }
}