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

namespace Cervantes.IFR.Parsers.Acunetix;

public class AcunetixParser : IAcunetixParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    // The finding ids of the import in progress; the project's ids are read once per import, not once per vuln.
    private FindingIdSequence findingIds = FindingIdSequence.Empty;
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

    /// <summary>
    /// The target of an Acunetix finding, from its own <c>Scan</c> (an export can hold several): the host of
    /// <c>Crawler/@StartUrl</c>, else of <c>StartURL</c> (often written without a scheme, e.g. <c>host:8080</c>), else of
    /// an absolute <c>Affects</c> (older exports), else <c>Affects</c> itself.
    /// </summary>
    public static string TargetOf(XmlNode reportItem)
    {
        static string? HostOf(string? url)
        {
            url = url?.Trim();
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            if (!url.Contains("://"))
            {
                url = "http://" + url;
            }
            return Uri.TryCreate(url, UriKind.Absolute, out var uri) && !string.IsNullOrEmpty(uri.Host) ? uri.Host : null;
        }

        var scan = reportItem.SelectSingleNode("ancestor::Scan");
        var affects = (reportItem.SelectSingleNode("Affects") ?? reportItem.SelectSingleNode("URL"))?.InnerText?.Trim() ?? "";
        return HostOf(scan?.SelectSingleNode("Crawler/@StartUrl")?.Value)
               ?? HostOf(scan?.SelectSingleNode("StartURL")?.InnerText)
               ?? (affects.Contains("://") ? HostOf(affects) : null)
               ?? affects;
    }

    /// <summary>The CVSS v3 score and vector of a finding (<c>CVSS3/Score</c>, <c>CVSS3/Descriptor</c>); the v2 block is not used.</summary>
    public static (double? Score, string? Vector) CvssOf(XmlNode reportItem)
    {
        var cvss3 = reportItem.SelectSingleNode("CVSS3");
        if (cvss3 == null)
        {
            return (null, null);
        }
        double? score = double.TryParse(cvss3.SelectSingleNode("Score")?.InnerText, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;
        var vector = cvss3.SelectSingleNode("Descriptor")?.InnerText?.Trim();
        return (score, string.IsNullOrEmpty(vector) ? null : vector);
    }

    /// <summary>The recorded HTTP request: <c>TechnicalDetails/Request</c>, or a direct <c>Request</c>/<c>HttpRequest</c> in older exports.</summary>
    public static string RequestOf(XmlNode reportItem)
    {
        return (reportItem.SelectSingleNode("TechnicalDetails/Request")
                ?? reportItem.SelectSingleNode("Request")
                ?? reportItem.SelectSingleNode("HttpRequest"))?.InnerText ?? "";
    }

    /// <summary>Every <c>CWEList/CWE</c> (or a direct <c>CWE</c>), comma-separated.</summary>
    public static string CwesOf(XmlNode reportItem)
    {
        var cwes = reportItem.SelectNodes("CWEList/CWE | CWE")!.Cast<XmlNode>().Select(x => x.InnerText.Trim()).Where(x => x.Length > 0);
        return string.Join(", ", cwes.Distinct());
    }

    /// <summary>The references of a finding, one per line: <c>References/Reference</c> database name and URL.</summary>
    public static string ReferencesOf(XmlNode reportItem)
    {
        return string.Join("\n", reportItem.SelectNodes("References/Reference")!.Cast<XmlNode>()
            .Select(x => $"{x.SelectSingleNode("Database")?.InnerText?.Trim()} {x.SelectSingleNode("URL")?.InnerText?.Trim()}".Trim())
            .Where(x => x.Length > 0));
    }

    /// <summary>
    /// The proof of concept of a finding as stored (HTML): affected path and parameter, then the request and any
    /// response in <c>&lt;pre&gt;</c>. All of it is plain text from the report (a parameter can be "&lt;empty&gt;"), so it is encoded.
    /// </summary>
    public static string PocOf(XmlNode reportItem)
    {
        var affects = (reportItem.SelectSingleNode("Affects") ?? reportItem.SelectSingleNode("URL"))?.InnerText ?? "";
        var parameter = reportItem.SelectSingleNode("Parameter")?.InnerText ?? "";
        var request = RequestOf(reportItem);
        var response = (reportItem.SelectSingleNode("Response") ?? reportItem.SelectSingleNode("HttpResponse"))?.InnerText ?? "";

        var poc = "";
        if (affects.Length > 0) poc += $"Affects: {WebUtility.HtmlEncode(affects)}\n";
        if (parameter.Length > 0) poc += $"Parameter: {WebUtility.HtmlEncode(parameter)}\n";
        if (request.Length > 0) poc += $"HTTP Request:\n<pre>{WebUtility.HtmlEncode(request)}</pre>\n";
        if (response.Length > 0) poc += $"HTTP Response:\n<pre>{WebUtility.HtmlEncode(response)}</pre>\n";
        return poc;
    }

    /// <summary>Loads an Acunetix XML export; anything else (for example the JSON of Acunetix 360) throws <see cref="XmlException"/>.</summary>
    public static XmlDocument LoadReport(string path)
    {
        var doc = new XmlDocument();
        doc.Load(path);
        return doc;
    }

    public void Parse(Guid? project, string user, string path)
    {
        findingIds = FindingIdSequence.Empty;
        XmlDocument doc = LoadReport(path);

        // Acunetix puede tener diferentes estructuras XML, buscamos vulnerabilidades
        XmlNodeList vulnerabilities = doc.GetElementsByTagName("Vulnerability");
        if (vulnerabilities.Count == 0)
        {
            vulnerabilities = doc.GetElementsByTagName("ReportItem");
        }

        var sanitizer = new HtmlSanitizer().AllowDataImagesOnly();

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            findingIds = FindingIdGenerator.Start(vulnManager, pro);
            
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

                // Attack details (HTML); TechnicalDetails only holds the request, which goes to the PoC
                string details = "";
                if (vuln.SelectSingleNode("Description") != null && vuln.SelectSingleNode("Details") != null) { details = vuln.SelectSingleNode("Details").InnerText; }

                string impact = "";
                if (vuln.SelectSingleNode("Impact") != null) { impact = vuln.SelectSingleNode("Impact").InnerText; }

                string recommendation = "";
                if (vuln.SelectSingleNode("Recommendation") != null) { recommendation = vuln.SelectSingleNode("Recommendation").InnerText; }
                else if (vuln.SelectSingleNode("Recommendations") != null) { recommendation = vuln.SelectSingleNode("Recommendations").InnerText; }

                string cwe = CwesOf(vuln);
                string references = ReferencesOf(vuln);
                var cvss = CvssOf(vuln);

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

                string targetHost = TargetOf(vuln);

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
                // Affects, parameter and the request are plain text (a parameter can be "<empty>"); Details is HTML
                if (!string.IsNullOrEmpty(parameter))
                {
                    detailedDescription += $"\n\nParameter: {WebUtility.HtmlEncode(parameter)}";
                }
                if (!string.IsNullOrEmpty(details))
                {
                    detailedDescription += $"\n\nAttack Details:\n{details}";
                }
                if (!string.IsNullOrEmpty(cwe))
                {
                    detailedDescription += $"\n\nCWE: {WebUtility.HtmlEncode(cwe)}";
                }
                if (cvss.Vector != null)
                {
                    detailedDescription += $"\n\nCVSS v3: {cvss.Score?.ToString(CultureInfo.InvariantCulture)} {WebUtility.HtmlEncode(cvss.Vector)}";
                }
                if (!string.IsNullOrEmpty(references))
                {
                    detailedDescription += $"\n\nReferences:\n{WebUtility.HtmlEncode(references)}";
                }

                string poc = PocOf(vuln);

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
                if (cvss.Score is { } cvssScore)
                {
                    vulnerability.CVSS3 = cvssScore;
                }
                if (cvss.Vector != null)
                {
                    vulnerability.CVSSVector = sanitizer.Sanitize(cvss.Vector);
                }

                vulnerability.FindingId = findingIds.Next();
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