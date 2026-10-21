using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace Cervantes.Web.Components.Account;

/// <summary>
/// One-time tickets that let the Profile page get a new sign-in cookie after the user's own security change (password,
/// authenticator). The change rotates the security stamp inside an interactive circuit, which cannot write a cookie, so
/// the page navigates to RefreshSignIn with a ticket. A stale cookie without a ticket is never renewed there.
/// A ticket only works for the user it was issued to, as read from the browser's own sign-in cookie, so a ticket handed
/// to another browser cannot sign that browser in.
/// </summary>
public sealed class RefreshSignInTickets(TimeProvider time)
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(2);
    private readonly ConcurrentDictionary<string, (string UserId, DateTimeOffset Expires)> tickets = new();

    public int Count => tickets.Count;

    public string Issue(string userId)
    {
        var now = time.GetUtcNow();
        foreach (var stored in tickets)
        {
            if (stored.Value.Expires <= now)
            {
                tickets.TryRemove(stored.Key, out _);
            }
        }

        var ticket = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        tickets[ticket] = (userId, now + Lifetime);
        return ticket;
    }

    /// <summary>
    /// The ticket's user id, once, when it was issued to <paramref name="requesterUserId"/>; null for an unknown, used,
    /// expired or someone else's ticket. A ticket is used up by any attempt.
    /// </summary>
    public string? Redeem(string? ticket, string? requesterUserId)
    {
        if (string.IsNullOrEmpty(ticket) || !tickets.TryRemove(ticket, out var entry))
        {
            return null;
        }

        return entry.Expires > time.GetUtcNow() && entry.UserId == requesterUserId ? entry.UserId : null;
    }
}
