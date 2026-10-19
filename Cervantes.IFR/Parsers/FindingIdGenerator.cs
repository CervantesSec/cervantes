using System.Globalization;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.IFR.Parsers;

public static class FindingIdGenerator
{
    /// <summary>
    /// Finding id for a vuln about to be added to a project: the project's prefix and the number after the highest
    /// one already used in the project. Counting the vulns would hand out a number again once a finding is deleted.
    /// Parsers save each vuln as they add it, so numbers stay consecutive within one import. Vulns without a project
    /// have no finding id.
    /// </summary>
    public static string Next(IVulnManager vulnManager, Project? project)
    {
        if (project == null)
        {
            return string.Empty;
        }

        return Next(ProjectFindingIds(vulnManager.GetAll(), project.Id), project.FindingsId);
    }

    // Template rows count too: older builds let a project vuln be a template and numbered it, and the flag can be
    // turned off later, which would bring a reused number into the report.
    internal static List<string?> ProjectFindingIds(IQueryable<Vuln> vulns, Guid projectId)
        => vulns.Where(x => x.ProjectId == projectId).Select(x => x.FindingId).ToList()!;

    /// <summary>
    /// The next finding id after the highest "&lt;prefix&gt;-N" in <paramref name="existingIds"/>. Ids written by hand or
    /// with another prefix are ignored. The prefix may itself contain dashes, so the number is whatever follows the
    /// prefix and its dash.
    /// </summary>
    internal static string Next(IEnumerable<string?> existingIds, string? findingsId)
    {
        var prefix = (findingsId ?? string.Empty) + "-";
        var highest = 0;
        foreach (var id in existingIds)
        {
            if (id != null && id.StartsWith(prefix, StringComparison.Ordinal)
                && int.TryParse(id.AsSpan(prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, out var number)
                && number > highest)
            {
                highest = number;
            }
        }

        return $"{findingsId}-{highest + 1:D2}";
    }
}
