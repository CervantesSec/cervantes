using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.Acunetix;

public class AcunetixParser : IAcunetixParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public AcunetixParser(ITargetManager targetManager, IVulnManager vulnManager,
        IVulnTargetManager vulnTargetManager, IProjectManager projectManager)
    {
        this.targetManager = targetManager;
        this.vulnManager = vulnManager;
        this.vulnTargetManager = vulnTargetManager;
        this.projectManager = projectManager;
    }

    public void Parse(Guid? project, string user, string path)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        // Acunetix puede tener diferentes estructuras XML, buscamos vulnerabilidades
        XmlNodeList vulnerabilities = doc.GetElementsByTagName("Vulnerability");
        if (vulnerabilities.Count == 0)
        {
            vulnerabilities = doc.GetElementsByTagName("ReportItem");
        }

        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedSchemes.Add("data");

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            
            foreach (XmlNode vuln in vulnerabilities)
            {
                string name = "";
                if (vuln.SelectSingleNode("Name") != null) { name = vuln.SelectSingleNode("Name").InnerText; }
                else if (vuln.SelectSingleNode("@name") != null) { name = vuln.Attributes["name"]?.Value ?? ""; }
                else if (vuln.SelectSingleNode("VulnName") != null) { name = vuln.SelectSingleNode("VulnName").InnerText; }

                string description = "";
                if (vuln.SelectSingleNode("Description") != null) { description = vuln.SelectSingleNode("Description").InnerText; }
                else if (vuln.SelectSingleNode("Details") != null) { description = vuln.SelectSingleNode("Details").InnerText; }

                string severity = "";
                if (vuln.SelectSingleNode("Severity") != null) { severity = vuln.SelectSingleNode("Severity").InnerText; }
                else if (vuln.SelectSingleNode("@severity") != null) { severity = vuln.Attributes["severity"]?.Value ?? ""; }

                string type = "";
                if (vuln.SelectSingleNode("Type") != null) { type = vuln.SelectSingleNode("Type").InnerText; }
                else if (vuln.SelectSingleNode("VulnType") != null) { type = vuln.SelectSingleNode("VulnType").InnerText; }

                string affects = "";
                if (vuln.SelectSingleNode("Affects") != null) { affects = vuln.SelectSingleNode("Affects").InnerText; }
                else if (vuln.SelectSingleNode("URL") != null) { affects = vuln.SelectSingleNode("URL").InnerText; }

                string parameter = "";
                if (vuln.SelectSingleNode("Parameter") != null) { parameter = vuln.SelectSingleNode("Parameter").InnerText; }

                string details = "";
                if (vuln.SelectSingleNode("TechnicalDetails") != null) { details = vuln.SelectSingleNode("TechnicalDetails").InnerText; }
                else if (vuln.SelectSingleNode("Details") != null) { details = vuln.SelectSingleNode("Details").InnerText; }

                string impact = "";
                if (vuln.SelectSingleNode("Impact") != null) { impact = vuln.SelectSingleNode("Impact").InnerText; }

                string recommendation = "";
                if (vuln.SelectSingleNode("Recommendation") != null) { recommendation = vuln.SelectSingleNode("Recommendation").InnerText; }
                else if (vuln.SelectSingleNode("Recommendations") != null) { recommendation = vuln.SelectSingleNode("Recommendations").InnerText; }

                string request = "";
                if (vuln.SelectSingleNode("Request") != null) { request = vuln.SelectSingleNode("Request").InnerText; }
                else if (vuln.SelectSingleNode("HttpRequest") != null) { request = vuln.SelectSingleNode("HttpRequest").InnerText; }

                string response = "";
                if (vuln.SelectSingleNode("Response") != null) { response = vuln.SelectSingleNode("Response").InnerText; }
                else if (vuln.SelectSingleNode("HttpResponse") != null) { response = vuln.SelectSingleNode("HttpResponse").InnerText; }

                string cwe = "";
                if (vuln.SelectSingleNode("CWE") != null) { cwe = vuln.SelectSingleNode("CWE").InnerText; }

                string cvss = "";
                if (vuln.SelectSingleNode("CVSS") != null) { cvss = vuln.SelectSingleNode("CVSS").InnerText; }
                else if (vuln.SelectSingleNode("CVSSScore") != null) { cvss = vuln.SelectSingleNode("CVSSScore").InnerText; }

                // Parse risk level from Acunetix format
                VulnRisk risk = VulnRisk.Info;
                if (!string.IsNullOrEmpty(severity))
                {
                    switch (severity.ToLower())
                    {
                        case "critical":
                        case "4":
                            risk = VulnRisk.Critical;
                            break;
                        case "high":
                        case "3":
                            risk = VulnRisk.High;
                            break;
                        case "medium":
                        case "2":
                            risk = VulnRisk.Medium;
                            break;
                        case "low":
                        case "1":
                            risk = VulnRisk.Low;
                            break;
                        case "informational":
                        case "info":
                        case "0":
                            risk = VulnRisk.Info;
                            break;
                    }
                }

                // Parse target information from affects/URL
                string targetHost = "";
                int targetPort = 80;
                
                if (!string.IsNullOrEmpty(affects))
                {
                    try
                    {
                        var uri = new Uri(affects);
                        targetHost = uri.Host;
                        targetPort = uri.Port;
                    }
                    catch
                    {
                        // If URI parsing fails, extract host manually
                        if (affects.Contains("://"))
                        {
                            var parts = affects.Split(new[] { "://" }, StringSplitOptions.None);
                            if (parts.Length > 1)
                            {
                                var hostPart = parts[1].Split('/')[0];
                                if (hostPart.Contains(':'))
                                {
                                    var hostPortParts = hostPart.Split(':');
                                    targetHost = hostPortParts[0];
                                    if (hostPortParts.Length > 1 && int.TryParse(hostPortParts[1], out int parsedPort))
                                    {
                                        targetPort = parsedPort;
                                    }
                                }
                                else
                                {
                                    targetHost = hostPart;
                                }
                            }
                        }
                        else
                        {
                            targetHost = affects;
                        }
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
                            Description = $"Target imported from Acunetix scan",
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
                string detailedDescription = description;
                if (!string.IsNullOrEmpty(type))
                {
                    detailedDescription += $"\n\nVulnerability Type: {type}";
                }
                if (!string.IsNullOrEmpty(parameter))
                {
                    detailedDescription += $"\n\nParameter: {parameter}";
                }
                if (!string.IsNullOrEmpty(details))
                {
                    detailedDescription += $"\n\nTechnical Details:\n{details}";
                }
                if (!string.IsNullOrEmpty(cwe))
                {
                    detailedDescription += $"\n\nCWE: {cwe}";
                }

                // Build proof of concept
                string poc = "";
                if (!string.IsNullOrEmpty(affects))
                {
                    poc += $"Affects: {affects}\n";
                }
                if (!string.IsNullOrEmpty(parameter))
                {
                    poc += $"Parameter: {parameter}\n";
                }
                if (!string.IsNullOrEmpty(request))
                {
                    poc += $"HTTP Request:\n{request}\n\n";
                }
                if (!string.IsNullOrEmpty(response))
                {
                    poc += $"HTTP Response:\n{response}\n";
                }

                // Skip if no name or description
                if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(description))
                {
                    continue;
                }

                // Create vulnerability
                var vulnerability = new Vuln
                {
                    Id = Guid.NewGuid(),
                    Name = sanitizer.Sanitize(name),
                    Description = sanitizer.Sanitize(detailedDescription),
                    Risk = risk,
                    Status = VulnStatus.Open,
                    Remediation = sanitizer.Sanitize(recommendation),
                    RemediationComplexity = RemediationComplexity.Medium,
                    RemediationPriority = RemediationPriority.Medium,
                    Impact = sanitizer.Sanitize(string.IsNullOrEmpty(impact) ? description : impact),
                    ProofOfConcept = sanitizer.Sanitize(poc),
                    Project = pro,
                    UserId = user,
                    Template = false,
                    Language = pro.Language,
                    CreatedDate = DateTime.Now.ToUniversalTime()
                };

                // Set CVSS if available
                if (!string.IsNullOrEmpty(cvss) && double.TryParse(cvss, NumberStyles.Float, CultureInfo.InvariantCulture, out double cvssScore))
                {
                    vulnerability.CVSS3 = cvssScore;
                }

                // Set CVE if available (some Acunetix reports include CVE)
                if (!string.IsNullOrEmpty(cwe) && cwe.ToUpper().StartsWith("CVE-"))
                {
                    vulnerability.cve = cwe;
                }

                vulnManager.Add(vulnerability);
                vulnManager.Context.SaveChanges();

                // Associate vulnerability with target if available
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
    }
}