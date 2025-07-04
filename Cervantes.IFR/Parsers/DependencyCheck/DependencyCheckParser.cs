using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.DependencyCheck;

public class DependencyCheckParser : IDependencyCheckParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public DependencyCheckParser(ITargetManager targetManager, IVulnManager vulnManager,
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
            
            // Detectar formato (XML o JSON)
            if (fileContent.TrimStart().StartsWith("{") || fileContent.TrimStart().StartsWith("["))
            {
                ParseJsonFormat(fileContent, pro, user, sanitizer);
            }
            else if (fileContent.TrimStart().StartsWith("<"))
            {
                ParseXmlFormat(path, pro, user, sanitizer);
            }
        }
    }

    private void ParseJsonFormat(string fileContent, Project pro, string user, HtmlSanitizer sanitizer)
    {
        try
        {
            var dependencyCheckReport = JsonSerializer.Deserialize<DependencyCheckReport>(fileContent);
            if (dependencyCheckReport != null)
            {
                ProcessDependencyCheckReport(dependencyCheckReport, pro, user, sanitizer);
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing Dependency-Check JSON: {ex.Message}");
        }
    }

    private void ParseXmlFormat(string path, Project pro, string user, HtmlSanitizer sanitizer)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        // Crear target para dependencias
        Target target = CreateDependenciesTarget(pro, user);

        XmlNodeList dependencies = doc.GetElementsByTagName("dependency");
        
        foreach (XmlNode dependency in dependencies)
        {
            string fileName = dependency.SelectSingleNode("fileName")?.InnerText ?? "";
            string filePath = dependency.SelectSingleNode("filePath")?.InnerText ?? "";
            
            XmlNodeList vulnerabilities = dependency.SelectNodes(".//vulnerability");
            foreach (XmlNode vulnerability in vulnerabilities)
            {
                ProcessXmlVulnerability(vulnerability, fileName, filePath, target, pro, user, sanitizer);
            }
        }
    }

    private void ProcessDependencyCheckReport(DependencyCheckReport report, Project pro, string user, HtmlSanitizer sanitizer)
    {
        // Crear target para dependencias
        Target target = CreateDependenciesTarget(pro, user);

        foreach (var dependency in report.Dependencies ?? new DependencyCheckDependency[0])
        {
            foreach (var vulnerability in dependency.Vulnerabilities ?? new DependencyCheckVulnerability[0])
            {
                ProcessJsonVulnerability(vulnerability, dependency, target, pro, user, sanitizer);
            }
        }
    }

    private Target CreateDependenciesTarget(Project pro, string user)
    {
        string targetName = "Project Dependencies";
        var targets = targetManager.GetAll().Where(x => x.Project.Id == pro.Id).ToList();
        var target = targets.FirstOrDefault(x => x.Name == targetName);

        if (target == null)
        {
            target = new Target
            {
                Id = Guid.NewGuid(),
                Name = targetName,
                Description = $"Dependencies imported from OWASP Dependency-Check scan",
                Type = TargetType.Hostname, // Usamos hostname para dependencias
                Project = pro,
                UserId = user
            };
            targetManager.Add(target);
            targetManager.Context.SaveChanges();
        }

        return target;
    }

    private void ProcessJsonVulnerability(DependencyCheckVulnerability vuln, DependencyCheckDependency dependency, 
                                        Target target, Project pro, string user, HtmlSanitizer sanitizer)
    {
        if (string.IsNullOrEmpty(vuln.Name) && string.IsNullOrEmpty(vuln.Description))
            return;

        // Mapear severidad CVSS a riesgo
        VulnRisk risk = VulnRisk.Medium; // Default para dependency vulnerabilities
        
        if (vuln.CvssV3?.BaseScore != null)
        {
            double cvssScore = vuln.CvssV3.BaseScore.Value;
            risk = MapCvssToRisk(cvssScore);
        }
        else if (vuln.CvssV2?.Score != null)
        {
            double cvssScore = vuln.CvssV2.Score.Value;
            risk = MapCvssToRisk(cvssScore);
        }
        else if (!string.IsNullOrEmpty(vuln.Severity))
        {
            risk = MapSeverityToRisk(vuln.Severity);
        }

        // Construir descripción detallada
        string description = vuln.Description ?? "";
        if (!string.IsNullOrEmpty(vuln.Name))
        {
            description += $"\n\nCVE ID: {vuln.Name}";
        }
        if (!string.IsNullOrEmpty(dependency.FileName))
        {
            description += $"\nAffected File: {dependency.FileName}";
        }
        if (!string.IsNullOrEmpty(dependency.FilePath))
        {
            description += $"\nFile Path: {dependency.FilePath}";
        }
        if (dependency.Packages?.Any() == true)
        {
            var package = dependency.Packages.First();
            if (!string.IsNullOrEmpty(package.Id))
            {
                description += $"\nPackage: {package.Id}";
            }
        }

        // Construir PoC
        string poc = "";
        if (!string.IsNullOrEmpty(dependency.FileName))
        {
            poc += $"Vulnerable Dependency: {dependency.FileName}\n";
        }
        if (!string.IsNullOrEmpty(dependency.FilePath))
        {
            poc += $"Location: {dependency.FilePath}\n";
        }
        if (vuln.CvssV3?.BaseScore != null)
        {
            poc += $"CVSS v3 Score: {vuln.CvssV3.BaseScore}\n";
        }
        if (!string.IsNullOrEmpty(vuln.Severity))
        {
            poc += $"Severity: {vuln.Severity}\n";
        }

        // Construir remediación
        string remediation = "Update the vulnerable dependency to a secure version";
        if (dependency.Packages?.Any() == true)
        {
            var package = dependency.Packages.First();
            remediation = $"Update {package.Id ?? dependency.FileName} to a version that addresses {vuln.Name}";
        }

        // Construir impacto
        string impact = vuln.Description ?? "Vulnerable dependency detected in project";

        // Generar nombre de vulnerabilidad
        string vulnName = vuln.Name ?? "Dependency Vulnerability";
        if (!string.IsNullOrEmpty(dependency.FileName))
        {
            vulnName += $" in {dependency.FileName}";
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
            RemediationPriority = RemediationPriority.High, // Dependencies son alta prioridad
            Impact = sanitizer.Sanitize(impact),
            ProofOfConcept = sanitizer.Sanitize(poc),
            Project = pro,
            UserId = user,
            Template = false,
            Language = pro.Language,
            CreatedDate = DateTime.Now.ToUniversalTime()
        };

        // Agregar CVE si está disponible
        if (!string.IsNullOrEmpty(vuln.Name) && vuln.Name.StartsWith("CVE-"))
        {
            vulnerability.cve = vuln.Name;
        }

        // Agregar CVSS si está disponible
        if (vuln.CvssV3?.BaseScore != null)
        {
            vulnerability.CVSS3 = vuln.CvssV3.BaseScore.Value;
            if (!string.IsNullOrEmpty(vuln.CvssV3.AttackVector))
            {
                vulnerability.CVSSVector = vuln.CvssV3.AttackVector;
            }
        }

        vulnManager.Add(vulnerability);
        vulnManager.Context.SaveChanges();

        // Asociar vulnerabilidad con target
        var vulnTarget = new VulnTargets
        {
            Id = Guid.NewGuid(),
            VulnId = vulnerability.Id,
            TargetId = target.Id
        };
        vulnTargetManager.Add(vulnTarget);
        vulnTargetManager.Context.SaveChanges();
    }

    private void ProcessXmlVulnerability(XmlNode vulnNode, string fileName, string filePath, 
                                       Target target, Project pro, string user, HtmlSanitizer sanitizer)
    {
        string name = vulnNode.SelectSingleNode("name")?.InnerText ?? "";
        string description = vulnNode.SelectSingleNode("description")?.InnerText ?? "";
        string severity = vulnNode.SelectSingleNode("severity")?.InnerText ?? "";
        string cvssScore = vulnNode.SelectSingleNode("cvssScore")?.InnerText ?? "";
        
        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description))
            return;

        VulnRisk risk = VulnRisk.Medium;
        if (!string.IsNullOrEmpty(cvssScore) && double.TryParse(cvssScore, NumberStyles.Float, CultureInfo.InvariantCulture, out double score))
        {
            risk = MapCvssToRisk(score);
        }
        else if (!string.IsNullOrEmpty(severity))
        {
            risk = MapSeverityToRisk(severity);
        }

        // Construir descripción
        string detailedDescription = description;
        if (!string.IsNullOrEmpty(name))
        {
            detailedDescription += $"\n\nCVE ID: {name}";
        }
        if (!string.IsNullOrEmpty(fileName))
        {
            detailedDescription += $"\nAffected File: {fileName}";
        }
        if (!string.IsNullOrEmpty(filePath))
        {
            detailedDescription += $"\nFile Path: {filePath}";
        }

        // Crear vulnerabilidad similar al método JSON
        var vulnerability = new Vuln
        {
            Id = Guid.NewGuid(),
            Name = sanitizer.Sanitize(name ?? "Dependency Vulnerability"),
            Description = sanitizer.Sanitize(detailedDescription),
            Risk = risk,
            Status = VulnStatus.Open,
            Remediation = sanitizer.Sanitize($"Update {fileName} to a version that addresses {name}"),
            RemediationComplexity = RemediationComplexity.Medium,
            RemediationPriority = RemediationPriority.High,
            Impact = sanitizer.Sanitize(description),
            ProofOfConcept = sanitizer.Sanitize($"Vulnerable Dependency: {fileName}\nLocation: {filePath}\nCVSS Score: {cvssScore}"),
            Project = pro,
            UserId = user,
            Template = false,
            Language = pro.Language,
            CreatedDate = DateTime.Now.ToUniversalTime()
        };

        if (!string.IsNullOrEmpty(name) && name.StartsWith("CVE-"))
        {
            vulnerability.cve = name;
        }

        if (!string.IsNullOrEmpty(cvssScore) && double.TryParse(cvssScore, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedScore))
        {
            vulnerability.CVSS3 = parsedScore;
        }

        vulnManager.Add(vulnerability);
        vulnManager.Context.SaveChanges();

        var vulnTarget = new VulnTargets
        {
            Id = Guid.NewGuid(),
            VulnId = vulnerability.Id,
            TargetId = target.Id
        };
        vulnTargetManager.Add(vulnTarget);
        vulnTargetManager.Context.SaveChanges();
    }

    private VulnRisk MapCvssToRisk(double cvssScore)
    {
        if (cvssScore >= 9.0) return VulnRisk.Critical;
        if (cvssScore >= 7.0) return VulnRisk.High;
        if (cvssScore >= 4.0) return VulnRisk.Medium;
        if (cvssScore > 0.0) return VulnRisk.Low;
        return VulnRisk.Info;
    }

    private VulnRisk MapSeverityToRisk(string severity)
    {
        switch (severity.ToLower())
        {
            case "critical": return VulnRisk.Critical;
            case "high": return VulnRisk.High;
            case "medium": return VulnRisk.Medium;
            case "low": return VulnRisk.Low;
            default: return VulnRisk.Info;
        }
    }
}

