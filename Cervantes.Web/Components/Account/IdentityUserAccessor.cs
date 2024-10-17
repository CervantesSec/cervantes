using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Identity;

namespace Cervantes.Web.Components.Account;

internal sealed class IdentityUserAccessor(UserManager<ApplicationUser> userManager,
    IdentityRedirectManager redirectManager)
{
    public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser",
                $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }

    public async Task<ApplicationUser> FindByIdAsync(string userId) {
        var user = await userManager.FindByIdAsync(userId);
        return user;
    }

    public async Task<ApplicationUser> FindByNameAsync(string username) {
        var user = await userManager.FindByNameAsync(username);
        return user;
    }

    public async Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo login) {
        var result = await userManager.AddLoginAsync(user, login);
        return result;
    }
}