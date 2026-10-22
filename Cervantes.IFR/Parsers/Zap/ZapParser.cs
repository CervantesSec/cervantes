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
    /// The places an alert was found: <c>instances/instance</c> since ZAP 2.10 (URI, method, parameter, attack, evidence
    /// and the instance's other info), or the same fields written directly on the alert by older versions. Empty
    /// elements are null.
    /// </summary>
    public static IReadOnlyList<ZapInstance> InstancesOf(XmlNode alertItem)
    {
        static string? Text(XmlNode node, string name)
        {
            var text = node.SelectSingleNode(name)?.InnerText;
            return string.IsNullOrEmpty(text) ? null : text;
        }

        static ZapInstance Read(XmlNode node, string uri) =>
            new(uri, Text(node, "method"), Text(node, "param"), Text(node, "attack"), Text(node, "evidence"), Text(node, "otherinfo"));

        var instances = alertItem.SelectNodes("instances/instance")!.Cast<XmlNode>()
            .Where(x => Text(x, "uri") != null)
            .Select(x => Read(x, Text(x, "uri")!))
            .ToList();
        if (instances.Count == 0 && Text(alertItem, "uri") is { } uri)
        {
            // Older reports: the fields are on the alert; its otherinfo is the alert's, shown once by Parse
            instances.Add(Read(alertItem, uri) with { OtherInfo = null });
        }
        return instances;
    }

    /// <summary>
    /// The description of a ZAP alert: its HTML <c>desc</c>, its other information, and every instance's URL, method,
    /// parameter, attack, evidence and other information as encoded text (evidence is often markup: a form, a header).
    /// An instance's other information that only repeats the alert's is left out (ZAP 2.17 writes it in both places).
    /// </summary>
    public static string DescriptionOf(string description, string otherInfo, IReadOnlyList<ZapInstance> instances)
    {
        var text = description;
        if (!string.IsNullOrEmpty(otherInfo))
        {
            text += "\n\nAdditional Information:\n" + otherInfo;
        }
        foreach (var instance in instances)
        {
            text += "\n\nURL: " + HttpUtility.HtmlEncode(instance.Uri);
            if (!string.IsNullOrEmpty(instance.Method)) text += "\nMethod: " + HttpUtility.HtmlEncode(instance.Method);
            if (!string.IsNullOrEmpty(instance.Param)) text += "\nParameter: " + HttpUtility.HtmlEncode(instance.Param);
            if (!string.IsNullOrEmpty(instance.Attack)) text += "\nAttack: " + HttpUtility.HtmlEncode(instance.Attack);
            if (!string.IsNullOrEmpty(instance.Evidence)) text += "\nEvidence: " + HttpUtility.HtmlEncode(instance.Evidence);
            if (!string.IsNullOrEmpty(instance.OtherInfo) && PlainText(instance.OtherInfo) != PlainText(otherInfo))
            {
                text += "\nOther Information: " + HttpUtility.HtmlEncode(instance.OtherInfo);
            }
        }
        return text;
    }

    // The alert writes its other information as HTML (<p>...</p>), the instance as plain text: compare the words only.
    private static string PlainText(string value) =>
        System.Text.RegularExpressions.Regex.Replace(HttpUtility.HtmlDecode(System.Text.RegularExpressions.Regex.Replace(value, "<[^>]+>", " ")), @"\s+", " ").Trim();

    /// <summary>The proof of concept of a ZAP alert: URL, method, parameter and attack of every instance, encoded.</summary>
    public static string PocOf(IReadOnlyList<ZapInstance> instances)
    {
        var poc = "";
        foreach (var instance in instances)
        {
            poc += "URL: " + HttpUtility.HtmlEncode(instance.Uri) + "\n";
            if (!string.IsNullOrEmpty(instance.Method)) poc += "Method: " + HttpUtility.HtmlEncode(instance.Method) + "\n";
            if (!string.IsNullOrEmpty(instance.Param)) poc += "Parameter: " + HttpUtility.HtmlEncode(instance.Param) + "\n";
            if (!string.IsNullOrEmpty(instance.Attack)) poc += "Attack Vector: " + HttpUtility.HtmlEncode(instance.Attack) + "\n";
        }
        return poc;
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

                string detailedDescription = DescriptionOf(description, otherinfo, instances);
                string poc = PocOf(instances);


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
// One place a ZAP alert was found
public sealed record ZapInstance(string Uri, string? Method, string? Param, string? Attack, string? Evidence, string? OtherInfo);
