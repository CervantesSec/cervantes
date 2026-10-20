using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace Cervantes.Web.Components.Account;

/// <summary>
/// One-time tickets that let the Profile page get a new sign-in cookie after the user's own security change (password,
/// authenticator). The change rotates the security stamp inside an interactive circuit, which cannot write a cookie, so
/// the page navigates to RefreshSignIn with a ticket. A stale cookie without a ticket is never renewed there.
/// </summary>
public sealed class RefreshSignInTickets(TimeProvider time)
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(2);
    private readonly ConcurrentDictionary<string, (string UserId, DateTimeOffset Expires)> tickets = new();

    public string Issue(string userId)
    {
        var ticket = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        tickets[ticket] = (userId, time.GetUtcNow() + Lifetime);
        return ticket;
    }

    /// <summary>The ticket's user id, once; null for an unknown, used or expired ticket.</summary>
    public string? Redeem(string? ticket)
    {
        if (string.IsNullOrEmpty(ticket) || !tickets.TryRemove(ticket, out var entry))
        {
            return null;
        }

        return entry.Expires > time.GetUtcNow() ? entry.UserId : null;
    }
}
