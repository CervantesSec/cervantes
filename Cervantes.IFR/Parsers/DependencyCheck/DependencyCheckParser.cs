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
    // The finding ids of the import in progress; the project's ids are read once per import, not once per vuln.
    private FindingIdSequence findingIds = FindingIdSequence.Empty;
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
        findingIds = FindingIdSequence.Empty;
        var sanitizer = new HtmlSanitizer().AllowDataImagesOnly();

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            findingIds = FindingIdGenerator.Start(vulnManager, pro);
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
            else
            {
                throw new System.IO.InvalidDataException("Not a Dependency-Check JSON or XML report");
            }
        }
    }

    private void ParseJsonFormat(string fileContent, Project pro, string user, HtmlSanitizer sanitizer)
    {
        // Malformed JSON throws, so the import answers 400 instead of reporting success with nothing imported
        var dependencyCheckReport = Read(fileContent).FirstOrDefault();
        if (dependencyCheckReport != null)
        {
            ProcessDependencyCheckReport(dependencyCheckReport, pro, user, sanitizer);
        }
    }

    /// <summary>Reads a Dependency-Check JSON report (`--format JSON`). Throws <see cref="JsonException"/> if it is not valid JSON.</summary>
    public static System.Collections.Generic.IReadOnlyList<DependencyCheckReport> Read(string content)
    {
        var report = JsonSerializer.Deserialize<DependencyCheckReport>(content, ParserJson.Options);
        return report == null ? Array.Empty<DependencyCheckReport>() : new[] { report };
    }

    private void ParseXmlFormat(string path, Project pro, string user, HtmlSanitizer sanitizer)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        // A report without vulnerabilities imports nothing, not even the target
        var findings = ReadXml(doc);
        if (findings.Count == 0)
        {
            return;
        }
        Target target = CreateDependenciesTarget(pro, user);

        foreach (var finding in findings)
        {
            ProcessXmlVulnerability(finding, target, pro, user, sanitizer);
        }
    }

    /// <summary>
    /// Reads the vulnerabilities of a Dependency-Check XML report. Real reports declare a default namespace
    /// (<c>dependency-check.N.xsd</c>), so elements are matched by local name. The score is the old
    /// <c>cvssScore</c>, else <c>cvssV3/baseScore</c>, else <c>cvssV2/score</c>.
    /// </summary>
    public static IReadOnlyList<DependencyCheckXmlFinding> ReadXml(XmlDocument doc)
    {
        static string Text(XmlNode node, string path) =>
            node.SelectSingleNode(string.Join("/", path.Split('/').Select(x => $"*[local-name()='{x}']")))?.InnerText ?? "";

        var findings = new List<DependencyCheckXmlFinding>();
        foreach (XmlNode dependency in doc.SelectNodes("//*[local-name()='dependency']")!)
        {
            var fileName = Text(dependency, "fileName");
            var filePath = Text(dependency, "filePath");
            foreach (XmlNode vulnerability in dependency.SelectNodes("*[local-name()='vulnerabilities']/*[local-name()='vulnerability']")!)
            {
                var score = Text(vulnerability, "cvssScore");
                if (score.Length == 0) score = Text(vulnerability, "cvssV3/baseScore");
                if (score.Length == 0) score = Text(vulnerability, "cvssV2/score");
                // Only a v3 (else v4) score goes into the CVSS3 field; cvssScore and cvssV2 are version 2
                var cvss3 = Text(vulnerability, "cvssV3/baseScore");
                var vector = "";
                if (cvss3.Length > 0)
                {
                    vector = Text(vulnerability, "cvssV3/vectorString");
                    if (vector.Length == 0)
                    {
                        vector = Vector(Text(vulnerability, "cvssV3/version"), Text(vulnerability, "cvssV3/attackVector"),
                            Text(vulnerability, "cvssV3/attackComplexity"), Text(vulnerability, "cvssV3/privilegesRequired"),
                            Text(vulnerability, "cvssV3/userInteraction"), Text(vulnerability, "cvssV3/scope"),
                            Text(vulnerability, "cvssV3/confidentialityImpact"), Text(vulnerability, "cvssV3/integrityImpact"),
                            Text(vulnerability, "cvssV3/availabilityImpact"));
                    }
                }
                else
                {
                    cvss3 = Text(vulnerability, "cvssV4/baseScore");
                    vector = cvss3.Length > 0 ? Text(vulnerability, "cvssV4/vectorString") : "";
                }
                if (score.Length == 0) score = cvss3;
                findings.Add(new DependencyCheckXmlFinding(fileName, filePath, Text(vulnerability, "name"),
                    Text(vulnerability, "description"), Text(vulnerability, "severity"), score, cvss3, vector));
            }
        }
        return findings;
    }

    /// <summary>
    /// The CVSS v3 vector of a vulnerability: the report's <c>vectorString</c> (recent Dependency-Check), else one built from
    /// the eight metrics (older reports only have those), else "" when a metric is missing.
    /// </summary>
    public static string VectorOf(DependencyCheckCvssV3 v)
    {
        if (!string.IsNullOrWhiteSpace(v.VectorString))
        {
            return v.VectorString.Trim();
        }
        return Vector(v.Version, v.AttackVector, v.AttackComplexity, v.PrivilegesRequired, v.UserInteraction, v.Scope,
            v.ConfidentialityImpact, v.IntegrityImpact, v.AvailabilityImpact);
    }

    // CVSS:3.x/AV:_/AC:_/PR:_/UI:_/S:_/C:_/I:_/A:_ from the metric names Dependency-Check writes (NETWORK, LOW, ...).
    private static string Vector(string? version, params string?[] metrics)
    {
        string[] keys = { "AV", "AC", "PR", "UI", "S", "C", "I", "A" };
        var letters = metrics.Select(x => (x ?? "").Trim().ToUpperInvariant() switch
        {
            "NETWORK" => "N",
            "ADJACENT_NETWORK" or "ADJACENT" => "A",
            "LOCAL" => "L",
            "PHYSICAL" => "P",
            "LOW" => "L",
            "HIGH" => "H",
            "NONE" => "N",
            "REQUIRED" => "R",
            "UNCHANGED" => "U",
            "CHANGED" => "C",
            _ => "",
        }).ToArray();
        if (letters.Length != keys.Length || letters.Any(x => x.Length == 0))
        {
            return "";
        }
        var prefix = string.IsNullOrWhiteSpace(version) ? "3.1" : version.Trim();
        return $"CVSS:{prefix}/" + string.Join("/", keys.Select((key, i) => $"{key}:{letters[i]}"));
    }

    /// <summary>The risk of a Dependency-Check severity; the old reports' <c>Moderate</c> is Medium.</summary>
    public static VulnRisk RiskOf(string severity)
    {
        switch (severity.Trim().ToLowerInvariant())
        {
            case "critical": return VulnRisk.Critical;
            case "high": return VulnRisk.High;
            case "medium":
            case "moderate": return VulnRisk.Medium;
            case "low": return VulnRisk.Low;
            default: return VulnRisk.Info;
        }
    }

    private void ProcessDependencyCheckReport(DependencyCheckReport report, Project pro, string user, HtmlSanitizer sanitizer)
    {
        // A report without vulnerabilities imports nothing, not even the target
        if (!(report.Dependencies ?? Array.Empty<DependencyCheckDependency>()).Any(x => x.Vulnerabilities?.Length > 0))
        {
            return;
        }
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
        else if (vuln.CvssV4?.BaseScore != null)
        {
            risk = MapCvssToRisk(vuln.CvssV4.BaseScore.Value);
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
            vulnerability.CVSSVector = sanitizer.Sanitize(VectorOf(vuln.CvssV3));
        }
        else if (vuln.CvssV4?.BaseScore != null)
        {
            // No v3 score: the v4 one is the best there is (never the v2 score)
            vulnerability.CVSS3 = vuln.CvssV4.BaseScore.Value;
            vulnerability.CVSSVector = sanitizer.Sanitize(vuln.CvssV4.VectorString ?? "");
        }

        vulnerability.FindingId = findingIds.Next();
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

    private void ProcessXmlVulnerability(DependencyCheckXmlFinding finding,
                                       Target target, Project pro, string user, HtmlSanitizer sanitizer)
    {
        var (fileName, filePath, name, description, severity, cvssScore, _, _) = finding;
        
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

        if (double.TryParse(finding.Cvss3Score, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedScore))
        {
            vulnerability.CVSS3 = parsedScore;
        }
        if (!string.IsNullOrEmpty(finding.Vector))
        {
            vulnerability.CVSSVector = sanitizer.Sanitize(finding.Vector);
        }

        vulnerability.FindingId = findingIds.Next();
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

    private VulnRisk MapSeverityToRisk(string severity) => RiskOf(severity);
}

// One vulnerability of a dependency in the XML report
public record DependencyCheckXmlFinding(string FileName, string FilePath, string Name, string Description,
    string Severity, string CvssScore, string Cvss3Score = "", string Vector = "");

// Modelos para JSON de Dependency-Check
public class DependencyCheckReport
{
    public string ReportSchema { get; set; } = "";
    // Objects in the real report (engine version, data sources); not used by the import.
    public JsonElement ScanInfo { get; set; }
    public JsonElement ProjectInfo { get; set; }
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
    public DependencyCheckCvssV4 CvssV4 { get; set; } = new();
    public DependencyCheckReference[] References { get; set; } = Array.Empty<DependencyCheckReference>();
}

public class DependencyCheckReference
{
    public string Source { get; set; } = "";
    public string Url { get; set; } = "";
    public string Name { get; set; } = "";
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
    public string VectorString { get; set; } = "";
    public string Version { get; set; } = "";
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

// CVSS v4 block (cvssv4), written by Dependency-Check 10+
public class DependencyCheckCvssV4
{
    public double? BaseScore { get; set; }
    public string VectorString { get; set; } = "";
    public string BaseSeverity { get; set; } = "";
}
