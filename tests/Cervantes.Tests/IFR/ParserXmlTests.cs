using System.Xml;
using Cervantes.IFR.Parsers.Acunetix;
using Cervantes.IFR.Parsers.DependencyCheck;
using Cervantes.IFR.Parsers.OpenVAS;
using Cervantes.IFR.Parsers.Zap;

namespace Cervantes.Tests.IFR;

// Real reports and tool output (see Fixtures/Parsers/NOTICE.md); inline XML only for elements the samples leave empty.
public class ParserXmlTests
{
    private static XmlDocument Fixture(string tool, string file) => ParserFixtures.Xml(tool, file);

    [Fact]
    public void InstancesOf_ReadsZap210Instances()
    {
        var alerts = Fixture("Zap", "zap-baseline.xml").SelectNodes("//alertitem")!;

        var csrf = ZapParser.InstancesOf(alerts[0]!);
        var server = ZapParser.InstancesOf(alerts[1]!);
        var login = Assert.Single(ZapParser.InstancesOf(alerts[2]!));

        var form = Assert.Single(csrf);
        Assert.Equal("http://rs-target/", form.Uri);
        Assert.Equal("GET", form.Method);
        Assert.Equal("<form method=\"post\" action=\"/login.html\">", form.Evidence);
        Assert.Null(form.Param);
        Assert.Null(form.Attack);
        Assert.Equal(5, server.Count);
        Assert.Equal("POST", server[4].Method);
        Assert.Equal("user", login.Param);
        Assert.Equal("userParam=user\nuserValue=ZAP\npasswordParam=pass\nreferer=http://rs-target/", login.OtherInfo?.Replace("\r\n", "\n").Trim());
    }

    [Fact]
    public void InstancesOf_ReadsAttack()
    {
        // Active scans fill the instance's <attack>; the baseline (passive) report leaves it empty
        var doc = new XmlDocument();
        doc.LoadXml("<alertitem><instances><instance><uri>http://rs-target/search?q=1</uri><method>GET</method><param>q</param>" +
                    "<attack>&lt;script&gt;alert(1)&lt;/script&gt;</attack><evidence>&lt;script&gt;alert(1)&lt;/script&gt;</evidence>" +
                    "<otherinfo></otherinfo></instance></instances></alertitem>");

        var instance = Assert.Single(ZapParser.InstancesOf(doc.DocumentElement!));

        Assert.Equal("<script>alert(1)</script>", instance.Attack);
    }

    [Fact]
    public void InstancesOf_LegacyAlertLevelFields()
    {
        var doc = new XmlDocument();
        doc.LoadXml("<alertitem><uri>http://old.example/a</uri><param>id</param><attack>1'</attack><evidence>SQL error</evidence></alertitem>");

        var instance = Assert.Single(ZapParser.InstancesOf(doc.DocumentElement!));

        Assert.Equal(("http://old.example/a", "id", "1'", "SQL error"), (instance.Uri, instance.Param, instance.Attack, instance.Evidence));
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
