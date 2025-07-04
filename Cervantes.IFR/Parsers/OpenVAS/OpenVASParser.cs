using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.OpenVAS;

public class OpenVASParser : IOpenVASParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public OpenVASParser(ITargetManager targetManager, IVulnManager vulnManager,
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

        XmlNodeList results = doc.GetElementsByTagName("result");
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedSchemes.Add("data");

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            foreach (XmlNode result in results)
            {
                string name = "";
                if (result.SelectSingleNode("name") != null) { name = result.SelectSingleNode("name").InnerText; }

                string description = "";
                if (result.SelectSingleNode("description") != null) { description = result.SelectSingleNode("description").InnerText; }

                string solution = "";
                if (result.SelectSingleNode("solution") != null) { solution = result.SelectSingleNode("solution").InnerText; }

                string references = "";
                XmlNodeList refs = result.SelectNodes("refs/ref");
                foreach (XmlNode refNode in refs)
                {
                    string refType = refNode.Attributes["type"]?.Value ?? "";
                    string refId = refNode.Attributes["id"]?.Value ?? "";
                    if (!string.IsNullOrEmpty(refType) && !string.IsNullOrEmpty(refId))
                    {
                        references += refType + ": " + refId + "\n";
                    }
                }

                string severity = "";
                if (result.SelectSingleNode("severity") != null) { severity = result.SelectSingleNode("severity").InnerText; }

                string threat = "";
                if (result.SelectSingleNode("threat") != null) { threat = result.SelectSingleNode("threat").InnerText; }

                string port = "";
                if (result.SelectSingleNode("port") != null) { port = result.SelectSingleNode("port").InnerText; }

                string host = "";
                XmlNode hostNode = result.SelectSingleNode("host");
                if (hostNode != null)
                {
                    host = hostNode.InnerText;
                    if (string.IsNullOrEmpty(host) && hostNode.Attributes["asset"] != null)
                    {
                        host = hostNode.Attributes["asset"].Value;
                    }
                }

                string nvtOid = "";
                XmlNode nvtNode = result.SelectSingleNode("nvt");
                if (nvtNode != null && nvtNode.Attributes["oid"] != null)
                {
                    nvtOid = nvtNode.Attributes["oid"].Value;
                }

                string family = "";
                if (nvtNode?.SelectSingleNode("family") != null) { family = nvtNode.SelectSingleNode("family").InnerText; }

                string cvss = "";
                if (nvtNode?.SelectSingleNode("cvss_base") != null) { cvss = nvtNode.SelectSingleNode("cvss_base").InnerText; }

                string cve = "";
                if (nvtNode?.SelectSingleNode("cve") != null) { cve = nvtNode.SelectSingleNode("cve").InnerText; }

                string tags = "";
                if (nvtNode?.SelectSingleNode("tags") != null) { tags = nvtNode.SelectSingleNode("tags").InnerText; }

                // Parse risk level from OpenVAS format
                VulnRisk risk = VulnRisk.Info;
                if (!string.IsNullOrEmpty(threat))
                {
                    switch (threat.ToLower())
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
                        case "log":
                        case "debug":
                        case "false positive":
                            risk = VulnRisk.Info;
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(severity))
                {
                    if (double.TryParse(severity, NumberStyles.Float, CultureInfo.InvariantCulture, out double severityScore))
                    {
                        if (severityScore >= 7.0)
                            risk = VulnRisk.High;
                        else if (severityScore >= 4.0)
                            risk = VulnRisk.Medium;
                        else if (severityScore > 0.0)
                            risk = VulnRisk.Low;
                        else
                            risk = VulnRisk.Info;
                    }
                }

                // Create or find target
                Target target = null;
                if (!string.IsNullOrEmpty(host))
                {
                    var targets = targetManager.GetAll().Where(x => x.Project.Id == project).ToList();
                    target = targets.FirstOrDefault(x => x.Name == host);

                    if (target == null)
                    {
                        target = new Target
                        {
                            Id = Guid.NewGuid(),
                            Name = host,
                            Description = $"Target imported from OpenVAS scan",
                            Type = TargetType.Hostname,
                            Project = pro,
                            UserId = user
                        };
                        targetManager.Add(target);
                        targetManager.Context.SaveChanges();
                    }
                }

                // Build detailed description
                string detailedDescription = description;
                if (!string.IsNullOrEmpty(family))
                {
                    detailedDescription += "\n\nFamily: " + family;
                }
                if (!string.IsNullOrEmpty(tags))
                {
                    detailedDescription += "\n\nTags: " + tags;
                }
                if (!string.IsNullOrEmpty(nvtOid))
                {
                    detailedDescription += "\n\nNVT OID: " + nvtOid;
                }

                // Build proof of concept
                string poc = "";
                if (!string.IsNullOrEmpty(host))
                {
                    poc += "Host: " + host + "\n";
                }
                if (!string.IsNullOrEmpty(port))
                {
                    poc += "Port: " + port + "\n";
                }
                if (!string.IsNullOrEmpty(severity))
                {
                    poc += "Severity Score: " + severity + "\n";
                }
                if (!string.IsNullOrEmpty(threat))
                {
                    poc += "Threat Level: " + threat + "\n";
                }

                // Build references string
                string referencesText = references;
                if (!string.IsNullOrEmpty(cve))
                {
                    referencesText += "\nCVE: " + cve;
                }
                if (!string.IsNullOrEmpty(cvss))
                {
                    referencesText += "\nCVSS Base Score: " + cvss;
                }

                // Skip non-vulnerability entries (informational only)
                if (risk == VulnRisk.Info && (threat == "Log" || threat == "Debug" || threat == "False Positive"))
                {
                    continue;
                }

                // Create vulnerability
                var vuln = new Vuln
                {
                    Id = Guid.NewGuid(),
                    Name = sanitizer.Sanitize(name),
                    Description = sanitizer.Sanitize(detailedDescription),
                    Risk = risk,
                    Status = VulnStatus.Open,
                    Remediation = sanitizer.Sanitize(solution),
                    RemediationComplexity = RemediationComplexity.Medium,
                    RemediationPriority = RemediationPriority.Medium,
                    Impact = sanitizer.Sanitize(description),
                    ProofOfConcept = sanitizer.Sanitize(poc),
                    Project = pro,
                    UserId = user,
                    Template = false,
                    Language = pro.Language,
                    CreatedDate = DateTime.Now.ToUniversalTime()
                };

                // Set CVE if available
                if (!string.IsNullOrEmpty(cve))
                {
                    vuln.cve = cve;
                }

                // Set CVSS if available
                if (!string.IsNullOrEmpty(cvss) && double.TryParse(cvss, NumberStyles.Float, CultureInfo.InvariantCulture, out double cvssScore))
                {
                    vuln.CVSS3 = cvssScore;
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
        }
    }
}