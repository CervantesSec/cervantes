using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ProjectUserManager : GenericManager<ProjectUser>, IProjectUserManager
{
    /// <summary>
    /// ProjectUser Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public ProjectUserManager(IApplicationDbContext context) : base(context)
    {
    }

    public ProjectUser VerifyUser(Guid project,string user)
    {
        return Context.Set<ProjectUser>().Where(x => x.ProjectId == project&& x.UserId == user).FirstOrDefault();
    }
}