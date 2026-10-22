using System.Xml;
using Cervantes.IFR.Parsers.Acunetix;
using Cervantes.IFR.Parsers.DependencyCheck;
using Cervantes.IFR.Parsers.OpenVAS;
using Cervantes.IFR.Parsers.Zap;

namespace Cervantes.Tests.IFR;

// Fixtures are handwritten in each tool's real report format (element names, attributes and nesting).
public class ParserXmlTests
{
    private static XmlDocument Fixture(string tool, string file) => ParserFixtures.Xml(tool, file);

    [Fact]
    public void HostOf_IgnoresHostnameChild()
    {
        var host = Fixture("OpenVAS", "openvas.xml").SelectSingleNode("//result/host")!;

        Assert.Equal("10.5.5.5", OpenVASParser.HostOf(host));
    }

    [Fact]
    public void InstancesOf_ReadsZap210Instances()
    {
        var alert = Fixture("Zap", "zap-2.14.xml").SelectSingleNode("//alertitem")!;

        var instances = ZapParser.InstancesOf(alert);

        Assert.Equal(2, instances.Count);
        Assert.Equal("https://app.example/login", instances[0].Uri);
        Assert.Equal("<form action=\"/login\" method=\"post\">", instances[0].Evidence);
        Assert.Equal("q", instances[1].Param);
    }

    [Fact]
    public void DependencyCheckXml_WithNamespace_ReadsDependencies()
    {
        var finding = Assert.Single(DependencyCheckParser.ReadXml(Fixture("DependencyCheck", "dependency-check.xml")));

        Assert.Equal("log4j-core-2.14.1.jar", finding.FileName);
        Assert.Equal("/app/lib/log4j-core-2.14.1.jar", finding.FilePath);
        Assert.Equal("CVE-2021-44228", finding.Name);
        Assert.Equal("CRITICAL", finding.Severity);
        Assert.Equal("10.0", finding.CvssScore);
        Assert.StartsWith("Apache Log4j2", finding.Description);
    }
}
