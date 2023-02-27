using System;
using Cervantes.CORE;

namespace Cervantes.Contracts;

public interface IWSTGManager: IGenericManager<WSTG>
{
    public WSTG GetTargetId(Guid project, Guid target);
}