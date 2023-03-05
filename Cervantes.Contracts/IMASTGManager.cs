using System;
using Cervantes.CORE;

namespace Cervantes.Contracts;

public interface IMASTGManager: IGenericManager<MASTG>
{
    public MASTG GetTargetId(Guid project, Guid target);
}