using Cervantes.Web.Helpers;

namespace Cervantes.Tests;

public class LocalUrlTests
{
    [Theory]
    [InlineData("/vulns")]
    [InlineData("/")]
    [InlineData("~/x")]
    public void IsLocal_AcceptsPathsOnThisSite(string url)
    {
        Assert.True(LocalUrl.IsLocal(url));
    }

    [Theory]
    [InlineData("//example.com")]
    [InlineData(@"/\example.com")]
    [InlineData("~//x")]
    [InlineData(@"~/\x")]
    [InlineData("https://example.com")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("vulns")]
    public void IsLocal_RejectsOtherSitesAndNonPaths(string? url)
    {
        Assert.False(LocalUrl.IsLocal(url));
    }

    [Theory]
    [InlineData("/\t/example.com")]
    [InlineData("~/\t/example.com")]
    [InlineData("/a\r\nb")]
    public void IsLocal_RejectsControlCharacters(string url)
    {
        // Browsers strip tabs and newlines from URLs, so "/\t/host" ends up as "//host".
        Assert.False(LocalUrl.IsLocal(url));
    }
}
