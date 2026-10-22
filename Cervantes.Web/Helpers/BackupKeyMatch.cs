namespace Cervantes.Web.Helpers;

public static class BackupKeyMatch
{
    /// <summary>
    /// Pairs backup rows with existing rows that have the same natural key but another id, and returns backup id →
    /// existing id. Seeded data (categories, report components, templates) gets random ids on each instance, so a
    /// restore into another instance would otherwise insert a second copy of every seeded row. A backup row whose id
    /// already exists is left out: the restore skips it as before. When several existing rows share a key, the one
    /// with the lowest id is used, so the choice does not depend on query order.
    /// </summary>
    public static Dictionary<Guid, Guid> Match<T>(IEnumerable<T> backupRows, IEnumerable<T> existingRows, Func<T, Guid> id,
        Func<T, string> key)
    {
        var existing = existingRows.ToList();
        var existingIds = existing.Select(id).ToHashSet();
        var byKey = existing
            .GroupBy(key, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.Select(id).Min(), StringComparer.Ordinal);

        var map = new Dictionary<Guid, Guid>();
        foreach (var row in backupRows)
        {
            var rowId = id(row);
            if (!existingIds.Contains(rowId) && byKey.TryGetValue(key(row), out var existingId))
            {
                map[rowId] = existingId;
            }
        }

        return map;
    }

    /// <summary>
    /// Whether a report component in the backup has the same HTML and CSS as the one this instance already has
    /// (ignoring line endings and surrounding whitespace). A restore only warns about discarding a component that differs.
    /// </summary>
    public static bool SameComponentContent(string? contentA, string? cssA, string? contentB, string? cssB)
    {
        static string Normalize(string? value) => (value ?? "").Replace("\r\n", "\n").Trim();
        return Normalize(contentA) == Normalize(contentB) && Normalize(cssA) == Normalize(cssB);
    }
}
