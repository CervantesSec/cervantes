using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.Email;
using Cervantes.IFR.File;
using Cervantes.Server.Helpers;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectController : ControllerBase
{
    private IProjectManager projectManager = null;
    private IClientManager clientManager = null;
    private IProjectUserManager projectUserManager = null;
    private IProjectNoteManager projectNoteManager = null;
    private IProjectAttachmentManager projectAttachmentManager = null;
    private ITargetManager targetManager = null;
    private ITaskManager taskManager = null;
    private IUserManager userManager = null;
    private IVulnManager vulnManager = null;
    private IReportManager reportManager = null;
    private IReportTemplateManager reportTemplateManager = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private readonly ILogger<ProjectController> _logger = null;
    private IFileCheck fileCheck;
    private IEmailService emailService;
    private Sanitizer sanitizer;
    private Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager = null;


    /// <summary>
    /// ProjectController Constructor
    /// </summary>
    /// <param name="projectManager">ProjectManager</param>
    /// <param name="clientManager">ClientManager</param>
    public ProjectController(IProjectManager projectManager, IClientManager clientManager,
        IProjectUserManager projectUserManager, IProjectNoteManager projectNoteManager, IEmailService emailService,
        IProjectAttachmentManager projectAttachmentManager, ITargetManager targetManager, ITaskManager taskManager,
        IUserManager userManager, IVulnManager vulnManager, IReportManager reportManager,
        IReportTemplateManager reportTemplateManager,
        IWebHostEnvironment env, IHttpContextAccessor HttpContextAccessor, ILogger<ProjectController> logger,
        IFileCheck fileCheck,Sanitizer sanitizer)
    {
        this.projectManager = projectManager;
        this.clientManager = clientManager;
        this.projectUserManager = projectUserManager;
        this.projectNoteManager = projectNoteManager;
        this.projectAttachmentManager = projectAttachmentManager;
        this.targetManager = targetManager;
        this.taskManager = taskManager;
        this.userManager = userManager;
        this.vulnManager = vulnManager;
        this.reportManager = reportManager;
        this.reportTemplateManager = reportTemplateManager;
        this.env = env;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        _logger = logger;
        this.emailService = emailService;
        this.fileCheck = fileCheck;
        this.sanitizer = sanitizer;
    }

    [HttpGet]
    [HasPermission(Permissions.ProjectsRead)]
    public IEnumerable<CORE.Entities.Project> Get()
    {
        try
        {
            IEnumerable<CORE.Entities.Project> model = projectManager.GetAll().Include(x => x.Client)
                .Include(x => x.User).ToArray();

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting projects. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("{projectId}")]
    [HasPermission(Permissions.ProjectsRead)]
    public CORE.Entities.Project GetById(Guid projectId)
    {
        try
        {
            return projectManager.GetAll().Where(x => x.Id == projectId).Include(x => x.User).Include(x => x.Client)
                .FirstOrDefault();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting project. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("Client/{id}")]
    [HasPermission(Permissions.ProjectsRead)]
    public IEnumerable<CORE.Entities.Project> GetByClientId(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.Project> model = projectManager.GetAll().Where(x => x.ClientId == id).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting client projects. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPost]
    [HasPermission(Permissions.ProjectsAdd)]
    public async Task<IActionResult> Add([FromBody] ProjectCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var project = new CORE.Entities.Project();
                project.Template = model.Template;
                project.Name = sanitizer.Sanitize(model.Name);
                project.Description = sanitizer.Sanitize(model.Description);
                project.UserId = aspNetUserId;
                project.ClientId = model.ClientId;
                project.StartDate = model.StartDate.ToUniversalTime();
                project.EndDate = model.EndDate.ToUniversalTime();
                project.Status = model.Status;
                project.ClientId = model.ClientId;
                project.Template = model.Template;
                project.ProjectType = model.ProjectType;
                project.Language = model.Language;
                project.Score = model.Score;
                project.FindingsId = sanitizer.Sanitize(model.FindingsId);
                project.ExecutiveSummary = "";
                project.BusinessImpact = model.BusinessImpact;

                await projectManager.AddAsync(project);
                await projectManager.Context.SaveChangesAsync();
                await projectUserManager.AddAsync(new ProjectUser
                {
                    ProjectId = project.Id,
                    UserId = aspNetUserId
                });
                await projectUserManager.Context.SaveChangesAsync();
                _logger.LogInformation("Project created successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred adding project. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding project. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPut]
    [HasPermission(Permissions.ProjectsEdit)]
    public async Task<IActionResult> Edit([FromBody] ProjectEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var project = projectManager.GetById(model.Id);
                if (project != null)
                {
                    project.Template = model.Template;
                    project.Name = sanitizer.Sanitize(model.Name);
                    project.Description = sanitizer.Sanitize(model.Description);
                    project.Language = model.Language;
                    project.StartDate = model.StartDate.ToUniversalTime();
                    project.EndDate = model.EndDate.ToUniversalTime();
                    project.Status = model.Status;
                    project.ClientId = model.ClientId;
                    project.ProjectType = model.ProjectType;
                    project.Score = model.Score;
                    project.FindingsId = sanitizer.Sanitize(model.FindingsId);
                    project.BusinessImpact = model.BusinessImpact;
                }
                else
                {
                    return BadRequest();
                }

                await projectManager.Context.SaveChangesAsync();
                _logger.LogInformation("Project edited successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred adding project. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing project. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpDelete]
    [Route("{projectId}")]
    [HasPermission(Permissions.ProjectsDelete)]
    public async Task<IActionResult> Delete(Guid projectId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var project = projectManager.GetById(projectId);
                if (project != null)
                {
                    projectManager.Remove(project);
                }
                else
                {
                    return BadRequest();
                }

                await projectManager.Context.SaveChangesAsync();
                _logger.LogInformation("Project deleted successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred deleting project. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting project. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("Members/{project}")]
    [HasPermission(Permissions.ProjectMembersRead)]
    public IEnumerable<CORE.Entities.ProjectUser> GetMembers(Guid project)
    {
        try
        {
            IEnumerable<CORE.Entities.ProjectUser> model = projectUserManager.GetAll()
                .Where(x => x.ProjectId == project).Include(x => x.User).ToArray();

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting project members. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPost]
    [Route("Member")]
    [HasPermission(Permissions.ProjectMembersAdd)]
    public async Task<IActionResult> AddMember([FromBody] MemberViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                foreach (var item in model.MemberId)
                {
                    var result = projectUserManager.GetAll()
                        .Where(x => x.UserId == item && x.ProjectId == model.ProjectId);

                    if (result.FirstOrDefault() == null)
                    {
                        var user = new ProjectUser
                        {
                            ProjectId = model.ProjectId,
                            UserId = item
                        };
                        await projectUserManager.AddAsync(user);
                        await projectUserManager.Context.SaveChangesAsync();


                        _logger.LogInformation("Project member added successfully. User: {0}",
                            aspNetUserId);

                        if (emailService.IsEnabled())
                        {
                            BackgroundJob.Enqueue(
                                () => emailService.SendAsignedProject(item, model.ProjectId));
                        }
                        
                    }
                }
                
                return Ok();
            }

            _logger.LogError("An error ocurred adding project member. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding project member. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpDelete]
    [Route("Member/{memberId}")]
    [HasPermission(Permissions.ProjectMembersDelete)]
    public async Task<IActionResult> DeleteMember(Guid memberId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = projectUserManager.GetById(memberId);
                if (result != null)
                {
                    projectUserManager.Remove(result);
                    projectUserManager.Context.SaveChanges();
                    _logger.LogInformation("Project member deleted successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred deleting project member. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred deleting project member. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting project member. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("Note/{project}")]
    [HasPermission(Permissions.ProjectNotesRead)]
    public IEnumerable<CORE.Entities.ProjectNote> GetNotes(Guid project)
    {
        try
        {
            IEnumerable<CORE.Entities.ProjectNote> model = projectNoteManager.GetAll()
                .Where(x => x.ProjectId == project).Include(x => x.User).ToArray();

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting project notes. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPost]
    [Route("Note")]
    [HasPermission(Permissions.ProjectNotesAdd)]
    public async Task<IActionResult> AddNote(ProjectCreateNoteViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return BadRequest();
                }
                var note = new CORE.Entities.ProjectNote();
                note.Id = Guid.NewGuid();
                note.ProjectId = model.ProjectId;
                note.UserId = aspNetUserId;
                note.Name = sanitizer.Sanitize(model.Name);
                note.Description = sanitizer.Sanitize(model.Description);

                await projectNoteManager.AddAsync(note);
                await projectNoteManager.Context.SaveChangesAsync();
                _logger.LogInformation("Project note added successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred adding project notes. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding project notes. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPut]
    [Route("Note")]
    [HasPermission(Permissions.ProjectNotesEdit)]
    public async Task<IActionResult> EditNote(ProjectNoteEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return BadRequest();
                }
                var note = projectNoteManager.GetById(model.Id);
                note.Id = model.Id;
                note.ProjectId = model.ProjectId;
                note.UserId = aspNetUserId;
                note.Name = sanitizer.Sanitize(model.Name);
                note.Description = sanitizer.Sanitize(model.Description);

                await projectNoteManager.Context.SaveChangesAsync();
                _logger.LogInformation("Project Note edited successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred editing project notes. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing project notes. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpDelete]
    [Route("Note/{noteId}")]
    [HasPermission(Permissions.ProjectNotesDelete)]
    public async Task<IActionResult> DeleteNote(Guid noteId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = projectNoteManager.GetById(noteId);
                if (result != null)
                {
                    var user = projectUserManager.VerifyUser(result.ProjectId, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest();
                    }

                    projectNoteManager.Remove(result);
                    await projectNoteManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Project note deleted successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred deleting project notes. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred deleting project notes. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting project notes. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("Attachment/{project}")]
    [HasPermission(Permissions.ProjectAttachmentsRead)]
    public IEnumerable<CORE.Entities.ProjectAttachment> GetAttachments(Guid project)
    {
        try
        {
            IEnumerable<CORE.Entities.ProjectAttachment> model = projectAttachmentManager.GetAll()
                .Where(x => x.ProjectId == project).Include(x => x.User).ToArray();

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting project attachments. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPost]
    [Route("Attachment")]
    [HasPermission(Permissions.ProjectAttachmentsAdd)]
    public async Task<IActionResult> AddAttachment(ProjectAttachmentViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return BadRequest();
                }

                var path = "";
                var unique = "";
                var file = "";
                if (model.FileContent != null)
                {
                    if (fileCheck.CheckFile(model.FileContent))
                    {
                        unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
                        path = $"{env.WebRootPath}/Attachments/Projects/{model.ProjectId}";
                        file = $"{env.WebRootPath}/Attachments/Projects/{model.ProjectId}/{unique}";
                        if (Directory.Exists(path))
                        {
                            var fs = System.IO.File.Create(file);
                            fs.Write(model.FileContent, 0,
                                model.FileContent.Length);
                            fs.Close();
                        }
                        else
                        {
                            Directory.CreateDirectory(path);
                            var fs = System.IO.File.Create(file);
                            fs.Write(model.FileContent, 0,
                                model.FileContent.Length);
                            fs.Close();
                        }
                    }
                    else
                    {
                        _logger.LogError("An error ocurred adding a attachment filetype not admitted. User: {0}",
                            aspNetUserId);
                        return BadRequest("Invalid file type");
                    }
                }

                var attachment = new CORE.Entities.ProjectAttachment();
                attachment.Id = Guid.NewGuid();
                attachment.ProjectId = model.ProjectId;
                attachment.UserId = aspNetUserId;
                attachment.Name = sanitizer.Sanitize(model.Name);
                attachment.FilePath = "Attachments/Projects/" + model.ProjectId.ToString() + "/" + unique;

                await projectAttachmentManager.AddAsync(attachment);
                await projectAttachmentManager.Context.SaveChangesAsync();

                _logger.LogInformation("Project attachment added successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }

            _logger.LogError("An error ocurred adding project attachments. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding project attachments. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpDelete]
    [Route("Attachment/{attachmentId}")]
    [HasPermission(Permissions.ProjectAttachmentsDelete)]
    public async Task<IActionResult> DeleteAttahcment(Guid attachmentId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = projectAttachmentManager.GetById(attachmentId);
                if (result != null)
                {
                    var user = projectUserManager.VerifyUser(result.ProjectId, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest();
                    }

                    projectAttachmentManager.Remove(result);
                    await projectAttachmentManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Project attachment deleted successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                return BadRequest();
            }

            _logger.LogError("An error ocurred deleting project attachments. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting project attachments. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPost]
    [Route("ExecutiveSummary")]
    [HasPermission(Permissions.ProjectExecutiveSummaryEdit)]
    public async Task<IActionResult> ExecutiveSumamry([FromBody] ExecutiveSummaryViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");

                var project = projectManager.GetById(model.Project);

                if (project != null)
                {
                    var user = projectUserManager.VerifyUser(project.Id, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest();
                    }

                    project.ExecutiveSummary = sanitizer.Sanitize(model.ExecutiveSummary);
                    await projectManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Project executive summary updated successfully. User: {0}",
                        aspNetUserId);
                    return Ok();
                }

                _logger.LogError("An error ocurred adding project executive summary. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }

            _logger.LogError("An error ocurred adding project executive summary. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding project executive summary. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpGet]
    [Route("VerifyUser/{projectId}")]
    public async Task<bool> VerifyUser(Guid projectId)
    {
        try
        {
            var user = projectUserManager.VerifyUser(projectId, aspNetUserId);
            if (user != null)
            {
                return true;
            }
            /*else
            {
                var u = userManager.GetByUserId(aspNetUserId);
                if (u != null)
                {
                    var role = await _userManager.GetRolesAsync(u);
                    if (role.Contains("Admin") || role.Contains("SuperUser"))
                    {
                        return true;
                    }
                    return false;
                }


            }*/

            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding verifying project user. User: {0}",
                aspNetUserId);
            throw;
        }
    }
}