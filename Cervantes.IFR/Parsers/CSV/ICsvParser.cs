using System;

namespace Cervantes.IFR.Parsers.CSV;

public interface ICsvParser
{
    void Parse(Guid? project, string user, string path);
}