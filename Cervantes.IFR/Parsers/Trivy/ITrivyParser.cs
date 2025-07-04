using System;

namespace Cervantes.IFR.Parsers.Trivy;

public interface ITrivyParser
{
    void Parse(Guid? project, string user, string path);
}