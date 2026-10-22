using System.Text.Json;
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
    private static string Fixture(string tool, string file) =>
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "IFR", "Fixtures", "Parsers", tool, file));

    [Fact]
    public void Trivy_ReadsRealOutput()
    {
        var report = Assert.Single(TrivyParser.Read(Fixture("Trivy", "trivy-image.json")));

        var vuln = report.Results[0].Vulnerabilities[0];
        Assert.Equal("CVE-2026-0001", vuln.VulnerabilityID);
        Assert.Equal("HIGH", vuln.Severity);
        Assert.Equal(7.5, vuln.CVSS["nvd"].V3Score);
        Assert.Empty(report.Results[1].Vulnerabilities);
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

        var result = Assert.Single(report.Results);
        Assert.Equal("B602", result.TestId);
        Assert.Equal("HIGH", result.IssueSeverity);
        Assert.Equal("subprocess call with shell=True identified, security issue.", result.Issue);
        Assert.Equal(10, result.LineNumber);
        Assert.Equal("app/views.py", result.Filename);
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
        var vuln = Assert.Single(report.Dependencies[0].Vulnerabilities);
        Assert.Equal("CVE-2026-0003", vuln.Name);
        Assert.Equal(10.0, vuln.CvssV3.BaseScore);
        Assert.Empty(report.Dependencies[1].Vulnerabilities);
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
