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

        var existingIds = vulnManager.GetAll()
            .Where(x => x.ProjectId == project.Id && x.Template == false)
            .Select(x => x.FindingId)
            .ToList();
        return Next(existingIds, project.FindingsId);
    }

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
