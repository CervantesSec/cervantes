namespace Cervantes.Web.Helpers;

public static class CustomFieldRules
{
    /// <summary>
    /// Labels of the active required custom fields that have no value, or only whitespace, in <paramref name="values"/>.
    /// For an edit, pass the stored values merged with the submitted ones.
    /// </summary>
    public static IReadOnlyList<string> MissingRequired(IEnumerable<(Guid Id, string Label, bool IsRequired, bool IsActive)> fields,
        IReadOnlyDictionary<Guid, string>? values)
    {
        return fields
            .Where(f => f.IsActive && f.IsRequired)
            .Where(f => values == null || !values.TryGetValue(f.Id, out var value) || string.IsNullOrWhiteSpace(value))
            .Select(f => f.Label)
            .ToList();
    }

    /// <summary>Stored values with the submitted ones on top, for checking an edit.</summary>
    public static IReadOnlyDictionary<Guid, string> Merge(IEnumerable<KeyValuePair<Guid, string?>> stored, IReadOnlyDictionary<Guid, string>? submitted)
    {
        var merged = stored.ToDictionary(x => x.Key, x => x.Value ?? "");
        foreach (var (key, value) in submitted ?? new Dictionary<Guid, string>())
        {
            merged[key] = value;
        }
        return merged;
    }
}
