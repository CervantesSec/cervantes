using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Application;

public class LogManager : GenericManager<Log>, ILogManager
{
    public LogManager(IApplicationDbContext context) : base(context)
    {
    }
    
    public async Task DeleteAllAsync()
    {
        var logs = await Context.Set<Log>().ToListAsync();
        foreach (var log in logs)
        {
            Context.Set<Log>().Remove(log);
        }
        await Context.SaveChangesAsync();
        Console.Write("Logs deleted successfully");
    }
}