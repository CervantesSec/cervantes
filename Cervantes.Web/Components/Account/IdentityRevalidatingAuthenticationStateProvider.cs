using System.Security.Claims;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Cervantes.Web.Components.Account;

// This is a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
// while an interactive circuit is connected, on the same interval as the cookie.
internal sealed class IdentityRevalidatingAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory,
        IOptions<IdentityOptions> options,
        IOptions<SecurityStampValidatorOptions> stampOptions)
    : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    // The base class waits this long between checks in a loop, so a zero interval (allowed for the cookie) would poll
    // without pause; a minute is the floor.
    protected override TimeSpan RevalidationInterval =>
        stampOptions.Value.ValidationInterval < TimeSpan.FromMinutes(1) ? TimeSpan.FromMinutes(1) : stampOptions.Value.ValidationInterval;

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // Get the user manager from a new scope to ensure it fetches fresh data
        await using var scope = scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        return await ValidateSecurityStampAsync(userManager, authenticationState.User);
    }

    private async Task<bool> ValidateSecurityStampAsync(UserManager<ApplicationUser> userManager,
        ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user is null)
        {
            return false;
        }
        else if (!userManager.SupportsUserSecurityStamp)
        {
            return true;
        }
        else
        {
            var principalStamp = principal.FindFirstValue(options.Value.ClaimsIdentity.SecurityStampClaimType);
            var userStamp = await userManager.GetSecurityStampAsync(user);
            return principalStamp == userStamp;
        }
    }
}