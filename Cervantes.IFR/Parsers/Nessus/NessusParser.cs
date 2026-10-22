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

namespace Cervantes.IFR.Parsers.Nessus;

public class NessusParser: INessusParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    // The finding ids of the import in progress; the project's ids are read once per import, not once per vuln.
    private FindingIdSequence findingIds = FindingIdSequence.Empty;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;
    public NessusParser(ITargetManager targetManager, IVulnManager vulnManager, 
        IVulnTargetManager vulnTargetManager, IProjectManager projectManager )
    {
        this.targetManager = targetManager;
        this.vulnManager = vulnManager;
        this.vulnTargetManager = vulnTargetManager;
        this.projectManager = projectManager;
    }
    /// <summary>
    /// The fields of one <c>ReportItem</c>, read from that item only (absent fields are empty or null). <c>Cves</c> joins
    /// every <c>&lt;cve&gt;</c>; port, protocol and service come from the item's attributes.
    /// </summary>
    public static NessusItem ReadItem(XmlNode reportItem)
    {
        string Child(string name) => reportItem.SelectSingleNode(name)?.InnerText ?? "";
        string Attribute(string name) => reportItem.Attributes?[name]?.Value ?? "";

        var pluginName = Child("plugin_name");
        double? cvss3 = double.TryParse(Child("cvss3_base_score"), NumberStyles.Float, CultureInfo.InvariantCulture, out var score)
            ? score
            : null;
        var cves = reportItem.SelectNodes("cve")!.Cast<XmlNode>().Select(x => x.InnerText.Trim()).Where(x => x.Length > 0).Distinct();
        return new NessusItem(
            pluginName.Length > 0 ? pluginName : Attribute("pluginName"),
            Child("risk_factor"),
            Child("description"),
            Child("solution"),
            Child("synopsis"),
            Child("plugin_output"),
            Child("see_also"),
            string.Join(", ", cves),
            cvss3,
            Child("cvss3_vector"),
            Attribute("port"),
            Attribute("protocol"),
            Attribute("svc_name"));
    }

    /// <summary>The text of <c>HostProperties/tag[@name=…]</c> of a <c>ReportHost</c>, or "" when it has none.</summary>
    public static string HostTag(XmlNode reportHost, string name)
    {
        foreach (XmlNode tag in reportHost.SelectNodes("HostProperties/tag")!)
        {
            if (tag.Attributes?["name"]?.Value == name)
            {
                return tag.InnerText;
            }
        }
        return "";
    }

    // Plain report text for a rich-text field.
    private static string Text(string value) => WebUtility.HtmlEncode(HttpUtility.HtmlDecode(value));

    public void Parse(Guid? project, string user, string path)
    {
        findingIds = FindingIdSequence.Empty;
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList hosts = doc.GetElementsByTagName("ReportHost");
        var sanitizer = new HtmlSanitizer().AllowDataImagesOnly();
        if (project != null)
        {
            var pro = projectManager.GetById((Guid) project);
            findingIds = FindingIdGenerator.Start(vulnManager, pro);
            foreach (XmlNode host in hosts)
            {

                string hostName = "";
                if(host.Attributes["name"] != null){hostName = host.Attributes["name"].Value; }
                string os = HostTag(host, "operating-system");
                string hostIp = HostTag(host, "host-ip");
                
                var targetName = sanitizer.Sanitize(HttpUtility.HtmlDecode(hostName));
                var tar = targetManager.GetByName(targetName, project);
                Target target = null;
                if (tar == null)
                {
                    target = new Target
                    {
                        UserId = user,
                        ProjectId = project,
                        Name = targetName,
                        Description = "<p>Host created from Nessus Scan Import</p>" + "<p>OS: "+ sanitizer.Sanitize(HttpUtility.HtmlDecode(os)) +"</p>"+ "<p>Host Ip: "+ sanitizer.Sanitize(HttpUtility.HtmlDecode(hostIp)) +"</p>",
                        Type = TargetType.Hostname
                    };

                    targetManager.Add(target);
                    targetManager.Context.SaveChanges();
                }
                
                XmlNodeList items = host.SelectNodes("ReportItem");

                foreach (XmlNode item in items)
                {
                    // Every field comes from this item only: a field it lacks must not keep the previous item's value
                    var read = ReadItem(item);
                    string description = read.Description;
                    string pluginName = read.PluginName;
                    string risk = read.RiskFactor;
                    string solution = read.Solution;
                    string pluginOutput = read.PluginOutput;
                    string cve = read.Cves;
                    string cvssVector = read.Cvss3Vector;
                    string impact = read.Synopsis;
                    string refrences = read.SeeAlso;
                    string service = string.Join(" ", new[] { read.Port, read.Protocol, read.Service }.Where(x => !string.IsNullOrEmpty(x)));
                    
                    var vuln = new Vuln();
                    vuln.FindingId = findingIds.Next();
                    vuln.Template = false;
                    vuln.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(pluginName));
                    vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                    vuln.ModifiedDate = DateTime.Now.ToUniversalTime();
                    vuln.UserId = user;
                    vuln.ProjectId = project;
                    vuln.VulnCategoryId = null;
                    switch (risk)
                    {
                        case "None":
                            vuln.Risk = VulnRisk.Info;
                            break;
                        case "Low":
                            vuln.Risk = VulnRisk.Low;
                            break;
                        case "Medium":
                            vuln.Risk = VulnRisk.Medium;
                            break;
                        case "High":
                            vuln.Risk = VulnRisk.High;
                            break;
                        case "Critical":
                            vuln.Risk = VulnRisk.Critical;
                            break;
                    }
                    vuln.Status = VulnStatus.Open;
                    vuln.cve = sanitizer.Sanitize(HttpUtility.HtmlDecode(cve));
                    // Nessus fields are plain text: encode them so "<" in a plugin output is shown, not parsed
                    vuln.Description = "<p>"+sanitizer.Sanitize(Text(description))+"</p>"+ "<p>References: " +"<p>"+sanitizer.Sanitize(Text(refrences))+"</p>";
                    vuln.ProofOfConcept = sanitizer.Sanitize((service.Length > 0 ? "<p>Port: " + Text(service) + "</p>" : "") +
                                                             (pluginOutput.Length > 0 ? "<pre>" + Text(pluginOutput) + "</pre>" : ""));
                    vuln.Impact = sanitizer.Sanitize(Text(impact));
                    vuln.CVSS3 = read.Cvss3 ?? 0;

                    vuln.CVSSVector = sanitizer.Sanitize(HttpUtility.HtmlDecode(cvssVector));
                    vuln.Remediation = sanitizer.Sanitize(Text(solution));
                    vuln.RemediationComplexity = RemediationComplexity.Low;
                    vuln.RemediationPriority = RemediationPriority.Low;
                    vuln.JiraCreated = false;
                    vuln.OWASPRisk = "No Data";
                    vuln.OWASPImpact = "No Data";
                    vuln.OWASPLikehood = "No Data";
                    vuln.OWASPVector = "No Data";
                    vuln.MitreTechniques = "";
                    vuln.MitreValues = "";

                    vulnManager.Add(vuln);
                    vulnManager.Context.SaveChanges();
                    
                    if (tar == null)
                    {
                        var vulnTarget = new VulnTargets
                        {
                            VulnId = vuln.Id,
                            TargetId = target.Id
                        };
                        vulnTargetManager.Add(vulnTarget);
                        vulnTargetManager.Context.SaveChanges();
                    }
                    else
                    {
                        var vulnTarget = new VulnTargets
                        {
                            VulnId = vuln.Id,
                            TargetId = tar.Id
                        };
                        vulnTargetManager.Add(vulnTarget);
                        vulnTargetManager.Context.SaveChanges();
                    }
                }


            }
        }
        else
        {
            foreach (XmlNode host in hosts)
            {
                
                XmlNodeList items = host.SelectNodes("ReportItem");

                foreach (XmlNode item in items)
                {
                    // Every field comes from this item only: a field it lacks must not keep the previous item's value
                    var read = ReadItem(item);
                    string description = read.Description;
                    string pluginName = read.PluginName;
                    string risk = read.RiskFactor;
                    string solution = read.Solution;
                    string pluginOutput = read.PluginOutput;
                    string cve = read.Cves;
                    string cvssVector = read.Cvss3Vector;
                    string impact = read.Synopsis;
                    string refrences = read.SeeAlso;
                    string service = string.Join(" ", new[] { read.Port, read.Protocol, read.Service }.Where(x => !string.IsNullOrEmpty(x)));
                    
                    var vuln = new Vuln();
                    vuln.Template = true; // no project: imported as a template
                    vuln.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(pluginName));
                    vuln.FindingId = "No Project";
                    vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                    vuln.ModifiedDate = DateTime.Now.ToUniversalTime();
                    vuln.UserId = user;
                    vuln.ProjectId = null;
                    vuln.VulnCategoryId = null;
                    switch (risk)
                    {
                        case "None":
                            vuln.Risk = VulnRisk.Info;
                            break;
                        case "Low":
                            vuln.Risk = VulnRisk.Low;
                            break;
                        case "Medium":
                            vuln.Risk = VulnRisk.Medium;
                            break;
                        case "High":
                            vuln.Risk = VulnRisk.High;
                            break;
                        case "Critical":
                            vuln.Risk = VulnRisk.Critical;
                            break;
                    }
                    vuln.Status = VulnStatus.Open;
                    vuln.cve = sanitizer.Sanitize(HttpUtility.HtmlDecode(cve));
                    // Nessus fields are plain text: encode them so "<" in a plugin output is shown, not parsed
                    vuln.Description = "<p>"+sanitizer.Sanitize(Text(description))+"</p>"+ "<p>References: " +"<p>"+sanitizer.Sanitize(Text(refrences))+"</p>";
                    vuln.ProofOfConcept = sanitizer.Sanitize((service.Length > 0 ? "<p>Port: " + Text(service) + "</p>" : "") +
                                                             (pluginOutput.Length > 0 ? "<pre>" + Text(pluginOutput) + "</pre>" : ""));
                    vuln.Impact = sanitizer.Sanitize(Text(impact));
                    vuln.CVSS3 = read.Cvss3 ?? 0;

                    vuln.CVSSVector = sanitizer.Sanitize(HttpUtility.HtmlDecode(cvssVector));
                    vuln.Remediation = sanitizer.Sanitize(Text(solution));
                    vuln.RemediationComplexity = RemediationComplexity.Low;
                    vuln.RemediationPriority = RemediationPriority.Low;
                    vuln.JiraCreated = false;
                    vuln.OWASPRisk = "No Data";
                    vuln.OWASPImpact = "No Data";
                    vuln.OWASPLikehood = "No Data";
                    vuln.OWASPVector = "No Data";
                    vuln.MitreTechniques = "";
                    vuln.MitreValues = "";
                    vulnManager.Add(vuln);
                    vulnManager.Context.SaveChanges();
                    
                    
                }


            }
        }
        
    }
}

// One ReportItem of a .nessus report
public sealed record NessusItem(string PluginName, string RiskFactor, string Description, string Solution, string Synopsis,
    string PluginOutput, string SeeAlso, string Cves, double? Cvss3, string Cvss3Vector, string Port, string Protocol, string Service);
