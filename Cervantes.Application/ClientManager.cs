using Cervantes.Contracts;
using Cervantes.CORE;

namespace Cervantes.Application;

public class ClientManager : GenericManager<Client>, IClientManager
{
    /// <summary>
    /// Client Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public ClientManager(IApplicationDbContext context) : base(context)
    {
    }
}