using System;

namespace Cervantes.IFR.Parsers.Nikto;

public interface INiktoParser
{
    void Parse(Guid? project, string user, string path);
}