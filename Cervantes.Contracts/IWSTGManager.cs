using System;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

public interface IWSTGManager: IGenericManager<WSTG>
{
    public WSTG GetTargetId(Guid project, Guid target);
}