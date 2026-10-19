namespace Cervantes.Web.Helpers;

public static class LocalUrl
{
    /// <summary>
    /// The same rule as ASP.NET Core's IsLocalUrl, for places without an IUrlHelper (minimal API endpoints): a path
    /// that starts with "/" or "~/" and is not protocol-relative ("//host", "/\host"). LocalRedirect throws for
    /// anything else, so redirect targets are checked with this first. Like IsLocalUrl, it also rejects control
    /// characters, which LocalRedirect would refuse too.
    /// </summary>
    public static bool IsLocal(string? url)
    {
        if (string.IsNullOrEmpty(url) || url.Any(char.IsControl))
        {
            return false;
        }

        if (url[0] == '/')
        {
            return url.Length == 1 || (url[1] != '/' && url[1] != '\\');
        }

        if (url.Length > 1 && url[0] == '~' && url[1] == '/')
        {
            return url.Length == 2 || (url[2] != '/' && url[2] != '\\');
        }

        return false;
    }
}
