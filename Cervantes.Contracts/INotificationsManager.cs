using System.Collections.Generic;
using Cervantes.CORE;

namespace Cervantes.Contracts;

public interface INotificationsManager: IGenericManager<Notifications>
{
     IEnumerable<Notifications> GetById(string id);
}