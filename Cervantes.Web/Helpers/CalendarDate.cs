namespace Cervantes.Web.Helpers;

public static class CalendarDate
{
    /// <summary>
    /// The calendar day a stored task or project date stands for. The date pickers return midnight in the server's
    /// time zone and the controllers store it as UTC, so showing the stored value as it is moves the date one day back
    /// whenever the server is ahead of UTC. Values read from the database are UTC; one with no kind is taken as UTC too.
    /// </summary>
    public static DateTime ToLocalDate(this DateTime stored) => ToLocalDate(stored, TimeZoneInfo.Local);

    internal static DateTime ToLocalDate(DateTime stored, TimeZoneInfo zone)
        => DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(stored, DateTimeKind.Utc), zone).Date,
            DateTimeKind.Unspecified);
}
