using Cervantes.Web.Components.Account;

namespace Cervantes.Tests.Web.Components;

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

        Assert.Equal("u1", tickets.Redeem(ticket, "u1"));
        Assert.Null(tickets.Redeem(ticket, "u1"));
    }

    [Fact]
    public void Redeem_Unknown()
    {
        var tickets = new RefreshSignInTickets(new Clock());
        tickets.Issue("u1");

        Assert.Null(tickets.Redeem("not-a-ticket", "u1"));
    }

    [Fact]
    public void Redeem_Null()
    {
        Assert.Null(new RefreshSignInTickets(new Clock()).Redeem(null, "u1"));
    }

    [Fact]
    public void Redeem_AfterTwoMinutes()
    {
        var clock = new Clock();
        var tickets = new RefreshSignInTickets(clock);
        var ticket = tickets.Issue("u1");

        clock.Now = clock.Now.AddMinutes(2).AddSeconds(1);

        Assert.Null(tickets.Redeem(ticket, "u1"));
    }

    [Fact]
    public void Issue_TicketsDiffer()
    {
        var tickets = new RefreshSignInTickets(new Clock());
        var first = tickets.Issue("u1");
        var second = tickets.Issue("u1");

        Assert.NotEqual(first, second);
        Assert.Equal("u1", tickets.Redeem(first, "u1"));
        Assert.Equal("u1", tickets.Redeem(second, "u1"));
    }

    [Fact]
    public void Redeem_OtherRequester()
    {
        var tickets = new RefreshSignInTickets(new Clock());
        var ticket = tickets.Issue("u1");

        Assert.Null(tickets.Redeem(ticket, "u2"));
        Assert.Null(tickets.Redeem(ticket, "u1"));
    }

    [Fact]
    public void Redeem_NoRequester()
    {
        var tickets = new RefreshSignInTickets(new Clock());

        Assert.Null(tickets.Redeem(tickets.Issue("u1"), null));
    }
}
