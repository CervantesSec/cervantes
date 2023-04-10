using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.CORE;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace Cervantes.IFR.Parsers.CSV;

public class CsvParser: ICsvParser
{
    private IVulnManager vulnManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private IProjectManager projectManager = null;
    private readonly IHostingEnvironment _appEnvironment;

    public CsvParser(IVulnManager vulnManager, IHostingEnvironment _appEnvironment, IVulnCategoryManager vulnCategoryManager,
        IProjectManager projectManager)
    {
        this.vulnManager = vulnManager;
        this._appEnvironment = _appEnvironment;
        this.vulnCategoryManager = vulnCategoryManager;
        this.projectManager = projectManager;
    }

    public void Parse(Guid? project, string user, string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, config))
        {
                    
            var records = csv.GetRecords<VulnImportCsv>();

            if (project != null)
            {
                var pro = projectManager.GetById((Guid) project);
                    
                foreach (var vul in records)
                {
                    Vuln vuln = new Vuln();
                    vuln.Name = vul.Name == "" ? "No Name" : vul.Name;
                    
                    vuln.Template = vul.Template.HasValue && vul.Template.Value;
                    vuln.cve = vul.CVE == "" ? "None" : vul.CVE;
                    vuln.Description = vul.Description == "" ? "No Data" : vul.Description;
                    vuln.Impact = vul.Impact == "" ? "No Data" : vul.Impact;
                    vuln.Risk = vul.Risk ?? VulnRisk.Info;
                    vuln.Status = vul.Status ?? VulnStatus.Open;
                    vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                    vuln.JiraCreated = false;
                    vuln.Remediation = vul.Remediation == "" ? "No Data" : vul.Remediation;
                    vuln.RemediationPriority = vul.RemediationPriority ?? RemediationPriority.Low;
                    vuln.RemediationComplexity = vul.RemediationComplexity ?? RemediationComplexity.Low;
                    vuln.OWASPRisk = vul.OwaspRisk == "" ? "No Data" : vul.OwaspRisk;
                    vuln.OWASPImpact = vul.OwaspImpact == "" ? "No Data" : vul.OwaspImpact;
                    vuln.OWASPLikehood = vul.OwaspLikehood == "" ? "No Data" : vul.OwaspLikehood;
                    vuln.OWASPVector = vul.OwaspVector == "" ? "No Data" : vul.OwaspVector;
                    vuln.ProjectId = project;
                    vuln.ProofOfConcept = vul.Poc == "" ? "No Data" : vul.Poc;
                    vuln.UserId = user;
                    vuln.CVSS3 = vul.CVSS3 ?? 0;
                    vuln.CVSSVector = vul.CVSSVector == "" ? "No Data" : vul.CVSSVector;
                    var cat = vulnCategoryManager.GetByName(vul.VulnCategory);
                    if (cat != null)
                    {
                        vuln.VulnCategoryId = cat.Id;
                    }
                    else
                    {
                        continue;
                    }
                    var vulNum = vulnManager.GetAll().Count(x => x.ProjectId == project && x.Template == false) + 1;
                    vuln.FindingId = pro.FindingsId + "-" + vulNum.ToString("D2");
                    vulnManager.Add(vuln);
                    vulnManager.Context.SaveChanges();
                }
            }
            else
            {
                foreach (var vul in records)
                {
                    Vuln vuln = new Vuln();
                    vuln.Name = vul.Name == "" ? "No Name" : vul.Name;
                    
                    vuln.Template = vul.Template.HasValue && vul.Template.Value;
                    vuln.cve = vul.CVE == "" ? "None" : vul.CVE;
                    vuln.Description = vul.Description == "" ? "No Data" : vul.Description;
                    vuln.Impact = vul.Impact == "" ? "No Data" : vul.Impact;
                    vuln.Risk = vul.Risk ?? VulnRisk.Info;
                    vuln.Status = vul.Status ?? VulnStatus.Open;
                    vuln.CreatedDate = DateTime.Now.ToUniversalTime();
                    vuln.JiraCreated = false;
                    vuln.Remediation = vul.Remediation == "" ? "No Data" : vul.Remediation;
                    vuln.RemediationPriority = vul.RemediationPriority ?? RemediationPriority.Low;
                    vuln.RemediationComplexity = vul.RemediationComplexity ?? RemediationComplexity.Low;
                    vuln.OWASPRisk = vul.OwaspRisk == "" ? "No Data" : vul.OwaspRisk;
                    vuln.OWASPImpact = vul.OwaspImpact == "" ? "No Data" : vul.OwaspImpact;
                    vuln.OWASPLikehood = vul.OwaspLikehood == "" ? "No Data" : vul.OwaspLikehood;
                    vuln.OWASPVector = vul.OwaspVector == "" ? "No Data" : vul.OwaspVector;
                    vuln.ProjectId = null;
                    vuln.ProofOfConcept = vul.Poc == "" ? "No Data" : vul.Poc;
                    vuln.UserId = user;
                    vuln.CVSS3 = vul.CVSS3 ?? 0;
                    vuln.CVSSVector = vul.CVSSVector == "" ? "No Data" : vul.CVSSVector;
                    var cat = vulnCategoryManager.GetByName(vul.VulnCategory);
                    if (cat != null)
                    {
                        vuln.VulnCategoryId = cat.Id;
                    }
                    else
                    {
                        continue;
                    }
                    vulnManager.Add(vuln);
                    vulnManager.Context.SaveChanges();
                }
            }
                    

                    
        }
    }
}