// Modelos para JSON de Dependency-Check
public class DependencyCheckReport
{
    public string ReportSchema { get; set; } = "";
    public string ScanInfo { get; set; } = "";
    public string ProjectInfo { get; set; } = "";
    public DependencyCheckDependency[] Dependencies { get; set; } = Array.Empty<DependencyCheckDependency>();
}

public class DependencyCheckDependency
{
    public string FileName { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string Md5 { get; set; } = "";
    public string Sha1 { get; set; } = "";
    public string Sha256 { get; set; } = "";
    public DependencyCheckPackage[] Packages { get; set; } = Array.Empty<DependencyCheckPackage>();
    public DependencyCheckVulnerability[] Vulnerabilities { get; set; } = Array.Empty<DependencyCheckVulnerability>();
}

public class DependencyCheckPackage
{
    public string Id { get; set; } = "";
    public string Confidence { get; set; } = "";
    public string Url { get; set; } = "";
}

public class DependencyCheckVulnerability
{
    public string Source { get; set; } = "";
    public string Name { get; set; } = "";
    public string Severity { get; set; } = "";
    public string Description { get; set; } = "";
    public string Notes { get; set; } = "";
    public DependencyCheckCvss CvssV2 { get; set; } = new();
    public DependencyCheckCvssV3 CvssV3 { get; set; } = new();
    public string[] References { get; set; } = Array.Empty<string>();
}

public class DependencyCheckCvss
{
    public double? Score { get; set; }
    public string AccessVector { get; set; } = "";
    public string AccessComplexity { get; set; } = "";
    public string Authentication { get; set; } = "";
    public string ConfidentialityImpact { get; set; } = "";
    public string IntegrityImpact { get; set; } = "";
    public string AvailabilityImpact { get; set; } = "";
    public string Severity { get; set; } = "";
}

public class DependencyCheckCvssV3
{
    public double? BaseScore { get; set; }
    public string AttackVector { get; set; } = "";
    public string AttackComplexity { get; set; } = "";
    public string PrivilegesRequired { get; set; } = "";
    public string UserInteraction { get; set; } = "";
    public string Scope { get; set; } = "";
    public string ConfidentialityImpact { get; set; } = "";
    public string IntegrityImpact { get; set; } = "";
    public string AvailabilityImpact { get; set; } = "";
    public string BaseSeverity { get; set; } = "";
}