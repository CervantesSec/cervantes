using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application;

public class TargetManager : GenericManager<Target>, ITargetManager
{
    /// <summary>
    /// Target Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public TargetManager(IApplicationDbContext context) : base(context)
    {
    }
    
    public Target GetByName(string name)
    {
        return Context.Set<Target>().FirstOrDefault(x => x.Name == name);
    }
}