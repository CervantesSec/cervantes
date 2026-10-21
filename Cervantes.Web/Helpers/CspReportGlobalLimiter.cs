using System.Threading.RateLimiting;

namespace Cervantes.Web.Helpers;

/// <summary>Overall cap on CSP reports per minute, shared by all clients (the per-client limit is a separate policy).</summary>
public sealed class CspReportGlobalLimiter
{
    private readonly FixedWindowRateLimiter limiter;

    /// <param name="permitPerMinute">Per-client limit; the overall cap is ten times it (clamped so it cannot overflow).</param>
    public CspReportGlobalLimiter(int permitPerMinute)
    {
        var perClient = Math.Clamp(permitPerMinute, 1, int.MaxValue / 10);
        limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            PermitLimit = perClient * 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    }

    public RateLimitLease AttemptAcquire() => limiter.AttemptAcquire();
}
