using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.Zap;

public class ZapParser : IZapParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public ZapParser(ITargetManager targetManager, IVulnManager vulnManager,
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

        XmlNodeList alerts = doc.GetElementsByTagName("alertitem");
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedSchemes.Add("data");

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            foreach (XmlNode alert in alerts)
            {
                string name = "";
                if (alert.SelectSingleNode("name") != null) { name = alert.SelectSingleNode("name").InnerText; }

                string description = "";
                if (alert.SelectSingleNode("desc") != null) { description = alert.SelectSingleNode("desc").InnerText; }

                string solution = "";
                if (alert.SelectSingleNode("solution") != null) { solution = alert.SelectSingleNode("solution").InnerText; }


                string riskcode = "";
                if (alert.SelectSingleNode("riskcode") != null) { riskcode = alert.SelectSingleNode("riskcode").InnerText; }



                string uri = "";
                if (alert.SelectSingleNode("uri") != null) { uri = alert.SelectSingleNode("uri").InnerText; }

                string param = "";
                if (alert.SelectSingleNode("param") != null) { param = alert.SelectSingleNode("param").InnerText; }

                string attack = "";
                if (alert.SelectSingleNode("attack") != null) { attack = alert.SelectSingleNode("attack").InnerText; }

                string evidence = "";
                if (alert.SelectSingleNode("evidence") != null) { evidence = alert.SelectSingleNode("evidence").InnerText; }

                string otherinfo = "";
                if (alert.SelectSingleNode("otherinfo") != null) { otherinfo = alert.SelectSingleNode("otherinfo").InnerText; }



                // Parse risk level from ZAP format
                VulnRisk risk = VulnRisk.Info;
                switch (riskcode.ToLower())
                {
                    case "3":
                        risk = VulnRisk.High;
                        break;
                    case "2":
                        risk = VulnRisk.Medium;
                        break;
                    case "1":
                        risk = VulnRisk.Low;
                        break;
                    case "0":
                        risk = VulnRisk.Info;
                        break;
                }

                // Parse target information
                Uri targetUri = null;
                string targetHost = "";
                int targetPort = 80;
                
                try
                {
                    targetUri = new Uri(uri);
                    targetHost = targetUri.Host;
                    targetPort = targetUri.Port;
                }
                catch
                {
                    // If URI parsing fails, try to extract host from uri string
                    if (!string.IsNullOrEmpty(uri))
                    {
                        var parts = uri.Split('/');
                        if (parts.Length > 2)
                        {
                            var hostPart = parts[2];
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
                            Description = $"Target imported from ZAP scan",
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
                if (!string.IsNullOrEmpty(otherinfo))
                {
                    detailedDescription += "\n\nAdditional Information:\n" + otherinfo;
                }
                if (!string.IsNullOrEmpty(param))
                {
                    detailedDescription += "\n\nParameter: " + param;
                }
                if (!string.IsNullOrEmpty(attack))
                {
                    detailedDescription += "\n\nAttack: " + attack;
                }
                if (!string.IsNullOrEmpty(evidence))
                {
                    detailedDescription += "\n\nEvidence: " + evidence;
                }

                // Build proof of concept
                string poc = "";
                if (!string.IsNullOrEmpty(uri))
                {
                    poc += "URL: " + uri + "\n";
                }
                if (!string.IsNullOrEmpty(param))
                {
                    poc += "Parameter: " + param + "\n";
                }
                if (!string.IsNullOrEmpty(attack))
                {
                    poc += "Attack Vector: " + attack + "\n";
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