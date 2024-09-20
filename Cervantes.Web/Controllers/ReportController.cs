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
using Cervantes.Web.Helpers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Ganss.Xss;
using HandlebarsDotNet;
using HandlebarsDotNet.Extension.NewtonsoftJson;
using Hangfire.Dashboard.Resources;
using HtmlAgilityPack;
using HtmlToOpenXml;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using Mammoth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeDetective;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;
using Scriban.Runtime;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using PageSize = iText.Kernel.Geom.PageSize;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using TextAlignment = DocumentFormat.OpenXml.Wordprocessing.TextAlignment;
using VerticalAlignment = DocumentFormat.OpenXml.Drawing.Wordprocessing.VerticalAlignment;

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
    private IJiraManager jiraManager = null;
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
    private Sanitizer sanitizer;
    private Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager = null;

    public ReportController(IProjectManager projectManager, IClientManager clientManager,
        IOrganizationManager organizationManager, IProjectUserManager projectUserManager,
        IProjectNoteManager projectNoteManager, IVulnTargetManager vulnTargetManager,
        IProjectAttachmentManager projectAttachmentManager, ITargetManager targetManager, ITaskManager taskManager,
        IUserManager userManager, IVulnManager vulnManager, IVulnCweManager vulnCweManager,
        ILogger<ReportController> logger, IReportManager reportManager, IReportTemplateManager reportTemplateManager,
        IWebHostEnvironment env, IHttpContextAccessor HttpContextAccessor, IFileCheck fileCheck,Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager,
        IReportComponentsManager reportComponentsManager, IReportsPartsManager reportsPartsManager, 
        IVaultManager vaultManager, IJiraManager jiraManager,Sanitizer sanitizer)
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
        this.jiraManager = jiraManager;
        this.sanitizer = sanitizer;
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

                    report.Name = sanitizer.Sanitize(model.Name);
                    report.Description = sanitizer.Sanitize(model.Description);
                    report.Language = model.Language;
                    report.Version = sanitizer.Sanitize(model.Version);
                    report.HtmlCode = sanitizer.Sanitize(model.HtmlCode);

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
                var template = new ReportTemplate();
                template.Name = sanitizer.Sanitize(model.Name);
                template.Description = sanitizer.Sanitize(model.Description);
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

                    template.Name = sanitizer.Sanitize(model.Name);
                    template.Description = sanitizer.Sanitize(model.Description);
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
                var comp = new ReportComponents();
                comp.Name = sanitizer.Sanitize(model.Name);
                var test = sanitizer.Sanitize(model.Content);
                comp.Content = HttpUtility.HtmlDecode(test);
                var test2 = sanitizer.Sanitize(model.CssContent);
                comp.ContentCss = HttpUtility.HtmlDecode(test2);
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

                result.Name = sanitizer.Sanitize(model.Name);
                var test = sanitizer.Sanitize(model.Content);
                result.Content = HttpUtility.HtmlDecode(test); 
                var test2 = sanitizer.Sanitize(model.CssContent);
                result.ContentCss = HttpUtility.HtmlDecode(test2);
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
                //var template = reportTemplateManager.GetById(model.ReportTemplateId);
                var userInPro = projectUserManager.VerifyUser(pro.Id, aspNetUserId);
                if (userInPro == null)
                {
                    return BadRequest();
                }

                Report rep = new Report
                {
                    Id = Guid.NewGuid(),
                    Name = sanitizer.Sanitize(model.Name),
                    ProjectId = model.ProjectId,
                    UserId = aspNetUserId,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    Description = sanitizer.Sanitize(model.Description),
                    Version = sanitizer.Sanitize(model.Version),
                    Language = model.Language,
                    ReportType = ReportType.General
                };


                var Organization = organizationManager.GetAll().First();
                var Client = clientManager.GetById(pro.ClientId);
                var Project = pro;
                var Vulns = vulnManager.GetAll()
                    .Where(x => x.ProjectId == model.ProjectId && x.Template == false).ToList();
                var Targets = targetManager.GetAll().Where(x => x.ProjectId == model.ProjectId).ToList();
                var Users = projectUserManager.GetAll().Where(x => x.ProjectId == model.ProjectId).ToList();
                var Reports = reportManager.GetAll().Where(x => x.ProjectId == model.ProjectId && x.ReportType == ReportType.General).ToList();
                Reports.Add(rep);
                var VulnTargets = vulnTargetManager.GetAll().Where(x => vul.Contains(x.VulnId)).Include(x => x.Target).ToList();
                var VulnCwes = vulnCweManager.GetAll().Where(x => vul.Contains(x.VulnId)).Include(x => x.Cwe).ToList();
                var Tasks = taskManager.GetAll().Where(x => x.ProjectId == model.ProjectId).Include(x => x.AsignedUser).Include(x => x.CreatedUser).ToList();
                var Vaults = vaultManager.GetAll().Where(x => x.ProjectId == model.ProjectId).ToList();
                var reportParts = reportsPartsManager.GetAll().Where(x => x.TemplateId == model.ReportTemplateId)
                    .OrderBy(x => x.Order).Include(x => x.Component).ToList();
                string source = @"<!DOCTYPE HTML>
                <html lang='en' xmlns='http://www.w3.org/1999/xhtml'>
                <head>
                    <meta charset='utf-8'/>
                    <title></title>
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
                    var parsed = PreMailer.Net.PreMailer.MoveCssInline(part.Component.Content, css: part.Component.ContentCss);
                    string pattern = @"<html\s*(?:[^>]*)\s*>";
                    string pattern2 = @"<body\s*(?:[^>]*)\s*>";
                    string pattern3 = @"<head\s*(?:[^>]*)\s*>";
                    string html = parsed.Html;
                    html = Regex.Replace(html, pattern, "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, pattern2, "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, pattern3, "", RegexOptions.IgnoreCase);
                    html =html.Replace("</html>", "");
                    html =html.Replace("</head>", "");
                    html =html.Replace("</body>", "");
                    sbHeader.Append(html);
                }

                source = source.Replace("{{HeaderComponents}}", sbHeader.ToString());


                StringBuilder sbCover = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Cover)
                             .OrderBy(x => x.Order))
                {
                    var parsed = PreMailer.Net.PreMailer.MoveCssInline(part.Component.Content, css: part.Component.ContentCss);
                    string pattern = @"<html\s*(?:[^>]*)\s*>";
                    string pattern2 = @"<body\s*(?:[^>]*)\s*>";
                    string pattern3 = @"<head\s*(?:[^>]*)\s*>";
                    string html = parsed.Html;
                    html = Regex.Replace(html, pattern, "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, pattern2, "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, pattern3, "", RegexOptions.IgnoreCase);
                    html =html.Replace("</html>", "");
                    html =html.Replace("</head>", "");
                    html =html.Replace("</body>", "");
                    sbCover.Append(html);
                }

                source = source.Replace("{{CoverComponents}}", sbCover.ToString());

                StringBuilder sbBody = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Body)
                             .OrderBy(x => x.Order))
                {
                    var parsed = PreMailer.Net.PreMailer.MoveCssInline(part.Component.Content, css: part.Component.ContentCss);
                    string pattern = @"<html\s*(?:[^>]*)\s*>";
                    string pattern2 = @"<body\s*(?:[^>]*)\s*>";
                    string pattern3 = @"<head\s*(?:[^>]*)\s*>";
                    string html = parsed.Html;
                    html = Regex.Replace(html, pattern, "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, pattern2, "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, pattern3, "", RegexOptions.IgnoreCase);
                    html =html.Replace("</html>", "");
                    html =html.Replace("</head>", "");
                    html =html.Replace("</body>", "");
                    sbBody.Append(html);
                }

                source = source.Replace("{{BodyComponents}}", sbBody.ToString());


                StringBuilder sbFooter = new StringBuilder();
                foreach (var part in reportParts.Where(x => x.Component.ComponentType == ReportPartType.Footer)
                             .OrderBy(x => x.Order))
                {
                    var parsed = PreMailer.Net.PreMailer.MoveCssInline(part.Component.Content, css: part.Component.ContentCss);
                    string pattern = @"<html\s*(?:[^>]*)\s*>";
                    string pattern2 = @"<body\s*(?:[^>]*)\s*>";
                    string pattern3 = @"<head\s*(?:[^>]*)\s*>";
                    string html = parsed.Html;
                    html = Regex.Replace(html, pattern, "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, pattern2, "", RegexOptions.IgnoreCase);
                    html = Regex.Replace(html, pattern3, "", RegexOptions.IgnoreCase);
                    html =html.Replace("</html>", "");
                    html =html.Replace("</head>", "");
                    html =html.Replace("</body>", "");
                    sbFooter.Append(html);
                }

                source = source.Replace("{{FooterComponents}}", sbFooter.ToString());
                
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
                        {"DocumentDescription", report.Description},
                        {"DocumentCreatedDate", report.CreatedDate.ToShortDateString()},
                    });
                }

                
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

                    string vulnJira = "";
                    string vulnJiraStatus = ""; 
                    string vulnJiraDueDate = "";
                    string vulnJiraCreatedDate = "";
                    string vulnJiraAssigned = "";
                    string vulnJiraPriority = "";
                    string vulnJiraResolution = "";
                    string vulnJiraResolutionDate = "";
                    string vulnJiraType = "";
                    string vulnJiraComponent = "";
                    string vulnJiraProject = "";
                    if (vuln.JiraCreated)
                    {
                        var  jira = jiraManager.GetByVulnId(vuln.Id);
                        vulnJira = jira.JiraKey;
                        vulnJiraStatus = jira.JiraStatus;
                        vulnJiraDueDate = jira.DueDate.ToString();
                        vulnJiraCreatedDate = jira.JiraCreatedDate.ToString();
                        vulnJiraResolutionDate = jira.ResolutionDate.ToString();
                        vulnJiraAssigned = jira.Assignee;
                        vulnJiraPriority = jira.Priority;
                        vulnJiraResolution = jira.Resolution;
                        vulnJiraType = jira.JiraType;
                        vulnJiraComponent = jira.JiraComponent;
                        vulnJiraProject = jira.JiraProject;
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
                        {"VulnJira", vulnJira},
                        {"VulnJiraStatus",vulnJiraStatus},
                        {"VulnJiraDueDate",vulnJiraDueDate},
                        {"VulnJiraCreatedDate",vulnJiraCreatedDate},
                        {"VulnJiraResolutionDate",vulnJiraResolutionDate},
                        {"VulnJiraResolution",vulnJiraResolution},
                        {"VulnJiraPriority",vulnJiraPriority},
                        {"VulnJiraAssigned",vulnJiraAssigned},
                        {"VulnJiraType",vulnJiraType},
                        {"VulnJiraComponent",vulnJiraComponent},
                        {"VulnJiraProject",vulnJiraProject},
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
                
                var scriptObject = new ScriptObject();
                scriptObject.Add("OrganizationName", Organization.Name);
                scriptObject.Add("OrganizationEmail", Organization.ContactEmail);
                scriptObject.Add("OrganizationPhone", Organization.ContactPhone);
                scriptObject.Add("OrganizationDescription", Organization.Description);
                scriptObject.Add("OrganizationContactName", Organization.ContactName);
                scriptObject.Add("OrganizationUrl", Organization.Url);
                scriptObject.Add("ClientName", Client.Name);
                scriptObject.Add("ClientDescription", Client.Description);
                scriptObject.Add("ClientUrl", Client.Url);
                scriptObject.Add("ClientContactName", Client.ContactName);
                scriptObject.Add("ClientEmail", Client.ContactEmail);
                scriptObject.Add("ClientPhone", Client.ContactPhone);
                scriptObject.Add("Year", DateTime.Now.Year.ToString());
                scriptObject.Add("Documents", DocumentsList);
                scriptObject.Add("Users", UsersList);
                scriptObject.Add("ProjectName", Project.Name);
                scriptObject.Add("ProjectDescription", Project.Description);
                scriptObject.Add("ProjectLanguage", Project.Language);
                scriptObject.Add("ProjecStatus", Project.Status);
                scriptObject.Add("StartDate", Project.StartDate.ToString("dd/MM/yyyy"));
                scriptObject.Add("EndDate", Project.EndDate.ToString("dd/MM/yyyy"));
                scriptObject.Add("ProjectType", Project.ProjectType.ToString());
                scriptObject.Add("ProjectScore", Project.Score);
                scriptObject.Add("ProjectExecutiveSummary", Project.ExecutiveSummary);
                scriptObject.Add("Targets", TargetList);
                scriptObject.Add("Vulns", VulnsList);
                scriptObject.Add("VulnCriticalCount", Vulns.Count(x => x.Risk == VulnRisk.Critical));
                scriptObject.Add("VulnHighCount", Vulns.Count(x => x.Risk == VulnRisk.High));
                scriptObject.Add("VulnMediumCount", Vulns.Count(x => x.Risk == VulnRisk.Medium));
                scriptObject.Add("VulnLowCount", Vulns.Count(x => x.Risk == VulnRisk.Low));
                scriptObject.Add("VulnInfoCount", Vulns.Count(x => x.Risk == VulnRisk.Info));
                scriptObject.Add("VulnTotalCount", Vulns.Count());
                scriptObject.Add("Tasks", TasksList);
                scriptObject.Add("Vaults", VaultsList);
                scriptObject.Add("PageBreak", @"<div style=""page-break-after: always;""></div>");
                scriptObject.Add("Today", DateTime.Now.ToShortDateString());
                

                var context = new TemplateContext();
                context.PushGlobal(scriptObject);

                source = ReplaceTableRowWithFor(source);
                //Console.WriteLine(source);
                var templateScriban = Template.Parse(source);
                // Check for any errors
                if (templateScriban.HasErrors)
                {
                    foreach(var error in templateScriban.Messages)
                    {
                        Console.WriteLine(error);
                    }
                }
                
                var result = await templateScriban.RenderAsync(context);

                rep.HtmlCode = result;
                
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
    
public static string ReplaceTableRowWithFor(string htmlContent)
{
    string pattern = @"<table[^>]*>.*?<tbody>\s*(.*?)\s*</tbody>.*?</table>";
    
    return Regex.Replace(htmlContent, pattern, match =>
    {
        string tableContent = match.Groups[1].Value;
        if (!tableContent.Contains("{{tablerow"))
        {
            // If the table doesn't contain tablerow syntax, return it unchanged
            return match.Value;
        }

        string[] rows = Regex.Split(tableContent, @"(?=<tr>)");
        StringBuilder updatedTableContent = new StringBuilder();

        bool forLoopAdded = false;

        foreach (string row in rows)
        {
            if (row.Contains("{{tablerow"))
            {
                string rowPattern = @"<tr>\s*(.*?{{tablerow\s+(\w+)\s+in\s+(\w+)}}.*?{{end}}.*?)\s*</tr>";
                string updatedRow = Regex.Replace(row, rowPattern, rowMatch =>
                {
                    string rowContent = rowMatch.Groups[1].Value;
                    string itemName = rowMatch.Groups[2].Value;
                    string collectionName = rowMatch.Groups[3].Value;
                    
                    string forLoopStart = $"{{{{for {itemName} in {collectionName}}}}}";
                    string forLoopEnd = "{{end}}";
                    
                    // Remove the tablerow and end syntax from the row content
                    rowContent = Regex.Replace(rowContent, @"{{tablerow\s+\w+\s+in\s+\w+}}", "");
                    rowContent = Regex.Replace(rowContent, @"{{end}}", "");
                    
                    // Ensure proper structure of td tags
                    rowContent = Regex.Replace(rowContent, @"</td>\s*<td>", "</td><td>");
                    
                    if (!forLoopAdded)
                    {
                        updatedTableContent.AppendLine(forLoopStart);
                        forLoopAdded = true;
                    }
                    
                    return $"<tr>{rowContent}</tr>";
                }, RegexOptions.Singleline);

                updatedTableContent.AppendLine(updatedRow);
            }
            else
            {
                // This is likely the header row or a row without tablerow syntax
                updatedTableContent.AppendLine(row);
            }
        }

        if (forLoopAdded)
        {
            updatedTableContent.AppendLine("{{end}}");
        }

        return match.Value.Replace(tableContent, updatedTableContent.ToString());
    }, RegexOptions.Singleline);
}
    
    [HttpPost]
    [Route("Download")]
    public async Task<FileContentResult> DownloadReport([FromBody] ReportDownloadModel model)
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
                                    await converter.ParseHtml(updatedHtmlCode);
                                    
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
                        case ReportFileType.Pdf:
                            byte[] pdfContent;
                            // Parse the HTML code to extract the header, footer, and cover
                            var htmlDoc2 = new HtmlDocument();
                            htmlDoc2.LoadHtml(report.HtmlCode);
                            var coverHtml2 = htmlDoc2.DocumentNode.SelectSingleNode("//cover")?.InnerHtml;

                            // Remove header, footer, and cover nodes from the main content
                            var nodesToRemove = htmlDoc2.DocumentNode.SelectNodes("//header|//footer|//cover");
                            if (nodesToRemove != null)
                            {
                                foreach (var node in nodesToRemove)
                                {
                                    node.Remove();
                                }
                            }
                            
                            string mainContent = htmlDoc2.DocumentNode.OuterHtml;

                            using (MemoryStream finalPdfStream = new MemoryStream())
                            {
                                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(finalPdfStream));
                                pdfDoc.SetDefaultPageSize(PageSize.A4);
                                iText.Layout.Document document = new iText.Layout.Document(pdfDoc);

                                try
                                {
                                    // Step 1: Add cover page if it exists
                                    if (!string.IsNullOrEmpty(coverHtml2))
                                    {
                                        using (MemoryStream coverStream = new MemoryStream())
                                        {
                                            ConverterProperties coverProps = new ConverterProperties();
                                            iText.Html2pdf.HtmlConverter.ConvertToPdf(coverHtml2, coverStream, coverProps);
                                            PdfDocument coverPdfDoc = new PdfDocument(new PdfReader(new MemoryStream(coverStream.ToArray())));
                                            coverPdfDoc.CopyPagesTo(1, 1, pdfDoc);
                                            coverPdfDoc.Close();
                                        }
                                    }

                                    htmlDoc2.LoadHtml(report.HtmlCode);
                                    var nodesToRemove2 = htmlDoc2.DocumentNode.SelectNodes("//cover");
                                    if (nodesToRemove2 != null)
                                    {
                                        foreach (var node in nodesToRemove2)
                                        {
                                            node.Remove();
                                        }
                                    }
                                    
                                    mainContent = htmlDoc2.DocumentNode.OuterHtml;
                                    
                                    string cssStyle = @"<style>
                                      #header {
                                        position: running(header);
                                      }

                                      #footer {
                                        position: running(footer);
                                      }

                                      @page {
                                        @top-center {
                                          content: element(header);
                                        }

                                        @bottom-center {
                                          content: element(footer);
                                        }
                                      }

                                      #current-page-placeholder::before {
                                        content: counter(page);
                                      }

                                      #total-pages-placeholder::before {
                                        content: counter(pages);
                                      }
                                    </style>";
      
                                    Match footerMatch = Regex.Match(mainContent, @"<footer>(.*?)</footer>", RegexOptions.Singleline);
                                    string footerContent = footerMatch.Success ? footerMatch.Groups[1].Value : string.Empty;

                                    // Remove the original footer
                                    mainContent = Regex.Replace(mainContent, @"<footer>.*?</footer>", "", RegexOptions.Singleline);

                                    // Replace header tag, insert CSS, and add footer content
                                    string pattern = @"(<body>).*?(<header>.*?</header>)";
                                    string replacement = "$1" + cssStyle + @"
                                    $2
                                    <div id=""footer"">" + footerContent + "</div>";
        
                                    mainContent = Regex.Replace(mainContent, pattern, replacement, RegexOptions.Singleline);

                                    // Now replace the header tag with div
                                    string headerPattern = @"<header>(.*?)</header>";
                                    string headerReplacement = "<div id=\"header\">$1</div>";
                                    mainContent = Regex.Replace(mainContent, headerPattern, headerReplacement, RegexOptions.Singleline);
                                    
                                    // Step 2: Convert main content HTML to PDF
                                    using (MemoryStream mainContentStream = new MemoryStream())
                                    {
                                        ConverterProperties props = new ConverterProperties();
                                        iText.Html2pdf.HtmlConverter.ConvertToPdf(mainContent, mainContentStream, props);
                                        PdfDocument mainContentPdfDoc = new PdfDocument(new PdfReader(new MemoryStream(mainContentStream.ToArray())));
                                        mainContentPdfDoc.CopyPagesTo(1, mainContentPdfDoc.GetNumberOfPages(), pdfDoc);
                                        mainContentPdfDoc.Close();
                                    }
                                    
                                }
                                finally
                                {
                                    document.Close();
                                }

                                pdfContent = finalPdfStream.ToArray();
                            }

                            string fileName3 = $"{report.Name}_{report.Version}.pdf";
                            var mimeType3 = "application/pdf";
                            _logger.LogInformation("Report downloaded successfully. User: {0}", aspNetUserId);

                            return new FileContentResult(pdfContent, mimeType3)
                            {
                                FileDownloadName = fileName3
                            };
                                                      
                            

                            break;
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
    
    [HttpPost]
    [Route("Template")]
    [Authorize (Roles = "Admin,SuperUser")]
    public async Task<ReportImportResultViewModel> Import([FromBody] ReportImportViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (model.FileContent != null)
                {
                    if (fileCheck.CheckFile(model.FileContent))
                    {
                        var converter = new DocumentConverter().ImageConverter(image => {
                            using (var stream = image.GetStream()) {
                                var base64 = StreamToBase64(stream);
                                var src = "data:" + image.ContentType + ";base64," + base64;
                                return new Dictionary<string, string> { { "src", src } };
                            }
                        });
                        MemoryStream stream = new MemoryStream(model.FileContent);

                        var result = converter.ConvertToHtml(stream);
                        var html = result.Value; // The generated HTML
                        var warnings = result.Warnings;
                        ReportImportResultViewModel res = new ReportImportResultViewModel();
                        res.Body = html;
                        res.WarningMessages = warnings.ToList();
                        return res;

                    }
                    return null;

                }

                return null;
            }
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding report templates. User: {0}",
                aspNetUserId);
            throw;
        }
    }
   
    private static string StreamToBase64(System.IO.Stream stream) {
        var memoryStream = new System.IO.MemoryStream();
        stream.CopyTo(memoryStream);
        return System.Convert.ToBase64String(memoryStream.ToArray());
    }
    
}