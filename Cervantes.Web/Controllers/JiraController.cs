using System.Security.Claims;
using System.Web;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.Jira;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JiraController : Controller
{
   private IVulnManager vulnManager = null;
    private IProjectManager projectManager = null;
    private ITargetManager targetManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private IVulnNoteManager vulnNoteManager = null;
    private IVulnAttachmentManager vulnAttachmentManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private readonly ILogger<JiraController> _logger = null;
    private IJIraService jiraService = null;
    private IJiraManager jiraManager = null;
    private IJiraCommentManager jiraCommentManager = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private Sanitizer sanitizer;
    
    public JiraController(IVulnManager vulnManager, IProjectManager projectManager, ILogger<JiraController> logger,
        ITargetManager targetManager, IVulnTargetManager vulnTargetManager,
        IVulnCategoryManager vulnCategoryManager, IVulnNoteManager vulnNoteManager,
        IVulnAttachmentManager vulnAttachmentManager, IWebHostEnvironment env, IJIraService jiraService,
        IJiraManager jiraManager, IJiraCommentManager jiraCommentManager, IHttpContextAccessor HttpContextAccessor, Sanitizer sanitizer)
    {
        this.vulnManager = vulnManager;
        this.projectManager = projectManager;
        this.targetManager = targetManager;
        this.vulnCategoryManager = vulnCategoryManager;
        this.vulnAttachmentManager = vulnAttachmentManager;
        this.vulnNoteManager = vulnNoteManager;
        this.vulnTargetManager = vulnTargetManager;
        this.env = env;
        _logger = logger;
        this.jiraService = jiraService;
        this.jiraManager = jiraManager;
        this.jiraCommentManager = jiraCommentManager;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.sanitizer = sanitizer;
    }
    
    [HttpGet]
    [HasPermission(Permissions.JiraRead)]
    public IEnumerable<CORE.Entities.Jira> GetJiras()
    {
        try
        {
            var model = jiraManager.GetAll().ToArray();

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting client vulns. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("{vulnId}")]
    [HasPermission(Permissions.JiraRead)]
    public CORE.Entities.Jira GetJiraByVuln(Guid vulnId)
    {
        try
        {
            var model = jiraManager.GetAll().FirstOrDefault(x => x.VulnId == vulnId);
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting Jira. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    
    
    [HttpGet]
    [Route("Comments/{vulnId}")]
    [HasPermission(Permissions.JiraCommentsRead)]
    public IEnumerable<CORE.Entities.JiraComments> GetCommentsByVuln(Guid vulnId)
    {
        try
        {
            var jira = jiraManager.GetAll().FirstOrDefault(x => x.VulnId == vulnId);
            var model = jiraCommentManager.GetAll().Where(x => x.JiraId == jira.Id).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting Jira. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpPost]
    [Route("{vulnId}")]
    [HasPermission(Permissions.JiraAdd)]
    public async Task<IActionResult> Add(Guid vulnId)
    {
        try{
            if (ModelState.IsValid)
            {
                var result = jiraManager.GetByVulnId(vulnId);
                if (result != null)
                {
                    _logger.LogError("Jira already exists. User: {0}",aspNetUserId);
                    return BadRequest("Invalid request");
                }
                
                
                jiraService.CreateIssue(vulnId,aspNetUserId);
                
                var vuln = vulnManager.GetById(vulnId);
                vuln.JiraCreated = true;
                await vulnManager.Context.SaveChangesAsync();
                
                _logger.LogInformation("Jira added successfully. User: {0}",aspNetUserId);
                return CreatedAtAction(nameof(GetJiraByVuln), new { vulnId = vuln.Id }, vuln);
            }
            _logger.LogError("An error ocurred adding a jira. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            var vuln = vulnManager.GetById(vulnId);
            vuln.JiraCreated = false;
            vulnManager.Context.SaveChanges();
            _logger.LogError(e,"An error ocurred adding a jira. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
    }

    [HttpDelete]
    [HasPermission(Permissions.JiraDelete)]
    public async Task<IActionResult> DeleteIssue(Guid vulnId)
    {
        try
        {
            var jira = jiraManager.GetByVulnId(vulnId);
            var issue = jiraService.DeleteIssue(jira.JiraKey);
            if (issue == false)
            {
                _logger.LogError("An error ocurred deleting Jira Issue on Vuln {0} User {1}", vulnId,
                    User.FindFirstValue(ClaimTypes.Name));
                return BadRequest("Invalid request");
            }

            jiraManager.Remove(jira);
            jiraManager.Context.SaveChanges();

            var vuln = vulnManager.GetById(vulnId);
            vuln.JiraCreated = false;
            vulnManager.Context.SaveChanges();
            _logger.LogInformation("Jira {0} deleted successfully. User {1}", jira.JiraKey, aspNetUserId);
            return NoContent();

        }
        catch (Exception e)
        {
            _logger.LogError("An error ocurred deleting Jira Issue on Vuln {0} User {1}", vulnId,
                User.FindFirstValue(ClaimTypes.Name));

            return BadRequest("Invalid request");
        }
    }
    
    [HttpPost]
    [Route("UpdateIssue/{vulnId}")]
    [HasPermission(Permissions.JiraEdit)]
    public async Task<IActionResult> UpdateIssue(Guid vulnId)
    {
        try
        {
            var jira = jiraManager.GetByVulnId(vulnId);
            jiraService.UpdateIssue(jira.JiraKey);
            _logger.LogInformation("Jira {0} updated successfully. User {1}", jira.JiraKey, aspNetUserId);

            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred updating Jira Issue on Vuln {0} User {1}", vulnId,
                aspNetUserId);
            return BadRequest("Invalid request");
        }
    }
    
    [HttpPost]
    [Route("Comment")]
    [HasPermission(Permissions.JiraCommentsAdd)]
    public async Task<IActionResult> AddComment(JiraCommentCreate model)
    {
        try
        {

            var comment = sanitizer.Sanitize(model.Comment);
            var jira = jiraManager.GetByVulnId(model.VulnId);
            var issue = await jiraService.AddCommentAsync(jira.JiraKey, comment);
            if (issue == false)
            {
                _logger.LogError( "An error ocurred adding comment in Jira on Vuln {0} User {1}", model.VulnId,aspNetUserId);
                return BadRequest("Invalid request");
            }

            _logger.LogInformation( "Jira comment added successfully on Vuln {0} User {1}", model.VulnId,aspNetUserId);
            return Created();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred creating Jira comment on Vuln {0} User {1}", model.VulnId,
                aspNetUserId);
            return BadRequest("Invalid request");
        }

    }
}