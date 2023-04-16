using Cervantes.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Contracts;

public interface ITargetManager : IGenericManager<Target>
{
    Target GetByName(string name);
}