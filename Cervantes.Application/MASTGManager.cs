using System;
using System.Linq;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class MASTGManager: GenericManager<MASTG>, IMASTGManager
{
    public MASTGManager(IApplicationDbContext context) : base(context)
    {
    }
    
    public MASTG GetTargetId(Guid project,Guid target)
    {
        return Context.Set<MASTG>().Where(x => x.ProjectId == project&& x.TargetId == target).FirstOrDefault();
    }
}