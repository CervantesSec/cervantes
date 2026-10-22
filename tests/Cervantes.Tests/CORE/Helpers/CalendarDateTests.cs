using Cervantes.CORE.Helpers;

namespace Cervantes.Tests.CORE.Helpers;

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

    [Fact]
    public void ToLocalDate_KeepsLocalKind()
    {
        var stored = new DateTime(2026, 10, 19, 0, 0, 0, DateTimeKind.Local);
        var date = CalendarDate.ToLocalDate(stored, Zone(2));
        Assert.Equal(new DateTime(2026, 10, 19), date);
        Assert.Equal(DateTimeKind.Unspecified, date.Kind);
        Assert.Equal(new DateTime(2026, 10, 19), CalendarDate.ToLocalDate(new DateTime(2026, 10, 19, 23, 0, 0, DateTimeKind.Local), Zone(2)));
        Assert.Equal(new DateTime(2026, 10, 19), CalendarDate.ToLocalDate(stored, Zone(-5)));
    }

    // Stored values are UTC midnights that stand for Madrid (UTC+2) dates.
    private static DateTime Madrid(int day) => new DateTime(2026, 10, day, 0, 0, 0, DateTimeKind.Utc).AddHours(-2);

    [Fact]
    public void ProgressPercent_Midway()
    {
        Assert.Equal(50, CalendarDate.ProgressPercent(Madrid(1), Madrid(11), new DateTime(2026, 10, 6), Zone(2)));
    }

    [Fact]
    public void ProgressPercent_ClampsBeforeAndAfter()
    {
        Assert.Equal(0, CalendarDate.ProgressPercent(Madrid(10), Madrid(20), new DateTime(2026, 10, 5), Zone(2)));
        Assert.Equal(100, CalendarDate.ProgressPercent(Madrid(10), Madrid(20), new DateTime(2026, 10, 25), Zone(2)));
    }

    [Fact]
    public void ProgressPercent_SameDay()
    {
        Assert.Equal(100, CalendarDate.ProgressPercent(Madrid(10), Madrid(10), new DateTime(2026, 10, 10), Zone(2)));
        Assert.Equal(0, CalendarDate.ProgressPercent(Madrid(10), Madrid(10), new DateTime(2026, 10, 9), Zone(2)));
    }
}
