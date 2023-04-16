using System;

namespace Cervantes.IFR.Parsers.Burp;

public interface IBurpParser
{
    void Parse(Guid? project, string user, string path);
}