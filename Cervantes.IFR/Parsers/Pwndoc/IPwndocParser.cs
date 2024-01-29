using System;
using System.IO;

namespace Cervantes.IFR.Parsers.Pwndoc;

public interface IPwndocParser
{

    public void Parse(Guid? project, string user, string path);
}