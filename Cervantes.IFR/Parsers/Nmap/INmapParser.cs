using System;
using System.IO;

namespace Cervantes.IFR.Parsers.Nmap;

public interface INmapParser
{
    void Parse(Guid project, string user, string path);
}