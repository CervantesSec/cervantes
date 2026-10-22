using System.Text.Json;
using Cervantes.CORE.Entities;
using Cervantes.IFR.Parsers.Bandit;
using Cervantes.IFR.Parsers.DependencyCheck;
using Cervantes.IFR.Parsers.Masscan;
using Cervantes.IFR.Parsers.Nikto;
using Cervantes.IFR.Parsers.Nuclei;
using Cervantes.IFR.Parsers.Trivy;

namespace Cervantes.Tests.IFR;

// Fixtures are handwritten in each tool's real output format (field names, nesting and types).
public class ParserJsonTests
{
    private static string Fixture(string tool, string file) => ParserFixtures.Text(tool, file);

    [Fact]
    public void Trivy_ReadsRealOutput()
    {
        var report = Assert.Single(TrivyParser.Read(Fixture("Trivy", "trivy-image.json")));

        Assert.Equal("httpd:2.4.49", report.ArtifactName);
        Assert.Equal(3, report.Results[0].Vulnerabilities.Length);
        var vuln = report.Results[0].Vulnerabilities[0];
        Assert.Equal("CVE-2021-37600", vuln.VulnerabilityID);
        Assert.Equal("bsdutils", vuln.PkgName);
        Assert.Equal("MEDIUM", vuln.Severity);
    }

    [Fact]
    public void Trivy_CvssOfUsesSeveritySource()
    {
        var report = Assert.Single(TrivyParser.Read(Fixture("Trivy", "trivy-image.json")));
        var openssl = report.Results[0].Vulnerabilities.Single(x => x.VulnerabilityID == "CVE-2022-2097");

        var cvss = TrivyParser.CvssOf(openssl);

        Assert.Equal(5.3, cvss!.V3Score);
        Assert.Equal("CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:N", cvss.V3Vector);
    }

    [Fact]
    public void Trivy_CvssOfWithoutSourceOrNvdTakesFirstScored()
    {
        var report = Assert.Single(TrivyParser.Read(Fixture("Trivy", "trivy-image.json")));
        var util = report.Results[0].Vulnerabilities.Single(x => x.VulnerabilityID == "CVE-2024-28085");

        Assert.Same(util.CVSS.Values.First(x => x.V3Score != null), TrivyParser.CvssOf(util));
    }

    [Fact]
    public void Trivy_ReadsLegacyStringSchemaVersion()
    {
        var report = Assert.Single(TrivyParser.Read("{\"SchemaVersion\": \"2\", \"Results\": []}"));

        Assert.Empty(report.Results);
    }

    [Fact]
    public void Nuclei_ReadsRealOutput()
    {
        var results = NucleiParser.Read(Fixture("Nuclei", "nuclei.jsonl"));

        Assert.Equal(2, results.Count);
        var rce = results[0];
        Assert.Equal("CVE-2026-0002", rce.TemplateId);
        Assert.Equal("Example App - Remote Code Execution", rce.Info.Name);
        Assert.Equal("critical", rce.Info.Severity);
        Assert.Equal("https://app.example/api/run", rce.MatchedAt);
        Assert.Equal("https://app.example", rce.Host);
        Assert.Equal("9.8", rce.Info.Classification.CvssScore);
        Assert.Equal("CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H", rce.Info.Classification.CvssVector);
        Assert.Equal("uid=0(root)", rce.ExtractedResults[0]);
    }

    [Fact]
    public void Nuclei_ReadsLegacyPascalCase()
    {
        var result = Assert.Single(NucleiParser.Read("{\"TemplateId\": \"t1\", \"Info\": {\"Name\": \"N\", \"Severity\": \"low\"}, \"Host\": \"h\", \"MatchedAt\": \"m\"}"));

        Assert.Equal("t1", result.TemplateId);
        Assert.Equal("N", result.Info.Name);
        Assert.Equal("m", result.MatchedAt);
    }

    [Fact]
    public void Bandit_ReadsRealOutput()
    {
        var report = Assert.Single(BanditParser.Read(Fixture("Bandit", "bandit.json")));

        Assert.Equal(7, report.Results.Length);
        var result = report.Results.Single(x => x.TestId == "B602");
        Assert.Equal("HIGH", result.IssueSeverity);
        Assert.Equal("subprocess call with shell=True identified, security issue.", result.Issue);
        Assert.Equal(8, result.LineNumber);
        Assert.Equal("/src/app.py", result.Filename);
    }

    [Theory]
    [InlineData("/src/app.py", "src (Source Code)")]
    [InlineData("app/views.py", "app (Source Code)")]
    [InlineData("main.py", "Source Code Analysis")]
    public void Bandit_TargetNameOf(string filename, string expected)
    {
        Assert.Equal(expected, BanditParser.TargetNameOf(filename));
    }

