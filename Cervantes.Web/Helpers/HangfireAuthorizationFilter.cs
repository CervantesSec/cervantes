using Hangfire.Dashboard;

namespace Cervantes.Server.Helpers;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Ensure the user is authenticated.
        if (httpContext.User.Identity.IsAuthenticated)
        {
            // Check if the user is in the "Admin" role.
            if (httpContext.User.IsInRole("Admin"))
            {
                return true;
            }
        }

        return false;
    }
}