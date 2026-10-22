using System.Globalization;
using Cervantes.CORE.Helpers;

namespace Cervantes.Web.Helpers;

public static class DateSearch
{
    /// <summary>
    /// Whether the search text is part of the date as a grid column shows it (<paramref name="format"/> in the current
    /// culture). An empty search matches nothing: callers keep their own "empty search shows every row" check.
    /// </summary>
    public static bool Matches(DateTime value, string format, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return false;
        }

        return value.ToString(format, CultureInfo.CurrentCulture).Contains(search.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Same as <see cref="Matches"/> for a stored project or task date, using the calendar day the user picked.</summary>
    public static bool MatchesCalendarDate(DateTime stored, string format, string? search)
        => Matches(stored.ToLocalDate(), format, search);

    internal static bool MatchesCalendarDate(DateTime stored, string format, string? search, TimeZoneInfo zone)
        => Matches(CalendarDate.ToLocalDate(stored, zone), format, search);
}