    [Fact]
    public void Masscan_ReadsRealOutput()
    {
        var results = MasscanParser.Read(Fixture("Masscan", "masscan.json"));

        Assert.Equal(2, results.Count);
        Assert.Equal("10.2.2.2", results[0].Ip);
        Assert.Equal(443, results[0].Ports[0].Port);
        Assert.Equal("open", results[0].Ports[0].Status);
    }

    [Fact]
    public void DependencyCheck_ReadsRealOutput()
    {
        var report = Assert.Single(DependencyCheckParser.Read(Fixture("DependencyCheck", "dependency-check.json")));

        Assert.Equal(2, report.Dependencies.Length);
        Assert.Equal("log-lib-2.14.1.jar", report.Dependencies[0].FileName);
        Assert.Equal(2, report.Dependencies[0].Vulnerabilities.Length);
        var vuln = report.Dependencies[0].Vulnerabilities[0];
        Assert.Equal("CVE-2026-0003", vuln.Name);
        Assert.Equal(10.0, vuln.CvssV3.BaseScore);
        Assert.Empty(report.Dependencies[1].Vulnerabilities);
    }

    [Fact]
    public void DependencyCheck_VectorOfBuildsCvss31()
    {
        var report = Assert.Single(DependencyCheckParser.Read(Fixture("DependencyCheck", "dependency-check.json")));

        Assert.Equal("CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:C/C:H/I:H/A:H",
            DependencyCheckParser.VectorOf(report.Dependencies[0].Vulnerabilities[0].CvssV3));
    }

    [Fact]
    public void DependencyCheck_VectorOfPrefersVectorString()
    {
        var v = new DependencyCheckCvssV3 { VectorString = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H", AttackVector = "NETWORK" };

        Assert.Equal("CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H", DependencyCheckParser.VectorOf(v));
    }

    [Fact]
    public void DependencyCheck_VectorOfMissingMetricIsEmpty()
    {
        Assert.Equal("", DependencyCheckParser.VectorOf(new DependencyCheckCvssV3 { BaseScore = 5, AttackVector = "NETWORK" }));
    }

    [Fact]
    public void DependencyCheck_ReadsCvssV4()
    {
        var report = Assert.Single(DependencyCheckParser.Read(Fixture("DependencyCheck", "dependency-check.json")));

        var v4 = report.Dependencies[0].Vulnerabilities[1].CvssV4;
        Assert.Equal(6.9, v4.BaseScore);
        Assert.Equal("CVSS:4.0/AV:N/AC:L/AT:N/PR:N/UI:N/VC:L/VI:N/VA:N/SC:N/SI:N/SA:N", v4.VectorString);
    }

    [Theory]
    [InlineData("Moderate", VulnRisk.Medium)]
    [InlineData("CRITICAL", VulnRisk.Critical)]
    [InlineData("LOW", VulnRisk.Low)]
    [InlineData("unknown", VulnRisk.Info)]
    public void DependencyCheck_RiskOf(string severity, VulnRisk expected)
    {
        Assert.Equal(expected, DependencyCheckParser.RiskOf(severity));
    }

    [Fact]
    public void Nikto_ReadsRealOutput()
    {
        var host = Assert.Single(NiktoParser.Read(Fixture("Nikto", "nikto.json")));

        Assert.Equal("app.example", host.Host);
        Assert.Equal("10.3.3.3", host.Ip);
        Assert.Equal(8080, host.Port);
        Assert.Equal(2, host.Vulnerabilities.Length);
        Assert.Equal("999986", host.Vulnerabilities[0].Id);
    }

    [Theory]
    [InlineData("Trivy")]
    [InlineData("Nuclei")]
    [InlineData("Bandit")]
    [InlineData("DependencyCheck")]
    public void Read_MalformedThrows(string tool)
    {
        const string broken = "{\"Results\": [ {\"Target\": ";
        Func<object> read = tool switch
        {
            "Trivy" => () => TrivyParser.Read(broken),
            "Nuclei" => () => NucleiParser.Read(broken),
            "Bandit" => () => BanditParser.Read(broken),
            _ => () => DependencyCheckParser.Read(broken),
        };

        Assert.ThrowsAny<JsonException>(read);
    }

    [Fact]
    public void Nuclei_Read_SkipsOneBadLine()
    {
        var lines = Fixture("Nuclei", "nuclei.jsonl").Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var results = NucleiParser.Read(lines[0] + "\n{\"template-id\": \n" + lines[1]);

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void Import_EmptyReportIsNotAnError()
    {
        Assert.Empty(Assert.Single(TrivyParser.Read("{\"SchemaVersion\": 2, \"Results\": []}")).Results);
        Assert.Empty(NucleiParser.Read("[]"));
        Assert.Empty(NucleiParser.Read(""));
        Assert.Empty(Assert.Single(BanditParser.Read("{\"errors\": [], \"results\": []}")).Results);
    }
}
