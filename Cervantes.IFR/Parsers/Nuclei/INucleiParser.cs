using System;

namespace Cervantes.IFR.Parsers.Nuclei;

public interface INucleiParser
{
    void Parse(Guid? project, string user, string path);
}