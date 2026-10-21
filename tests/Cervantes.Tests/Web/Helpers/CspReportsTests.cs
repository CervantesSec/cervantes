using System.Net;
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

    private static string Report(string fields) => "{\"csp-report\":{" + fields + "}}";

    [Fact]
    public void Parse_ReturnsNullForLoneSurrogate()
    {
        Assert.Null(CspReports.Parse(Report("\"effective-directive\":\"img-src\",\"blocked-uri\":\"\\ud800\"")));
    }

    [Fact]
    public void Parse_CutsLongFieldsTo500()
    {
        var violation = CspReports.Parse(Report("\"effective-directive\":\"img-src\",\"blocked-uri\":\"" + new string('a', 900) + "\""));
        Assert.NotNull(violation);
        Assert.Equal(500, violation.BlockedUri.Length);
    }

    [Fact]
    public void Parse_DoesNotSplitASurrogatePairWhenCutting()
    {
        var violation = CspReports.Parse(Report("\"effective-directive\":\"img-src\",\"blocked-uri\":\"" + new string('a', 499) + "\\ud83d\\ude00\""));
        Assert.NotNull(violation);
        Assert.Equal(499, violation.BlockedUri.Length);
    }

    [Fact]
    public void Parse_UsesViolatedDirectiveFirstToken()
    {
        var violation = CspReports.Parse(Report("\"violated-directive\":\"script-src 'self'\",\"blocked-uri\":\"x\""));
        Assert.NotNull(violation);
        Assert.Equal("script-src", violation.Directive);
    }

    [Fact]
    public void Parse_RelativeDocumentUriGivesEmptyPath()
    {
        var violation = CspReports.Parse(Report("\"effective-directive\":\"img-src\",\"document-uri\":\"/vulns?id=1\",\"blocked-uri\":\"x\""));
        Assert.NotNull(violation);
        Assert.Equal("", violation.DocumentPath);
    }

    [Fact]
    public void Parse_ReplacesControlCharacters()
    {
        var violation = CspReports.Parse(Report("\"effective-directive\":\"img-src\",\"blocked-uri\":\"a\\nb\\u001bc\""));
        Assert.NotNull(violation);
        Assert.Equal("a?b?c", violation.BlockedUri);
    }

    [Fact]
    public void PartitionKey_Ipv4()
    {
        Assert.Equal("203.0.113.7", CspReports.PartitionKey(IPAddress.Parse("203.0.113.7")));
    }

    [Fact]
    public void PartitionKey_Ipv6UsesSlash64()
    {
        var key = CspReports.PartitionKey(IPAddress.Parse("2001:db8:1:2::1"));
        Assert.Equal(key, CspReports.PartitionKey(IPAddress.Parse("2001:db8:1:2:ffff:ffff:ffff:ffff")));
        Assert.NotEqual(key, CspReports.PartitionKey(IPAddress.Parse("2001:db8:1:3::1")));
    }

    [Fact]
    public void PartitionKey_MappedIpv4()
    {
        Assert.Equal("203.0.113.7", CspReports.PartitionKey(IPAddress.Parse("::ffff:203.0.113.7")));
    }

    [Fact]
    public void PartitionKey_Null()
    {
        Assert.Equal("unknown", CspReports.PartitionKey(null));
    }
}
