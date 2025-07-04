using System;

namespace Cervantes.IFR.Parsers.DependencyCheck;

public interface IDependencyCheckParser
{
    void Parse(Guid? project, string user, string path);
}