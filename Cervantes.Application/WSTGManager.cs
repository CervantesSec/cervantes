using Cervantes.Contracts;
using Cervantes.CORE;

namespace Cervantes.Application;

public class WSTGManager: GenericManager<WSTG>, IWSTGManager
{
    public WSTGManager(IApplicationDbContext context) : base(context)
    {
    }
}