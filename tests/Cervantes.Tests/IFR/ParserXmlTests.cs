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

        Assert.Equal("log4j-api-2.12.4.jar", finding.FileName);
        Assert.Equal("/home/user/vulnerabilities/apache-log4j-2.12.4-bin/log4j-api-2.12.4.jar", finding.FilePath);
        Assert.Equal("CVE-2020-9488", finding.Name);
        Assert.Equal("LOW", finding.Severity);
        Assert.Equal("3.7", finding.CvssScore);
        Assert.Equal("CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:L/I:N/A:N", finding.Vector);
        Assert.StartsWith("Improper validation of certificate", finding.Description);
    }

    [Fact]
    public void DependencyCheckXml_Schema13WithCvssScore()
    {
        var finding = Assert.Single(DependencyCheckParser.ReadXml(Fixture("DependencyCheck", "dependency-check-1.3.xml")));

        Assert.Equal("CVE-0000-0001", finding.Name);
        Assert.Equal("Moderate", finding.Severity);
        Assert.Equal("7.5", finding.CvssScore);
        Assert.Equal("", finding.Vector);
    }
}
