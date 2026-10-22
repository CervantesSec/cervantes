using Microsoft.AspNetCore.Http;

namespace Cervantes.Web.Helpers;

public static class AccountPages
{
    // Account pages read HttpContext and set cookies, so they render statically. A null check in Login is not enough:
    // a live circuit would run LoginUser, where no cookie can be set. Profile, built as an interactive page, keeps the
    // interactive router: as an island under a static router it renders before its user loads and crashes.
    private static readonly PathString[] InteractivePages = { new("/Account/Profile") };

    public static bool UsesStaticRender(PathString path)
    {
        return path.StartsWithSegments("/Account", StringComparison.OrdinalIgnoreCase)
               && !InteractivePages.Any(page => path.StartsWithSegments(page, StringComparison.OrdinalIgnoreCase));
    }
}
