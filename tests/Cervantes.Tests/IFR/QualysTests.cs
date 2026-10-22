using System.Xml;
using Cervantes.CORE.Entities;
using Cervantes.IFR.Parsers.Qualys;

namespace Cervantes.Tests.IFR;

// Real Qualys reports (DefectDojo samples, see Fixtures/Parsers/NOTICE.md).
public class QualysTests
{
    private static XmlNode Vuln(string file, string number) =>
        ParserFixtures.Xml("Qualys", file).SelectSingleNode($"//*[@number='{number}']")!;

    private static XmlDocument Inline(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        return doc;
    }

    [Fact]
    public void HostOf_UsesIpAttributes()
    {
        Assert.Equal(("10.1.10.1", "host1.example.com"), QualysParser.HostOf(Vuln("qualys-scan-multiple.xml", "82024")));
    }

    [Fact]
    public void HostOf_NoRegisteredHostnameIsNull()
    {
        Assert.Equal(("10.1.10.1", (string?)null), QualysParser.HostOf(Vuln("qualys-scan.xml", "82024")));
    }

    [Fact]
    public void HostOf_Ipv6()
    {
        var doc = Inline("<SCAN><IPV6 value=\"2001:db8::1\"><VULNS><CAT><VULN number=\"1\" severity=\"3\"><TITLE>t</TITLE></VULN></CAT></VULNS></IPV6></SCAN>");

        Assert.Equal(("2001:db8::1", (string?)null), QualysParser.HostOf(doc.SelectSingleNode("//VULN")!));
    }

    [Fact]
    public void SeverityOf_FromAttribute()
    {
        Assert.Equal(VulnRisk.Low, QualysParser.SeverityOf(Vuln("qualys-scan-multiple.xml", "82024")));
        Assert.Equal(VulnRisk.Critical, QualysParser.SeverityOf(Vuln("qualys-scan-multiple.xml", "123168")));
    }

    [Fact]
    public void SeverityOf_UnknownIsInfo()
    {
        var doc = Inline("<VULN severity=\"9\"><TITLE>t</TITLE></VULN>");

        Assert.Equal(VulnRisk.Info, QualysParser.SeverityOf(doc.DocumentElement!));
    }

    [Fact]
    public void CvssOf_SplitsV2AndV3()
    {
        Assert.Equal((3.4, 10.0, (double?)null), QualysParser.CvssOf(Vuln("qualys-scan-multiple.xml", "123168")));
        Assert.Equal(((double?)null, 5.0, (double?)null), QualysParser.CvssOf(Vuln("qualys-scan.xml", "82024")));
    }

    [Fact]
    public void CvesOf_ReadsAttributeAndList()
    {
        Assert.Equal("CVE-2002-0510", QualysParser.CvesOf(Vuln("qualys-scan.xml", "82024")));
        var java = QualysParser.CvesOf(Vuln("qualys-scan-multiple.xml", "123168"));
        Assert.StartsWith("CVE-2014-6601, CVE-2015-0412, ", java);
        Assert.Equal(19, java.Split(", ").Length);
        Assert.Equal("", QualysParser.CvesOf(Vuln("qualys-scan-multiple.xml", "11")));
    }

    [Fact]
    public void FindingsOf_IncludesPotentialVulnerabilities()
    {
        var findings = QualysParser.FindingsOf(ParserFixtures.Xml("Qualys", "qualys-scan-multiple.xml"));

        Assert.Equal(8, findings.Count);
        Assert.Equal(2, findings.Count(QualysParser.IsPotential));
        Assert.DoesNotContain(findings, x => x.Name is "INFO" or "SERVICE");
    }

    [Fact]
    public void EnsureSupported_AssetDataThrows()
    {
        var e = Assert.Throws<InvalidDataException>(() => QualysParser.EnsureSupported(ParserFixtures.Xml("Qualys", "qualys-asset-data.xml")));
        Assert.Contains("ASSET_DATA_REPORT", e.Message);
    }

    [Fact]
    public void EnsureSupported_WasThrows()
    {
        Assert.Throws<InvalidDataException>(() => QualysParser.EnsureSupported(ParserFixtures.Xml("Qualys", "qualys-was.xml")));
    }

    [Fact]
    public void EnsureSupported_ScanPasses()
    {
        QualysParser.EnsureSupported(ParserFixtures.Xml("Qualys", "qualys-scan.xml"));
        QualysParser.EnsureSupported(ParserFixtures.Xml("Qualys", "qualys-scan-multiple.xml"));
    }
}
