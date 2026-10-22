using System.Xml;
using Cervantes.IFR.Parsers.Acunetix;

namespace Cervantes.Tests.IFR;

// Real Acunetix XML exports (DefectDojo samples, see Fixtures/Parsers/NOTICE.md).
public class AcunetixTests
{
    private static XmlNode Item(string file, string id) =>
        ParserFixtures.Xml("Acunetix", file).SelectSingleNode($"//ReportItem[@id='{id}']")!;

    private static XmlDocument Inline(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        return doc;
    }

    [Fact]
    public void TargetOf_UsesCrawlerStartUrl()
    {
        Assert.Equal("example.co.id", AcunetixParser.TargetOf(Item("acunetix.xml", "1")));
        Assert.Equal("www.itsecgames.com", AcunetixParser.TargetOf(Item("acunetix-many.xml", "1")));
    }

    [Fact]
    public void TargetOf_StartUrlWithoutScheme()
    {
        var doc = Inline("<ScanGroup><Scan><StartURL><![CDATA[www.itsecgames.com:8080]]></StartURL><ReportItems>" +
                         "<ReportItem id=\"0\"><Affects>/sitemap.xml</Affects></ReportItem></ReportItems></Scan></ScanGroup>");

        Assert.Equal("www.itsecgames.com", AcunetixParser.TargetOf(doc.SelectSingleNode("//ReportItem")!));
    }

    [Fact]
    public void TargetOf_PerScan()
    {
        var doc = Inline("<ScanGroup>" +
                         "<Scan><StartURL>http://one.example/</StartURL><ReportItems><ReportItem id=\"0\"><Affects>/a</Affects></ReportItem></ReportItems></Scan>" +
                         "<Scan><StartURL>http://two.example/</StartURL><ReportItems><ReportItem id=\"0\"><Affects>/b</Affects></ReportItem></ReportItems></Scan>" +
                         "</ScanGroup>");

        var items = doc.SelectNodes("//ReportItem")!;
        Assert.Equal("one.example", AcunetixParser.TargetOf(items[0]!));
        Assert.Equal("two.example", AcunetixParser.TargetOf(items[1]!));
    }

    [Fact]
    public void TargetOf_AbsoluteAffectsWithoutScan()
    {
        var doc = Inline("<Vulnerabilities><Vulnerability><Affects>http://legacy.example/x</Affects></Vulnerability></Vulnerabilities>");

        Assert.Equal("legacy.example", AcunetixParser.TargetOf(doc.SelectSingleNode("//Vulnerability")!));
    }

    [Fact]
    public void CvssOf_ReadsCvss3()
    {
        Assert.Equal((4.3, "CVSS:3.0/AV:N/AC:L/PR:N/UI:R/S:U/C:N/I:L/A:N"), AcunetixParser.CvssOf(Item("acunetix.xml", "1")));
    }

    [Fact]
    public void CvssOf_Version2OnlyIsNull()
    {
        Assert.Equal(((double?)null, (string?)null), AcunetixParser.CvssOf(Item("acunetix.xml", "6")));
    }

    [Fact]
    public void RequestCweAndReferences()
    {
        var item = Item("acunetix.xml", "1");

        Assert.StartsWith("GET /h/search HTTP/1.1", AcunetixParser.RequestOf(item));
        Assert.Equal("CWE-352", AcunetixParser.CwesOf(item));
        Assert.Contains("https://www.acunetix.com/websitesecurity/csrf-attacks/", AcunetixParser.ReferencesOf(item));
    }

    [Fact]
    public void LoadReport_Acunetix360JsonThrows()
    {
        Assert.Throws<XmlException>(() => AcunetixParser.LoadReport(ParserFixtures.Path("Acunetix", "acunetix360.json")));
    }
}
