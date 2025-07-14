using Cervantes.CORE;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ClientCustomFieldManager : GenericManager<ClientCustomField>, IClientCustomFieldManager
{
    /// <summary>
    /// Client Custom Field Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public ClientCustomFieldManager(IApplicationDbContext context) : base(context)
    {
    }
}