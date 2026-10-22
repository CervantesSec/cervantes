using System.Xml;
using Cervantes.IFR.Parsers.Nessus;

namespace Cervantes.Tests.IFR;

// Real .nessus v2 report (DefectDojo sample, see Fixtures/Parsers/NOTICE.md).
public class NessusTests
{
    private static XmlNodeList Items() =>
        ParserFixtures.Xml("Nessus", "nessus-cvssv3.nessus").SelectNodes("//ReportItem")!;

    [Fact]
    public void ReadItem_ReadsItsOwnFields()
    {
        var item = NessusParser.ReadItem(Items()[0]!);

        Assert.Equal("Browsable Web Directories", item.PluginName);
        Assert.Equal("Medium", item.RiskFactor);
        Assert.Equal(5.3, item.Cvss3);
        Assert.Equal("CVSS:3.0/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:N", item.Cvss3Vector);
        Assert.Equal("http://www.nessus.org/u?0a35179e", item.SeeAlso.Trim());
    }

    [Fact]
    public void ReadItem_DoesNotInheritCvss()
    {
        var items = Items();
        NessusParser.ReadItem(items[0]!);

        var device = NessusParser.ReadItem(items[1]!);

        Assert.Equal("Device Type", device.PluginName);
        Assert.Null(device.Cvss3);
        Assert.Equal("", device.Cvss3Vector);
        Assert.Equal("", device.SeeAlso);
        Assert.Equal("", device.Cves);
    }

    [Fact]
    public void ReadItem_JoinsAllCves()
    {
        var php = NessusParser.ReadItem(Items()[3]!);

        Assert.Equal("CVE-2007-1001, CVE-2007-1583, CVE-2007-1649, CVE-2007-1717, CVE-2007-1718", php.Cves);
        Assert.Equal(7.5, php.Cvss3);
    }

    [Fact]
    public void ReadItem_ReadsPortAttributes()
    {
        var item = NessusParser.ReadItem(Items()[0]!);

        Assert.Equal("80", item.Port);
        Assert.Equal("tcp", item.Protocol);
        Assert.Equal("www", item.Service);
    }

    [Fact]
    public void HostTag_ReadsHostIpAndOs()
    {
        var host = ParserFixtures.Xml("Nessus", "nessus-cvssv3.nessus").SelectSingleNode("//ReportHost")!;

        Assert.Equal("44.228.249.3", NessusParser.HostTag(host, "host-ip"));
        Assert.Equal("Linux Kernel 2.6", NessusParser.HostTag(host, "operating-system"));
        Assert.Equal("", NessusParser.HostTag(host, "no-such-tag"));
    }
}
