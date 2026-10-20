using Cervantes.Web.Helpers;

namespace Cervantes.Tests;

public class CalendarDateTests
{
    private static TimeZoneInfo Zone(int hours)
        => TimeZoneInfo.CreateCustomTimeZone($"qa{hours}", TimeSpan.FromHours(hours), $"qa{hours}", $"qa{hours}");

    [Fact]
    public void ToLocalDate_PositiveOffset()
    {
        // Midnight of 19 October in Madrid (summer time) is stored as 18 October 22:00 UTC.
        var stored = new DateTime(2026, 10, 18, 22, 0, 0, DateTimeKind.Utc);
        Assert.Equal(new DateTime(2026, 10, 19), CalendarDate.ToLocalDate(stored, Zone(2)));
    }

    [Fact]
    public void ToLocalDate_NegativeOffset()
    {
        var stored = new DateTime(2026, 10, 19, 5, 0, 0, DateTimeKind.Utc);
        Assert.Equal(new DateTime(2026, 10, 19), CalendarDate.ToLocalDate(stored, Zone(-5)));
    }

    [Fact]
    public void ToLocalDate_UnspecifiedIsUtc()
    {
        var stored = new DateTime(2026, 10, 18, 22, 0, 0, DateTimeKind.Unspecified);
        Assert.Equal(new DateTime(2026, 10, 19), CalendarDate.ToLocalDate(stored, Zone(2)));
    }

    [Fact]
    public void ToLocalDate_UtcServer()
    {
        var stored = new DateTime(2026, 10, 19, 0, 0, 0, DateTimeKind.Utc);
        var date = CalendarDate.ToLocalDate(stored, TimeZoneInfo.Utc);
        Assert.Equal(new DateTime(2026, 10, 19), date);
        Assert.Equal(DateTimeKind.Unspecified, date.Kind);
    }
}
