using System;

namespace Cervantes.IFR.Parsers.Prowler;

public interface IProwlerParser
{
    void Parse(Guid? project, string user, string path);
}