using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace Cervantes.Web.Components.Account;

internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    public const string StatusCookieName = "Identity.StatusMessage";

    private static readonly CookieBuilder StatusCookieBuilder = new()
    {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        IsEssential = true,
        MaxAge = TimeSpan.FromSeconds(5),
    };

    [DoesNotReturn]
    public void RedirectTo(string? uri)
    {
        // Prevent open redirects: only follow targets under the app base, otherwise go to the app root.
        // Uri.IsBaseOf compares parsed URIs, so a Host header in another case or with an explicit :443 still matches.
        // Do not "simplify" this to NavigationManager.ToBaseRelativePath: it compares ordinally against
        // BaseUri.OriginalString, so those same requests would fail it, and code that then falls back to the raw
        // target reopens the open redirect.
        // Known limits: the path part of the comparison is case-sensitive, so an app behind a PathBase reached with
        // a different path case would send users to the root; and Unicode (IDN) hosts are only compared in the form
        // the browser sends, which is punycode. Neither matters today: Program.cs does not call UsePathBase, and
        // browsers always send punycode hosts.
        var baseUri = new Uri(navigationManager.BaseUri);
        uri = Uri.TryCreate(baseUri, uri ?? "", out var target) && baseUri.IsBaseOf(target) ? target.AbsoluteUri : "";

        // During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
        // So as long as this is called from a statically rendered Identity component, the InvalidOperationException is never thrown.
        navigationManager.NavigateTo(uri);
        throw new InvalidOperationException(
            $"{nameof(IdentityRedirectManager)} can only be used during static rendering.");
    }

    [DoesNotReturn]
    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);
    }

    [DoesNotReturn]
    public void RedirectToWithStatus(string uri, string message, HttpContext context)
    {
        context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));
        RedirectTo(uri);
    }

    private string CurrentPath => navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);

    [DoesNotReturn]
    public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

    [DoesNotReturn]
    public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
        => RedirectToWithStatus(CurrentPath, message, context);
}