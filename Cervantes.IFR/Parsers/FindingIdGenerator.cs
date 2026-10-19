using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.IFR.Parsers;

public static class FindingIdGenerator
{
    /// <summary>
    /// Finding id for a vuln about to be added to a project: the project's prefix and the next number, as the CSV,
    /// Nessus and Burp imports do. Parsers save each vuln as they add it, so counting the project's vulns gives
    /// consecutive numbers within one import and across imports. Vulns without a project have no finding id.
    /// </summary>
    public static string Next(IVulnManager vulnManager, Project? project)
    {
        if (project == null)
        {
            return string.Empty;
        }

        var number = vulnManager.GetAll().Count(x => x.ProjectId == project.Id && x.Template == false) + 1;
        return $"{project.FindingsId}-{number:D2}";
    }
}
