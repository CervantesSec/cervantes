using System;
using System.Globalization;
using System.Linq;
using System.Net;
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
    // The finding ids of the import in progress; the project's ids are read once per import, not once per vuln.
    private FindingIdSequence findingIds = FindingIdSequence.Empty;
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

    /// <summary>
    /// The address of an OpenVAS/GVM <c>&lt;host&gt;</c>: its own text. GVM nests <c>&lt;asset&gt;</c> and
    /// <c>&lt;hostname&gt;</c> inside it, so <c>InnerText</c> would glue the hostname to the address.
    /// </summary>
    public static string HostOf(XmlNode host)
    {
        var text = string.Concat(host.ChildNodes.OfType<XmlNode>()
            .Where(x => x.NodeType is XmlNodeType.Text or XmlNodeType.CDATA)
            .Select(x => x.Value)).Trim();
        return text.Length > 0 ? text : host.SelectSingleNode("hostname")?.InnerText.Trim() ?? "";
    }

    /// <summary>
    /// The remediation of a result: <c>nvt/solution</c> (GMP 20+), else the <c>solution=</c> part of the
    /// <c>|</c>-separated <c>nvt/tags</c> (older GMP), else a <c>solution</c> on the result itself, else "".
    /// </summary>
    public static string SolutionOf(XmlNode result)
    {
        var solution = result.SelectSingleNode("nvt/solution")?.InnerText;
        if (!string.IsNullOrWhiteSpace(solution))
        {
            return solution.Trim();
        }
        var fromTags = (result.SelectSingleNode("nvt/tags")?.InnerText ?? "").Split('|')
            .FirstOrDefault(x => x.StartsWith("solution=", StringComparison.Ordinal));
        if (fromTags != null && fromTags.Length > "solution=".Length)
        {
            return fromTags.Substring("solution=".Length).Trim();
        }
        return result.SelectSingleNode("solution")?.InnerText?.Trim() ?? "";
    }

    /// <summary>
    /// The CVSS vector of a result (GMP 20+): the <c>cvss_base_v3</c> entry of <c>nvt/severities</c>, else the
    /// <c>cvss_base_vector=</c> tag when it is a "CVSS:" vector; "" for older reports, which carry only a score.
    /// </summary>
    public static string VectorOf(XmlNode result)
    {
        var fromSeverities = result.SelectSingleNode("nvt/severities/severity[@type='cvss_base_v3']/value")?.InnerText?.Trim();
        if (!string.IsNullOrEmpty(fromSeverities))
        {
            return fromSeverities;
        }
        var fromTags = (result.SelectSingleNode("nvt/tags")?.InnerText ?? "").Split('|')
            .FirstOrDefault(x => x.StartsWith("cvss_base_vector=", StringComparison.Ordinal))?["cvss_base_vector=".Length..].Trim();
        return fromTags != null && fromTags.StartsWith("CVSS:", StringComparison.Ordinal) ? fromTags : "";
    }

    /// <summary>False for results GVM marks as <c>Log</c>, <c>Debug</c> or <c>False Positive</c>: they are not findings.</summary>
    public static bool IsFinding(XmlNode result)
    {
        var threat = result.SelectSingleNode("threat")?.InnerText?.Trim() ?? "";
        return threat is not ("Log" or "Debug" or "False Positive");
    }

    /// <summary>The CVEs of a result: every <c>ref type="cve"</c> (under the NVT or the result) and a legacy <c>nvt/cve</c>.</summary>
    public static string CvesOf(XmlNode result)
    {
        var fromRefs = result.SelectNodes("nvt/refs/ref[@type='cve'] | refs/ref[@type='cve']")!.Cast<XmlNode>()
            .Select(x => x.Attributes?["id"]?.Value ?? "");
        var legacy = (result.SelectSingleNode("nvt/cve")?.InnerText ?? "").Split(',', StringSplitOptions.TrimEntries);
        return string.Join(", ", fromRefs.Concat(legacy)
            .Where(x => x.StartsWith("CVE-", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase));
    }

    public void Parse(Guid? project, string user, string path)
    {
        findingIds = FindingIdSequence.Empty;
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList results = doc.GetElementsByTagName("result");
        var sanitizer = new HtmlSanitizer().AllowDataImagesOnly();

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            findingIds = FindingIdGenerator.Start(vulnManager, pro);
            foreach (XmlNode result in results)
            {
                string name = "";
                if (result.SelectSingleNode("name") != null) { name = result.SelectSingleNode("name").InnerText; }

                string description = "";
                if (result.SelectSingleNode("description") != null) { description = result.SelectSingleNode("description").InnerText; }

                string solution = SolutionOf(result);

                // GVM writes the references under the NVT; older reports had them on the result
                string references = "";
                XmlNodeList refs = result.SelectNodes("nvt/refs/ref | refs/ref");
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
                    host = HostOf(hostNode);
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

                string cve = CvesOf(result);

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

                // Log, debug and false-positive results are not findings: skip them before a target is created
                if (!IsFinding(result))
                {
                    continue;
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

                // Create vulnerability
                var vuln = new Vuln
                {
                    Id = Guid.NewGuid(),
                    Name = sanitizer.Sanitize(name),
                    Description = sanitizer.Sanitize(WebUtility.HtmlEncode(detailedDescription)),
                    Risk = risk,
                    Status = VulnStatus.Open,
                    Remediation = sanitizer.Sanitize(WebUtility.HtmlEncode(solution)),
                    RemediationComplexity = RemediationComplexity.Medium,
                    RemediationPriority = RemediationPriority.Medium,
                    Impact = sanitizer.Sanitize(WebUtility.HtmlEncode(description)),
                    ProofOfConcept = sanitizer.Sanitize(WebUtility.HtmlEncode(poc)),
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
                if (VectorOf(result) is { Length: > 0 } vector)
                {
                    vuln.CVSSVector = sanitizer.Sanitize(vector);
                }

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