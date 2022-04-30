using System.Collections.Generic;
using System.Linq;
using Cervantes.Contracts;
using Cervantes.CORE;

namespace Cervantes.Application;

public class NotificationsManager: GenericManager<Notifications>, INotificationsManager
{
    /// <summary>
    /// Note Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public NotificationsManager(IApplicationDbContext context) : base(context)
    {
    }
    
    /// <summary>
    /// Get entity by id key
    /// </summary>
    /// <param name="key">id</param>
    /// <returns>Entity</returns>
    public IEnumerable<Notifications> GetById(string id)
    {
        return Context.Set<Notifications>().Where(x => x.UserId == id);
    }
}