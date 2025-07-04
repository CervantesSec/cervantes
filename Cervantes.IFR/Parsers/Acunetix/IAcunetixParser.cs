using System;

namespace Cervantes.IFR.Parsers.Acunetix;

public interface IAcunetixParser
{
    void Parse(Guid? project, string user, string path);
}