using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using HtmlToOpenXml;
using Microsoft.AspNetCore.Authorization;
using MimeDetective;

namespace Cervantes.Web.Controllers;
[Authorize(Roles = "Admin,SuperUser,User")]
public class ReportController : Controller
{
    private readonly ILogger<ReportController> _logger = null;
    private readonly IHostingEnvironment _appEnvironment;
    private IProjectManager projectManager = null;
    private IClientManager clientManager = null;
    private IProjectUserManager projectUserManager = null;
    private IProjectNoteManager projectNoteManager = null;
    private IProjectAttachmentManager projectAttachmentManager = null;
    private ITargetManager targetManager = null;
    private ITaskManager taskManager = null;
    private IUserManager userManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IOrganizationManager organizationManager = null;
    private IReportManager reportManager = null;
    private IReportTemplateManager reportTemplateManager = null;

    public ReportController(IProjectManager projectManager, IClientManager clientManager,
        IOrganizationManager organizationManager, IProjectUserManager projectUserManager,
        IProjectNoteManager projectNoteManager, IVulnTargetManager vulnTargetManager,
        IProjectAttachmentManager projectAttachmentManager, ITargetManager targetManager, ITaskManager taskManager,
        IUserManager userManager, IVulnManager vulnManager, IHostingEnvironment _appEnvironment,
        ILogger<ReportController> logger, IReportManager reportManager,IReportTemplateManager reportTemplateManager )
    {
        this.projectManager = projectManager;
        this.clientManager = clientManager;
        this.organizationManager = organizationManager;
        this.projectUserManager = projectUserManager;
        this.projectNoteManager = projectNoteManager;
        this.projectAttachmentManager = projectAttachmentManager;
        this.targetManager = targetManager;
        this.taskManager = taskManager;
        this.userManager = userManager;
        this.vulnManager = vulnManager;
        this.vulnTargetManager = vulnTargetManager;
        this._appEnvironment = _appEnvironment;
        this.reportManager = reportManager;
        this.reportTemplateManager = reportTemplateManager;
        _logger = logger;
    }
    
    public ActionResult Download(Guid id)
    {
        var report = reportManager.GetById(id);

        var filePath = _appEnvironment.WebRootPath + report.FilePath;
        var fileName = report.Project.Name + "_" + report.Name + "_v" + report.Version + ".docx";

        var fileBytes = System.IO.File.ReadAllBytes(filePath);

        return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
    }

    [HttpPost]
    public ActionResult Delete(Guid id)
    {
        try
        {
            var report = reportManager.GetById(id);
            System.IO.File.Delete(_appEnvironment.WebRootPath+report.FilePath);
            reportManager.Remove(report);
            reportManager.Context.SaveChanges();

            _logger.LogInformation("User: {0} deleted report {1} in Project: {2}", User.FindFirstValue(ClaimTypes.Name),
                id, report.ProjectId);
            TempData["reportDeleted"] = "report deleted";
            return RedirectToAction("Details", "Project", new {id = report.ProjectId});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred deleting report for Project: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index", "Project");
        }
    }


