using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.Qualys;

public class QualysParser : IQualysParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public QualysParser(ITargetManager targetManager, IVulnManager vulnManager,
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

        // Qualys can have different XML structures, try both common formats
        XmlNodeList vulnerabilities = doc.GetElementsByTagName("VULN");
        if (vulnerabilities.Count == 0)
        {
            vulnerabilities = doc.GetElementsByTagName("vulnerability");
        }

        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedSchemes.Add("data");

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            foreach (XmlNode vuln in vulnerabilities)
            {
                string qid = "";
                if (vuln.Attributes["number"] != null) { qid = vuln.Attributes["number"].Value; }
                else if (vuln.SelectSingleNode("QID") != null) { qid = vuln.SelectSingleNode("QID").InnerText; }

                string title = "";
                if (vuln.SelectSingleNode("TITLE") != null) { title = vuln.SelectSingleNode("TITLE").InnerText; }
                else if (vuln.SelectSingleNode("title") != null) { title = vuln.SelectSingleNode("title").InnerText; }

                string category = "";
                if (vuln.SelectSingleNode("CATEGORY") != null) { category = vuln.SelectSingleNode("CATEGORY").InnerText; }
                else if (vuln.SelectSingleNode("category") != null) { category = vuln.SelectSingleNode("category").InnerText; }

                string severity = "";
                if (vuln.SelectSingleNode("SEVERITY") != null) { severity = vuln.SelectSingleNode("SEVERITY").InnerText; }
                else if (vuln.SelectSingleNode("severity") != null) { severity = vuln.SelectSingleNode("severity").InnerText; }

                string cvssBase = "";
                if (vuln.SelectSingleNode("CVSS_BASE") != null) { cvssBase = vuln.SelectSingleNode("CVSS_BASE").InnerText; }
                else if (vuln.SelectSingleNode("cvss_base") != null) { cvssBase = vuln.SelectSingleNode("cvss_base").InnerText; }

                string cvssVector = "";
                if (vuln.SelectSingleNode("CVSS_VECTOR") != null) { cvssVector = vuln.SelectSingleNode("CVSS_VECTOR").InnerText; }
                else if (vuln.SelectSingleNode("cvss_vector") != null) { cvssVector = vuln.SelectSingleNode("cvss_vector").InnerText; }

                string description = "";
                if (vuln.SelectSingleNode("DIAGNOSIS") != null) { description = vuln.SelectSingleNode("DIAGNOSIS").InnerText; }
                else if (vuln.SelectSingleNode("diagnosis") != null) { description = vuln.SelectSingleNode("diagnosis").InnerText; }
                else if (vuln.SelectSingleNode("description") != null) { description = vuln.SelectSingleNode("description").InnerText; }

                string consequence = "";
                if (vuln.SelectSingleNode("CONSEQUENCE") != null) { consequence = vuln.SelectSingleNode("CONSEQUENCE").InnerText; }
                else if (vuln.SelectSingleNode("consequence") != null) { consequence = vuln.SelectSingleNode("consequence").InnerText; }

                string solution = "";
                if (vuln.SelectSingleNode("SOLUTION") != null) { solution = vuln.SelectSingleNode("SOLUTION").InnerText; }
                else if (vuln.SelectSingleNode("solution") != null) { solution = vuln.SelectSingleNode("solution").InnerText; }

                string compliance = "";
                if (vuln.SelectSingleNode("COMPLIANCE") != null) { compliance = vuln.SelectSingleNode("COMPLIANCE").InnerText; }
                else if (vuln.SelectSingleNode("compliance") != null) { compliance = vuln.SelectSingleNode("compliance").InnerText; }

                // Get host information - can be in parent or sibling nodes
                string hostIp = "";
                string hostName = "";
                string port = "";
                string protocol = "";
                string service = "";

                // Look for host info in parent HOST_LIST_VM_DETECTION or similar
                XmlNode hostNode = vuln.ParentNode;
                while (hostNode != null && string.IsNullOrEmpty(hostIp))
                {
                    if (hostNode.SelectSingleNode("IP") != null) { hostIp = hostNode.SelectSingleNode("IP").InnerText; }
                    else if (hostNode.Attributes["ip"] != null) { hostIp = hostNode.Attributes["ip"].Value; }
                    
                    if (hostNode.SelectSingleNode("DNS") != null) { hostName = hostNode.SelectSingleNode("DNS").InnerText; }
                    else if (hostNode.Attributes["hostname"] != null) { hostName = hostNode.Attributes["hostname"].Value; }

                    hostNode = hostNode.ParentNode;
                }

                // Look for port/service info in current node or siblings
                if (vuln.SelectSingleNode("PORT") != null) { port = vuln.SelectSingleNode("PORT").InnerText; }
                else if (vuln.Attributes["port"] != null) { port = vuln.Attributes["port"].Value; }

                if (vuln.SelectSingleNode("PROTOCOL") != null) { protocol = vuln.SelectSingleNode("PROTOCOL").InnerText; }
                else if (vuln.Attributes["protocol"] != null) { protocol = vuln.Attributes["protocol"].Value; }

                if (vuln.SelectSingleNode("SERVICE") != null) { service = vuln.SelectSingleNode("SERVICE").InnerText; }

                // Parse risk level from Qualys format
                VulnRisk risk = VulnRisk.Info;
                if (!string.IsNullOrEmpty(severity))
                {
                    switch (severity)
                    {
                        case "5":
                            risk = VulnRisk.Critical;
                            break;
                        case "4":
                            risk = VulnRisk.High;
                            break;
                        case "3":
                            risk = VulnRisk.Medium;
                            break;
                        case "2":
                            risk = VulnRisk.Low;
                            break;
                        case "1":
                            risk = VulnRisk.Info;
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(cvssBase))
                {
                    if (double.TryParse(cvssBase, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedCvssScore))
                    {
                        if (parsedCvssScore >= 9.0)
                            risk = VulnRisk.Critical;
                        else if (parsedCvssScore >= 7.0)
                            risk = VulnRisk.High;
                        else if (parsedCvssScore >= 4.0)
                            risk = VulnRisk.Medium;
                        else if (parsedCvssScore > 0.0)
                            risk = VulnRisk.Low;
                        else
                            risk = VulnRisk.Info;
                    }
                }

                // Create or find target
                Target target = null;
                string targetName = !string.IsNullOrEmpty(hostName) ? hostName : hostIp;
                if (!string.IsNullOrEmpty(targetName))
                {
                    var targets = targetManager.GetAll().Where(x => x.Project.Id == project).ToList();
                    target = targets.FirstOrDefault(x => x.Name == targetName);

                    if (target == null)
                    {
                        target = new Target
                        {
                            Id = Guid.NewGuid(),
                            Name = targetName,
                            Description = $"Target imported from Qualys scan",
                            Type = !string.IsNullOrEmpty(hostName) ? TargetType.Hostname : TargetType.IP,
                            Project = pro,
                            UserId = user
                        };
                        targetManager.Add(target);
                        targetManager.Context.SaveChanges();
                    }
                }

                // Build detailed description
                string detailedDescription = description;
                if (!string.IsNullOrEmpty(category))
                {
                    detailedDescription += "\n\nCategory: " + category;
                }
                if (!string.IsNullOrEmpty(consequence))
                {
                    detailedDescription += "\n\nConsequence: " + consequence;
                }
                if (!string.IsNullOrEmpty(compliance))
                {
                    detailedDescription += "\n\nCompliance: " + compliance;
                }
                if (!string.IsNullOrEmpty(qid))
                {
                    detailedDescription += "\n\nQualys ID (QID): " + qid;
                }

                // Build proof of concept
                string poc = "";
                if (!string.IsNullOrEmpty(hostIp))
                {
                    poc += "Host IP: " + hostIp + "\n";
                }
                if (!string.IsNullOrEmpty(hostName))
                {
                    poc += "Hostname: " + hostName + "\n";
                }
                if (!string.IsNullOrEmpty(port))
                {
                    poc += "Port: " + port + "\n";
                }
                if (!string.IsNullOrEmpty(protocol))
                {
                    poc += "Protocol: " + protocol + "\n";
                }
                if (!string.IsNullOrEmpty(service))
                {
                    poc += "Service: " + service + "\n";
                }
                if (!string.IsNullOrEmpty(severity))
                {
                    poc += "Qualys Severity: " + severity + "\n";
                }

                // Build references
                string refs = "";
                if (!string.IsNullOrEmpty(cvssBase))
                {
                    refs += "CVSS Base Score: " + cvssBase + "\n";
                }
                if (!string.IsNullOrEmpty(cvssVector))
                {
                    refs += "CVSS Vector: " + cvssVector + "\n";
                }
                if (!string.IsNullOrEmpty(qid))
                {
                    refs += "Qualys QID: " + qid + "\n";
                }

                // Skip if no title or description
                if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(description))
                {
                    continue;
                }

                // Create vulnerability
                var vulnerability = new Vuln
                {
                    Id = Guid.NewGuid(),
                    Name = sanitizer.Sanitize(title),
                    Description = sanitizer.Sanitize(detailedDescription),
                    Risk = risk,
                    Status = VulnStatus.Open,
                    Remediation = sanitizer.Sanitize(solution),
                    RemediationComplexity = RemediationComplexity.Medium,
                    RemediationPriority = RemediationPriority.Medium,
                    Impact = sanitizer.Sanitize(string.IsNullOrEmpty(consequence) ? description : consequence),
                    ProofOfConcept = sanitizer.Sanitize(poc),
                    Project = pro,
                    UserId = user,
                    Template = false,
                    Language = pro.Language,
                    CreatedDate = DateTime.Now.ToUniversalTime()
                };

                // Set CVSS if available
                if (!string.IsNullOrEmpty(cvssBase) && double.TryParse(cvssBase, NumberStyles.Float, CultureInfo.InvariantCulture, out double finalCvssScore))
                {
                    vulnerability.CVSS3 = finalCvssScore;
                }

                if (!string.IsNullOrEmpty(cvssVector))
                {
                    vulnerability.CVSSVector = cvssVector;
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