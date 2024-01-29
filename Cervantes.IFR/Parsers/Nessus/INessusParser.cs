using System;

namespace Cervantes.IFR.Parsers.Nessus;

public interface INessusParser
{
    void Parse(Guid? project, string user, string path);
}