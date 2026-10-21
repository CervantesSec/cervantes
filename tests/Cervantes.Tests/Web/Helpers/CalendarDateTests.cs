using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

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
    [Fact]
    public void LocalDays_CoversThePickedDays()
    {
        // Picked 19 to 21 October in Madrid: stored from 18 October 22:00 UTC to 20 October 22:00 UTC.
        var days = CalendarDate.LocalDays(new DateTime(2026, 10, 18, 22, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 10, 20, 22, 0, 0, DateTimeKind.Utc), Zone(2)).ToList();

        Assert.Equal(new[] { new DateTime(2026, 10, 19), new DateTime(2026, 10, 20), new DateTime(2026, 10, 21) }, days);
    }

    [Fact]
    public void LocalDays_EndBeforeStartIsEmpty()
    {
        Assert.Empty(CalendarDate.LocalDays(new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 10, 18, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Utc));
    }
}
