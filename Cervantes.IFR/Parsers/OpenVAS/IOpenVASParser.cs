using System;

namespace Cervantes.IFR.Parsers.OpenVAS;

public interface IOpenVASParser
{
    void Parse(Guid? project, string user, string path);
}