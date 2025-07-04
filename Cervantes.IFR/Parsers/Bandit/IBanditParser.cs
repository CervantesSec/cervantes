using System;

namespace Cervantes.IFR.Parsers.Bandit;

public interface IBanditParser
{
    void Parse(Guid? project, string user, string path);
}