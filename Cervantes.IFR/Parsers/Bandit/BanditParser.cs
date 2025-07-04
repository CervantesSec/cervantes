using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.Bandit;

public class BanditParser : IBanditParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public BanditParser(ITargetManager targetManager, IVulnManager vulnManager,
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
            
            try
            {
                var banditReport = JsonSerializer.Deserialize<BanditReport>(fileContent);
                if (banditReport != null)
                {
                    ProcessBanditReport(banditReport, pro, user, sanitizer);
                }
            }
            catch (JsonException ex)
            {
                // Log error but don't throw - might be malformed JSON
                Console.WriteLine($"Error parsing Bandit JSON: {ex.Message}");
            }
        }
    }

    private void ProcessBanditReport(BanditReport report, Project pro, string user, HtmlSanitizer sanitizer)
    {
        // Crear target basado en el proyecto de código
        Target target = null;
        string targetName = "Source Code Analysis";
        
        // Si tenemos archivos escaneados, usar el directorio base como nombre del target
        if (report.Results?.Any() == true)
        {
            var firstFile = report.Results.First().Filename;
            if (!string.IsNullOrEmpty(firstFile))
            {
                // Extraer el directorio base del proyecto
                var pathParts = firstFile.Split('/', '\\');
                if (pathParts.Length > 1)
                {
                    targetName = pathParts[0] + " (Source Code)";
                }
            }
        }

        var targets = targetManager.GetAll().Where(x => x.Project.Id == pro.Id).ToList();
        target = targets.FirstOrDefault(x => x.Name == targetName);

        if (target == null)
        {
            target = new Target
            {
                Id = Guid.NewGuid(),
                Name = targetName,
                Description = $"Source code target imported from Bandit scan",
                Type = TargetType.Hostname, // Usamos hostname para código fuente
                Project = pro,
                UserId = user
            };
            targetManager.Add(target);
            targetManager.Context.SaveChanges();
        }

        // Procesar cada resultado de Bandit
        foreach (var result in report.Results ?? new BanditResult[0])
        {
            ProcessBanditResult(result, report, target, pro, user, sanitizer);
        }
    }

    private void ProcessBanditResult(BanditResult result, BanditReport report, Target target, 
                                   Project pro, string user, HtmlSanitizer sanitizer)
    {
        if (string.IsNullOrEmpty(result.TestName) && string.IsNullOrEmpty(result.Issue))
            return;

        // Mapear severidad de Bandit a riesgo
        VulnRisk risk = VulnRisk.Medium; // Default para code security issues
        
        if (!string.IsNullOrEmpty(result.IssueSeverity))
        {
            switch (result.IssueSeverity.ToLower())
            {
                case "high":
                    risk = VulnRisk.High;
                    break;
                case "medium":
                    risk = VulnRisk.Medium;
                    break;
                case "low":
                    risk = VulnRisk.Low;
                    break;
            }
        }

        // Si tenemos confidence, ajustar el riesgo
        if (!string.IsNullOrEmpty(result.IssueConfidence))
        {
            switch (result.IssueConfidence.ToLower())
            {
                case "high":
                    // Mantener el riesgo actual
                    break;
                case "medium":
                    // Reducir un nivel si es crítico o alto
                    if (risk == VulnRisk.Critical) risk = VulnRisk.High;
                    else if (risk == VulnRisk.High) risk = VulnRisk.Medium;
                    break;
                case "low":
                    // Reducir a low o info
                    risk = VulnRisk.Low;
                    break;
            }
        }

        // Construir descripción detallada
        string description = result.Issue ?? "";
        if (!string.IsNullOrEmpty(result.TestName))
        {
            description += $"\n\nBandit Test: {result.TestName}";
        }
        if (!string.IsNullOrEmpty(result.TestId))
        {
            description += $"\nTest ID: {result.TestId}";
        }
        if (!string.IsNullOrEmpty(result.Filename))
        {
            description += $"\nFile: {result.Filename}";
        }
        if (result.LineNumber > 0)
        {
            description += $"\nLine: {result.LineNumber}";
        }
        if (!string.IsNullOrEmpty(result.IssueConfidence))
        {
            description += $"\nConfidence: {result.IssueConfidence}";
        }

        // Construir PoC con el código afectado
        string poc = "";
        if (!string.IsNullOrEmpty(result.Filename))
        {
            poc += $"File: {result.Filename}\n";
        }
        if (result.LineNumber > 0)
        {
            poc += $"Line Number: {result.LineNumber}\n";
        }
        if (result.LineRange?.Any() == true)
        {
            poc += $"Line Range: {string.Join("-", result.LineRange)}\n";
        }
        if (!string.IsNullOrEmpty(result.Code))
        {
            poc += $"Vulnerable Code:\n{result.Code}\n";
        }
        if (!string.IsNullOrEmpty(result.TestId))
        {
            poc += $"Bandit Rule: {result.TestId}\n";
        }

        // Construir remediación basada en el tipo de issue
        string remediation = "Review and remediate the identified security issue in the source code";
        if (!string.IsNullOrEmpty(result.TestName))
        {
            // Generar remediación específica basada en el tipo de test
            remediation = GenerateRemediationAdvice(result.TestName, result.Issue);
        }

        // Construir impacto
        string impact = result.Issue ?? "Source code security vulnerability detected";
        if (!string.IsNullOrEmpty(result.TestName))
        {
            impact += $" ({result.TestName})";
        }

        // Generar nombre de vulnerabilidad
        string vulnName = result.TestName ?? "Bandit Security Finding";
        if (!string.IsNullOrEmpty(result.Filename))
        {
            var fileName = System.IO.Path.GetFileName(result.Filename);
            vulnName += $" in {fileName}";
        }

        // Crear vulnerabilidad
        var vulnerability = new Vuln
        {
            Id = Guid.NewGuid(),
            Name = sanitizer.Sanitize(vulnName),
            Description = sanitizer.Sanitize(description),
            Risk = risk,
            Status = VulnStatus.Open,
            Remediation = sanitizer.Sanitize(remediation),
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

        vulnManager.Add(vulnerability);
        vulnManager.Context.SaveChanges();

        // Asociar vulnerabilidad con target si está disponible
        if (target != null)
        {
            var vulnTarget = new VulnTargets
            {
                Id = Guid.NewGuid(),
                VulnId = vulnerability.Id,
                TargetId = target.Id
            };
            vulnTargetManager.Add(vulnTarget);
            vulnTargetManager.Context.SaveChanges();
        }
    }

    private string GenerateRemediationAdvice(string testName, string issue)
    {
        string advice = "Review and remediate the identified security issue in the source code";
        
        if (string.IsNullOrEmpty(testName)) return advice;

        var lowerTestName = testName.ToLower();
        
        if (lowerTestName.Contains("hardcoded_password") || lowerTestName.Contains("hardcoded_sql"))
        {
            advice = "Remove hardcoded credentials and use secure configuration management or environment variables";
        }
        else if (lowerTestName.Contains("sql_injection"))
        {
            advice = "Use parameterized queries or prepared statements to prevent SQL injection attacks";
        }
        else if (lowerTestName.Contains("shell_injection") || lowerTestName.Contains("subprocess"))
        {
            advice = "Validate and sanitize input before using in shell commands, or use safer alternatives";
        }
        else if (lowerTestName.Contains("yaml_load") || lowerTestName.Contains("pickle"))
        {
            advice = "Use safe loading methods and avoid deserializing untrusted data";
        }
        else if (lowerTestName.Contains("random") || lowerTestName.Contains("weak_crypto"))
        {
            advice = "Use cryptographically secure random number generators and strong encryption algorithms";
        }
        else if (lowerTestName.Contains("assert") || lowerTestName.Contains("debug"))
        {
            advice = "Remove debug code and assertions from production code";
        }
        else if (lowerTestName.Contains("request") || lowerTestName.Contains("ssl"))
        {
            advice = "Use HTTPS and proper SSL/TLS certificate validation for all external requests";
        }
        else if (lowerTestName.Contains("flask") || lowerTestName.Contains("django"))
        {
            advice = "Review framework-specific security configurations and enable security features";
        }

        return advice;
    }
}

// Modelos para JSON de Bandit
public class BanditReport
{
    public BanditError[] Errors { get; set; } = Array.Empty<BanditError>();
    public string[] GeneratedAt { get; set; } = Array.Empty<string>();
    public BanditMetrics Metrics { get; set; } = new();
    public BanditResult[] Results { get; set; } = Array.Empty<BanditResult>();
}

public class BanditError
{
    public string Filename { get; set; } = "";
    public string Reason { get; set; } = "";
}

public class BanditMetrics
{
    public Dictionary<string, int> ConfidenceLevels { get; set; } = new();
    public Dictionary<string, int> SeverityLevels { get; set; } = new();
    public int TotalLinesOfCode { get; set; }
    public int TotalIssues { get; set; }
}

public class BanditResult
{
    public string Code { get; set; } = "";
    public string Filename { get; set; } = "";
    public string Issue { get; set; } = "";
    public string IssueConfidence { get; set; } = "";
    public string IssueSeverity { get; set; } = "";
    public int LineNumber { get; set; }
    public int[] LineRange { get; set; } = Array.Empty<int>();
    public string TestId { get; set; } = "";
    public string TestName { get; set; } = "";
}