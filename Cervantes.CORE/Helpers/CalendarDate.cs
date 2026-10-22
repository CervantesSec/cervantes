namespace Cervantes.CORE.Helpers;

public static class CalendarDate
{
    /// <summary>
    /// The calendar day a stored task or project date stands for. The date pickers return midnight in the server's
    /// time zone and the controllers store it as UTC, so showing the stored value as it is moves the date one day back
    /// whenever the server is ahead of UTC. Values read from the database are UTC; one with no kind is taken as UTC too.
    /// </summary>
    public static DateTime ToLocalDate(this DateTime stored) => ToLocalDate(stored, TimeZoneInfo.Local);

    public static DateTime ToLocalDate(DateTime stored, TimeZoneInfo zone)
    {
        // A value already in local time is the calendar day itself; anything else is taken as UTC.
        if (stored.Kind == DateTimeKind.Local)
        {
            return DateTime.SpecifyKind(stored.Date, DateTimeKind.Unspecified);
        }

        return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(stored, DateTimeKind.Utc), zone).Date,
            DateTimeKind.Unspecified);
    }

    /// <summary>The calendar days from a stored start date to a stored end date, both included.</summary>
    public static IEnumerable<DateTime> LocalDays(DateTime start, DateTime end) => LocalDays(start, end, TimeZoneInfo.Local);

    internal static IEnumerable<DateTime> LocalDays(DateTime start, DateTime end, TimeZoneInfo zone)
    {
        var last = ToLocalDate(end, zone);
        for (var day = ToLocalDate(start, zone); day <= last; day = day.AddDays(1))
        {
            yield return day;
        }
    }

    /// <summary>Percentage of the stored start..end range elapsed on <paramref name="today"/>, from 0 to 100.</summary>
    public static double ProgressPercent(DateTime storedStart, DateTime storedEnd, DateTime today) => ProgressPercent(storedStart, storedEnd, today, TimeZoneInfo.Local);

    internal static double ProgressPercent(DateTime storedStart, DateTime storedEnd, DateTime today, TimeZoneInfo zone)
    {
        var start = ToLocalDate(storedStart, zone);
        var end = ToLocalDate(storedEnd, zone);
        if (end <= start)
        {
            return today.Date >= end ? 100 : 0;
        }

        var percent = Math.Round(100 * (today.Date - start).TotalDays / (end - start).TotalDays);
        return Math.Clamp(percent, 0, 100);
    }
}
