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
    private IProjectCustomFieldManager projectCustomFieldManager = null;
    private IProjectCustomFieldValueManager projectCustomFieldValueManager = null;
    private readonly IWebHostEnvironment env;
    private readonly ProjectTreeDeleter projectTreeDeleter;
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
        IReportTemplateManager reportTemplateManager, IProjectCustomFieldManager projectCustomFieldManager,
        IProjectCustomFieldValueManager projectCustomFieldValueManager,
        IWebHostEnvironment env, IHttpContextAccessor HttpContextAccessor, ILogger<ProjectController> logger,
        IFileCheck fileCheck,Sanitizer sanitizer, ProjectTreeDeleter projectTreeDeleter)
    {
        this.projectTreeDeleter = projectTreeDeleter;
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
        this.projectCustomFieldManager = projectCustomFieldManager;
        this.projectCustomFieldValueManager = projectCustomFieldValueManager;
        this.env = env;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        _logger = logger;
        this.emailService = emailService;
        this.fileCheck = fileCheck;
        this.sanitizer = sanitizer;
        this.HttpContextAccessor = HttpContextAccessor;
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

    [HasPermission(Permissions.ProjectsRead)]
    [HttpGet]
    [Route("Name/{projectName}")]
    public IEnumerable<CORE.Entities.Project> GetByName(string projectName)
    {
        try
        {
            return projectManager.GetAll().Where(x => x.Name.Contains(projectName));
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred getting a project. User: {0}",
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

    [HttpGet]
    [Route("Client/{clientName}")]
    [HasPermission(Permissions.ProjectsRead)]
    public IEnumerable<CORE.Entities.Project> GetByClientName(string clientName)
    {
        try
        {
            IEnumerable<CORE.Entities.Project> model = projectManager.GetAll().Where(x => x.Client.Name.Contains(clientName)).ToArray();
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
                var missingFields = MissingRequiredCustomFields(model.CustomFieldValues, null);
                if (missingFields.Count > 0)
                {
                    return BadRequest($"Required fields: {string.Join(", ", missingFields)}");
                }

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
                
                // Process custom field values
                if (model.CustomFieldValues != null && model.CustomFieldValues.Any())
                {
                    // Create custom field values using basic GenericManager methods
                    foreach (var kvp in model.CustomFieldValues)
                    {
                        var customFieldValue = new ProjectCustomFieldValue
                        {
                            Id = Guid.NewGuid(),
                            ProjectId = project.Id,
                            ProjectCustomFieldId = kvp.Key,
                            Value = sanitizer.Sanitize(kvp.Value),
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = DateTime.UtcNow,
                            UserId = aspNetUserId
                        };
                        await projectCustomFieldValueManager.AddAsync(customFieldValue);
                    }
                    await projectCustomFieldValueManager.Context.SaveChangesAsync();
                }
                
                _logger.LogInformation("Project created successfully. User: {0}",
                    aspNetUserId);
                return CreatedAtAction(nameof(GetById), new { projectId = project.Id }, project);
            }

            _logger.LogError("Validation failed when adding project. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred adding project. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while creating the project. Please try again later.");
        }
    }

    [HttpPut]
    [HasPermission(Permissions.ProjectsEdit)]
    public async Task<IActionResult> Edit([FromBody] ProjectEditViewModel model)
    {
        // [HasPermission] is not evaluated when a Blazor component calls this action in-process.
        if (!HttpContextAccessor.CurrentUserHas(Permissions.ProjectsEdit))
        {
            return StatusCode(403, "You do not have permission to perform this action");
        }

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
                    return NotFound();
                }

                await projectManager.Context.SaveChangesAsync();
                _logger.LogInformation("Project edited successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }

            _logger.LogError("Validation failed when editing project. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred editing project. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while updating the project. Please try again later.");
        }
    }

    [HttpDelete]
    [Route("{projectId}")]
    [HasPermission(Permissions.ProjectsDelete)]
    public async Task<IActionResult> Delete(Guid projectId)
    {
        // [HasPermission] is not evaluated when a Blazor component calls this action in-process.
        if (!HttpContextAccessor.CurrentUserHas(Permissions.ProjectsDelete))
        {
            return StatusCode(403, "You do not have permission to perform this action");
        }

        try
        {
            if (ModelState.IsValid)
            {
                if (projectManager.GetById(projectId) == null)
                {
                    return NotFound($"Project with ID {projectId} not found");
                }

                var tree = projectTreeDeleter.Stage(new[] { projectId });
                // One save: EF runs it in a single transaction, so the project and its content go together or not at all.
                await projectManager.Context.SaveChangesAsync();
                projectTreeDeleter.DeleteFolders(tree, aspNetUserId);
                _logger.LogInformation("Project deleted successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }

            _logger.LogError("Validation failed when deleting project. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred deleting project. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the project. Please try again later.");
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
                var addedIds = new List<string>();
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
                        addedIds.Add(item);


                        _logger.LogInformation("Project member added successfully. User: {0}",
                            aspNetUserId);

                        if (emailService.IsEnabled())
                        {
                            BackgroundJob.Enqueue(
                                () => emailService.SendAsignedProject(item, model.ProjectId));
                            BackgroundJob.Enqueue(
                                () => emailService.SendMemberAddedToProject(item, aspNetUserId, model.ProjectId));
                        }
                        
                    }
                }

                // Existing members are skipped, so a request that only names them changed nothing; the caller is told.
                if (addedIds.Count == 0 && model.MemberId.Any())
                {
                    return Conflict("All selected users are already members of the project");
                }
                
                return Created((string?)null, addedIds);
            }

            _logger.LogError("Validation failed when adding project member. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred adding project member. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpDelete]
    [Route("Member/{memberId}")]
    [HasPermission(Permissions.ProjectMembersDelete)]
    public async Task<IActionResult> DeleteMember(Guid memberId)
    {
        // [HasPermission] is not evaluated when a Blazor component calls this action in-process.
        if (!HttpContextAccessor.CurrentUserHas(Permissions.ProjectMembersDelete))
        {
            return StatusCode(403, "You do not have permission to remove project members");
        }

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
                    return NoContent();
                }

                _logger.LogError("An error ocurred deleting project member. User: {0}",
                    aspNetUserId);
                return NotFound();
            }

            _logger.LogError("Validation failed when deleting project member. User: {0}",
                aspNetUserId);
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred deleting project member. User: {0}",
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
                    return StatusCode(403, "Access denied: User does not have permission to access this project");
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
                return Created($"/api/Project/Note/{note.Id}", note);
            }

            _logger.LogError("An error ocurred adding project notes. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding project notes. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    
    [NonAction]
    public async Task<ProjectNote> GetNoteById(Guid noteId)
    {
        try
        {
            return projectNoteManager.GetById(noteId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing project notes. User: {0}",
                aspNetUserId);
            throw;
        }
    }

    [HttpPut]
    [Route("Note")]
    [HasPermission(Permissions.ProjectNotesEdit)]
    public async Task<IActionResult> EditNote(ProjectNoteEditViewModel model)
    {
        // [HasPermission] is not evaluated when a Blazor component calls this action in-process.
        if (!HttpContextAccessor.CurrentUserHas(Permissions.ProjectNotesEdit))
        {
            return StatusCode(403, "You do not have permission to perform this action");
        }

        try
        {
            if (ModelState.IsValid)
            {
                var note = projectNoteManager.GetById(model.Id);
                if (note == null)
                {
                    return NotFound("Project note not found");
                }

                // Authorize against the project stored in the database, never the one sent in the request.
                if (projectUserManager.VerifyUser(note.ProjectId, aspNetUserId) == null)
                {
                    _logger.LogWarning("Access denied editing project note {NoteId}. User: {UserId}", note.Id, aspNetUserId);
                    return StatusCode(403, "Access denied: User does not have permission to access this project");
                }

                // An edit never moves a note to another project.
                if (model.ProjectId != note.ProjectId)
                {
                    return BadRequest("A note cannot be moved to another project");
                }

                // The author (note.UserId) is not changed by an edit.
                note.Name = sanitizer.Sanitize(model.Name);
                note.Description = sanitizer.Sanitize(model.Description);

                await projectNoteManager.Context.SaveChangesAsync();
                _logger.LogInformation("Project Note edited successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }

            _logger.LogError("An error ocurred editing project notes. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
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
                        return StatusCode(403, "Access denied: User does not have permission to access this project");
                    }

                    projectNoteManager.Remove(result);
                    await projectNoteManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Project note deleted successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }

                _logger.LogError("An error ocurred deleting project notes. User: {0}",
                    aspNetUserId);
                return NotFound();
            }

            _logger.LogError("An error ocurred deleting project notes. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
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

    [NonAction]
    public async Task<ProjectAttachment> GetAttachmentById(Guid attachmentId)
    {
        try
        {
            return projectAttachmentManager.GetById(attachmentId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing project notes. User: {0}",
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
                    return StatusCode(403, "Access denied: User does not have permission to access this project");
                }

                var path = "";
                var unique = "";
                var file = "";
                if (model.FileContent != null)
                {
                    if (fileCheck.CheckAttachment(model.FileContent))
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
                return Created($"/api/Project/Attachment/{attachment.Id}", attachment);
            }

            _logger.LogError("An error ocurred adding project attachments. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
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
                        return StatusCode(403, "Access denied: User does not have permission to access this project");
                    }

                    projectAttachmentManager.Remove(result);
                    await projectAttachmentManager.Context.SaveChangesAsync();
                    AttachmentFiles.DeleteFile(env.WebRootPath, result.FilePath, "Projects", result.ProjectId, _logger, "ProjectAttachment", result.Id, aspNetUserId);
                    _logger.LogInformation("Project attachment deleted successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }

                return NotFound("Project attachment not found");
            }

            _logger.LogError("An error ocurred deleting project attachments. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
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
                var project = projectManager.GetById(model.Project);

                if (project != null)
                {
                    var user = projectUserManager.VerifyUser(project.Id, aspNetUserId);
                    if (user == null)
                    {
                        return StatusCode(403, "Access denied: User does not have permission to access this project");
                    }

                    project.ExecutiveSummary = sanitizer.Sanitize(model.ExecutiveSummary);
                    await projectManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Project executive summary updated successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }

                _logger.LogError("An error ocurred adding project executive summary. User: {0}",
                    aspNetUserId);
                return BadRequest("Invalid request");
            }

            _logger.LogError("An error ocurred adding project executive summary. User: {0}",
                aspNetUserId);
            return BadRequest("Invalid request");
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

    [HttpGet]
    [Route("GetCustomFieldValues/{projectId}")]
    [HasPermission(Permissions.ProjectsRead)]
    public async Task<List<ProjectCustomFieldValue>> GetCustomFieldValues(Guid projectId)
    {
        try
        {
            return projectCustomFieldValueManager.GetAll()
                .Where(v => v.ProjectId == projectId)
                .ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting project custom field values. User: {0}", aspNetUserId);
            return new List<ProjectCustomFieldValue>();
        }
    }

    [HttpPost]
    [Route("UpdateCustomFieldValues/{projectId}")]
    [HasPermission(Permissions.ProjectCustomFieldsEdit)]
    public async Task<IActionResult> UpdateCustomFieldValues(Guid projectId, [FromBody] Dictionary<Guid, string> customFieldValues)
    {
        try
        {
            var project = projectManager.GetById(projectId);
            if (project == null)
            {
                return NotFound();
            }

            // Changing project data requires membership in the project
            if (projectUserManager.VerifyUser(projectId, aspNetUserId) == null)
            {
                _logger.LogWarning("Access denied updating custom fields of project {ProjectId}. User: {UserId}",
                    projectId, aspNetUserId);
                return StatusCode(403, "Access denied: User does not have permission to access this project");
            }

            var missingValues = MissingRequiredCustomFields(customFieldValues, projectId);
            if (missingValues.Count > 0)
            {
                return BadRequest($"Required fields: {string.Join(", ", missingValues)}");
            }

            // Get existing values
            var existingValues = projectCustomFieldValueManager.GetAll()
                .Where(v => v.ProjectId == projectId)
                .ToList();

            foreach (var kvp in customFieldValues)
            {
                var existingValue = existingValues.FirstOrDefault(v => v.ProjectCustomFieldId == kvp.Key);
                
                if (existingValue != null)
                {
                    // Update existing value
                    existingValue.Value = sanitizer.Sanitize(kvp.Value);
                    existingValue.ModifiedDate = DateTime.UtcNow;
                    projectCustomFieldValueManager.Update(existingValue);
                }
                else
                {
                    // Create new value
                    var newValue = new ProjectCustomFieldValue
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = projectId,
                        ProjectCustomFieldId = kvp.Key,
                        Value = sanitizer.Sanitize(kvp.Value),
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        UserId = aspNetUserId
                    };
                    await projectCustomFieldValueManager.AddAsync(newValue);
                }
            }

            await projectCustomFieldValueManager.Context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred updating project custom field values. User: {0}", aspNetUserId);
            return BadRequest(new { message = "An error occurred updating custom field values" });
        }
    }

    // Labels of the active required project custom fields left empty. An edit keeps stored values it does not send.
    private IReadOnlyList<string> MissingRequiredCustomFields(IReadOnlyDictionary<Guid, string>? submitted, Guid? entityId)
    {
        var fields = projectManager.Context.Set<CORE.Entities.ProjectCustomField>().AsNoTracking().ToList()
            .Select(f => (f.Id, f.Label, f.IsRequired, f.IsActive));
        var stored = entityId == null
            ? new List<KeyValuePair<Guid, string?>>()
            : projectManager.Context.Set<CORE.Entities.ProjectCustomFieldValue>().AsNoTracking().Where(v => v.ProjectId == entityId)
                .Select(v => new KeyValuePair<Guid, string?>(v.ProjectCustomFieldId, v.Value)).ToList();
        return CustomFieldRules.MissingRequired(fields, CustomFieldRules.Merge(stored, submitted));
    }
}
