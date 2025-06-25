using System.Security.Claims;
using System.Web;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.Email;
using Cervantes.IFR.File;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReverseMarkdown.Converters;
using TaskStatus = Cervantes.CORE.Entities.TaskStatus;

namespace Cervantes.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController: ControllerBase
{

    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private ITaskManager taskManager = null;
    private ITaskNoteManager taskNoteManager = null;
    private ITaskAttachmentManager taskAttachmentManager = null;
    private ITaskTargetManager taskTargetManager = null;
    private ITargetManager targetManager = null;
    private IUserManager userManager = null;
    private readonly ILogger<TaskController> _logger = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private IFileCheck fileCheck;
    private IEmailService emailService;
    private Sanitizer sanitizer;

    public TaskController(ITaskManager taskManager, IProjectManager projectManager,
        ITargetManager targetManager, ITaskNoteManager taskNoteManager, ITaskAttachmentManager taskAttachmentManager,
        IProjectUserManager projectUserManager, ITaskTargetManager taskTargetManager,
        IUserManager userManager,ILogger<TaskController> logger, IWebHostEnvironment env,IHttpContextAccessor HttpContextAccessor,
        IEmailService emailService,IFileCheck fileCheck, Sanitizer sanitizer)
    {
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        this.taskManager = taskManager;
        this.targetManager = targetManager;
        this.taskNoteManager = taskNoteManager;
        this.taskAttachmentManager = taskAttachmentManager;
        this.taskTargetManager = taskTargetManager;
        this.userManager = userManager;
        _logger = logger;
        this.env = env;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.emailService = emailService;
        this.fileCheck = fileCheck;
        this.sanitizer = sanitizer;
    }
    
    [HttpGet]
    [HasPermission(Permissions.TasksRead)]
    public IEnumerable<CORE.Entities.Task> Get()
    {
        try
        {
            var model = taskManager.GetAll().Include(x => x.AsignedUser).ToList();
        
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting tasks. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("Client/{id}")]
    [HasPermission(Permissions.TasksRead)]
    public IEnumerable<CORE.Entities.Task> GetByClientId(Guid id)
    {
        try
        {
            var ids = projectManager.GetAll().Where(x => x.ClientId == id).Select(x => x.Id).ToList();
            List<CORE.Entities.Task> model = new List<CORE.Entities.Task>();
            foreach (var pro in ids)
            {
                var tasks = taskManager.GetAll().Where(x => x.ProjectId == pro).ToList();
                foreach (var task in tasks)
                {
                    model.Add(task);
                }
            
            }
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting client tasks. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("Project/{id}")]
    [HasPermission(Permissions.TasksRead)]
    public IEnumerable<CORE.Entities.Task> GetByProject(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.Task> model = taskManager.GetAll().Where(x => x.ProjectId == id).Include(x=> x.AsignedUser).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting project tasks. User: {0}",
                aspNetUserId);
            throw;
        }
       
    }
    
