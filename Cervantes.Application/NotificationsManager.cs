using System.Collections.Generic;
using System.Linq;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class NotificationsManager : GenericManager<Notifications>, INotificationManager
{
    
    NotificationsManager(IApplicationDbContext context) : base(context)
    {
    }

    
}