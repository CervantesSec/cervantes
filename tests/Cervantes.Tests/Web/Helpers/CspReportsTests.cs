using System.Text;
using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class CspReportsTests
{
    [Fact]
    public void Parse_ReadsAChromeReport()
    {
        var body = "{\"csp-report\":{\"document-uri\":\"http://127.0.0.1:5299/vulns?id=1\",\"violated-directive\":\"img-src\",\"effective-directive\":\"img-src\",\"blocked-uri\":\"https://evil.example/x.png\",\"source-file\":\"http://127.0.0.1:5299/_framework/blazor.web.js\",\"line-number\":1}}";

        var violation = CspReports.Parse(body);

        Assert.NotNull(violation);
        Assert.Equal("img-src", violation.Directive);
        Assert.Equal("https://evil.example/x.png", violation.BlockedUri);
        Assert.Equal("/vulns", violation.DocumentPath);
        Assert.Equal("http://127.0.0.1:5299/_framework/blazor.web.js", violation.SourceFile);
        Assert.Equal(1, violation.Line);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("{}")]
    [InlineData("{\"csp-report\":{}}")]
    public void Parse_ReturnsNullForMalformed(string body)
    {
        Assert.Null(CspReports.Parse(body));
    }

    [Fact]
    public void IsIgnored_MudExEval()
    {
        Assert.True(CspReports.IsIgnored(new CspReports.CspViolation("script-src", "eval", "/", null, null)));
        Assert.False(CspReports.IsIgnored(new CspReports.CspViolation("script-src-elem", "https://evil.example/a.js", "/", null, null)));
    }

    [Fact]
    public async Task ReadLimitedAsync_ReturnsNullOverTheLimit()
    {
        using var big = new MemoryStream(new byte[9 * 1024]);
        Assert.Null(await CspReports.ReadLimitedAsync(big, CspReports.MaxBodyBytes));

        using var small = new MemoryStream(Encoding.UTF8.GetBytes("hello"));
        Assert.Equal("hello", await CspReports.ReadLimitedAsync(small, CspReports.MaxBodyBytes));
    }
}
