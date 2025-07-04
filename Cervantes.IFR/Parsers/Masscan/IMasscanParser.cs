using System;

namespace Cervantes.IFR.Parsers.Masscan;

public interface IMasscanParser
{
    void Parse(Guid? project, string user, string path);
}