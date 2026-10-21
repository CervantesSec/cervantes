namespace Cervantes.Web.Helpers;

/// <summary>
/// Security headers sent on every response: the Content-Security-Policy, X-Content-Type-Options and Referrer-Policy.
/// </summary>
public static class SecurityHeaders
{
    // Content-Security-Policy, enforced. Before changing it, try the new policy under the header name
    // "Content-Security-Policy-Report-Only" and walk the UI: the browser then logs violations and blocks nothing.
    // script-src has no 'unsafe-inline' on purpose: that is what stops an injected <script> or inline handler.
    // script-src has no 'unsafe-eval' either. MudBlazor.Extensions runs eval("typeof MudBlazorExtensions !== 'undefined'")
    // (through BlazorJS) on every ShowExAsync, inside a try/catch, so blocking it changes nothing (checked 2026-10-21).
    // These need 'unsafe-eval' or a new check before they are used: MudBlazor.Extensions components built on BlazorJS
    // DInvoke* (MudExHtmlEdit, MudExCodeView, MudExUploadEdit, ...), Blazored.LocalStorage KeysAsync/LengthAsync, and
    // Monaco's official AMD loader.js.
    // style-src 'unsafe-inline': MudBlazor and TinyMCE inject inline styles. The Google Fonts hosts serve Roboto (App.razor).
    // connect-src 'self' also covers the same-origin ws:/wss: Blazor Server SignalR circuit (CSP Level 3); no other host
    // gets a socket. blob: in img-src/connect-src is required by TinyMCE.
    // object-src 'none': attachment previews use <img>, <iframe> and <video>, never <embed>/<object>. The PDF <iframe> is
    // governed by frame-src, which falls back to default-src 'self' (the files come from FileController).
    // form-action does not fall back to default-src, so it is explicit: a stored <form action="https://evil"> cannot post offsite.
    // With OpenID Connect, the external login form is answered with a redirect to the provider, and browsers check
    // form-action on that redirect too, so the Authority's origin is allowed as well.
    // OpenIdConnect:FormActionOrigins lists provider origins other than the Authority that the login redirect passes
    // through (a hosted login page, federation hops); Chrome checks form-action on every redirect of the form's navigation.
    // reportUri, when set, is appended as report-uri (violation reports); it must be a trusted constant, it is not validated.
    public static string ContentSecurityPolicy(string? openIdAuthorityOrigin,
        IEnumerable<string>? extraFormActionOrigins = null, string? reportUri = null)
    {
        var sources = new List<string> { "'self'" };
        if (openIdAuthorityOrigin != null) sources.Add(openIdAuthorityOrigin);
        if (extraFormActionOrigins != null)
        {
            foreach (var origin in extraFormActionOrigins)
            {
                if (!sources.Contains(origin, StringComparer.OrdinalIgnoreCase)) sources.Add(origin);
            }
        }
        var policy = "default-src 'self'; " +
                     "script-src 'self'; " +
                     "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                     "img-src 'self' data: blob:; " +
                     "font-src 'self' data: https://fonts.gstatic.com; " +
                     "connect-src 'self' blob:; " +
                     "frame-ancestors 'self'; " +
                     $"form-action {string.Join(' ', sources)}; " +
                     "object-src 'none'; " +
                     "base-uri 'self'";
        return reportUri == null ? policy : policy + "; report-uri " + reportUri;
    }

    // Valid entry: absolute http(s) URI without '*' in the host; it is reduced to its origin. Anything else is returned in Invalid.
    public static (IReadOnlyList<string> Origins, IReadOnlyList<string> Invalid) ParseOrigins(IEnumerable<string?>? entries)
    {
        var origins = new List<string>();
        var invalid = new List<string>();
        foreach (var entry in entries ?? Array.Empty<string?>())
        {
            if (entry != null && Uri.TryCreate(entry, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                && uri.UserInfo.Length == 0 && !uri.Host.Contains('*') && !entry.Contains('*'))
            {
                // SchemeAndServer: scheme, host and non-default port, never userinfo or path.
                var origin = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped);
                if (origin.IndexOfAny(new[] { ';', ',', '\'', '"' }) >= 0 || origin.Any(char.IsWhiteSpace))
                {
                    invalid.Add(entry);
                    continue;
                }
                if (!origins.Contains(origin, StringComparer.OrdinalIgnoreCase)) origins.Add(origin);
            }
            else
            {
                invalid.Add(entry ?? "");
            }
        }
        return (origins, invalid);
    }

    // nosniff: every script and style the app serves has the right Content-Type (checked 2026-10-21).
    // same-origin: links to external sites do not learn the instance's origin, often an internal hostname.
    // sandbox: attachments that are not PDFs are served with "; sandbox", so even a file the browser would render
    // (or a user opens in its own tab) runs in an opaque origin with no scripts. FileController raises the flag in
    // HttpContext.Items[SandboxItem] instead of writing the header, because this runs last (OnStarting is LIFO).
    public const string SandboxItem = "Cervantes.CspSandbox";

    public static void Apply(IHeaderDictionary headers, string contentSecurityPolicy, bool sandbox = false)
    {
        headers["Content-Security-Policy"] = sandbox ? contentSecurityPolicy + "; sandbox" : contentSecurityPolicy;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["Referrer-Policy"] = "same-origin";
    }
}
