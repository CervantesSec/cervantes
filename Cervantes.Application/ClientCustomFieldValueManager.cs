using Cervantes.CORE;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ClientCustomFieldValueManager : GenericManager<ClientCustomFieldValue>, IClientCustomFieldValueManager
{
    /// <summary>
    /// Client Custom Field Value Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public ClientCustomFieldValueManager(IApplicationDbContext context) : base(context)
    {
    }
}