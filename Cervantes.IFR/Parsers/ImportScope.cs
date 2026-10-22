using Cervantes.CORE.Entities;

namespace Cervantes.IFR.Parsers;

public static class ImportScope
{
    /// <summary>
    /// True when an import of this type needs a project. CSV, Pwndoc, Burp and Nessus can also be imported without one,
    /// as vulnerability templates; scanner output of the other types only makes sense against a project's targets.
    /// </summary>
    public static bool RequiresProject(VulnImportType type)
    {
        return type is not (VulnImportType.CSV or VulnImportType.Pwndoc or VulnImportType.Burp or VulnImportType.Nessus);
    }
}