    [HttpGet]
    [Route("Project/{projectId}/User")]
    [HasPermission(Permissions.TasksRead)]
    public IEnumerable<CORE.Entities.Task> GetByUser(Guid projectId)
    {
        try
        {
            IEnumerable<CORE.Entities.Task> model = taskManager.GetAll().Where(x => x.ProjectId == projectId & x.AsignedUserId == aspNetUserId).Include(x=> x.AsignedUser).ToArray();

            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting project user tasks. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
   
    [NonAction]
    public CORE.Entities.Task GetTaskById(Guid taskId)
    {
        try
        {
            return taskManager.GetById(taskId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting project user tasks. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpPost]
    [HasPermission(Permissions.TasksAdd)]
    public async Task<IActionResult> Add([FromBody] TaskCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (model.ProjectId != null && model.ProjectId != Guid.Empty)
                {
                    var user = projectUserManager.VerifyUser(model.ProjectId.Value, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest("NotAllowed");
                    }
                }

                var task = new CORE.Entities.Task();
                task.Id = Guid.NewGuid();
                task.Template = false;
                task.Name = sanitizer.Sanitize(model.Name);
                task.Description = sanitizer.Sanitize(model.Description);
                task.StartDate = model.StartDate;
                task.EndDate = model.EndDate;
                if (model.ProjectId != null)
                {
                    task.ProjectId = model.ProjectId.Value;
                }
                else
                {
                    task.ProjectId = null;
                }
                task.Status = model.Status;
                if (model.Status == TaskStatus.Done)
                {
                    task.ClosedDate = DateTime.Today.ToUniversalTime();
                }
                task.AsignedUserId = model.AsignedUserId;
                task.CreatedUserId = model.CreatedUserId;
                
                await taskManager.AddAsync(task);
                await taskManager.Context.SaveChangesAsync();
                _logger.LogInformation("Task added successfully. User: {0}",
                    aspNetUserId);
                
                if (emailService.IsEnabled())
                {
                    if (task.ProjectId != null)
                    {
                        BackgroundJob.Enqueue(
                            () => emailService.SendAsignedTask(task.AsignedUserId,task.ProjectId.Value,task.Id));
                    }
                    else
                    {
                        BackgroundJob.Enqueue(
                            () => emailService.SendAsignedTask(task.AsignedUserId,Guid.Empty, task.Id));
                    }
                    
                }
                
                return CreatedAtAction(nameof(GetTaskById), new { taskId = task.Id }, task);
            }
            else
            {
                _logger.LogError("An error ocurred adding a Task. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Task. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpPut]
    [HasPermission(Permissions.TasksEdit)]
    public async Task<IActionResult> Edit([FromBody] TaskEditViewModel task)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = taskManager.GetById(task.Id);
                if (result != null)
                {
                    if (result.ProjectId != null && result.ProjectId != Guid.Empty)
                    {
                        var user = projectUserManager.VerifyUser(result.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return BadRequest("NotAllowed");
                        }
                    }

                    result.Name = sanitizer.Sanitize(task.Name);
                    result.Description = sanitizer.Sanitize(task.Description);
                    result.AsignedUserId = task.AsignedUserId;
                    result.CreatedUserId = task.CreatedUserId;
                    result.Status = task.Status;
                    result.StartDate = task.StartDate.ToUniversalTime();
                    result.EndDate = task.EndDate.ToUniversalTime();
                    if (task.Status == TaskStatus.Done)
                    {
                        result.ClosedDate = DateTime.Today.ToUniversalTime();
                    }
                    result.Template = false;
                    if (task.ProjectId != null)
                    {
                        result.ProjectId = task.ProjectId.Value;
                    }
                    else
                    {
                        result.ProjectId = null;
                    }
                    await taskManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Task edited successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }
                else
                {
                    _logger.LogError("An error occurred editing a Task. User: {0}",
                        aspNetUserId);
                    return NotFound();
                }

            }
            else
            {
                _logger.LogError("An error occurred editing a Task. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred editing a Task. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
                    
    }
    
    [HttpDelete]
    [Route("{taskId}")]
    [HasPermission(Permissions.TasksDelete)]
    public async Task<IActionResult> Delete(Guid taskId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = taskManager.GetById(taskId);
                if (result != null)
                {
                    if (result.ProjectId != null && result.ProjectId != Guid.Empty)
                    {
                        var user = projectUserManager.VerifyUser(result.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return BadRequest("NotAllowed");
                        }
                    }
                    
                    taskManager.Remove(result);
                    await taskManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Task deleted successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }

            }
            else
            {
                _logger.LogError("An error occurred deleting a Task. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred deleting a Task. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }

    [HttpPost]
    [Route("Update")]
    [HasPermission(Permissions.TasksEdit)]
    public async Task<IActionResult> UpdateStatus([FromBody] TaskUpdateViewModel task)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = taskManager.GetById(task.Id);
                if (result != null)
                {
                    if (result.ProjectId != null && result.ProjectId != Guid.Empty)
                    {
                        var user = projectUserManager.VerifyUser(result.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return BadRequest("NotAllowed");
                        }
                    }
                    
                    result.Status = task.Status;
                    await taskManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Task updated successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                _logger.LogError("An error occurred updating a Task. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred updating a Task. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [NonAction]
    public async Task<IActionResult> AssignToMe([FromBody] TaskAssignToMeViewModel task)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = taskManager.GetById(task.TaskId);
                if (result != null)
                {
                    if (result.ProjectId != null && result.ProjectId != Guid.Empty)
                    {
                        var user = projectUserManager.VerifyUser(result.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return BadRequest("NotAllowed");
                        }
                    }
                    
                    result.AsignedUserId = task.UserId;
                    await taskManager.Context.SaveChangesAsync();
                    _logger.LogInformation("Task updated successfully. User: {0}",
                        aspNetUserId);
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                _logger.LogError("An error occurred updating a Task. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred updating a Task. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }

    [HttpGet]
    [Route("Notes/{id}")]
    [HasPermission(Permissions.TaskNotesRead)]
    public IEnumerable<CORE.Entities.TaskNote> GetNotes(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.TaskNote> model = taskNoteManager.GetAll().Where(x => x.TaskId == id).Include(x=> x.User).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting Task notes. User: {0}",
                aspNetUserId);
            throw;
        }
    }
    
    [HttpGet]
    [Route("Targets/{id}")]
    [HasPermission(Permissions.TaskTargetsRead)]
    public IEnumerable<CORE.Entities.TaskTargets> GetTargets(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.TaskTargets> model = taskTargetManager.GetAll().Where(x => x.TaskId == id).Include(x=> x.Target).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting Task targets. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpGet]
    [Route("Attachments/{id}")]
    [HasPermission(Permissions.TaskAttachmentsRead)]
    public IEnumerable<CORE.Entities.TaskAttachment> GetAttachments(Guid id)
    {
        try
        {
            IEnumerable<CORE.Entities.TaskAttachment> model = taskAttachmentManager.GetAll().Where(x => x.TaskId == id).Include(x=> x.User).ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting Task attachments. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }

    [NonAction]
    public TaskTargets GetTaskTargetById(Guid targetId)
    {
        try
        {
            return taskTargetManager.GetById(targetId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting Task attachments. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    [HttpPost]
    [Route("Target")]
    [HasPermission(Permissions.TaskTargetsAdd)]
    public async Task<IActionResult> AddTarget([FromBody] TaskTargetViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var task = taskManager.GetById(model.TaskId);
                if (task.ProjectId != null && task.ProjectId != Guid.Empty)
                {
                    var user = projectUserManager.VerifyUser(task.ProjectId.Value, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest("NotAllowed");
                    }
                }
                
                var result = taskTargetManager.GetAll()
                    .Where(x => x.TaskId == model.TaskId && x.TargetId == model.TargetId);

                if (result.FirstOrDefault() == null)
                {
                    
                    
                    var target = new TaskTargets()
                    {
                        Id = Guid.NewGuid(),
                        TaskId = model.TaskId,
                        TargetId = model.TargetId,
                    };
                    await taskTargetManager.AddAsync(target);
                    await taskTargetManager.Context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetTaskTargetById), new { targetId = model.TaskId }, model);
                }
                else
                {
                    return Ok("TargetExists");
                }
            }

            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred adding target to a Task. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
   
    [HttpDelete]
    [Route("Target/{id}")]
    [HasPermission(Permissions.TaskTargetsDelete)]
    public async Task<IActionResult> DeleteTarget(Guid id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = taskTargetManager.GetById(id);

                if (result != null)
                {
                    var task = taskManager.GetById(result.TaskId);
                    if (task.ProjectId != null && task.ProjectId != Guid.Empty)
                    {
                        var user = projectUserManager.VerifyUser(task.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return BadRequest("NotAllowed");
                        }
                    }
                    
                    taskTargetManager.Remove(result);
                    await taskTargetManager.Context.SaveChangesAsync();

                    return NoContent();
                }

                return BadRequest();
            }

            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred deleting target from a Task. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [NonAction]
    public TaskNote GetTaskNoteById(Guid noteId)
    {
        try
        {
            return taskNoteManager.GetById(noteId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting Task attachments. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpPost]
    [Route("Notes")]
    [HasPermission(Permissions.TaskNotesAdd)]
    public async Task<IActionResult> AddNote([FromBody] TaskNoteViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var task = taskManager.GetById(model.TaskId);
                if (task.ProjectId != null && task.ProjectId != Guid.Empty)
                {
                    var user = projectUserManager.VerifyUser(task.ProjectId.Value, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest("NotAllowed");
                    }
                }
                
                var note = new TaskNote()
                    {
                        Id = Guid.NewGuid(),
                        TaskId = model.TaskId,
                        Name = sanitizer.Sanitize(model.Name),
                        Description = sanitizer.Sanitize(model.Description),
                        UserId = aspNetUserId
                    };
                await taskNoteManager.AddAsync(note);
                await taskNoteManager.Context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetTaskNoteById), new { noteId = note.TaskId }, note);
            }
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred adding note to a Task. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpPut]
    [Route("Notes")]
    [HasPermission(Permissions.TaskNotesEdit)]
    public async Task<IActionResult> EditNote([FromBody] TaskNoteEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var task = taskManager.GetById(model.TaskId);
                if (task.ProjectId != null && task.ProjectId != Guid.Empty)
                {
                    var user = projectUserManager.VerifyUser(task.ProjectId.Value, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest("NotAllowed");
                    }
                }

                var note = taskNoteManager.GetById(model.Id);
                note.Id = model.Id;
                note.TaskId = model.TaskId;
                note.Name = sanitizer.Sanitize(model.Name);
                note.Description = sanitizer.Sanitize(model.Description);
                await taskNoteManager.Context.SaveChangesAsync();
                _logger.LogInformation("Task Note edited successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }
            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred editing a Note. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
  
    
    [HttpDelete]
    [Route("Notes/{id}")]
    [HasPermission(Permissions.TaskNotesDelete)]
    public async Task<IActionResult> DeleteNote(Guid id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                
                var result = taskNoteManager.GetById(id);

                if (result != null)
                {
                    var task = taskManager.GetById(result.TaskId);
                    if (task.ProjectId != null && task.ProjectId != Guid.Empty)
                    {
                        var user = projectUserManager.VerifyUser(task.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return BadRequest("NotAllowed");
                        }
                    }
                    
                    taskNoteManager.Remove(result);
                    await taskNoteManager.Context.SaveChangesAsync();

                    return NoContent();
                }

                return BadRequest();
            }

            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred adding target to a Task. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }

    [NonAction]
    public TaskAttachment GetTaskAttachmentById(Guid attachmentId)
    {
        try
        {
            return taskAttachmentManager.GetById(attachmentId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting Task attachments. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
    
    [HttpPost]
    [Route("Attachments")]
    [HasPermission(Permissions.TaskAttachmentsAdd)]
    public async Task<IActionResult> AddAttachment([FromBody] TaskAttachmentViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var task = taskManager.GetById(model.TaskId);
                if (task.ProjectId != null && task.ProjectId != Guid.Empty)
                {
                    var user = projectUserManager.VerifyUser(task.ProjectId.Value, aspNetUserId);
                    if (user == null)
                    {
                        return BadRequest("NotAllowed");
                    }
                }

                if (model.FileContent != null)
                {
                    
                    
                    var path = "";
                    var unique = "";
                    var file = "";
                    if (model.FileContent != null)
                    {
                        
                        if (fileCheck.CheckFile(model.FileContent))
                        {
                            unique = Guid.NewGuid().ToString()+"."+fileCheck.GetExtension(model.FileContent);
                            path = $"{env.WebRootPath}/Attachments/Task/{model.TaskId}";
                            file = $"{env.WebRootPath}/Attachments/Task/{model.TaskId}/{unique}";
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


                    var attachment = new TaskAttachment
                    {
                        Name = sanitizer.Sanitize(model.Name),
                        TaskId = model.TaskId,
                        UserId = aspNetUserId,
                        FilePath = "Attachments/Task/"+model.TaskId+"/"+unique
                    };

                    await taskAttachmentManager.AddAsync(attachment);
                    await taskAttachmentManager.Context.SaveChangesAsync();
                    _logger.LogInformation("User: {0} added Task Attachment on Task: {1}",
                        aspNetUserId, model.TaskId);
                    return CreatedAtAction(nameof(GetTaskAttachmentById), new { attachmentId = attachment.TaskId }, attachment);
                }
            }

            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred adding target to a Task. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpDelete]
    [Route("Attachments/{id}")]
    [HasPermission(Permissions.TaskAttachmentsDelete)]
    public async Task<IActionResult> DeleteAttachment(Guid id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = taskAttachmentManager.GetById(id);

                if (result != null)
                {
                    var task = taskManager.GetById(result.TaskId);
                    if (task.ProjectId != null && task.ProjectId != Guid.Empty)
                    {
                        var user = projectUserManager.VerifyUser(task.ProjectId.Value, aspNetUserId);
                        if (user == null)
                        {
                            return BadRequest("NotAllowed");
                        }
                    }
                    
                    var path = $"{env.WebRootPath}/{result.FilePath}";
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    taskAttachmentManager.Remove(result);
                    await taskTargetManager.Context.SaveChangesAsync();

                    return NoContent();
                }

                return BadRequest();
            }

            return BadRequest();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred deleting target from a Task. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
}