using System.Collections.Generic;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

public interface INotificationsManager : IGenericManager<Notifications>
{
    IEnumerable<Notifications> GetById(string id);
}