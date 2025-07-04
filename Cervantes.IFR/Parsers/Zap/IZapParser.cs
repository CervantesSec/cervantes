using System;

namespace Cervantes.IFR.Parsers.Zap;

public interface IZapParser
{
    void Parse(Guid? project, string user, string path);
}