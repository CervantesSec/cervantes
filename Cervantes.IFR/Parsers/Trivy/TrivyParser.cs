using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.Trivy;

public class TrivyParser : ITrivyParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public TrivyParser(ITargetManager targetManager, IVulnManager vulnManager,
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
                var trivyReport = JsonSerializer.Deserialize<TrivyReport>(fileContent);
                if (trivyReport != null)
                {
                    ProcessTrivyReport(trivyReport, pro, user, sanitizer);
                }
            }
            catch (JsonException ex)
            {
                // Log error but don't throw - might be malformed JSON
                Console.WriteLine($"Error parsing Trivy JSON: {ex.Message}");
            }
        }
    }

    private void ProcessTrivyReport(TrivyReport report, Project pro, string user, HtmlSanitizer sanitizer)
    {
        // Crear target basado en el artefacto escaneado
        Target target = null;
        string targetName = report.ArtifactName ?? "Unknown Container";
        
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
                    Description = $"Container/Image imported from Trivy scan",
                    Type = TargetType.Hostname, // Usamos hostname para contenedores
                    Project = pro,
                    UserId = user
                };
                targetManager.Add(target);
                targetManager.Context.SaveChanges();
            }
        }

        // Procesar cada resultado de escaneo
        foreach (var result in report.Results ?? new TrivyResult[0])
        {
            // Procesar vulnerabilidades
            foreach (var vulnerability in result.Vulnerabilities ?? new TrivyVulnerability[0])
            {
                ProcessTrivyVulnerability(vulnerability, result, report, target, pro, user, sanitizer);
            }

            // Procesar misconfigurations si existen
            foreach (var misconfig in result.Misconfigurations ?? new TrivyMisconfiguration[0])
            {
                ProcessTrivyMisconfiguration(misconfig, result, report, target, pro, user, sanitizer);
            }
        }
    }

    private void ProcessTrivyVulnerability(TrivyVulnerability vuln, TrivyResult result, TrivyReport report, 
                                         Target target, Project pro, string user, HtmlSanitizer sanitizer)
    {
        if (string.IsNullOrEmpty(vuln.VulnerabilityID) && string.IsNullOrEmpty(vuln.Title))
            return;

        // Mapear severidad a riesgo
        VulnRisk risk = VulnRisk.Info;
        if (!string.IsNullOrEmpty(vuln.Severity))
        {
            switch (vuln.Severity.ToUpper())
            {
                case "CRITICAL":
                    risk = VulnRisk.Critical;
                    break;
                case "HIGH":
                    risk = VulnRisk.High;
                    break;
                case "MEDIUM":
                    risk = VulnRisk.Medium;
                    break;
                case "LOW":
                    risk = VulnRisk.Low;
                    break;
                case "UNKNOWN":
                case "NEGLIGIBLE":
                    risk = VulnRisk.Info;
                    break;
            }
        }

        // Construir descripción detallada
        string description = vuln.Description ?? "";
        if (!string.IsNullOrEmpty(vuln.VulnerabilityID))
        {
            description += $"\n\nVulnerability ID: {vuln.VulnerabilityID}";
        }
        if (!string.IsNullOrEmpty(vuln.PkgName))
        {
            description += $"\nPackage: {vuln.PkgName}";
        }
        if (!string.IsNullOrEmpty(vuln.InstalledVersion))
        {
            description += $"\nInstalled Version: {vuln.InstalledVersion}";
        }
        if (!string.IsNullOrEmpty(vuln.FixedVersion))
        {
            description += $"\nFixed Version: {vuln.FixedVersion}";
        }
        if (!string.IsNullOrEmpty(result.Target))
        {
            description += $"\nTarget: {result.Target}";
        }

        // Construir PoC
        string poc = "";
        if (!string.IsNullOrEmpty(report.ArtifactName))
        {
            poc += $"Container/Image: {report.ArtifactName}\n";
        }
        if (!string.IsNullOrEmpty(result.Target))
        {
            poc += $"Scan Target: {result.Target}\n";
        }
        if (!string.IsNullOrEmpty(vuln.PkgName))
        {
            poc += $"Vulnerable Package: {vuln.PkgName}\n";
        }
        if (!string.IsNullOrEmpty(vuln.InstalledVersion))
        {
            poc += $"Current Version: {vuln.InstalledVersion}\n";
        }
        if (!string.IsNullOrEmpty(vuln.FixedVersion))
        {
            poc += $"Fix Available: {vuln.FixedVersion}\n";
        }

        // Construir remediación
        string remediation = "Update the vulnerable package to the fixed version";
        if (!string.IsNullOrEmpty(vuln.FixedVersion))
        {
            remediation = $"Update {vuln.PkgName} from version {vuln.InstalledVersion} to {vuln.FixedVersion}";
        }

        // Generar nombre de vulnerabilidad
        string vulnName = vuln.Title ?? vuln.VulnerabilityID ?? "Trivy Container Vulnerability";
        if (!string.IsNullOrEmpty(vuln.PkgName))
        {
            vulnName += $" in {vuln.PkgName}";
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
            Impact = sanitizer.Sanitize(vuln.Description ?? "Container vulnerability detected"),
            ProofOfConcept = sanitizer.Sanitize(poc),
            Project = pro,
            UserId = user,
            Template = false,
            Language = pro.Language,
            CreatedDate = DateTime.Now.ToUniversalTime()
        };

        // Agregar CVE si está disponible
        if (!string.IsNullOrEmpty(vuln.VulnerabilityID) && vuln.VulnerabilityID.StartsWith("CVE-"))
        {
            vulnerability.cve = vuln.VulnerabilityID;
        }

        // Agregar CVSS si está disponible
        if (vuln.CVSS != null && vuln.CVSS.Count > 0)
        {
            var cvssInfo = vuln.CVSS.Values.FirstOrDefault();
            if (cvssInfo != null && cvssInfo.V3Score.HasValue)
            {
                vulnerability.CVSS3 = cvssInfo.V3Score.Value;
                if (!string.IsNullOrEmpty(cvssInfo.V3Vector))
                {
                    vulnerability.CVSSVector = cvssInfo.V3Vector;
                }
            }
        }

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

    private void ProcessTrivyMisconfiguration(TrivyMisconfiguration misconfig, TrivyResult result, TrivyReport report,
                                            Target target, Project pro, string user, HtmlSanitizer sanitizer)
    {
        if (string.IsNullOrEmpty(misconfig.ID) && string.IsNullOrEmpty(misconfig.Title))
            return;

        // Mapear severidad a riesgo
        VulnRisk risk = VulnRisk.Medium; // Default para misconfigurations
        if (!string.IsNullOrEmpty(misconfig.Severity))
        {
            switch (misconfig.Severity.ToUpper())
            {
                case "CRITICAL":
                    risk = VulnRisk.Critical;
                    break;
                case "HIGH":
                    risk = VulnRisk.High;
                    break;
                case "MEDIUM":
                    risk = VulnRisk.Medium;
                    break;
                case "LOW":
                    risk = VulnRisk.Low;
                    break;
                case "UNKNOWN":
                    risk = VulnRisk.Info;
                    break;
            }
        }

        // Construir descripción detallada
        string description = misconfig.Description ?? "";
        if (!string.IsNullOrEmpty(misconfig.ID))
        {
            description += $"\n\nMisconfiguration ID: {misconfig.ID}";
        }
        if (!string.IsNullOrEmpty(misconfig.Type))
        {
            description += $"\nType: {misconfig.Type}";
        }
        if (!string.IsNullOrEmpty(result.Target))
        {
            description += $"\nTarget: {result.Target}";
        }

        // Construir PoC
        string poc = "";
        if (!string.IsNullOrEmpty(report.ArtifactName))
        {
            poc += $"Container/Image: {report.ArtifactName}\n";
        }
        if (!string.IsNullOrEmpty(result.Target))
        {
            poc += $"Configuration File: {result.Target}\n";
        }
        if (!string.IsNullOrEmpty(misconfig.Type))
        {
            poc += $"Configuration Type: {misconfig.Type}\n";
        }

        // Generar nombre de vulnerabilidad
        string vulnName = misconfig.Title ?? misconfig.ID ?? "Trivy Configuration Issue";

        // Crear vulnerabilidad
        var vulnerability = new Vuln
        {
            Id = Guid.NewGuid(),
            Name = sanitizer.Sanitize(vulnName),
            Description = sanitizer.Sanitize(description),
            Risk = risk,
            Status = VulnStatus.Open,
            Remediation = sanitizer.Sanitize(misconfig.Resolution ?? "Review and fix the configuration issue"),
            RemediationComplexity = RemediationComplexity.Low,
            RemediationPriority = RemediationPriority.Medium,
            Impact = sanitizer.Sanitize(misconfig.Description ?? "Configuration misconfiguration detected"),
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

// Modelos para JSON de Trivy
public class TrivyReport
{
    public string SchemaVersion { get; set; } = "";
    public string ArtifactName { get; set; } = "";
    public string ArtifactType { get; set; } = "";
    public TrivyResult[] Results { get; set; } = Array.Empty<TrivyResult>();
}

public class TrivyResult
{
    public string Target { get; set; } = "";
    public string Class { get; set; } = "";
    public string Type { get; set; } = "";
    public TrivyVulnerability[] Vulnerabilities { get; set; } = Array.Empty<TrivyVulnerability>();
    public TrivyMisconfiguration[] Misconfigurations { get; set; } = Array.Empty<TrivyMisconfiguration>();
}

public class TrivyVulnerability
{
    public string VulnerabilityID { get; set; } = "";
    public string PkgName { get; set; } = "";
    public string InstalledVersion { get; set; } = "";
    public string FixedVersion { get; set; } = "";
    public string Severity { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Dictionary<string, TrivyCVSS> CVSS { get; set; } = new();
}

public class TrivyMisconfiguration
{
    public string Type { get; set; } = "";
    public string ID { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Message { get; set; } = "";
    public string Severity { get; set; } = "";
    public string Resolution { get; set; } = "";
}

public class TrivyCVSS
{
    public double? V2Score { get; set; }
    public string V2Vector { get; set; } = "";
    public double? V3Score { get; set; }
    public string V3Vector { get; set; } = "";
}