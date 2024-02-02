using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.File;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Ganss.Xss;
using HandlebarsDotNet;
using HandlebarsDotNet.Extension.NewtonsoftJson;
using HtmlToOpenXml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeDetective;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize (Roles = "Admin,SuperUser,User")]
public class ReportController : ControllerBase
{
    private readonly ILogger<ReportController> _logger = null;
    private IProjectManager projectManager = null;
    private IClientManager clientManager = null;
    private IProjectUserManager projectUserManager = null;
    private IProjectNoteManager projectNoteManager = null;
    private IProjectAttachmentManager projectAttachmentManager = null;
    private ITargetManager targetManager = null;
    private ITaskManager taskManager = null;
    private IUserManager userManager = null;
    private IVulnManager vulnManager = null;
    private IVulnCweManager vulnCweManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IOrganizationManager organizationManager = null;
    private IReportManager reportManager = null;
    private IReportTemplateManager reportTemplateManager = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private IFileCheck fileCheck;
    private IReportComponentsManager reportComponentsManager;
    private IReportsPartsManager reportsPartsManager;
    private IVaultManager vaultManager;
    private Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager = null;

    public ReportController(IProjectManager projectManager, IClientManager clientManager,
        IOrganizationManager organizationManager, IProjectUserManager projectUserManager,
        IProjectNoteManager projectNoteManager, IVulnTargetManager vulnTargetManager,
        IProjectAttachmentManager projectAttachmentManager, ITargetManager targetManager, ITaskManager taskManager,
        IUserManager userManager, IVulnManager vulnManager, IVulnCweManager vulnCweManager,
        ILogger<ReportController> logger, IReportManager reportManager, IReportTemplateManager reportTemplateManager,
        IWebHostEnvironment env, IHttpContextAccessor HttpContextAccessor, IFileCheck fileCheck,Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager,
        IReportComponentsManager reportComponentsManager, IReportsPartsManager reportsPartsManager, IVaultManager vaultManager)
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
        this.vulnCweManager = vulnCweManager;
        this.reportManager = reportManager;
        this.reportTemplateManager = reportTemplateManager;
        _logger = logger;
        this.env = env;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.fileCheck = fileCheck;
        this.reportComponentsManager = reportComponentsManager;
        this.reportsPartsManager = reportsPartsManager;
        this.vaultManager = vaultManager;
        this._userManager = _userManager;

    }

    [HttpGet]
    public IEnumerable<CORE.Entities.Report> Get()
    {
        try
        {
            IEnumerable<CORE.Entities.Report> model = reportManager.GetAll().Include(x => x.User).ToArray();
            if (model != null)
            {
                var user = projectUserManager.VerifyUser(model.First().ProjectId, aspNetUserId);
                if (user == null)
                {
                    return null;
                }
                
                var user2 = userManager.GetByUserId(user.UserId);
                var roles = _userManager.GetRolesAsync(user2).Result;
                if (roles.FirstOrDefault() == "Client")
                {
                    if (user2.ClientId != model.First().Project.ClientId)
                    {
                        return null;
                    }
                }
                
                return model;

            }

            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting reports. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("Project/{id}")]
    public IEnumerable<CORE.Entities.Report> GetByProject(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.Report> model = reportManager.GetAll().Where(x => x.ProjectId == id)
                .Include(x => x.User).ToArray();
            if (model != null)
            {
                var user = projectUserManager.VerifyUser(id, aspNetUserId);
                /*
                if (user == null)
                {
                    return model = new List<Report>();
                }
                */
                
                
                
                
                return model;

            }
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting project reports. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPut]
    [Authorize(Roles = "Admin,SuperUser,User")] 
    public async Task<IActionResult> EditReport([FromBody] ReportEditModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var report = reportManager.GetById(model.Id);
                if (report != null)
                {
                    var user = projectUserManager.VerifyUser(report.ProjectId, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest();
                    }
                    var sanitizer = new HtmlSanitizer();
                    sanitizer.AllowedSchemes.Add("data");
                    
                    report.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                    report.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                    report.Language = model.Language;
                    report.Version = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Version));
                    report.HtmlCode = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.HtmlCode));

                    await reportManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Report edited successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred editing reports User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred editing reports User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing reports. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("Templates")]
    [Authorize(Roles = "Admin,SuperUser,User")]
    public IEnumerable<CORE.Entities.ReportTemplate> Templates()
    {
        try
        {
            IEnumerable<CORE.Entities.ReportTemplate> model = reportTemplateManager.GetAll().Include(x => x.User)
                .ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting report templates. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    

    [HttpPost]
    [Route("Template")]
    [Authorize (Roles = "Admin,SuperUser")]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateReportModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");

                var template = new ReportTemplate();
                template.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                template.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                template.Language = model.Language;
                template.UserId = aspNetUserId;
                template.CreatedDate = DateTime.Now.ToUniversalTime();
                template.ReportType = model.ReportType;
                await reportTemplateManager.AddAsync(template);
                await reportTemplateManager.Context.SaveChangesAsync();

                foreach (var comp in model.Components)
                {
                    var com = new ReportParts();
                    com.ComponentId = comp.Id;
                    com.Order = comp.Order;
                    com.TemplateId = template.Id;
                    await reportsPartsManager.AddAsync(com);
                }

                await reportsPartsManager.Context.SaveChangesAsync();

                _logger.LogInformation("Report template added successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred adding report templates. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding report templates. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPut]
    [Route("Template")]
    [Authorize (Roles = "Admin,SuperUser")]
    public async Task<IActionResult> Edit([FromBody] EditReportTemplateModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var template = reportTemplateManager.GetById(model.Id);
                if (template != null)
                {
                    var sanitizer = new HtmlSanitizer();
                    sanitizer.AllowedSchemes.Add("data");

                    var parts = reportsPartsManager.GetAll().Where(x => x.TemplateId == model.Id).ToList();

                    foreach (var part in parts)
                    {
                        reportsPartsManager.Remove(part);
                    }

                    await reportsPartsManager.Context.SaveChangesAsync();

                    foreach (var comp in model.Components)
                    {
                        var com = new ReportParts();
                        com.ComponentId = comp.Id;
                        com.Order = comp.Order;
                        com.TemplateId = template.Id;
                        await reportsPartsManager.AddAsync(com);
                    }

                    await reportsPartsManager.Context.SaveChangesAsync();

                    template.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                    template.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                    template.Language = model.Language;
                    template.ReportType = model.ReportType;
                    await reportTemplateManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Report template edited successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred editing report templates. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred editing report templates. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing report templates. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpDelete]
    [Route("Template/{id}")]
    [Authorize (Roles = "Admin,SuperUser")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var report = reportTemplateManager.GetById(id);
                if (report != null)
                {

                    reportTemplateManager.Remove(report);
                    await reportTemplateManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Report template deletes successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred deleting report templates. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred deleting report templates. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting report templates. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpDelete]
    [Route("{reportId}")]
    [Authorize (Roles = "Admin,SuperUser")]
    public async Task<IActionResult> DeleteReport(Guid reportId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var report = reportManager.GetById(reportId);
                if (report != null)
                {
                    reportManager.Remove(report);
                    await reportTemplateManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Report deleted successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred deleting report. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred deleting report. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting report. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("Components")]
    [Authorize (Roles = "Admin,SuperUser")]
    public IEnumerable<CORE.Entities.ReportComponents> Components()
    {
        try
        {
            IEnumerable<CORE.Entities.ReportComponents> model = reportComponentsManager.GetAll().ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting report components. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("Parts/{templateId}")]
    [Authorize (Roles = "Admin,SuperUser")]
    public IEnumerable<CORE.Entities.ReportParts> GetParts(Guid templateId)
    {
        try
        {
            IEnumerable<CORE.Entities.ReportParts> model = reportsPartsManager.GetAll()
                .Where(x => x.TemplateId == templateId).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting report components. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPost]
    [Route("Components")]
    [Authorize (Roles = "Admin,SuperUser")]
    public async Task<IActionResult> CreateComponent([FromBody] CreateReportComponentModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");

                var comp = new ReportComponents();
                comp.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                comp.Content = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Content));
                comp.Language = model.Language;
                comp.Created = DateTime.Now.ToUniversalTime();
                comp.Updated = DateTime.Now.ToUniversalTime();
                comp.ComponentType = model.ComponentType;

                await reportComponentsManager.AddAsync(comp);
                await reportComponentsManager.Context.SaveChangesAsync();

                _logger.LogInformation("Report Components added successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred adding report Components. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding report Components. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPut]
    [Route("Components")]
    [Authorize (Roles = "Admin,SuperUser")]
    public async Task<IActionResult> EditComponent([FromBody] EditReportComponentModel model)
    {
        try
        {
            var result = reportComponentsManager.GetById(model.Id);
            if (result != null)
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");

                result.Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name));
                result.Content = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Content));
                result.Language = model.Language;
                result.Updated = DateTime.Now.ToUniversalTime();
                result.ComponentType = model.ComponentType;

                await reportComponentsManager.Context.SaveChangesAsync();

                _logger.LogInformation("Report Components edited successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred editing report Components. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing report Components. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpDelete]
    [Route("Components/{componentId}")]
    [Authorize (Roles = "Admin,SuperUser")]
    public async Task<IActionResult> DeleteComponent(Guid componentId)
    {
        try
        {
            var result = reportComponentsManager.GetById(componentId);
            if (result != null)
            {
                reportComponentsManager.Remove(result);
                await reportComponentsManager.Context.SaveChangesAsync();

                _logger.LogInformation("Report Components deleted successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred deleting report Components. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting report Components. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPost]
    [Route("Generate")]
    [Authorize (Roles = "Admin,SuperUser")]
    public async Task<IActionResult> GenerateNewReport([FromBody] ReportCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var pro = projectManager.GetById(model.ProjectId);
                var vul = vulnManager.GetAll().Where(x => x.ProjectId == model.ProjectId && x.Template == false)
                    .Select(y => y.Id).ToList();
                var template = reportTemplateManager.GetById(model.ReportTemplateId);
                var userInPro = projectUserManager.VerifyUser(pro.Id, aspNetUserId);
                if (userInPro == null)
                {
                    return BadRequest();
                }

                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");

                Report rep = new Report
                {
                    Id = Guid.NewGuid(),
                    Name = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Name)),
                    ProjectId = model.ProjectId,
                    UserId = aspNetUserId,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description)),
                    Version = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Version)),
                    Language = pro.Language,
                    ReportType = ReportType.General
                };


                var Organization = organizationManager.GetAll().First();
                var Client = clientManager.GetById(pro.ClientId);
                var Project = pro;
                var Vulns = vulnManager.GetAll()
                    .Where(x => x.ProjectId == model.ProjectId && x.Template == false).ToList();
                var Targets = targetManager.GetAll().Where(x => x.ProjectId == model.ProjectId).ToList();
                var Users = projectUserManager.GetAll().Where(x => x.ProjectId == model.ProjectId).ToList();
                var Reports = reportManager.GetAll().Where(x => x.ProjectId == model.ProjectId).ToList();
                var VulnTargets = vulnTargetManager.GetAll().Where(x => vul.Contains(x.VulnId)).Include(x => x.Target).ToList();
                var VulnCwes = vulnCweManager.GetAll().Where(x => vul.Contains(x.VulnId)).Include(x => x.Cwe).ToList();
                var Tasks = taskManager.GetAll().Where(x => x.ProjectId == model.ProjectId).Include(x => x.AsignedUser).Include(x => x.CreatedUser).ToList();
                var Vaults = vaultManager.GetAll().Where(x => x.ProjectId == model.ProjectId).ToList();
                var reportParts = reportsPartsManager.GetAll().Where(x => x.TemplateId == model.ReportTemplateId)
                    .OrderBy(x => x.Order).Include(x => x.Component).ToList();
                string source = @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"">
                    <title></title>
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                </head>
                <body>
                    <header>
                        {{HeaderComponents}}
                    </header>
                    <cover>
                        {{CoverComponents}}
                    </cover>
                    
                    {{BodyComponents}}

                    <footer> 
                        {{FooterComponents}}
                    </footer>
                </body>
                </html>";

                StringBuilder sbHeader = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Header)
                             .OrderBy(x => x.Order))
                {
                    sbHeader.Append(part.Component.Content);
                }

                source = source.Replace("{{HeaderComponents}}", sbHeader.ToString());


                StringBuilder sbCover = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Cover)
                             .OrderBy(x => x.Order))
                {
                    sbCover.Append(part.Component.Content);
                }

                source = source.Replace("{{CoverComponents}}", sbCover.ToString());

                StringBuilder sbBody = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Body)
                             .OrderBy(x => x.Order))
                {
                    sbBody.Append(part.Component.Content);
                }

                source = source.Replace("{{BodyComponents}}", sbBody.ToString());


                StringBuilder sbFooter = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Footer)
                             .OrderBy(x => x.Order))
                {
                    sbFooter.Append(part.Component.Content);
                }

                source = source.Replace("{{FooterComponents}}", sbFooter.ToString());

                var handlebars = Handlebars.Create();
                //handlebars.Configuration.UseNewtonsoftJson();
                //handlebars.RegisterHelper("lookup", (output, context, arguments) => { output.WriteSafeString(arguments[0]); }); 
                handlebars.RegisterHelper("lookup", (output, context, arguments) =>
                   {
                       var targetObject = arguments[0] as IDictionary<string, object>;
                       var propertyName = arguments[1].ToString();
                   
                       if (targetObject != null && targetObject.ContainsKey(propertyName))
                       {
                           output.WriteSafeString(targetObject[propertyName].ToString());
                       }
                   });
                
                
                var templateHtml = handlebars.Compile(source);
                
                var DocumentsList = new List<Dictionary<string, string>>();
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();

                foreach (var report in Reports)
                {
                    /*htmlDoc.LoadHtml(report.Description);
                    string descriptionWithoutTags = htmlDoc.DocumentNode.InnerText;*/
                    DocumentsList.Add(new Dictionary<string, string>
                    {
                        {"DocumentName", report.Name},
                        {"DocumentVersion", report.Version},
                        {"DocumentDescription", model.Description}
                    });
                }
                
                /*htmlDoc.LoadHtml(model.Description);
                string descriptionWithoutTags2 = htmlDoc.DocumentNode.InnerText;*/
                DocumentsList.Add(new Dictionary<string, string>
                {
                    {"DocumentName", model.Name},
                    {"DocumentVersion", model.Version},
                    {"DocumentDescription", model.Description}
                });
                
                var UsersList = new List<Dictionary<string, string>>();
                foreach (var user in Users)
                {
                    UsersList.Add(new Dictionary<string, string>
                    {
                        {"UserFullName", user.User.FullName},
                        {"UserEmail", user.User.Email},
                        {"UserPosition", user.User.Position},
                        {"UserPhone", user.User.PhoneNumber},
                        {"UserDescription", user.User.Description},
                        
                    });
                }
                
                var TargetList = new List<Dictionary<string, string>>();
                foreach (var tar in Targets)
                {
                    TargetList.Add(new Dictionary<string, string>
                    {
                        {"TargetName", tar.Name},
                        {"TargetDescription", tar.Description},
                        {"TargetType", tar.Type.ToString()},
                    });
                }
                
                
                var VulnsList = new List<Dictionary<string, string>>();
                foreach (var vuln in Vulns)
                {
                    var vulnCwe = VulnCwes.Where(vc => vc.VulnId == vuln.Id).ToList();
                    string cwes = "";
                    foreach (var asset in vulnCwe)
                    {
                        cwes = cwes + "CWE-" + asset.Cwe.Id + " - " + asset.Cwe.Name + " , ";
                    }
                    
                    var vulnTarget = VulnTargets.Where(vc => vc.VulnId == vuln.Id)
                        .ToList();
                    string targets = "";
                    foreach (var asset in vulnTarget)
                    {
                        targets = targets + asset.Target.Name + " (" + asset.Target.Type + "), ";
                    }

                    var cat = "No Category";
                    if (vuln.VulnCategory != null)
                    {
                        cat = vuln.VulnCategory.Name;
                    }

                    VulnsList.Add(new Dictionary<string, string>
                    {
                        {"VulnName", vuln.Name},
                        {"VulnLanguage", vuln.Language.ToString()},
                        {"VulnCve", vuln.cve},
                        {"VulnCwes", cwes},
                        {"VulnDescription", vuln.Description},
                        {"VulnCategory", cat},
                        {"VulnRisk", vuln.Risk.ToString()},
                        {"VulnStatus", vuln.Status.ToString()},
                        {"VulnImpact", vuln.Impact},
                        {"VulnCvss", vuln.CVSS3.ToString(CultureInfo.InvariantCulture)},
                        {"VulnCvssVector", vuln.CVSSVector},
                        {"VulnRemediation", vuln.Remediation},
                        {"VulnComplexity", vuln.RemediationComplexity.ToString()},
                        {"VulnPriority", vuln.RemediationPriority.ToString()},
                        {"VulnJiraCreated", vuln.JiraCreated.ToString()},
                        {"VulnJira", vuln.CVSSVector},
                        {"VulnPoc", vuln.ProofOfConcept},
                        {"VulnOwaspRisk", vuln.OWASPRisk},
                        {"VulnOwaspImpact", vuln.OWASPImpact},
                        {"VulnOwaspLikelihood", vuln.OWASPLikehood},
                        {"VulnOwaspVector", vuln.OWASPVector},
                        {"VulnTargets", targets},
                        {"VulnFindingId", vuln.FindingId}
                    });
                }
                
                var TasksList = new List<Dictionary<string, string>>();
                foreach (var tas in Tasks)
                {
                    TargetList.Add(new Dictionary<string, string>
                    {
                        {"TaskName", tas.Name},
                        {"TaskDescription", tas.Description},
                        {"TaskStatus", tas.Status.ToString()},
                        {"TaskStartDate", tas.StartDate.ToString("dd/MM/yyyy")},
                        {"TaskEndDate", tas.EndDate.ToString("dd/MM/yyyy")},
                        {"TaskAssignedTo", tas.AsignedUser.FullName},
                        {"TaskCreatedBy", tas.CreatedUser.FullName},
                    });
                }
                
                var VaultsList = new List<Dictionary<string, string>>();
                foreach (var vault in Vaults)
                {
                    VaultsList.Add(new Dictionary<string, string>
                    {
                        {"VaultName", vault.Name},
                        {"VaultDescription", vault.Description},
                        {"VaultType", vault.Type.ToString()},
                           {"VaultCreatedBy", vault.User.FullName},
                            {"VaultCreatedDate", vault.CreatedDate.ToString("dd/MM/yyyy")},
                            {"VaultValue", vault.Value}
                      
                    });
                }

                var data = new
                {
                    OrganizationName = Organization.Name,
                    OrganizationEmail = Organization.ContactEmail,
                    OrganizationPhone = Organization.ContactPhone,
                    OrganizationDescription = Organization.Description,
                    OrganizationContactName = Organization.ContactName,
                    OrganizationUrl = Organization.Url,
                    ClientName = Client.Name,
                    ClientDescription = Client.Description,
                    ClientUrl = Client.Url,
                    ClientContactName = Client.ContactName,
                    ClientEmail = Client.ContactEmail,
                    ClientPhone = Client.ContactPhone,
                    Year = DateTime.Now.Year.ToString(),
                    Documents = DocumentsList,
                    Users = UsersList,
                    ProjectName = Project.Name,
                    ProjectDescription = Project.Description,
                    ProjectLanguage = Project.Language,
                    ProjecStatus = Project.Status,
                    StartDate = Project.StartDate.ToString("dd/MM/yyyy"),
                    EndDate = Project.EndDate.ToString("dd/MM/yyyy"),
                    ProjectType = Project.ProjectType.ToString(),
                    ProjectScore = Project.Score,
                    ProjectExecutiveSummary = Project.ExecutiveSummary,
                    Targets = TargetList,
                    Vulns = VulnsList,
                    VulnCriticalCount = Vulns.Count(x => x.Risk == VulnRisk.Critical),
                    VulnHighCount = Vulns.Count(x => x.Risk == VulnRisk.High),
                    VulnMediumCount = Vulns.Count(x => x.Risk == VulnRisk.Medium),
                    VulnLowCount = Vulns.Count(x => x.Risk == VulnRisk.Low),
                    VulnInfoCount = Vulns.Count(x => x.Risk == VulnRisk.Info),
                    VulnTotalCount = Vulns.Count(),
                    Tasks = TasksList,
                    Vaults = VaultsList,
                    PageBreak = @"<span style=""page-break-after: always;""></span>",
                    Today = DateTime.Now.ToShortDateString()
                    
                };
                

                var resultHtml = templateHtml(data);

                rep.HtmlCode = resultHtml;
                reportManager.Add(rep);
                reportManager.Context.SaveChanges();

                _logger.LogInformation("Report generated successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred generating report. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred generating report. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    

    
    [HttpPost]
    [Route("Download")]
    public FileContentResult DownloadReport([FromBody] ReportDownloadModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var report = reportManager.GetById(model.Id);
                if (report != null)
                {
                    var user = projectUserManager.VerifyUser(report.ProjectId, aspNetUserId);
                    if (user == null)
                    {
                        return null;
                    }
                    
                    switch (model.FileType)
                    {
                        case ReportFileType.Html:
                            string fileName = report.Name + "_" + report.Version + ".html";

                            var fileBytes = Encoding.ASCII.GetBytes(report.HtmlCode);
                            var mimeType = "text/html";
                            _logger.LogInformation("Report downloaded successfully. User: {0}",
                                aspNetUserId);
                            return new FileContentResult(fileBytes, mimeType)
                            {
                                FileDownloadName = fileName
                            };
                        case ReportFileType.Docx:
                            using (MemoryStream generatedDocument = new MemoryStream())
                            {
                                using (WordprocessingDocument package = WordprocessingDocument.Create(generatedDocument,
                                           WordprocessingDocumentType.Document))
                                {
                                    MainDocumentPart mainPart = package.MainDocumentPart;
                                    if (mainPart == null)
                                    {
                                        mainPart = package.AddMainDocumentPart();
                                        new Document(new Body()).Save(mainPart);
                                    }
                                    
                                    

                                    // Parse the HTML code to extract the header, footer, and cover
                                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                                    htmlDoc.LoadHtml(report.HtmlCode);
                                    var headerHtml = htmlDoc.DocumentNode.SelectSingleNode("//header")?.InnerHtml;
                                    var footerHtml = htmlDoc.DocumentNode.SelectSingleNode("//footer")?.InnerHtml;
                                    var coverHtml =
                                        htmlDoc.DocumentNode.SelectSingleNode("//cover")
                                            ?.InnerHtml; // Assuming the cover is in a <cover> tag

                                    // Create a HeaderPart, FooterPart, and a Body for the cover
                                    HeaderPart headerPart = mainPart.AddNewPart<HeaderPart>();
                                    FooterPart footerPart = mainPart.AddNewPart<FooterPart>();
                                    Body body = mainPart.Document.Body;

                                    // Convert the header, footer, and cover HTML to OpenXML elements
                                    HtmlToOpenXml.HtmlConverter converter = new HtmlToOpenXml.HtmlConverter(mainPart);

                                    if (headerHtml != null)
                                    {
                                        var headerElements = converter.Parse(headerHtml);
                                        headerPart.Header = new Header(headerElements);
                                        headerPart.Header.Save();
                                    }

                                    if (footerHtml != null)
                                    {
                                        var footerElements = converter.Parse(footerHtml);
                                        footerPart.Footer = new Footer(footerElements);
                                        footerPart.Footer.Save();
                                    }

                                    if (coverHtml != null)
                                    {
                                        var coverElements = converter.Parse(coverHtml);
                                        foreach (var element in coverElements)
                                        {
                                            body.Append(element);
                                        }

                                        // Add a page break after the cover
                                        body.Append(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
                                        // Add a table of contents after the cover
                                        /*FieldCode tocFieldCode = new FieldCode("TOC \\o \"1-3\" \\h \\z \\u");
                                        Run tocFieldRun = new Run(tocFieldCode);
                                        Paragraph tocParagraph = new Paragraph(tocFieldRun);
                                        body.Append(tocParagraph);
                                        body.Append(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));*/

                                    }

                                    // Ensure that the SectionProperties exists
                                    SectionProperties sectionProps =
                                        body.Elements<SectionProperties>().FirstOrDefault();
                                    if (sectionProps == null)
                                    {
                                        sectionProps = new SectionProperties();
                                        body.Append(sectionProps);
                                    }

                                    // Add the HeaderPart and FooterPart to the Word document
                                    sectionProps.Append(new HeaderReference()
                                        { Id = mainPart.GetIdOfPart(headerPart) });
                                    sectionProps.Append(new FooterReference()
                                        { Id = mainPart.GetIdOfPart(footerPart) });

                                    // Select all <header>, <footer>, and <cover> nodes
                                    var headerNodes = htmlDoc.DocumentNode.SelectNodes("//header");
                                    var footerNodes = htmlDoc.DocumentNode.SelectNodes("//footer");
                                    var coverNodes = htmlDoc.DocumentNode.SelectNodes("//cover");

                                    // Remove all the nodes
                                    if (headerNodes != null)
                                    {
                                        foreach (var node in headerNodes)
                                        {
                                            node.Remove();
                                        }
                                    }

                                    if (footerNodes != null)
                                    {
                                        foreach (var node in footerNodes)
                                        {
                                            node.Remove();
                                        }
                                    }

                                    if (coverNodes != null)
                                    {
                                        foreach (var node in coverNodes)
                                        {
                                            node.Remove();
                                        }
                                    }

                                    string updatedHtmlCode = htmlDoc.DocumentNode.OuterHtml;
                                    converter.ParseHtml(updatedHtmlCode);
                                    
                                    mainPart.Document.Save();
                                }

                                string fileName2 = report.Name + "_" + report.Version + ".docx";

                                var fileBytes2 = generatedDocument.ToArray();
                                var mimeType2 =
                                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                                _logger.LogInformation("Report downloaded successfully. User: {0}",
                                    aspNetUserId);

                                return new FileContentResult(fileBytes2, mimeType2)
                                {
                                    FileDownloadName = fileName2
                                };
                            }

                            break;
                        /*
                        case ReportFileType.Pdf:
                            using (MemoryStream masterStream = new MemoryStream())
                            {
                                
                                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                                htmlDoc.LoadHtml(report.HtmlCode);
                                var headerHtml = htmlDoc.DocumentNode.SelectSingleNode("//header")?.InnerHtml;
                                var footerHtml = htmlDoc.DocumentNode.SelectSingleNode("//footer")?.InnerHtml;
                                var coverHtml =
                                    htmlDoc.DocumentNode.SelectSingleNode("//cover")
                                        ?.InnerHtml;
                                
                                var headerNodes = htmlDoc.DocumentNode.SelectNodes("//header");
                                var footerNodes = htmlDoc.DocumentNode.SelectNodes("//footer");
                                var coverNodes = htmlDoc.DocumentNode.SelectNodes("//cover");

                                // Remove all the nodes
                                if (headerNodes != null)
                                {
                                    foreach (var node in headerNodes)
                                    {
                                        node.Remove();
                                    }
                                }

                                if (footerNodes != null)
                                {
                                    foreach (var node in footerNodes)
                                    {
                                        node.Remove();
                                    }
                                }

                                if (coverNodes != null)
                                {
                                    foreach (var node in coverNodes)
                                    {
                                        node.Remove();
                                    }
                                }

                                string updatedHtmlCode = htmlDoc.DocumentNode.OuterHtml;
                                

                                // Add HTML to our PDF
                                if (!string.IsNullOrEmpty(coverHtml))
                                {
                                    AppendHTML(masterStream, coverHtml);

                                }

                                if (!string.IsNullOrEmpty(updatedHtmlCode))
                                {
                                    AppendHTML(masterStream, updatedHtmlCode);

                                }

                                if (!string.IsNullOrEmpty(headerHtml))
                                {
                                    DownloadReportHelper.AppendHTML(masterStream, headerHtml);

                                }

                                if (!string.IsNullOrEmpty(footerHtml))
                                {
                                    DownloadReportHelper.AppendHTML(masterStream, footerHtml);

                                }
                                
                                // Create a new directory for our output PDF
                                string newDirectory = $"{executingDirectory}{DateTime.Now.ToString("MM-dd-yyyy")}";
                                if (!Directory.Exists(newDirectory))
                                    Directory.CreateDirectory(newDirectory);

                                // Save our in-memory PDF as a PDF on our file system
                                using (FileStream fileStream = new FileStream($"{newDirectory}\\{DateTime.Now.ToString("HH-mm-ss")} output.pdf", FileMode.Create))
                                    masterStream.WriteTo(fileStream);

                                Console.Write("PDF was created");
                            }
                    
                                
                            
                                break;*/
                    }
                }

                _logger.LogError("An error ocurred generating report. User: {0}",
                    aspNetUserId);
                return null;
            }

            _logger.LogError("An error ocurred generating report. User: {0}",
                aspNetUserId);
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred generating report. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    
   
}