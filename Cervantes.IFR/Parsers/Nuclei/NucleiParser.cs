using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.Nuclei;

public class NucleiParser : INucleiParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    // The finding ids of the import in progress; the project's ids are read once per import, not once per vuln.
    private FindingIdSequence findingIds = FindingIdSequence.Empty;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public NucleiParser(ITargetManager targetManager, IVulnManager vulnManager,
        IVulnTargetManager vulnTargetManager, IProjectManager projectManager)
    {
        this.targetManager = targetManager;
        this.vulnManager = vulnManager;
        this.vulnTargetManager = vulnTargetManager;
        this.projectManager = projectManager;
    }

    public void Parse(Guid? project, string user, string path)
    {
        findingIds = FindingIdSequence.Empty;
        var sanitizer = new HtmlSanitizer().AllowDataImagesOnly();

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            findingIds = FindingIdGenerator.Start(vulnManager, pro);
            var fileContent = System.IO.File.ReadAllText(path);
            
            // Nuclei puede generar múltiples líneas JSON (JSONL) o un array JSON
            foreach (var nucleiResult in Read(fileContent))
            {
                try
                {

                    // Parse severity to risk level
                    VulnRisk risk = VulnRisk.Info;
                    if (!string.IsNullOrEmpty(nucleiResult.Info?.Severity))
                    {
                        switch (nucleiResult.Info.Severity.ToLower())
                        {
                            case "critical":
                                risk = VulnRisk.Critical;
                                break;
                            case "high":
                                risk = VulnRisk.High;
                                break;
                            case "medium":
                                risk = VulnRisk.Medium;
                                break;
                            case "low":
                                risk = VulnRisk.Low;
                                break;
                            case "info":
                            case "informational":
                                risk = VulnRisk.Info;
                                break;
                        }
                    }

                    // Parse target information
                    string targetHost = TargetOf(nucleiResult);

                    // Create or find target
                    Target target = null;
                    if (!string.IsNullOrEmpty(targetHost))
                    {
                        var targets = targetManager.GetAll().Where(x => x.Project.Id == project).ToList();
                        target = targets.FirstOrDefault(x => x.Name == targetHost);

                        if (target == null)
                        {
                            target = new Target
                            {
                                Id = Guid.NewGuid(),
                                Name = targetHost,
                                Description = $"Target imported from Nuclei scan",
                                Type = Uri.CheckHostName(targetHost) == UriHostNameType.IPv4 || 
                                       Uri.CheckHostName(targetHost) == UriHostNameType.IPv6 ? 
                                       TargetType.IP : TargetType.Hostname,
                                Project = pro,
                                UserId = user
                            };
                            targetManager.Add(target);
                            targetManager.Context.SaveChanges();
                        }
                    }

                    // Build detailed description
                    string description = nucleiResult.Info?.Description ?? "";
                    if (!string.IsNullOrEmpty(nucleiResult.TemplateId))
                    {
                        description += $"\n\nTemplate ID: {nucleiResult.TemplateId}";
                    }
                    if (nucleiResult.Info?.Tags?.Any() == true)
                    {
                        description += $"\n\nTags: {string.Join(", ", nucleiResult.Info.Tags)}";
                    }
                    if (!string.IsNullOrEmpty(nucleiResult.MatchedAt))
                    {
                        description += $"\n\nMatched At: {nucleiResult.MatchedAt}";
                    }

                    // Build proof of concept
                    string poc = "";
                    if (!string.IsNullOrEmpty(nucleiResult.Host))
                    {
                        poc += $"Target: {nucleiResult.Host}\n";
                    }
                    if (!string.IsNullOrEmpty(nucleiResult.MatchedAt))
                    {
                        poc += $"Matched At: {nucleiResult.MatchedAt}\n";
                    }
                    if (!string.IsNullOrEmpty(nucleiResult.ExtractedResults?.FirstOrDefault()))
                    {
                        poc += $"Extracted: {nucleiResult.ExtractedResults.FirstOrDefault()}\n";
                    }
                    if (!string.IsNullOrEmpty(nucleiResult.Request))
                    {
                        poc += $"Request:\n{nucleiResult.Request}\n";
                    }
                    if (!string.IsNullOrEmpty(nucleiResult.Response))
                    {
                        // Responses are whole pages; keep the start, as the Burp import does
                        var response = nucleiResult.Response.Length > 4000 ? nucleiResult.Response[..4000] + "\n[…]" : nucleiResult.Response;
                        poc += $"Response:\n{response}\n";
                    }

                    // Build impact from template info
                    string impact = nucleiResult.Info?.Description ?? "";
                    if (nucleiResult.Info?.Classification != null)
                    {
                        var classification = nucleiResult.Info.Classification;
                        if (!string.IsNullOrEmpty(classification.CvssScore))
                        {
                            impact += $"\n\nCVSS Score: {classification.CvssScore}";
                        }
                        if (classification.Cwe?.Any() == true)
                        {
                            impact += $"\nCWE: {string.Join(", ", classification.Cwe)}";
                        }
                    }

                    // Create vulnerability
                    var vuln = new Vuln
                    {
                        Id = Guid.NewGuid(),
                        Name = sanitizer.Sanitize(!string.IsNullOrEmpty(nucleiResult.Info?.Name) ? nucleiResult.Info.Name : !string.IsNullOrEmpty(nucleiResult.TemplateId) ? nucleiResult.TemplateId : "Unknown Nuclei Finding"),
                        Description = sanitizer.Sanitize(WebUtility.HtmlEncode(description)),
                        Risk = risk,
                        Status = VulnStatus.Open,
                        Remediation = sanitizer.Sanitize(WebUtility.HtmlEncode(!string.IsNullOrEmpty(nucleiResult.Info?.Remediation) ? nucleiResult.Info.Remediation : "Review and remediate the identified vulnerability")),
                        RemediationComplexity = RemediationComplexity.Medium,
                        RemediationPriority = RemediationPriority.Medium,
                        Impact = sanitizer.Sanitize(WebUtility.HtmlEncode(impact)),
                        ProofOfConcept = sanitizer.Sanitize("<pre>" + WebUtility.HtmlEncode(poc) + "</pre>"),
                        Project = pro,
                        UserId = user,
                        Template = false,
                        Language = pro.Language,
                        CreatedDate = DateTime.Now.ToUniversalTime()
                    };

                    // Set CVSS if available
                    if (nucleiResult.Info?.Classification?.CvssScore != null &&
                        double.TryParse(nucleiResult.Info.Classification.CvssScore, NumberStyles.Float, 
                                      CultureInfo.InvariantCulture, out double cvssScore))
                    {
                        vuln.CVSS3 = cvssScore;
                    }

                    // Set CVSSVector if available
                    if (!string.IsNullOrEmpty(nucleiResult.Info?.Classification?.CvssVector))
                    {
                        vuln.CVSSVector = sanitizer.Sanitize(nucleiResult.Info.Classification.CvssVector);
                    }

                    if (nucleiResult.Info?.Classification?.Cves?.Length > 0)
                    {
                        vuln.cve = sanitizer.Sanitize(string.Join(", ", nucleiResult.Info.Classification.Cves));
                    }

                    vuln.FindingId = findingIds.Next();
                    vulnManager.Add(vuln);
                    vulnManager.Context.SaveChanges();

                    // Associate vulnerability with target if available
                    if (target != null)
                    {
                        var vulnTarget = new VulnTargets
                        {
                            Id = Guid.NewGuid(),
                            VulnId = vuln.Id,
                            TargetId = target.Id
                        };
                        vulnTargetManager.Add(vulnTarget);
                        vulnTargetManager.Context.SaveChanges();
                    }
                }
                catch (JsonException)
                {
                    // Skip invalid JSON lines
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// The target of a result: its <c>host</c> (a bare host in current Nuclei, a URL in older versions), else the host of
    /// <c>matched-at</c>, else of <c>url</c>, else <c>ip</c>. Nuclei leaves <c>host</c> out for some inputs.
    /// </summary>
    public static string TargetOf(NucleiResult result)
    {
        static string? HostOf(string? value)
        {
            value = value?.Trim().TrimStart('/');
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            if (value.Contains("://"))
            {
                return Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Host.Length > 0 ? uri.Host : null;
            }
            // host or host:port
            return Uri.TryCreate("http://" + value, UriKind.Absolute, out var bare) && bare.Host.Length > 0 ? bare.Host : value;
        }

        return HostOf(result.Host) ?? HostOf(result.MatchedAt) ?? HostOf(result.Url) ?? HostOf(result.Ip) ?? "";
    }

    /// <summary>
    /// Reads Nuclei output: JSON lines (`-jsonl`) or a JSON array (`-je`). Field names changed between Nuclei
    /// versions (`templateID` / `template-id`, `matched` / `matched-at`), so each field is read under any of its names.
    /// A line that is not a JSON object is skipped; if no line can be read the content is not Nuclei output and it throws.
    /// </summary>
    public static System.Collections.Generic.IReadOnlyList<NucleiResult> Read(string content)
    {
        var results = new System.Collections.Generic.List<NucleiResult>();
        if (content.TrimStart().StartsWith("["))
        {
            using var array = JsonDocument.Parse(content, new JsonDocumentOptions { AllowTrailingCommas = true });
            results.AddRange(array.RootElement.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.Object).Select(FromJson));
            return results;
        }

        JsonException? firstError = null;
        foreach (var line in content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            try
            {
                using var doc = JsonDocument.Parse(line);
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new JsonException("A Nuclei result is a JSON object");
                }
                results.Add(FromJson(doc.RootElement));
            }
            catch (JsonException e)
            {
                firstError ??= e;
            }
        }
        if (results.Count == 0 && firstError != null)
        {
            throw firstError;
        }
        return results;
    }

    private static NucleiResult FromJson(JsonElement e)
    {
        var info = Field(e, "info");
        var classification = info is { } i ? Field(i, "classification") : null;
        return new NucleiResult
        {
            TemplateId = Text(e, "template-id", "templateID") ?? "",
            Host = Text(e, "host") ?? "",
            MatchedAt = Text(e, "matched-at", "matched", "matchedAt") ?? "",
            ExtractedResults = Strings(e, "extracted-results", "extracted_results", "extractedResults"),
            Url = Text(e, "url") ?? "",
            Ip = Text(e, "ip") ?? "",
            Request = Text(e, "request") ?? "",
            Response = Text(e, "response") ?? "",
            Info = info is not { } inf ? new NucleiInfo() : new NucleiInfo
            {
                Name = Text(inf, "name") ?? "",
                Description = Text(inf, "description") ?? "",
                Severity = Text(inf, "severity") ?? "",
                Tags = Strings(inf, "tags"),
                Remediation = Text(inf, "remediation") ?? "",
                Classification = classification is not { } c ? new NucleiClassification() : new NucleiClassification
                {
                    CvssScore = Text(c, "cvss-score", "cvssScore") ?? "",
                    CvssVector = Text(c, "cvss-metrics", "cvssMetrics", "cvssVector") ?? "",
                    Cwe = Strings(c, "cwe-id", "cweId", "cwe"),
                    Cves = Strings(c, "cve-id", "cveId", "cve").Select(x => x.ToUpperInvariant()).ToArray(),
                },
            },
        };
    }

    // The value of the first of the names present on the object, ignoring case.
    private static JsonElement? Field(JsonElement obj, params string[] names)
    {
        if (obj.ValueKind != JsonValueKind.Object)
        {
            return null;
        }
        foreach (var name in names)
        {
            foreach (var property in obj.EnumerateObject())
            {
                if (string.Equals(name, property.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return property.Value;
                }
            }
        }
        return null;
    }

    private static string? Text(JsonElement obj, params string[] names)
    {
        return Field(obj, names) switch
        {
            { ValueKind: JsonValueKind.String } v => v.GetString(),
            { ValueKind: JsonValueKind.Number } v => v.GetRawText(),
            { ValueKind: JsonValueKind.True } => "true",
            { ValueKind: JsonValueKind.False } => "false",
            _ => null,
        };
    }

    // An array of strings; older Nuclei versions write some of them (tags) as one comma-separated string.
    private static string[] Strings(JsonElement obj, params string[] names)
    {
        return Field(obj, names) switch
        {
            { ValueKind: JsonValueKind.Array } v => v.EnumerateArray()
                .Select(x => x.ValueKind == JsonValueKind.String ? x.GetString() ?? "" : x.GetRawText()).ToArray(),
            { ValueKind: JsonValueKind.String } v => (v.GetString() ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            _ => Array.Empty<string>(),
        };
    }
}

// Data models for Nuclei JSON structure
public class NucleiResult
{
    public string TemplateId { get; set; } = "";
    public NucleiInfo Info { get; set; } = new();
    public string Host { get; set; } = "";
    public string MatchedAt { get; set; } = "";
    public string Url { get; set; } = "";
    public string Ip { get; set; } = "";
    public string[] ExtractedResults { get; set; } = Array.Empty<string>();
    public string Request { get; set; } = "";
    public string Response { get; set; } = "";
}

public class NucleiInfo
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Severity { get; set; } = "";
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string Remediation { get; set; } = "";
    public NucleiClassification Classification { get; set; } = new();
}

public class NucleiClassification
{
    public string CvssScore { get; set; } = "";
    public string CvssVector { get; set; } = "";
    public string[] Cwe { get; set; } = Array.Empty<string>();
    public string[] Cves { get; set; } = Array.Empty<string>();
}