using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cervantes.Contracts;
using Cervantes.CORE;
using Microsoft.AspNetCore.Hosting;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cervantes.IFR.Parsers.Pwndoc;

public class PwndocParser: IPwndocParser
{
    private readonly IHostingEnvironment _appEnvironment;
    private IVulnManager vulnManager = null;
    private IProjectManager projectManager = null;

    public PwndocParser(IVulnManager vulnManager, IHostingEnvironment _appEnvironment, IProjectManager projectManager )
    {
        this._appEnvironment = _appEnvironment;
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

            if (project != null)
            {
                var pro = projectManager.GetById((Guid) project);
                foreach (var vul in vulns)
                {
                    int i = 0;
                    Vuln vuln = new Vuln();
                    vuln.Name = vul.Details[i].Title == "" ? "No Name" : vul.Details[i].Title;
                    vuln.Description = vul.Details[i].Description  == "" ? "No Data" : vul.Details[i].Description;
                    vuln.Impact = vul.Details[i].Observation == "" ? "No Data" : vul.Details[i].Observation;
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
                    vuln.JiraCreated = false;
                    vuln.Remediation = vul.Details[i].Remediation == "" ? "No Data" : vul.Details[i].Remediation;
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
                    vuln.CVSSVector = vul.Cvssv3 == "" ? "No Data" : vul.Cvssv3;
                    var vulNum = vulnManager.GetAll().Count(x => x.ProjectId == project && x.Template == false) + 1;
                    vuln.FindingId = pro.FindingsId + "-" + vulNum.ToString("D2");
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
                    vuln.Name = vul.Details[i].Title == "" ? "No Name" : vul.Details[i].Title;
                    vuln.Description = vul.Details[i].Description  == "" ? "No Data" : vul.Details[i].Description;
                    vuln.Impact = vul.Details[i].Observation == "" ? "No Data" : vul.Details[i].Observation;
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
                    vuln.JiraCreated = false;
                    vuln.Remediation = vul.Details[i].Remediation == "" ? "No Data" : vul.Details[i].Remediation;
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
                    vuln.CVSSVector = vul.Cvssv3 == "" ? "No Data" : vul.Cvssv3;
                    vulnManager.Add(vuln);
                    vulnManager.Context.SaveChanges();
                    i++;
                }
            }
            
        }
    }

}