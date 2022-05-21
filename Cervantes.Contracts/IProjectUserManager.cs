using Cervantes.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Contracts;

public interface IProjectUserManager : IGenericManager<ProjectUser>
{
    ProjectUser VerifyUser(int project, string user);
}