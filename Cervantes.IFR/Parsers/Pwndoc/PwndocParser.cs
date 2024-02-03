using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cervantes.IFR.Parsers.Pwndoc;

public class PwndocParser: IPwndocParser
{
    private IVulnManager vulnManager = null;
    private IProjectManager projectManager = null;

    public PwndocParser(IVulnManager vulnManager, IProjectManager projectManager )
    {
        this.vulnManager = vulnManager;
        this.projectManager = projectManager;
    }

    public void Parse(Guid? project, string user, string path)
    {
        using (var reader = new StreamReader(path)) {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) 
                .Build();

            //yml contains a string containing your YAML
           var vulns = deserializer.Deserialize<List<PwndocVuln>>(reader);
           var sanitizer = new HtmlSanitizer();
           sanitizer.AllowedSchemes.Add("data");
           
            if (project != null)
            {
                var pro = projectManager.GetById((Guid) project);
                foreach (var vul in vulns)
                {
                    int i = 0;
                    Vuln vuln = new Vuln();
                    vuln.Name = vul.Details[i].Title == "" ? "No Name" : sanitizer.Sanitize(HttpUtility.HtmlDecode(vul.Details[i].Title));
                    vuln.cve = "No Data";
                    vuln.Description = vul.Details[i].Description  == "" ? "No Data" : sanitizer.Sanitize(HttpUtility.HtmlDecode(vul.Details[i].Description));
                    vuln.Impact = vul.Details[i].Observation == "" ? "No Data" : sanitizer.Sanitize(HttpUtility.HtmlDecode(vul.Details[i].Observation));
                    switch (vul.Priority)
                    {
                        case 1:
                            vuln.Risk = VulnRisk.Low;
                            break;
                        case 2:
                            vuln.Risk = VulnRisk.Medium;
                            break;
                        case 3:
                            vuln.Risk = VulnRisk.High;
                            break;
                        case 4:
                            vuln.Risk = VulnRisk.Critical;
                            break;
                        
                    }
                    vuln.Status = VulnStatus.Open;
                    vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                    vuln.ModifiedDate = DateTime.Now.ToUniversalTime();
                    vuln.JiraCreated = false;
                    vuln.Remediation = vul.Details[i].Remediation == "" ? "No Data" : sanitizer.Sanitize(HttpUtility.HtmlDecode(vul.Details[i].Remediation));
                    switch (vul.Priority)
                    {
                        case 1:
                            vuln.RemediationPriority = RemediationPriority.Low;
                            break;
                        case 2:
                            vuln.RemediationPriority = RemediationPriority.Medium;
                            break;
                        case 3:
                            vuln.RemediationPriority = RemediationPriority.High;
                            break;
                        case 4:
                            vuln.RemediationPriority = RemediationPriority.High;
                            break;
                        
                    }
                    switch (vul.RemediationComplexity)
                    {
                        case 1:
                            vuln.RemediationComplexity = RemediationComplexity.Low;
                            break;
                        case 2:
                            vuln.RemediationComplexity = RemediationComplexity.Medium;
                            break;
                        case 3:
                            vuln.RemediationComplexity = RemediationComplexity.High;
                            break;
                        case 4:
                            vuln.RemediationComplexity = RemediationComplexity.High;
                            break;
                        
                    }                    
                    vuln.ProjectId = project;
                    vuln.ProofOfConcept = "No Data";
                    vuln.UserId = user;
                    vuln.CVSSVector = vul.Cvssv3 == null ? "No Data" : vul.Cvssv3;
                    var vulNum = vulnManager.GetAll().Count(x => x.ProjectId == project && x.Template == false) + 1;
                    vuln.FindingId = pro.FindingsId + "-" + vulNum.ToString("D2");
                    vuln.OWASPRisk = "No Data";
                    vuln.OWASPImpact = "No Data";
                    vuln.OWASPLikehood = "No Data";
                    vuln.OWASPVector = "No Data";
                    vulnManager.Add(vuln);
                    vulnManager.Context.SaveChanges();
                    i++;
                }

            }
            else
            {
                foreach (var vul in vulns)
                {
                    int i = 0;
                    Vuln vuln = new Vuln();
                    vuln.Name = vul.Details[i].Title == "" ? "No Name" : sanitizer.Sanitize(HttpUtility.HtmlDecode(vul.Details[i].Title));
                    vuln.FindingId = "No Project";
                    vuln.cve = "No Data";
                    vuln.Description = vul.Details[i].Description  == "" ? "No Data" : sanitizer.Sanitize(HttpUtility.HtmlDecode(vul.Details[i].Description));
                    vuln.Impact = vul.Details[i].Observation == "" ? "No Data" : sanitizer.Sanitize(HttpUtility.HtmlDecode(vul.Details[i].Observation));
                    switch (vul.Priority)
                    {
                        case 1:
                            vuln.Risk = VulnRisk.Low;
                            break;
                        case 2:
                            vuln.Risk = VulnRisk.Medium;
                            break;
                        case 3:
                            vuln.Risk = VulnRisk.High;
                            break;
                        case 4:
                            vuln.Risk = VulnRisk.Critical;
                            break;
                        
                    }
                    vuln.Status = VulnStatus.Open;
                    vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                    vuln.ModifiedDate = DateTime.Now.ToUniversalTime();
                    vuln.JiraCreated = false;
                    vuln.Remediation = vul.Details[i].Remediation == "" ? "No Data" : sanitizer.Sanitize(HttpUtility.HtmlDecode(vul.Details[i].Remediation));
                    switch (vul.Priority)
                    {
                        case 1:
                            vuln.RemediationPriority = RemediationPriority.Low;
                            break;
                        case 2:
                            vuln.RemediationPriority = RemediationPriority.Medium;
                            break;
                        case 3:
                            vuln.RemediationPriority = RemediationPriority.High;
                            break;
                        case 4:
                            vuln.RemediationPriority = RemediationPriority.High;
                            break;
                        
                    }
                    switch (vul.RemediationComplexity)
                    {
                        case 1:
                            vuln.RemediationComplexity = RemediationComplexity.Low;
                            break;
                        case 2:
                            vuln.RemediationComplexity = RemediationComplexity.Medium;
                            break;
                        case 3:
                            vuln.RemediationComplexity = RemediationComplexity.High;
                            break;
                        case 4:
                            vuln.RemediationComplexity = RemediationComplexity.High;
                            break;
                        
                    }                    
                    vuln.ProjectId = null;
                    vuln.ProofOfConcept = "No Data";
                    vuln.UserId = user;
                    vuln.CVSSVector = vul.Cvssv3 == null ? "No Data" : vul.Cvssv3;
                    vuln.OWASPRisk = "No Data";
                    vuln.OWASPImpact = "No Data";
                    vuln.OWASPLikehood = "No Data";
                    vuln.OWASPVector = "No Data";
                    vulnManager.Add(vuln);
                    vulnManager.Context.SaveChanges();
                    i++;
                }
            }
            
        }
    }

}