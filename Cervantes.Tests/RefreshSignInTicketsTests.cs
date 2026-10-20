using Cervantes.Web.Components.Account;

namespace Cervantes.Tests;

public class RefreshSignInTicketsTests
{
    private sealed class Clock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = new(2026, 10, 20, 12, 0, 0, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => Now;
    }

    [Fact]
    public void Redeem_ReturnsUserOnce()
    {
        var tickets = new RefreshSignInTickets(new Clock());
        var ticket = tickets.Issue("u1");

        Assert.Equal("u1", tickets.Redeem(ticket));
        Assert.Null(tickets.Redeem(ticket));
    }

    [Fact]
    public void Redeem_Unknown()
    {
        var tickets = new RefreshSignInTickets(new Clock());
        tickets.Issue("u1");

        Assert.Null(tickets.Redeem("not-a-ticket"));
    }

    [Fact]
    public void Redeem_Null()
    {
        Assert.Null(new RefreshSignInTickets(new Clock()).Redeem(null));
    }

    [Fact]
    public void Redeem_AfterTwoMinutes()
    {
        var clock = new Clock();
        var tickets = new RefreshSignInTickets(clock);
        var ticket = tickets.Issue("u1");

        clock.Now = clock.Now.AddMinutes(2).AddSeconds(1);

        Assert.Null(tickets.Redeem(ticket));
    }

    [Fact]
    public void Issue_TicketsDiffer()
    {
        var tickets = new RefreshSignInTickets(new Clock());
        var first = tickets.Issue("u1");
        var second = tickets.Issue("u1");

        Assert.NotEqual(first, second);
        Assert.Equal("u1", tickets.Redeem(first));
        Assert.Equal("u1", tickets.Redeem(second));
    }
}
