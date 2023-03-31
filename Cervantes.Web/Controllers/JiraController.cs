using System;
using System.Security.Claims;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.IFR.Jira;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cervantes.Web.Controllers;
[Authorize(Roles = "Admin,SuperUser,User")]
public class JiraController : Controller
{
    private IVulnManager vulnManager = null;
    private IProjectManager projectManager = null;
    private ITargetManager targetManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private IVulnNoteManager vulnNoteManager = null;
    private IVulnAttachmentManager vulnAttachmentManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private readonly IHostingEnvironment _appEnvironment;
    private readonly ILogger<VulnController> _logger = null;
    private IJIraService jiraService = null;
    private IJiraManager jiraManager = null;
    private IJiraCommentManager jiraCommentManager = null;

    public JiraController(IVulnManager vulnManager, IProjectManager projectManager, ILogger<VulnController> logger,
        ITargetManager targetManager, IVulnTargetManager vulnTargetManager,
        IVulnCategoryManager vulnCategoryManager, IVulnNoteManager vulnNoteManager,
        IVulnAttachmentManager vulnAttachmentManager, IHostingEnvironment _appEnvironment, IJIraService jiraService,
        IJiraManager jiraManager, IJiraCommentManager jiraCommentManager)
    {
        this.vulnManager = vulnManager;
        this.projectManager = projectManager;
        this.targetManager = targetManager;
        this.vulnCategoryManager = vulnCategoryManager;
        this.vulnAttachmentManager = vulnAttachmentManager;
        this.vulnNoteManager = vulnNoteManager;
        this.vulnTargetManager = vulnTargetManager;
        this._appEnvironment = _appEnvironment;
        _logger = logger;
        this.jiraService = jiraService;
        this.jiraManager = jiraManager;
        this.jiraCommentManager = jiraCommentManager;
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateIssue(IFormCollection form)
    {
        try
        {
            var vulnId = Guid.Parse(form["vulnId"]);
            var issue = jiraService.CreateIssue(vulnId, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (issue == false)
            {
                TempData["errorCreateJira"] = "Error editing vuln!";
                _logger.LogError( "An error ocurred creating Jira Issue on Vuln {0} User {1}", form["vulnId"],User.FindFirstValue(ClaimTypes.Name));
                if(form["from"] == "workspace")
                {
                    return RedirectToAction("Details", "Vuln", new {area="Workspace", project=form["project"],id = form["vulnId"]});
                }
                return RedirectToAction("Details", "Vuln", new {id = vulnId});
            }
            var vuln = vulnManager.GetById(vulnId);
            vuln.JiraCreated = true;
            vulnManager.Context.SaveChanges();
            if(form["from"] == "workspace")
            {
                return RedirectToAction("Details", "Vuln", new {area="Workspace", project=form["project"],id = vulnId});
            }
            TempData["createJira"] = "Error editing vuln!";
            _logger.LogInformation( "Jira Created Issue on Vuln {0} User {1}", form["vulnId"],User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Vuln", new {id = vulnId});
        }
        catch (Exception e)
        {
            var vuln = vulnManager.GetById(Guid.Parse(form["vulnId"]));
            vuln.JiraCreated = false;
            vulnManager.Context.SaveChanges();
            TempData["errorCreateJira"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred creating Jira Issue on Vuln {0} User {1}", form["vulnId"],
                User.FindFirstValue(ClaimTypes.Name));
            if(form["from"] == "workspace")
            {
                return RedirectToAction("Details", "Vuln", new {area="Workspace", project=form["project"],id = form["vulnId"]});
            }
            return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteIssue(IFormCollection form)
    {
        try
        {
            var vulnId = Guid.Parse(form["vulnId"]);
            var jira = jiraManager.GetByVulnId(vulnId);
            var issue = jiraService.DeleteIssue(jira.JiraKey);
            if (issue == false)
            {
                TempData["errorCreateJira"] = "Error editing vuln!";
                _logger.LogError( "An error ocurred creating Jira Issue on Vuln {0} User {1}", form["vulnId"],User.FindFirstValue(ClaimTypes.Name));
                if(form["from"] == "workspace")
                {
                    return RedirectToAction("Details", "Vuln", new {area="Workspace", project=form["project"],id = form["vulnId"]});
                }
                return RedirectToAction("Details", "Vuln", new {id = vulnId});
            }

            jiraManager.Remove(jira);
            jiraManager.Context.SaveChanges();
            
            var vuln = vulnManager.GetById(vulnId);
            vuln.JiraCreated = false;
            vulnManager.Context.SaveChanges();
            if(form["from"] == "workspace")
            {
                return RedirectToAction("Details", "Vuln", new {area="Workspace", project=form["project"],id = vulnId});
            }
            return RedirectToAction("Details", "Vuln", new {id = vulnId});
        }
        catch (Exception e)
        {
            TempData["errorCreateJira"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred creating Jira Issue on Vuln {0} User {1}", form["vulnId"],
                User.FindFirstValue(ClaimTypes.Name));
            if(form["from"] == "workspace")
            {
                return RedirectToAction("Details", "Vuln", new {area="Workspace", project=form["project"],id = form["vulnId"]});
            }
            return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
        }
        

    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateIssue(IFormCollection form)
    {
        try
        {
            var vulnId = Guid.Parse(form["vulnId"]);
            var jira = jiraManager.GetByVulnId(vulnId);
            jiraService.UpdateIssue(jira.JiraKey);
            if(form["from"] == "workspace")
            {
                return RedirectToAction("Details", "Vuln", new {area="Workspace", project=form["project"],id = vulnId});
            }
            return RedirectToAction("Details", "Vuln", new {id = vulnId});
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred updating Jira Issue on Vuln {0} User {1}", form["vulnId"],
                User.FindFirstValue(ClaimTypes.Name));
            if(form["from"] == "workspace")
            {
                return RedirectToAction("Details", "Vuln", new {area="Workspace", project=form["project"],id = form["vulnId"]});
            }
            return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
        }
        

    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddComment(IFormCollection form)
    {
        try
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");
            
            var vulnId = Guid.Parse(form["vulnId"]);
            var comment = sanitizer.Sanitize(HttpUtility.HtmlDecode(form["commentDescription"]));
            var jira = jiraManager.GetByVulnId(vulnId);
            var issue = jiraService.AddComment(jira.JiraKey, comment);
            if (issue == false)
            {
                TempData["errorCreateJira"] = "Error editing vuln!";
                _logger.LogError( "An error ocurred creating Jira Issue on Vuln {0} User {1}", form["vulnId"],User.FindFirstValue(ClaimTypes.Name));
                if(form["from"] == "workspace")
                {
                    return RedirectToAction("Details", "Vuln", new {area="Workspace", project=form["project"],id = form["vulnId"]});
                }
                return RedirectToAction("Details", "Vuln", new {id = vulnId});
            }
            if(form["from"] == "workspace")
            {
                return RedirectToAction("Details", "Vuln", new {area="Workspace", project=form["project"],id = form["vulnId"]});
            }
            
            return RedirectToAction("Details", "Vuln", new {id = vulnId});
        }
        catch (Exception e)
        {
            var vuln = vulnManager.GetById(Guid.Parse(form["vulnId"]));
            vuln.JiraCreated = false;
            vulnManager.Context.SaveChanges();
            TempData["errorCreateJira"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred creating Jira Issue on Vuln {0} User {1}", form["vulnId"],
                User.FindFirstValue(ClaimTypes.Name));
            if(form["from"] == "workspace")
            {
                return RedirectToAction("Details", "Vuln", new {area="Workspace", project=form["project"],id = form["vulnId"]});
            }
            return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
        }
        

    }
}