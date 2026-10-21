using Cervantes.Web.Helpers;
using Microsoft.AspNetCore.Http;

namespace Cervantes.Tests.Web.Helpers;

public class SecurityHeadersTests
{
    [Fact]
    public void Policy_HasNoUnsafeEval()
    {
        var policy = SecurityHeaders.ContentSecurityPolicy(null);

        Assert.DoesNotContain("unsafe-eval", policy);
        Assert.Contains("script-src 'self';", policy);
    }

    [Theory]
    [InlineData("default-src 'self'")]
    [InlineData("style-src 'self' 'unsafe-inline' https://fonts.googleapis.com")]
    [InlineData("img-src 'self' data: blob:")]
    [InlineData("font-src 'self' data: https://fonts.gstatic.com")]
    [InlineData("connect-src 'self' blob:")]
    [InlineData("frame-ancestors 'self'")]
    [InlineData("object-src 'none'")]
    [InlineData("base-uri 'self'")]
    public void Policy_KeepsTheOtherDirectives(string directive)
    {
        Assert.Contains(directive, SecurityHeaders.ContentSecurityPolicy(null));
    }

    [Fact]
    public void Policy_FormAction()
    {
        Assert.Contains("form-action 'self';", SecurityHeaders.ContentSecurityPolicy(null));
        Assert.Contains("form-action 'self' https://idp.example;", SecurityHeaders.ContentSecurityPolicy("https://idp.example"));
    }

    [Fact]
    public void ParseOrigins_ReducesToTheOrigin()
    {
        var (origins, invalid) = SecurityHeaders.ParseOrigins(new[] { "https://login.example/oauth2/authorize", "http://127.0.0.1:5556/dex" });

        Assert.Equal(new[] { "https://login.example", "http://127.0.0.1:5556" }, origins);
        Assert.Empty(invalid);
    }

    [Theory]
    [InlineData("/dex")]
    [InlineData("https://*.example")]
    [InlineData("ftp://x.example")]
    [InlineData("not a url")]
    [InlineData("https://a;b@x.example")]
    [InlineData("https://user@x.example")]
    [InlineData("")]
    public void ParseOrigins_RejectsInvalid(string entry)
    {
        var (origins, invalid) = SecurityHeaders.ParseOrigins(new[] { entry });

        Assert.Empty(origins);
        Assert.Equal(new[] { entry }, invalid);
    }

    [Fact]
    public void ParseOrigins_KeepsNonDefaultPort()
    {
        var (origins, _) = SecurityHeaders.ParseOrigins(new[] { "http://127.0.0.1:5556/dex", "https://x.example:443/" });

        Assert.Equal(new[] { "http://127.0.0.1:5556", "https://x.example" }, origins);
    }

    [Fact]
    public void ParseOrigins_DropsDuplicates()
    {
        var (origins, _) = SecurityHeaders.ParseOrigins(new[] { "https://a.example/x", "HTTPS://A.example/y" });

        Assert.Single(origins);
    }

    [Fact]
    public void Policy_AddsExtraFormActionOrigins()
    {
        var policy = SecurityHeaders.ContentSecurityPolicy("https://idp.example",
            new[] { "https://login.example", "https://idp.example" });

        Assert.Contains("form-action 'self' https://idp.example https://login.example;", policy);
    }

    [Fact]
    public void Policy_AddsReportUri()
    {
        Assert.EndsWith("report-uri /csp-report", SecurityHeaders.ContentSecurityPolicy(null, reportUri: "/csp-report"));
    }

    [Fact]
    public void Policy_HasNoReportUriByDefault()
    {
        Assert.DoesNotContain("report-uri", SecurityHeaders.ContentSecurityPolicy(null));
    }

    [Fact]
    public void Apply_SetsTheThreeHeaders()
    {
        var headers = new HeaderDictionary();

        SecurityHeaders.Apply(headers, "default-src 'self'");

        Assert.Equal("default-src 'self'", headers["Content-Security-Policy"]);
        Assert.Equal("nosniff", headers["X-Content-Type-Options"]);
        Assert.Equal("same-origin", headers["Referrer-Policy"]);
    }

    [Fact]
    public void Apply_WithSandbox_AppendsIt()
    {
        var headers = new HeaderDictionary();

        SecurityHeaders.Apply(headers, "default-src 'self'", sandbox: true);

        Assert.Equal("default-src 'self'; sandbox", headers["Content-Security-Policy"]);
    }

    [Fact]
    public void Apply_WithoutSandbox_KeepsThePolicy()
    {
        var headers = new HeaderDictionary();

        SecurityHeaders.Apply(headers, "default-src 'self'");

        Assert.Equal("default-src 'self'", headers["Content-Security-Policy"]);
    }
}
