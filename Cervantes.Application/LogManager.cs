using Cervantes.Contracts;
using Cervantes.CORE;

namespace Cervantes.Application
{
    public class LogManager : GenericManager<Log>, ILogManager
    {
        public LogManager(IApplicationDbContext context) : base(context)
        {
        }
    }
}
