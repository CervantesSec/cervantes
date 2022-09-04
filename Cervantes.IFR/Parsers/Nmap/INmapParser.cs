using System.IO;

namespace Cervantes.IFR.Parsers.Nmap;

public interface INmapParser
{
    void Parse(string path);
}