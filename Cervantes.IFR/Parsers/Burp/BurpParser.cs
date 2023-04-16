using System;
using System.Linq;
using System.Xml;
using Cervantes.Contracts;
using Cervantes.CORE;
using Microsoft.AspNetCore.Hosting;

namespace Cervantes.IFR.Parsers.Burp;

public class BurpParser: IBurpParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private readonly IHostingEnvironment _appEnvironment;
    private IProjectManager projectManager = null;

    
    public BurpParser(ITargetManager targetManager, IHostingEnvironment _appEnvironment, IVulnManager vulnManager, 
        IVulnTargetManager vulnTargetManager, IProjectManager projectManager )
    {
        this.targetManager = targetManager;
        this._appEnvironment = _appEnvironment;
        this.vulnManager = vulnManager;
        this.vulnTargetManager = vulnTargetManager;
        this.projectManager = projectManager;
    }
    public void Parse(Guid? project, string user, string path)
    {
       
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList issues = doc.GetElementsByTagName("issue");

        if (project != null)
        {
            var pro = projectManager.GetById((Guid) project);
            foreach (XmlNode issue in issues)
            {
                string name = "";
                if(issue.SelectSingleNode("name") != null){name = issue.SelectSingleNode("name").InnerText;}
                
                string severity = "";
                if(issue.SelectSingleNode("severity") != null){severity = issue.SelectSingleNode("severity").InnerText;}
                
                string remediation = "";
                if(issue.SelectSingleNode("remediationBackground") != null){remediation = issue.SelectSingleNode("remediationBackground").InnerText;}
                
                string remediationDet = "";
                if(issue.SelectSingleNode("remediationDetail") != null){remediationDet = issue.SelectSingleNode("remediationDetail").InnerText;}
                
                string references = "";
                if(issue.SelectSingleNode("references") != null){references = issue.SelectSingleNode("references").InnerText;}
                
                string issueBack = "";
                if(issue.SelectSingleNode("issueBackground") != null){issueBack = issue.SelectSingleNode("issueBackground").InnerText;}
                
                string issueDet = "";
                if(issue.SelectSingleNode("issueDetail") != null){issueDet = issue.SelectSingleNode("issueDetail").InnerText;}
                
                string host = "";
                if(issue.SelectSingleNode("host") != null){host = issue.SelectSingleNode("host").InnerText;}
                
                string location = "";
                if(issue.SelectSingleNode("location") != null){location = issue.SelectSingleNode("location").InnerText;}
                
                string pathIssue = "";
                if(issue.SelectSingleNode("path") != null){pathIssue = issue.SelectSingleNode("path").InnerText;}
                
                string classifications = "";
                if(issue.SelectSingleNode("vulnerabilityClassifications") != null){classifications = issue.SelectSingleNode("vulnerabilityClassifications").InnerText;}
                
                string poc = "";
                if(issue.SelectSingleNode("poc") != null){poc = issue.SelectSingleNode("poc").InnerText;}


                var tar = targetManager.GetByName(host);
                Target target = null;
                if (tar == null)
                {
                    target = new Target
                    {
                        UserId = user,
                        ProjectId = project,
                        Name = host,
                        Description = "Host created from Burp Suite Scan Import",
                        Type = TargetType.URL
                    };

                    targetManager.Add(target);
                    targetManager.Context.SaveChanges();
                }


                var vuln = new Vuln();
                var vulNum = vulnManager.GetAll().Count(x => x.ProjectId == project && x.Template == false) + 1;
                vuln.FindingId = pro.FindingsId + "-" + vulNum.ToString("D2");
                vuln.Template = false;
                vuln.Name = name;
                vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                vuln.UserId = user;
                vuln.ProjectId = project;
                vuln.VulnCategoryId = null;
                switch (severity)
                {
                    case "Information":
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
                vuln.cve = "None";
                vuln.Description =  issueBack + "<br><p>Path: " + pathIssue + "</p>"+ "<p>Location: " + location + "</p><br>"+ "<p>References</p>"+references+classifications;
                vuln.ProofOfConcept = poc;
                vuln.Impact = issueDet;
                vuln.CVSS3 = 0;
                vuln.CVSSVector = null;
                vuln.Remediation = remediation + remediationDet;
                vuln.RemediationComplexity = RemediationComplexity.Low;
                vuln.RemediationPriority = RemediationPriority.Low;
                vuln.JiraCreated = false;
                vuln.OWASPRisk = null;
                vuln.OWASPImpact = null;
                vuln.OWASPLikehood = null;
                vuln.OWASPVector = null;

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
        else
        {
             foreach (XmlNode issue in issues)
            {
                string name = "";
                if(issue.SelectSingleNode("name") != null){name = issue.SelectSingleNode("name").InnerText;}
                
                string severity = "";
                if(issue.SelectSingleNode("severity") != null){severity = issue.SelectSingleNode("severity").InnerText;}
                
                string remediation = "";
                if(issue.SelectSingleNode("remediationBackground") != null){remediation = issue.SelectSingleNode("remediationBackground").InnerText;}
                
                string remediationDet = "";
                if(issue.SelectSingleNode("remediationDetail") != null){remediationDet = issue.SelectSingleNode("remediationDetail").InnerText;}
                
                string references = "";
                if(issue.SelectSingleNode("references") != null){references = issue.SelectSingleNode("references").InnerText;}
                
                string issueBack = "";
                if(issue.SelectSingleNode("issueBackground") != null){issueBack = issue.SelectSingleNode("issueBackground").InnerText;}
                
                string issueDet = "";
                if(issue.SelectSingleNode("issueDetail") != null){issueDet = issue.SelectSingleNode("issueDetail").InnerText;}
                
                string host = "";
                if(issue.SelectSingleNode("host") != null){host = issue.SelectSingleNode("host").InnerText;}
                
                string location = "";
                if(issue.SelectSingleNode("location") != null){location = issue.SelectSingleNode("location").InnerText;}
                
                string pathIssue = "";
                if(issue.SelectSingleNode("path") != null){pathIssue = issue.SelectSingleNode("path").InnerText;}
                
                string classifications = "";
                if(issue.SelectSingleNode("vulnerabilityClassifications") != null){classifications = issue.SelectSingleNode("vulnerabilityClassifications").InnerText;}
                
                string poc = "";
                if(issue.SelectSingleNode("poc") != null){poc = issue.SelectSingleNode("poc").InnerText;}
                
                var vuln = new Vuln();
                var vulNum = vulnManager.GetAll().Count(x => x.ProjectId == project && x.Template == false) + 1;
                vuln.Template = false;
                vuln.Name = name;
                vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                vuln.UserId = user;
                vuln.ProjectId = null;
                vuln.VulnCategoryId = null;
                switch (severity)
                {
                    case "Information":
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
                vuln.cve = "None";
                vuln.Description =  issueBack + "<br><p>Path: " + pathIssue + "</p>"+ "<p>Location: " + location + "</p><br>"+ "<p>References</p>"+references+classifications;
                vuln.ProofOfConcept = poc;
                vuln.Impact = issueDet;
                vuln.CVSS3 = 0;
                vuln.CVSSVector = null;
                vuln.Remediation = remediation + remediationDet;
                vuln.RemediationComplexity = RemediationComplexity.Low;
                vuln.RemediationPriority = RemediationPriority.Low;
                vuln.JiraCreated = false;
                vuln.OWASPRisk = null;
                vuln.OWASPImpact = null;
                vuln.OWASPLikehood = null;
                vuln.OWASPVector = null;

                vulnManager.Add(vuln);
                vulnManager.Context.SaveChanges();

            }
        }

       
    }
}