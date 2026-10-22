using System.Xml;
using Cervantes.IFR.Parsers.OpenVAS;

namespace Cervantes.Tests.IFR;

// Real GVM reports, GMP 22.6 and GMP 9.0 (DefectDojo samples, see Fixtures/Parsers/NOTICE.md).
public class OpenVASTests
{
    private static XmlNode Result(string file) =>
        ParserFixtures.Xml("OpenVAS", file).SelectSingleNode("//results/result")!;

    [Fact]
    public void HostOf_RealReport()
    {
        Assert.Equal("10.99.99.99", OpenVASParser.HostOf(Result("openvas.xml").SelectSingleNode("host")!));
        Assert.Equal("10.0.101.2", OpenVASParser.HostOf(Result("openvas-gmp9.xml").SelectSingleNode("host")!));
    }

    [Fact]
    public void SolutionOf_Gmp22NvtSolution()
    {
        Assert.StartsWith("The vendor has released updates. Please see", OpenVASParser.SolutionOf(Result("openvas.xml")));
    }

    [Fact]
    public void SolutionOf_Gmp9Tags()
    {
        var solution = OpenVASParser.SolutionOf(Result("openvas-gmp9.xml"));

        Assert.StartsWith("Upgrade to version 117 or later,", solution);
        Assert.DoesNotContain("|", solution);
    }

    [Fact]
    public void CvesOf_FromRefs()
    {
        var gmp22 = OpenVASParser.CvesOf(Result("openvas.xml"));
        var gmp9 = OpenVASParser.CvesOf(Result("openvas-gmp9.xml"));

        Assert.StartsWith("CVE-2025-49659, CVE-2025-48823, ", gmp22);
        Assert.Equal(93, gmp22.Split(", ").Length);
        Assert.StartsWith("CVE-2023-4573, ", gmp9);
        Assert.Equal(12, gmp9.Split(", ").Length);
    }
}
