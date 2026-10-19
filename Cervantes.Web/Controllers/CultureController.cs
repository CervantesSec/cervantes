using Cervantes.Web.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Cervantes.Web.Controllers;

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
[ApiExplorerSettings(IgnoreApi = true)]
[Route("[controller]/[action]")]
[Authorize]
public class CultureController : Controller
{
    public IActionResult Set(string culture, string redirectUri)
    {
        // LocalRedirect throws for an off-site target and RequestCulture accepts any name, so both are checked first.
        if (!Url.IsLocalUrl(redirectUri) || (culture != null && !SupportedCultures.Names.Contains(culture)))
        {
            return BadRequest();
        }

        if (culture != null)
        {
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture, culture)));
        }

        return LocalRedirect(redirectUri);
    }
}
