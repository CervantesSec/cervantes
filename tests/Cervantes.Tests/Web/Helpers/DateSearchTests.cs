using System.Globalization;
using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class DateSearchTests
{
    private static TimeZoneInfo Zone(int hours)
        => TimeZoneInfo.CreateCustomTimeZone($"qa{hours}", TimeSpan.FromHours(hours), $"qa{hours}", $"qa{hours}");

    private static void UnderEsEs(Action action)
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("es-ES");
            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Matches_EmptySearchIsFalse(string? search)
    {
        Assert.False(DateSearch.Matches(new DateTime(2026, 10, 21), "dd/MM/yyyy", search));
    }

    [Fact]
    public void Matches_FormattedSubstring()
    {
        UnderEsEs(() =>
        {
            var value = new DateTime(2026, 10, 21);
            Assert.True(DateSearch.Matches(value, "dd/MM/yyyy", "21/10"));
            Assert.True(DateSearch.Matches(value, "dd/MM/yyyy", "2026"));
            Assert.False(DateSearch.Matches(value, "dd/MM/yyyy", "10/21"));
        });
    }

    [Fact]
    public void MatchesCalendarDate_UsesTheCalendarDay()
    {
        UnderEsEs(() =>
        {
            var stored = new DateTime(2026, 10, 20, 22, 0, 0, DateTimeKind.Utc);
            Assert.True(DateSearch.MatchesCalendarDate(stored, "dd/MM/yyyy", "21/10/2026", Zone(2)));
            Assert.False(DateSearch.MatchesCalendarDate(stored, "dd/MM/yyyy", "20/10/2026", Zone(2)));
        });
    }
}
