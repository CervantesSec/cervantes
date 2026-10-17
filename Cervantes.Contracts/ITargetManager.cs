using Cervantes.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

public interface ITargetManager : IGenericManager<Target>
{
    /// <summary>The target with this name inside the given project (null projectId: targets without a project).</summary>
    Target GetByName(string name, Guid? projectId);
}