using System;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

public interface IMASTGManager: IGenericManager<MASTG>
{
    public MASTG GetTargetId(Guid project, Guid target);
}