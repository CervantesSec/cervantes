using Cervantes.IFR.Parsers.Pwndoc;

namespace Cervantes.Tests.IFR;

// Fixture written with the structure of real Pwndoc vulnerability exports (see Fixtures/Parsers/NOTICE.md).
public class PwndocTests
{
    private static List<PwndocVuln> ReadFixture() =>
        PwndocParser.Read(new StringReader(ParserFixtures.Text("Pwndoc", "vulnerabilities.yml")));

    [Fact]
    public void Read_RealShape()
    {
        var vulns = ReadFixture();

        Assert.Equal(3, vulns.Count);
        var xss = vulns[0];
        Assert.Equal("CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:C/C:L/I:L/A:N", xss.Cvssv3);
        Assert.Equal(3, xss.Priority);
        Assert.Equal(2, xss.RemediationComplexity);
        Assert.Equal("Web Application", xss.Category);
        Assert.Equal(2, xss.Details.Count);
        Assert.Equal("Stored Cross-Site Scripting", xss.Details[0].Title);
        Assert.Equal(2, xss.Details[0].References.Count);
        Assert.Equal("FR", xss.Details[1].Locale);
    }

    [Fact]
    public void Read_Cvssv4DoesNotThrow()
    {
        Assert.Equal("CVSS:4.0/AV:N/AC:L/AT:N/PR:N/UI:N/VC:H/VI:N/VA:N/SC:N/SI:N/SA:N", ReadFixture()[1].Cvssv4);
    }

    [Fact]
    public void Read_CustomFieldObjects()
    {
        var fields = ReadFixture()[1].Details[0].CustomFields;

        Assert.Equal(2, fields.Count);
        Assert.Equal("64f1c2a9e4b0a1b2c3d4e5f6", fields[0].CustomField);
        Assert.Equal("Production", fields[0].Text);
    }

    [Fact]
    public void Read_MissingObservationAndPriority()
    {
        var banner = ReadFixture()[2];

        Assert.Null(banner.Priority);
        Assert.Null(banner.Details[0].Observation);
    }

    [Fact]
    public void Read_UnknownKeysAreIgnored()
    {
        var vulns = PwndocParser.Read(new StringReader("- cvssv3: ''\n  status: 0\n  details:\n    - locale: EN\n      title: t\n"));

        Assert.Equal("t", Assert.Single(vulns).Details[0].Title);
    }
}
