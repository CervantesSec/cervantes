using Microsoft.AspNetCore.Routing;

namespace Cervantes.Tests;

public class LdapReturnUrlTests
{
    [Theory]
    [InlineData("/Account/ExternalLogin?ReturnUrl=%2Fvulns&Action=LoginCallback", "/vulns")]
    [InlineData("/projects", "/projects")]
    [InlineData(null, "~/")]
    [InlineData("", "~/")]
    [InlineData("/Account/ExternalLogin?Action=LoginCallback", "~/")]
    public void UnwrapLdapReturnUrl_ReturnsThePageToOpen(string? returnUrl, string expected)
    {
        Assert.Equal(expected, IdentityComponentsEndpointRouteBuilderExtensions.UnwrapLdapReturnUrl(returnUrl));
    }

    [Fact]
    public void UnwrapLdapReturnUrl_LeavesOffSiteUrlsForTheCallerToReject()
    {
        Assert.Equal("//example.com", IdentityComponentsEndpointRouteBuilderExtensions.UnwrapLdapReturnUrl("//example.com"));
    }
}
