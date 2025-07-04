using System;

namespace Cervantes.IFR.Parsers.Qualys;

public interface IQualysParser
{
    void Parse(Guid? project, string user, string path);
}