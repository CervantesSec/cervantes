using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Contracts;

public interface ILogManager : IGenericManager<Log>
{
    Task DeleteAllAsync();
}