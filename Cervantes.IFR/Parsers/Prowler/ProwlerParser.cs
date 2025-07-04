using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.Prowler;

public class ProwlerParser : IProwlerParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public ProwlerParser(ITargetManager targetManager, IVulnManager vulnManager,
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
            
            // Prowler puede generar JSON o CSV
            if (fileContent.TrimStart().StartsWith("{") || fileContent.TrimStart().StartsWith("["))
            {
                ParseJsonFormat(fileContent, pro, user, sanitizer);
            }
            else
            {
                ParseCsvFormat(fileContent, pro, user, sanitizer);
            }
        }
    }

    private void ParseJsonFormat(string fileContent, Project pro, string user, HtmlSanitizer sanitizer)
    {
        try
        {
            // Prowler JSON puede ser un array o un objeto con múltiples checks
            if (fileContent.TrimStart().StartsWith("["))
            {
                var prowlerResults = JsonSerializer.Deserialize<ProwlerResult[]>(fileContent);
                foreach (var result in prowlerResults ?? new ProwlerResult[0])
                {
                    ProcessProwlerFinding(result, pro, user, sanitizer);
                }
            }
            else
            {
                var prowlerResult = JsonSerializer.Deserialize<ProwlerResult>(fileContent);
                if (prowlerResult != null)
                {
                    ProcessProwlerFinding(prowlerResult, pro, user, sanitizer);
                }
            }
        }
        catch (JsonException)
        {
            // Si falla JSON, intentar como CSV
            ParseCsvFormat(fileContent, pro, user, sanitizer);
        }
    }

    private void ParseCsvFormat(string fileContent, Project pro, string user, HtmlSanitizer sanitizer)
    {
        var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        if (lines.Length < 2) return; // Necesitamos al menos header y una línea de datos
        
        var headers = lines[0].Split(',').Select(h => h.Trim('"')).ToArray();
        
        // Mapear columnas comunes de Prowler CSV
        int statusIndex = Array.FindIndex(headers, h => h.Equals("Status", StringComparison.OrdinalIgnoreCase) || 
                                                       h.Equals("Result", StringComparison.OrdinalIgnoreCase));
        int checkIdIndex = Array.FindIndex(headers, h => h.Equals("Check_ID", StringComparison.OrdinalIgnoreCase) || 
                                                        h.Equals("CheckID", StringComparison.OrdinalIgnoreCase));
        int severityIndex = Array.FindIndex(headers, h => h.Equals("Severity", StringComparison.OrdinalIgnoreCase));
        int regionIndex = Array.FindIndex(headers, h => h.Equals("Region", StringComparison.OrdinalIgnoreCase));
        int accountIndex = Array.FindIndex(headers, h => h.Equals("Account", StringComparison.OrdinalIgnoreCase) || 
                                                        h.Equals("Account_ID", StringComparison.OrdinalIgnoreCase));
        int serviceIndex = Array.FindIndex(headers, h => h.Equals("Service", StringComparison.OrdinalIgnoreCase));
        int descriptionIndex = Array.FindIndex(headers, h => h.Equals("Description", StringComparison.OrdinalIgnoreCase) || 
                                                             h.Equals("Check_Title", StringComparison.OrdinalIgnoreCase));
        int resourceIndex = Array.FindIndex(headers, h => h.Equals("Resource", StringComparison.OrdinalIgnoreCase) || 
                                                          h.Equals("Resource_ID", StringComparison.OrdinalIgnoreCase));

        for (int i = 1; i < lines.Length; i++)
        {
            var columns = lines[i].Split(',').Select(c => c.Trim('"')).ToArray();
            
            if (columns.Length < headers.Length) continue;
            
            string status = statusIndex >= 0 && statusIndex < columns.Length ? columns[statusIndex] : "";
            string checkId = checkIdIndex >= 0 && checkIdIndex < columns.Length ? columns[checkIdIndex] : "";
            string severity = severityIndex >= 0 && severityIndex < columns.Length ? columns[severityIndex] : "";
            string region = regionIndex >= 0 && regionIndex < columns.Length ? columns[regionIndex] : "";
            string account = accountIndex >= 0 && accountIndex < columns.Length ? columns[accountIndex] : "";
            string service = serviceIndex >= 0 && serviceIndex < columns.Length ? columns[serviceIndex] : "";
            string description = descriptionIndex >= 0 && descriptionIndex < columns.Length ? columns[descriptionIndex] : "";
            string resource = resourceIndex >= 0 && resourceIndex < columns.Length ? columns[resourceIndex] : "";
            
            // Solo procesar findings que son FAIL o similares
            if (status.Equals("FAIL", StringComparison.OrdinalIgnoreCase) || 
                status.Equals("FAILED", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("CRITICAL", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("HIGH", StringComparison.OrdinalIgnoreCase))
            {
                var prowlerResult = new ProwlerResult
                {
                    CheckID = checkId,
                    Status = status,
                    Severity = severity,
                    Region = region,
                    Account = account,
                    Service = service,
                    Description = description,
                    Resource = resource
                };
                
                ProcessProwlerFinding(prowlerResult, pro, user, sanitizer);
            }
        }
    }

    private void ProcessProwlerFinding(ProwlerResult result, Project pro, string user, HtmlSanitizer sanitizer)
    {
        if (string.IsNullOrEmpty(result.Description) && string.IsNullOrEmpty(result.CheckID))
            return;

        // Solo procesar findings fallidos
        if (!string.IsNullOrEmpty(result.Status) && 
            !result.Status.Equals("FAIL", StringComparison.OrdinalIgnoreCase) &&
            !result.Status.Equals("FAILED", StringComparison.OrdinalIgnoreCase) &&
            !result.Status.Equals("CRITICAL", StringComparison.OrdinalIgnoreCase) &&
            !result.Status.Equals("HIGH", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Mapear severidad a riesgo
        VulnRisk risk = VulnRisk.Medium; // Default para hallazgos de cloud security
        
        if (!string.IsNullOrEmpty(result.Severity))
        {
            switch (result.Severity.ToLower())
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
        else if (!string.IsNullOrEmpty(result.Status))
        {
            // Si no hay severidad, usar el status
            switch (result.Status.ToLower())
            {
                case "critical":
                    risk = VulnRisk.Critical;
                    break;
                case "high":
                    risk = VulnRisk.High;
                    break;
                case "fail":
                case "failed":
                    risk = VulnRisk.Medium;
                    break;
            }
        }

        // Crear target basado en la región/cuenta
        Target target = null;
        string targetName = "";
        
        if (!string.IsNullOrEmpty(result.Region))
        {
            targetName = result.Region;
            if (!string.IsNullOrEmpty(result.Account))
            {
                targetName = $"{result.Account} ({result.Region})";
            }
        }
        else if (!string.IsNullOrEmpty(result.Account))
        {
            targetName = result.Account;
        }

        if (!string.IsNullOrEmpty(targetName))
        {
            var targets = targetManager.GetAll().Where(x => x.Project.Id == pro.Id).ToList();
            target = targets.FirstOrDefault(x => x.Name == targetName);

            if (target == null)
            {
                target = new Target
                {
                    Id = Guid.NewGuid(),
                    Name = targetName,
                    Description = $"Cloud account/region imported from Prowler scan",
                    Type = TargetType.Hostname, // Usamos hostname para entidades cloud
                    Project = pro,
                    UserId = user
                };
                targetManager.Add(target);
                targetManager.Context.SaveChanges();
            }
        }

        // Construir descripción detallada
        string detailedDescription = result.Description ?? "";
        if (!string.IsNullOrEmpty(result.CheckID))
        {
            detailedDescription += $"\n\nProwler Check ID: {result.CheckID}";
        }
        if (!string.IsNullOrEmpty(result.Service))
        {
            detailedDescription += $"\nCloud Service: {result.Service}";
        }
        if (!string.IsNullOrEmpty(result.Region))
        {
            detailedDescription += $"\nRegion: {result.Region}";
        }
        if (!string.IsNullOrEmpty(result.Account))
        {
            detailedDescription += $"\nAccount: {result.Account}";
        }

        // Construir PoC
        string poc = "";
        if (!string.IsNullOrEmpty(result.Resource))
        {
            poc += $"Affected Resource: {result.Resource}\n";
        }
        if (!string.IsNullOrEmpty(result.Service))
        {
            poc += $"Service: {result.Service}\n";
        }
        if (!string.IsNullOrEmpty(result.Region))
        {
            poc += $"Region: {result.Region}\n";
        }
        if (!string.IsNullOrEmpty(result.Account))
        {
            poc += $"Account: {result.Account}\n";
        }
        if (!string.IsNullOrEmpty(result.Status))
        {
            poc += $"Status: {result.Status}\n";
        }

        // Generar nombre de vulnerabilidad
        string vulnName = "Prowler Cloud Security Finding";
        if (!string.IsNullOrEmpty(result.CheckID))
        {
            vulnName = $"Prowler {result.CheckID}";
        }
        if (!string.IsNullOrEmpty(result.Service))
        {
            vulnName += $" - {result.Service}";
        }

        // Crear vulnerabilidad
        var vulnerability = new Vuln
        {
            Id = Guid.NewGuid(),
            Name = sanitizer.Sanitize(vulnName),
            Description = sanitizer.Sanitize(detailedDescription),
            Risk = risk,
            Status = VulnStatus.Open,
            Remediation = sanitizer.Sanitize("Review and remediate the cloud security misconfiguration according to best practices"),
            RemediationComplexity = RemediationComplexity.Medium,
            RemediationPriority = RemediationPriority.Medium,
            Impact = sanitizer.Sanitize(result.Description ?? "Cloud security misconfiguration detected"),
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
}

// Modelo para resultados de Prowler
public class ProwlerResult
{
    public string CheckID { get; set; } = "";
    public string Status { get; set; } = "";
    public string Severity { get; set; } = "";
    public string Region { get; set; } = "";
    public string Account { get; set; } = "";
    public string Service { get; set; } = "";
    public string Description { get; set; } = "";
    public string Resource { get; set; } = "";
    public string Compliance { get; set; } = "";
}