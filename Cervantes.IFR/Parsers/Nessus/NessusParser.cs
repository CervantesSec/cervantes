using System;
using System.Globalization;
using System.Linq;
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
    public void Parse(Guid? project, string user, string path)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList hosts = doc.GetElementsByTagName("ReportHost");
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedSchemes.Add("data");
        if (project != null)
        {
            var pro = projectManager.GetById((Guid) project);
            foreach (XmlNode host in hosts)
            {

                string hostName = "";
                if(host.Attributes["name"] != null){hostName = host.Attributes["name"].Value; }
                XmlNode hostPropertiesNode = host.SelectSingleNode("HostProperties");
                XmlNodeList hostTags = hostPropertiesNode.SelectNodes("tag");

                string os = "";
                string hostIp = "";
                foreach (XmlNode tag in hostTags)
                {
                    if(tag.Attributes["operating-system"] != null){os = tag.Attributes["operating-system"].InnerText;}
                    if(tag.Attributes["host-ip"] != null){hostIp = tag.Attributes["host-ip"].InnerText;}
                }
                
                var tar = targetManager.GetByName(hostName);
                Target target = null;
                if (tar == null)
                {
                    target = new Target
                    {
                        UserId = user,
                        ProjectId = project,
                        Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(hostName)),
                        Description = "<p>Host created from Nessus Scan Import</p>" + "<p>OS: "+ sanitizer.Sanitize(HttpUtility.HtmlDecode(os)) +"</p>"+ "<p>Host Ip: "+ sanitizer.Sanitize(HttpUtility.HtmlDecode(hostIp)) +"</p>",
                        Type = TargetType.Hostname
                    };

                    targetManager.Add(target);
                    targetManager.Context.SaveChanges();
                }
                
                XmlNodeList items = host.SelectNodes("ReportItem");

                string description = "";
                string pluginName = "";
                string risk = "";
                string solution = "";
                string pluginOutput = "";
                string cve = "";
                string cvss = "";
                string cvssVector = "";
                string impact = "";
                string refrences = "";
                
                foreach (XmlNode item in items)
                {
                    
                    if(item.SelectSingleNode("risk_factor") != null){risk = item.SelectSingleNode("risk_factor").InnerText;}
                    if(item.SelectSingleNode("plugin_name") != null){pluginName = item.SelectSingleNode("plugin_name").InnerText;}
                    if(item.SelectSingleNode("description") != null){description = item.SelectSingleNode("description").InnerText;}
                    if(item.SelectSingleNode("solution") != null){solution = item.SelectSingleNode("solution").InnerText;}
                    if(item.SelectSingleNode("plugin_output") != null){pluginOutput = item.SelectSingleNode("plugin_output").InnerText;}
                    if(item.SelectSingleNode("cve") != null){cve = item.SelectSingleNode("cve").InnerText;}
                    if(item.SelectSingleNode("cvss3_base_score") != null){cvss = item.SelectSingleNode("cvss3_base_score").InnerText;}
                    if(item.SelectSingleNode("cvss3_vector") != null){cvssVector = item.SelectSingleNode("cvss3_vector").InnerText;}
                    if(item.SelectSingleNode("synopsis") != null){impact = item.SelectSingleNode("synopsis").InnerText;}
                    if(item.SelectSingleNode("see_also") != null){refrences = item.SelectSingleNode("see_also").InnerText;}
                    
                    var vuln = new Vuln();
                    var vulNum = vulnManager.GetAll().Count(x => x.ProjectId == project && x.Template == false) + 1;
                    vuln.FindingId = pro.FindingsId + "-" + vulNum.ToString("D2");
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
                    vuln.Description = "<p>"+sanitizer.Sanitize(HttpUtility.HtmlDecode(description))+"</p>"+ "<p>References: " +"<p>"+sanitizer.Sanitize(HttpUtility.HtmlDecode(refrences))+"</p>";
                    vuln.ProofOfConcept = sanitizer.Sanitize(HttpUtility.HtmlDecode(pluginOutput));
                    vuln.Impact = sanitizer.Sanitize(HttpUtility.HtmlDecode(impact));
                    if (cvss != "")
                    {
                        vuln.CVSS3 = float.Parse(cvss,CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        vuln.CVSS3 = 0;
                    }
                    
                    vuln.CVSSVector = sanitizer.Sanitize(HttpUtility.HtmlDecode(cvssVector));
                    vuln.Remediation = sanitizer.Sanitize(HttpUtility.HtmlDecode(solution));
                    vuln.RemediationComplexity = RemediationComplexity.Low;
                    vuln.RemediationPriority = RemediationPriority.Low;
                    vuln.JiraCreated = false;
                    vuln.OWASPRisk = "No Data";
                    vuln.OWASPImpact = "No Data";
                    vuln.OWASPLikehood = "No Data";
                    vuln.OWASPVector = "No Data";

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

                string description = "";
                string pluginName = "";
                string risk = "";
                string solution = "";
                string pluginOutput = "";
                string cve = "";
                string cvss = "";
                string cvssVector = "";
                string impact = "";
                string refrences = "";
                
                foreach (XmlNode item in items)
                {
                    
                    if(item.SelectSingleNode("risk_factor") != null){risk = item.SelectSingleNode("risk_factor").InnerText;}
                    if(item.SelectSingleNode("plugin_name") != null){pluginName = item.SelectSingleNode("plugin_name").InnerText;}
                    if(item.SelectSingleNode("description") != null){description = item.SelectSingleNode("description").InnerText;}
                    if(item.SelectSingleNode("solution") != null){solution = item.SelectSingleNode("solution").InnerText;}
                    if(item.SelectSingleNode("plugin_output") != null){pluginOutput = item.SelectSingleNode("plugin_output").InnerText;}
                    if(item.SelectSingleNode("cve") != null){cve = item.SelectSingleNode("cve").InnerText;}
                    if(item.SelectSingleNode("cvss3_base_score") != null){cvss = item.SelectSingleNode("cvss3_base_score").InnerText;}
                    if(item.SelectSingleNode("cvss3_vector") != null){cvssVector = item.SelectSingleNode("cvss3_vector").InnerText;}
                    if(item.SelectSingleNode("synopsis") != null){impact = item.SelectSingleNode("synopsis").InnerText;}
                    if(item.SelectSingleNode("see_also") != null){refrences = item.SelectSingleNode("see_also").InnerText;}
                    
                    var vuln = new Vuln();
                    vuln.Template = false;
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
                    vuln.Description = "<p>"+sanitizer.Sanitize(HttpUtility.HtmlDecode(description))+"</p>"+ "<p>References: " +"<p>"+sanitizer.Sanitize(HttpUtility.HtmlDecode(refrences))+"</p>";
                    vuln.ProofOfConcept = sanitizer.Sanitize(HttpUtility.HtmlDecode(pluginOutput));
                    vuln.Impact = sanitizer.Sanitize(HttpUtility.HtmlDecode(impact));
                    if (cvss != "")
                    {
                        vuln.CVSS3 = float.Parse(cvss,CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        vuln.CVSS3 = 0;
                    }
                    
                    vuln.CVSSVector = sanitizer.Sanitize(HttpUtility.HtmlDecode(cvssVector));
                    vuln.Remediation = sanitizer.Sanitize(HttpUtility.HtmlDecode(solution));
                    vuln.RemediationComplexity = RemediationComplexity.Low;
                    vuln.RemediationPriority = RemediationPriority.Low;
                    vuln.JiraCreated = false;
                    vuln.OWASPRisk = "No Data";
                    vuln.OWASPImpact = "No Data";
                    vuln.OWASPLikehood = "No Data";
                    vuln.OWASPVector = "No Data";

                    vulnManager.Add(vuln);
                    vulnManager.Context.SaveChanges();
                    
                    
                }


            }
        }
        
    }
}