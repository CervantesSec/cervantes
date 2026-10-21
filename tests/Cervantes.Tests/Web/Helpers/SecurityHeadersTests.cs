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
    [InlineData("object-src 'self'")]
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
    public void Apply_SetsTheThreeHeaders()
    {
        var headers = new HeaderDictionary();

        SecurityHeaders.Apply(headers, "default-src 'self'");

        Assert.Equal("default-src 'self'", headers["Content-Security-Policy"]);
        Assert.Equal("nosniff", headers["X-Content-Type-Options"]);
        Assert.Equal("same-origin", headers["Referrer-Policy"]);
    }
}