    public IActionResult Details(Guid id)
    {
        try
        {
            var report = reportManager.GetById(id);
            var model = new Report
            {
                Name = report.Name,
                Description = report.Description,
                UserId = report.UserId,
                User = report.User,
                FilePath = report.FilePath,
                CreatedDate = report.CreatedDate.ToUniversalTime(),
                Version = report.Version
            };
            return View(model);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred loading Report Details. User: {0}. Report: {1}",
                User.FindFirstValue(ClaimTypes.Name), id);
            return RedirectToAction("Index", "Project");
        }
    }
    
    public IActionResult GenerateDoc(IFormCollection form)
    {
        var pro = projectManager.GetById(Guid.Parse(form["project"]));

        var vul = vulnManager.GetAll().Where(x => x.ProjectId == Guid.Parse(form["project"]) && x.Template == false).Select(y => y.Id)
            .ToList();
        var template = reportTemplateManager.GetById(Guid.Parse(form["ReportTemplates"]));


        
        var uniqueName = Guid.NewGuid().ToString() + "_" + form["reportName"]+"_v"+ form["version"] + ".docx";
        var templatePath = _appEnvironment.WebRootPath + template.FilePath;
        string resultPath = _appEnvironment.WebRootPath + "/Attachments/Reports/" + form["project"] + "/"+ uniqueName ;

        
        Report rep = new Report
        {
            Id = Guid.NewGuid(),
            Name = form["reportName"],
            ProjectId = Guid.Parse(form["project"]),
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedDate = DateTime.Now.ToUniversalTime(),
            Description = form["description"],
            Version = form["version"],
            FilePath = "/Attachments/Reports/" + form["project"] + "/" + uniqueName,
            Language = pro.Language
        };

        reportManager.Add(rep);
        reportManager.Context.SaveChanges();
        
        ReportViewModel model = new ReportViewModel
        {
            Organization = organizationManager.GetAll().First(),
            Project = pro,
            Vulns = vulnManager.GetAll().Where(x => x.ProjectId == Guid.Parse(form["project"]) && x.Template == false).ToList(),
            Targets = targetManager.GetAll().Where(x => x.ProjectId == Guid.Parse(form["project"])).ToList(),
            Users = projectUserManager.GetAll().Where(x => x.ProjectId == Guid.Parse(form["project"])).ToList(),
            Reports = reportManager.GetAll().Where(x => x.ProjectId == Guid.Parse(form["project"])).ToList(),
            VulnTargets = vulnTargetManager.GetAll().Where(x => vul.Contains(x.VulnId)).ToList()
        };

        
        
        if (!Directory.Exists(_appEnvironment.WebRootPath+ "/Attachments/Reports/" + form["project"]+"/"))
        {
            Directory.CreateDirectory(_appEnvironment.WebRootPath+ "/Attachments/Reports/" + form["project"]+"/");
        }


        using (WordprocessingDocument document = WordprocessingDocument.CreateFromTemplate(templatePath,true))
        {
            MainDocumentPart mainPart = document.MainDocumentPart;
            HtmlConverter converter = new HtmlConverter(mainPart);

            var header = document.MainDocumentPart.Document.MainDocumentPart.HeaderParts;
            var footer = document.MainDocumentPart.Document.MainDocumentPart.FooterParts;
            var body = document.MainDocumentPart.Document.Body;
            var paragraphs = body.Elements<Paragraph>();
            var texts = paragraphs.SelectMany(p => p.Elements<Run>()).SelectMany(r => r.Elements<Text>());

            foreach (var headerPart in document.MainDocumentPart.HeaderParts)
            {
                //Gets the text in headers
                foreach (var text in headerPart.RootElement.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                {
                    switch (text.Text)
                    {
                        case @"ProjectName":
                            text.Text = @model.Project.Name;
                            break;
                        case @"OrganizationName":
                            text.Text = @model.Organization.Name;
                            break;
                        case "OrganizationContactEmail":
                            text.Text = @model.Organization.ContactEmail;
                            break;
                        case "OrganizationContactPhone":
                            text.Text = @model.Organization.ContactPhone.ToString();
                            break;
                        case "ClientName":
                            text.Text = @model.Project.Client.Name;
                            break;
                        case "ClientContactEmail":
                            text.Text = @model.Project.Client.ContactEmail;
                            break;
                        case "ClientContactPhone":
                            text.Text = @model.Project.Client.ContactPhone;
                            break;
                        case "StartDate":
                            text.Text = @model.Project.StartDate.ToShortDateString();
                            break;
                        case "EndDate":
                            text.Text = @model.Project.EndDate.ToShortDateString();
                            break;
                        case "DateTimeYear":
                            text.Text = DateTime.Now.Year.ToString();
                            break;
                    }
                }
            }

            foreach (var footerPart in document.MainDocumentPart.FooterParts)
            {
                //Gets the text in footers
                foreach (var text in footerPart.RootElement.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                {
                    switch (text.Text)
                    {
                        case @"ProjectName":
                            text.Text = @model.Project.Name;
                            break;
                        case @"OrganizationName":
                            text.Text = @model.Organization.Name;
                            break;
                        case "OrganizationContactEmail":
                            text.Text = @model.Organization.ContactEmail;
                            break;
                        case "OrganizationContactPhone":
                            text.Text = @model.Organization.ContactPhone.ToString();
                            break;
                        case "ClientName":
                            text.Text = @model.Project.Client.Name;
                            break;
                        case "ClientContactEmail":
                            text.Text = @model.Project.Client.ContactEmail;
                            break;
                        case "ClientContactPhone":
                            text.Text = @model.Project.Client.ContactPhone;
                            break;
                        case "StartDate":
                            text.Text = @model.Project.StartDate.ToShortDateString();
                            break;
                        case "EndDate":
                            text.Text = @model.Project.EndDate.ToShortDateString();
                            break;
                        case "DateTimeYear":
                            text.Text = DateTime.Now.Year.ToString();
                            break;
                    }
                }
            }

            foreach (Text text in texts)
            {
                switch (text.Text)
                {
                    case @"ProjectName":
                        text.Text = @model.Project.Name;
                        break;
                    case "ProjectDescription":
                        
                        var parentProjectDesc = text.Parent;
                        var projectDescription = converter.Parse(pro.Description);
                        for (int i = projectDescription.Count() - 1 ; i >= 0; i--)
                        {
                            parentProjectDesc.InsertAfterSelf(projectDescription.ElementAt(i));
                        }
                        text.Text = String.Empty;
                        break;
                    case @"OrganizationName":
                        text.Text = @model.Organization.Name;
                        break;
                    case "OrganizationContactEmail":
                        text.Text = @model.Organization.ContactEmail;
                        break;
                    case "OrganizationContactPhone":
                        text.Text = @model.Organization.ContactPhone.ToString();
                        break;
                    case "ClientName":
                        text.Text = @model.Project.Client.Name;
                        break;
                    case "ClientContactEmail":
                        text.Text = @model.Project.Client.ContactEmail;
                        break;
                    case "ClientContactPhone":
                        text.Text = @model.Project.Client.ContactPhone;
                        break;
                    case "ClientDescription":
                        
                        var parentClientdescription = text.Parent;
                        var clientDescription = converter.Parse(pro.Client.Description);
                        for (int i = clientDescription.Count() - 1 ; i >= 0; i--)
                        {
                            parentClientdescription.InsertAfterSelf(clientDescription.ElementAt(i));
                        }
                        text.Text = String.Empty;
                        break;
                    case "StartDate":
                        text.Text = @model.Project.StartDate.ToShortDateString();
                        break;
                    case "EndDate":
                        text.Text = @model.Project.EndDate.ToShortDateString();
                        break;
                    case "DateTimeYear":
                        text.Text = DateTime.Now.Year.ToString();
                        break;
                    case "VulnCriticalCount":
                        text.Text = @model.Vulns.Where(x => x.Risk == VulnRisk.Critical).Count().ToString();
                        break;
                    case "VulnHighCount":
                        text.Text = @model.Vulns.Where(x => x.Risk == VulnRisk.High).Count().ToString();
                        break;
                    case "VulnMediumCount":
                        text.Text = @model.Vulns.Where(x => x.Risk == VulnRisk.Medium).Count().ToString();
                        break;
                    case "VulnLowCount":
                        text.Text = @model.Vulns.Where(x => x.Risk == VulnRisk.Low).Count().ToString();
                        break;
                    case "VulnInfoCount":
                        text.Text = @model.Vulns.Where(x => x.Risk == VulnRisk.Info).Count().ToString();
                        break;
                    case "ExecutiveSummaryDesc":
                        var parentExecutiveDesc = text.Parent;
                        var executiveDescription = converter.Parse(pro.ExecutiveSummary);
                        for (int i = executiveDescription.Count() - 1 ; i >= 0; i--)
                        {
                            parentExecutiveDesc.InsertAfterSelf(executiveDescription.ElementAt(i));
                        }
                        text.Text = String.Empty;
                        break;
                }
            }
            
            var tables = mainPart.Document.Descendants<Table>().ToList();
            
            var w = from c in tables
                where c.InnerText.Contains("DocumentName") ||
                      c.InnerText.Contains("DocumentVersion") || c.InnerText.Contains("DocumentDescription")
                select c;
            
            Table documentTable = w.First();

            TableRow rowDocTable = documentTable.Elements<TableRow>().ElementAt(1);
            List<TableRow> rowsDoc = new List<TableRow>();
            foreach (var doc in model.Reports)
            {
                var row = (TableRow)rowDocTable.CloneNode(true);
                
                /*var cellDocTable = row.SelectMany(p => p.Elements<TableCell>());
                // Find the first paragraph in the table cell.
                var pDocTable = cellDocTable.SelectMany(p => p.Elements<Paragraph>());*/

                var documentTableText = row.Descendants<Text>();
                
                foreach (Text textDoc in documentTableText)
                {
                    switch (textDoc.Text)
                    {
                        case "DocumentName":
                            textDoc.Text = doc.Name;
                            break;
                        case "DocumentVersion":
                            textDoc.Text = doc.Version;
                            break;
                        case "DocumentDescription":
                            textDoc.Text = doc.Description;
                            break;
                    }
                }

                rowsDoc.Add(row);
            }

            for (int i = 0; i < rowsDoc.Count; i++)
            {
                documentTable.AppendChild(rowsDoc[i]);
            }
            
            rowDocTable.Remove();
            
            
            var teamTables = from c in tables
                where c.InnerText.Contains("UserFullName") ||
                      c.InnerText.Contains("UserEmail") || c.InnerText.Contains("UserPosition")
                select c;
            
            Table teamTable = teamTables.First();
            TableRow rowTeamTable = teamTable.Elements<TableRow>().ElementAt(1);
            List<TableRow> rowsTeamTables = new List<TableRow>();
            
            foreach (var user in model.Users)
            {
                var row = (TableRow)rowTeamTable.CloneNode(true);
                
                var userTableText = row.Descendants<Text>();
                
                foreach (Text textDoc in userTableText)
                {
                    switch (textDoc.Text)
                    {
                        case "UserFullName":
                            textDoc.Text = user.User.FullName;
                            break;
                        case "UserEmail":
                            textDoc.Text = user.User.Email;
                            break;
                        case "UserPosition":
                            textDoc.Text = user.User.Position;
                            break;
                    }
                }

                rowsTeamTables.Add(row);
            }
            
            for (int i = 0; i < rowsTeamTables.Count; i++)
            {
                teamTable.AppendChild(rowsTeamTables[i]);
            }
            
            rowTeamTable.Remove();
            
             
            var targetTables = from c in tables
                where c.InnerText.Contains("TargetName") ||
                      c.InnerText.Contains("TargetType")
                select c;
            
            Table targetTable = targetTables.First();
            TableRow rowTargetTable = targetTable.Elements<TableRow>().ElementAt(1);
            List<TableRow> rowsTargetTables = new List<TableRow>();
            
            foreach (var target in model.Targets)
            {
                var row = (TableRow)rowTargetTable.CloneNode(true);
                
                var targetTableText = row.Descendants<Text>();
                
                foreach (Text textDoc in targetTableText)
                {
                    switch (textDoc.Text)
                    {
                        case "TargetName":
                            textDoc.Text = target.Name;
                            break;
                        case "TargetType":
                            textDoc.Text = target.Type.ToString();
                            break;
                    }
                }

                rowsTargetTables.Add(row);
            }
            
            for (int i = 0; i < rowsTargetTables.Count; i++)
            {
                targetTable.AppendChild(rowsTargetTables[i]);
            }
            
            rowTargetTable.Remove();
            
            
            //select summary count table
            var q = from c in tables
                where c.InnerText.Contains("VulnCriticalCount") ||
                      c.InnerText.Contains("VulnHighCount") || c.InnerText.Contains("VulnMediumCount") || 
                      c.InnerText.Contains("VulnLowCount") || c.InnerText.Contains("VulnInfoCount")
                select c;
            var vulnSummaryTable = q.First();

            var rowVulnSummary = vulnSummaryTable.Elements<TableRow>();

            var cellVulnSummary = rowVulnSummary.SelectMany(p => p.Elements<TableCell>());
            // Find the first paragraph in the table cell.
            var pVulnSummary = cellVulnSummary.SelectMany(p => p.Elements<Paragraph>());

            //get all texts
            var vulnSummaryTableText = pVulnSummary.SelectMany(p => p.Elements<Run>()).SelectMany(r => r.Elements<Text>());
            
            //substitutes variables
            foreach (Text textDoc in vulnSummaryTableText)
            {
                switch (textDoc.Text)
                {
                    case "VulnCriticalCount":
                        textDoc.Text = @model.Vulns.Where(x => x.Risk == VulnRisk.Critical).Count().ToString();
                        break;
                    case "VulnHighCount":
                        textDoc.Text = @model.Vulns.Where(x => x.Risk == VulnRisk.High).Count().ToString();
                        break;
                    case "VulnMediumCount":
                        textDoc.Text = @model.Vulns.Where(x => x.Risk == VulnRisk.Medium).Count().ToString();
                        break;
                    case "VulnLowCount":
                        textDoc.Text = @model.Vulns.Where(x => x.Risk == VulnRisk.Low).Count().ToString();
                        break;
                    case "VulnInfoCount":
                        textDoc.Text = @model.Vulns.Where(x => x.Risk == VulnRisk.Info).Count().ToString();
                        break;
                }
            }
            
            
            //select vuln table template
            var e = from c in tables
                where c.InnerText.Contains("VulnCVE") ||
                      c.InnerText.Contains("VulnImpact") || c.InnerText.Contains("VulnRemediation") || 
                      c.InnerText.Contains("VulnCVSS") || c.InnerText.Contains("VulnRisk")
                select c;
            
            var vulnTable = e.First();
            
            //saves vuln tablaes generated
            List<Table> vTables = new List<Table>();

            //foreach vuln create a table from table template
            foreach (var vuln in model.Vulns)
            {
                Table table = new Table(vulnTable.CloneNode(true));
                var vulnTableText = table.Descendants<Text>();

                foreach (Text textDoc in vulnTableText)
                {
                    switch (textDoc.Text)
                    {
                        case "VulnName":
                            textDoc.Text = vuln.Name;
                            break;
                        case "VulnCVE":
                            textDoc.Text = vuln.cve;
                            break;
                        case "VulnRisk":
                            textDoc.Text = vuln.Risk.ToString();
                            break;
                        case "VulnCVSS":
                            textDoc.Text = vuln.CVSS3.ToString();
                            break;
                        case "VulnCategory":
                            textDoc.Text = vuln.VulnCategory.Name;
                            break;
                        case "VulnAssets":
                            StringBuilder assets = new StringBuilder();
                            if (model.VulnTargets.Where(x => x.VulnId == vuln.Id).Count() != 0)
                            {
                                foreach (var asset in model.VulnTargets.Where(x => x.VulnId == vuln.Id))
                                {
                                    assets.Append(asset.Target.Name+" ("+asset.Target.Type+"), ");
                                }

                                textDoc.Text = assets.ToString();
                            }
                            else
                            {
                                textDoc.Text = String.Empty;
                            }
                            break;
                        case "VulnDescription":
                            var parentVulndescription = textDoc.Parent;
                            var vulnDescription = converter.Parse(vuln.Description);
                            for (int i = vulnDescription.Count() - 1 ; i >= 0; i--)
                            {
                                parentVulndescription.InsertAfterSelf(vulnDescription.ElementAt(i));
                            }
                            textDoc.Text = String.Empty;
                            break;
                        case "VulnImpact":
                            var parentVulnImpact = textDoc.Parent;
                            var vulnImpact = converter.Parse(vuln.Impact);
                            for (int i = vulnImpact.Count() - 1 ; i >= 0; i--)
                            {
                                parentVulnImpact.InsertAfterSelf(vulnImpact.ElementAt(i));
                            }
                            textDoc.Text = String.Empty;
                            break;
                        case "VulnPOC":
                            var parentVulnPoc = textDoc.Parent;
                            var vulnPoc = converter.Parse(vuln.ProofOfConcept);
                            for (int i = vulnPoc.Count() - 1 ; i >= 0; i--)
                            {
                                parentVulnPoc.InsertAfterSelf(vulnPoc.ElementAt(i));
                            }
                            textDoc.Text = String.Empty;
                            break;
                        case "VulnRemediation":
                            var parentVulnRemediation = textDoc.Parent;
                            var vulnRemediation = converter.Parse(vuln.Remediation);
                            for (int i = vulnRemediation.Count() - 1 ; i >= 0; i--)
                            {
                                parentVulnRemediation.InsertAfterSelf(vulnRemediation.ElementAt(i));
                            }
                            textDoc.Text = String.Empty;
                            break;
                        case "VulnComplexity":
                            textDoc.Text = vuln.RemediationComplexity.ToString();
                            break;
                        case "VulnPriority":
                            textDoc.Text = vuln.RemediationPriority.ToString();
                            break;
                            
                    }
                }
                
                vTables.Add(table);

            }

            
            for (int i = 0; i < vTables.Count();i++)
            {
                vulnTable.InsertAfterSelf(vTables[i]);
                vulnTable.InsertAfterSelf(new Paragraph());
            }
            
            vulnTable.Remove();
            
            var result = document.SaveAs(resultPath);
            result.Close();

        }
        return RedirectToAction("Details", "Project", new {id = form["project"]});
        }

    [Authorize(Roles = "Admin,SuperUser")]
    public IActionResult Templates()
    {
        try
        {
            var model = reportTemplateManager.GetAll();
            
                return View(model);
                
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading Report Templates!";

            _logger.LogError(ex, "An error ocurred loading Report Templates Index. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    public IActionResult Template(Guid id)
    {
        try
        {
            var result = reportTemplateManager.GetById(id);
            if (result != null)
            {
                var model = new ReportTemplateViewModel
                {
                    Id = result.Id,
                    Name = result.Name,
                    Description = result.Description,
                    Language = result.Language,
                    CreatedDate = result.CreatedDate,
                    User = result.User,
                    UserId = result.UserId,
                    FilePath = result.FilePath
                };

                return View(model);
            }

            return RedirectToAction("Templates");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading Report Template!";

            _logger.LogError(ex, "An error ocurred loading Report Template {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Templates");
        }
    }

    [Authorize(Roles = "Admin,SuperUser")]
    public IActionResult CreateTemplate()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public IActionResult CreateTemplate(ReportTemplateViewModel model)
    {
        try
        {
            var file = model.upload;
            
            var Inspector = new ContentInspectorBuilder() {
                Definitions = MimeDetective.Definitions.Default.FileTypes.Documents.MicrosoftOffice()
            }.Build();
            
            var Results = Inspector.Inspect(file.OpenReadStream());
            
            if (Results.ByFileExtension().Length == 0 && Results.ByMimeType().Length == 0)
            {
                TempData["createTemplateFile"] = "User is not in the project";
                return View("CreateTemplate");
            }
            
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Templates");
            var uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            var template = new ReportTemplate
            {
                Name = model.Name,
                Description = model.Description,
                Language = model.Language,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                FilePath = "/Attachments/Templates/" + uniqueName,
                CreatedDate = DateTime.UtcNow,
            };

            reportTemplateManager.Add(template);
            reportTemplateManager.Context.SaveChanges();
            
            TempData["createdTemplate"] = "created";
            _logger.LogInformation("User: {0} Created a new report template: {1}", User.FindFirstValue(ClaimTypes.Name),
                template.Name);
            
            return RedirectToAction("Templates");
        }
        catch(Exception ex)
        {
            TempData["errorCreatedTemplate"] = "Error creating report template!";
            _logger.LogError(ex, "An error ocurred adding a new report template. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("CreateTemplate");
        }
        
    }

    [Authorize(Roles = "Admin,SuperUser")]
    public IActionResult EditTemplate(Guid id)
    {
        try
        {
            var result = reportTemplateManager.GetById(id);

            if (result != null)
            {
                var model = new ReportTemplateViewModel
                {
                    Id = result.Id,
                    Name = result.Name,
                    Description = result.Description,
                    Language = result.Language,
                    FilePath = result.FilePath,
                    CreatedDate = result.CreatedDate,
                    User = result.User,
                    UserId = result.UserId
                };
                return View(model);
            }

            return RedirectToAction("Templates");
        }
        catch (Exception ex)
        {
            TempData["errorEditedTemplate"] = "Error loading edit form";
            _logger.LogError(ex, "An error ocurred loadin edit form report template. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Templates");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public IActionResult EditTemplate(Guid id,ReportTemplateViewModel model)
    {
        try
        {
            
            var result = reportTemplateManager.GetById(id);
            
            if (result != null)
            {
                result.Name = model.Name;
                result.Description = model.Description;
                result.Language = model.Language;

                reportTemplateManager.Context.SaveChanges();
                TempData["editedTemplate"] = "edited";
                _logger.LogInformation("User: {0} edited report template: {1}", User.FindFirstValue(ClaimTypes.Name), result.Name);
                return RedirectToAction("Templates");
            }

            return RedirectToAction("EditTemplate");
        }
        catch (Exception ex)
        {
            TempData["errorEditedTemplate"] = "Error loading edit form";
            _logger.LogError(ex, "An error ocurred loadin edit form report template. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Templates");
        }
    }

    [Authorize(Roles = "Admin,SuperUser")]
    public IActionResult DeleteTemplate(Guid id)
    {
        try
        {
            var result = reportTemplateManager.GetById(id);
            if (result != null)
            {
                return View(result);
            }

            return RedirectToAction("Templates");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading delete report template!";
            _logger.LogError(ex, "An error ocurred loading delete form on report template Id: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Templates");
        }
        
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public IActionResult DeleteTemplate(Guid id,IFormCollection formCollection)
    {
        try
        {
            var result = reportTemplateManager.GetById(id);
            if (result != null)
            {
                reportTemplateManager.Remove(result);
                reportTemplateManager.Context.SaveChanges();
            }

            TempData["deletedTemplate"] = "deleted";
            _logger.LogInformation("User: {0} deleted report template: {1}", User.FindFirstValue(ClaimTypes.Name), id);
            return RedirectToAction("Templates");
        }
        catch (Exception ex)
        {
            TempData["errorDeletedTemplate"] = "Error deleting report template!";

            _logger.LogError(ex, "An error ocurred deleting report template Id: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Templates");
        }
    }
    
    public ActionResult DownloadTemplate(Guid id)
    {
        try
        {
            var report = reportTemplateManager.GetById(id);

            var filePath = _appEnvironment.WebRootPath + report.FilePath;
            var fileName = report.Name + ".dotx";

            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.template", fileName);
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error downloading report template!";

            _logger.LogError(ex, "An error ocurred downlaoding report template Id: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Templates");
        }
        
    }


}