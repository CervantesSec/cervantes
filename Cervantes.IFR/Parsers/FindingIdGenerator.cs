using System.Globalization;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.IFR.Parsers;

public static class FindingIdGenerator
{
    /// <summary>
    /// Finding id for one vuln about to be added to a project: the project's prefix and the number after the highest
    /// one already used in the project. Counting the vulns would hand out a number again once a finding is deleted.
    /// Vulns without a project have no finding id.
    /// </summary>
    public static string Next(IVulnManager vulnManager, Project? project) => Start(vulnManager, project).Next();

    /// <summary>
    /// The finding ids for the vulns of one import into <paramref name="project"/>. The project's ids are read once
    /// here, and each <see cref="FindingIdSequence.Next"/> hands out the following number, so an import of N findings
    /// does not read the project's ids N times.
    /// </summary>
    public static FindingIdSequence Start(IVulnManager vulnManager, Project? project)
    {
        if (project == null)
        {
            return FindingIdSequence.Empty;
        }

        return Start(ProjectFindingIds(vulnManager.GetAll(), project.Id), project.FindingsId);
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
    internal static string Next(IEnumerable<string?> existingIds, string? findingsId) => Start(existingIds, findingsId).Next();

    internal static FindingIdSequence Start(IEnumerable<string?> existingIds, string? findingsId)
    {
        var prefix = (findingsId ?? string.Empty) + "-";
        long highest = 0;
        foreach (var id in existingIds)
        {
            // long.MaxValue is skipped so that the next number cannot overflow; a number that large never comes
            // from numbering, only from a hand-edited or restored row.
            if (id != null && id.StartsWith(prefix, StringComparison.Ordinal)
                && long.TryParse(id.AsSpan(prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, out var number)
                && number > highest && number < long.MaxValue)
            {
                highest = number;
            }
        }

        return new FindingIdSequence(findingsId, highest);
    }
}

/// <summary>
/// Hands out consecutive finding ids of one project, starting after the highest one that existed when the sequence
/// was made. Callers that save vulns in between must not mix it with another sequence for the same project.
/// </summary>
public sealed class FindingIdSequence
{
    internal static readonly FindingIdSequence Empty = new(null, 0, true);

    private readonly string? findingsId;
    private readonly bool empty;
    private long last;

    internal FindingIdSequence(string? findingsId, long highest, bool empty = false)
    {
        this.findingsId = findingsId;
        last = highest;
        this.empty = empty;
    }

    public string Next()
    {
        if (empty)
        {
            return string.Empty;
        }

        last++;
        return $"{findingsId}-{last:D2}";
    }
}
