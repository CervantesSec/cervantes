using System.Text.Json;
using Cervantes.IFR.Parsers.Prowler;

namespace Cervantes.Tests.IFR;

public class ProwlerTests
{
    [Fact]
    public void Read_V3Json()
    {
        var results = ProwlerParser.Read(ParserFixtures.Text("Prowler", "prowler-v3.json"));

        Assert.Equal(3, results.Count);
        var acm = results[0];
        Assert.Equal("acm_certificates_expiration_check", acm.CheckID);
        Assert.Equal("Check if ACM Certificates are about to expire in specific days or less", acm.CheckTitle);
        Assert.Equal("FAIL", acm.Status);
        Assert.Equal("high", acm.Severity);
        Assert.Equal("acm", acm.Service);
        Assert.Equal("999999999999", acm.Account);
        Assert.Equal("us-east-1", acm.Region);
        Assert.Equal("api.teste.teste.com", acm.Resource);
        Assert.Equal("Expired certificates can impact service availability.", acm.Risk);
        Assert.StartsWith("Monitor certificate expiration", acm.Remediation);
        Assert.Equal("https://docs.aws.amazon.com/config/latest/developerguide/acm-certificate-expiration-check.html", acm.RemediationUrl);
    }

    [Fact]
    public void Read_Ocsf()
    {
        var results = ProwlerParser.Read(ParserFixtures.Text("Prowler", "prowler-v4.ocsf.json"));

        Assert.Equal(3, results.Count);
        var analyzer = results[0];
        Assert.Equal("accessanalyzer_enabled", analyzer.CheckID);
        Assert.Equal("Check if IAM Access Analyzer is enabled", analyzer.CheckTitle);
        Assert.Equal("FAIL", analyzer.Status);
        Assert.Equal("Low", analyzer.Severity);
        Assert.Equal("accessanalyzer", analyzer.Service);
        Assert.Equal("<account_uid>", analyzer.Account);
        Assert.Equal("<region>", analyzer.Region);
        Assert.Equal("<resource_uid>", analyzer.Resource);
        Assert.StartsWith("Enable IAM Access Analyzer", analyzer.Remediation);
        Assert.Equal("MANUAL", results[1].Status);
    }

    [Fact]
    public void Read_V4Csv()
    {
        var results = ProwlerParser.Read(ParserFixtures.Text("Prowler", "prowler-v4.csv"));

        Assert.Equal(4, results.Count);
        var analyzer = results[0];
        Assert.Equal("accessanalyzer_enabled", analyzer.CheckID);
        Assert.Equal("FAIL", analyzer.Status);
        Assert.Equal("low", analyzer.Severity);
        Assert.Equal("accessanalyzer", analyzer.Service);
        Assert.Equal("<account_uid>", analyzer.Account);
        Assert.Equal("<region>", analyzer.Region);
        Assert.Equal("<resource_uid>", analyzer.Resource);
        Assert.False(analyzer.Muted);
        Assert.Equal(2, results.Count(ProwlerParser.IsFinding));
    }

    [Fact]
    public void Read_V3Csv()
    {
        var results = ProwlerParser.Read(ParserFixtures.Text("Prowler", "prowler-v3.csv"));

        Assert.Equal(2, results.Count);
        var bucket = results[0];
        Assert.Equal("s3_bucket_public_access", bucket.CheckID);
        Assert.Equal("FAIL", bucket.Status);
        Assert.Equal("critical", bucket.Severity);
        Assert.Equal("s3", bucket.Service);
        Assert.Equal("111122223333", bucket.Account);
        Assert.Equal("eu-west-1", bucket.Region);
        Assert.Equal("cervantes-qa-bucket", bucket.Resource);
        Assert.Equal("Block public access on the bucket and the account.", bucket.Remediation);
        Assert.Single(results, ProwlerParser.IsFinding);
    }

    [Fact]
    public void Read_AllPassIsEmpty()
    {
        var lines = ParserFixtures.Text("Prowler", "prowler-v3.csv").Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var results = ProwlerParser.Read(lines[0] + "\n" + lines[2]);

        Assert.Single(results);
        Assert.DoesNotContain(results, ProwlerParser.IsFinding);
    }

    // Prowler mutelist docs: a muted finding in JSON-OCSF keeps status_code FAIL and carries status "Suppressed" (status_id 3).
    [Fact]
    public void Read_OcsfSuppressedIsMuted()
    {
        var json = ParserFixtures.Text("Prowler", "prowler-v4.ocsf.json");
        var first = json.IndexOf("\"status\": \"New\"", StringComparison.Ordinal);
        var suppressed = json[..first] + "\"status\": \"Suppressed\"" + json[(first + "\"status\": \"New\"".Length)..];

        var results = ProwlerParser.Read(suppressed);

        Assert.True(results[0].Muted);
        Assert.All(results.Skip(1), r => Assert.False(r.Muted));
    }

    [Fact]
    public void IsFinding_SkipsMuted()
    {
        Assert.False(ProwlerParser.IsFinding(new ProwlerResult { Status = "FAIL", Muted = true }));
        Assert.True(ProwlerParser.IsFinding(new ProwlerResult { Status = "fail" }));
    }

    [Fact]
    public void Read_UnknownJsonThrows()
    {
        Assert.Throws<InvalidDataException>(() => ProwlerParser.Read("[{\"foo\": 1}]"));
        Assert.Throws<InvalidDataException>(() => ProwlerParser.Read("just some text"));
    }

    [Fact]
    public void Read_BrokenJsonThrows()
    {
        Assert.ThrowsAny<JsonException>(() => ProwlerParser.Read("[{\"CheckID\": "));
    }
}
