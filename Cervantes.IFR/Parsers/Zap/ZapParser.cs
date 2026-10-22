using System;
using System.Collections.Generic;
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
    // The finding ids of the import in progress; the project's ids are read once per import, not once per vuln.
    private FindingIdSequence findingIds = FindingIdSequence.Empty;
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

    /// <summary>
    /// The places an alert was found: <c>instances/instance</c> since ZAP 2.10, or the <c>uri</c>, <c>param</c> and
    /// <c>evidence</c> written directly on the alert by older versions.
    /// </summary>
    public static IReadOnlyList<(string Uri, string? Param, string? Evidence)> InstancesOf(XmlNode alertItem)
    {
        static string? Text(XmlNode node, string name)
        {
            var text = node.SelectSingleNode(name)?.InnerText;
            return string.IsNullOrEmpty(text) ? null : text;
        }

        var instances = alertItem.SelectNodes("instances/instance")!.Cast<XmlNode>()
            .Select(x => (Text(x, "uri") ?? "", Text(x, "param"), Text(x, "evidence")))
            .Where(x => x.Item1.Length > 0)
            .ToList();
        if (instances.Count == 0 && Text(alertItem, "uri") is { } uri)
        {
            instances.Add((uri, Text(alertItem, "param"), Text(alertItem, "evidence")));
        }
        return instances;
    }

    public void Parse(Guid? project, string user, string path)
    {
        findingIds = FindingIdSequence.Empty;
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList alerts = doc.GetElementsByTagName("alertitem");
        var sanitizer = new HtmlSanitizer().AllowDataImagesOnly();

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            findingIds = FindingIdGenerator.Start(vulnManager, pro);
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



                var instances = InstancesOf(alert);
                string uri = instances.Count > 0 ? instances[0].Uri : "";

                string attack = "";
                if (alert.SelectSingleNode("attack") != null) { attack = alert.SelectSingleNode("attack").InnerText; }

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

                // Reports group alerts by <site host="…">; the host there wins over the one taken from the URL
                var siteHost = alert.SelectSingleNode("ancestor::site")?.Attributes?["host"]?.Value;
                if (!string.IsNullOrEmpty(siteHost))
                {
                    targetHost = siteHost;
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
                // Build proof of concept
                string poc = "";
                // The description is HTML; evidence is often markup (a form, a header) that must show as text
                foreach (var instance in instances)
                {
                    detailedDescription += "\n\nURL: " + HttpUtility.HtmlEncode(instance.Uri);
                    poc += "URL: " + instance.Uri + "\n";
                    if (!string.IsNullOrEmpty(instance.Param))
                    {
                        detailedDescription += "\nParameter: " + HttpUtility.HtmlEncode(instance.Param);
                        poc += "Parameter: " + instance.Param + "\n";
                    }
                    if (!string.IsNullOrEmpty(instance.Evidence))
                    {
                        detailedDescription += "\nEvidence: " + HttpUtility.HtmlEncode(instance.Evidence);
                    }
                }
                if (!string.IsNullOrEmpty(attack))
                {
                    detailedDescription += "\n\nAttack: " + attack;
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

                vuln.FindingId = findingIds.Next();
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