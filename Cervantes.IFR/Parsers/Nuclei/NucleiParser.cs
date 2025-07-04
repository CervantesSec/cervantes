using System;
using System.Globalization;
using System.IO;
using System.Linq;
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
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedSchemes.Add("data");

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            var fileContent = System.IO.File.ReadAllText(path);
            
            // Nuclei puede generar múltiples líneas JSON (JSONL) o un array JSON
            var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                try
                {
                    var nucleiResult = JsonSerializer.Deserialize<NucleiResult>(line);
                    if (nucleiResult == null) continue;

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
                    string targetHost = "";
                    int targetPort = 80;
                    
                    if (!string.IsNullOrEmpty(nucleiResult.Host))
                    {
                        try
                        {
                            var uri = new Uri(nucleiResult.Host);
                            targetHost = uri.Host;
                            targetPort = uri.Port;
                        }
                        catch
                        {
                            // If URI parsing fails, use the host as-is
                            targetHost = nucleiResult.Host;
                        }
                    }

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
                        poc += $"Response:\n{nucleiResult.Response}\n";
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
                        Name = sanitizer.Sanitize(nucleiResult.Info?.Name ?? nucleiResult.TemplateId ?? "Unknown Nuclei Finding"),
                        Description = sanitizer.Sanitize(description),
                        Risk = risk,
                        Status = VulnStatus.Open,
                        Remediation = sanitizer.Sanitize(nucleiResult.Info?.Remediation ?? "Review and remediate the identified vulnerability"),
                        RemediationComplexity = RemediationComplexity.Medium,
                        RemediationPriority = RemediationPriority.Medium,
                        Impact = sanitizer.Sanitize(impact),
                        ProofOfConcept = sanitizer.Sanitize(poc),
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
                        vuln.CVSSVector = nucleiResult.Info.Classification.CvssVector;
                    }

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
}

// Data models for Nuclei JSON structure
public class NucleiResult
{
    public string TemplateId { get; set; } = "";
    public NucleiInfo Info { get; set; } = new();
    public string Host { get; set; } = "";
    public string MatchedAt { get; set; } = "";
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
}