using System;
using System.Linq;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class WSTGManager: GenericManager<WSTG>, IWSTGManager
{
    public WSTGManager(IApplicationDbContext context) : base(context)
    {
    }
    
    public WSTG GetTargetId(Guid project,Guid target)
    {
        return Context.Set<WSTG>().Where(x => x.ProjectId == project&& x.TargetId == target).FirstOrDefault();
    }
}