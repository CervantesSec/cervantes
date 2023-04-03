using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Areas.Workspace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Cervantes.IFR.Email;
using Ganss.XSS;

namespace Cervantes.Web.Areas.Workspace.Controllers;
[Authorize(Roles = "Admin,SuperUser,User")]
[Area("Workspace")]
public class TaskController : Controller
{
    private readonly IHostingEnvironment _appEnvironment;
    private readonly ILogger<TaskController> _logger = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private ITaskManager taskManager = null;
    private ITaskNoteManager taskNoteManager = null;
    private ITaskAttachmentManager taskAttachmentManager = null;
    private ITaskTargetManager taskTargetManager = null;
    private ITargetManager targetManager = null;
    private IEmailService email = null;
    private IUserManager userManager = null;

    public TaskController(IHostingEnvironment _appEnvironment, ITaskManager taskManager, IProjectManager projectManager,
        ITargetManager targetManager, ITaskNoteManager taskNoteManager, ITaskAttachmentManager taskAttachmentManager,
        IProjectUserManager projectUserManager, ITaskTargetManager taskTargetManager, ILogger<TaskController> logger, 
        IUserManager userManager,IEmailService email)
    {
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        this.taskManager = taskManager;
        this.targetManager = targetManager;
        this.taskNoteManager = taskNoteManager;
        this.taskAttachmentManager = taskAttachmentManager;
        this.taskTargetManager = taskTargetManager;
        this._appEnvironment = _appEnvironment;
        _logger = logger;
        this.email = email;
        this.userManager = userManager;
    }

    public ActionResult Index(Guid project)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var model = new TaskViewModel
            {
                Project = projectManager.GetById(project),
                Tasks = taskManager.GetAll().Where(x =>
                        x.AsignedUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.ProjectId == project)
                    .ToList()
            };
            return View(model);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred loading Task Workspace Index. Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return Redirect("/Home/Error");
        }
    }

    public ActionResult Project(Guid project)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var model = new TaskViewModel
            {
                Project = projectManager.GetById(project),
                Tasks = taskManager.GetAll().Where(x => x.ProjectId == project).ToList()
            };
            return View(model);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred loading Task Workspace Project Index. Project: {0} User: {1}",
                project, User.FindFirstValue(ClaimTypes.Name));
            return Redirect("/Home/Error");
        }
    }

    // GET: TaskController/Details/5
    public ActionResult Details(Guid project, Guid id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            
            var result = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new VulnCreateViewModel
            {
                TargetId = e.Id,
                TargetName = e.Name
            }).ToList();

            var targets = new List<SelectListItem>();

            foreach (var item in result)
                targets.Add(new SelectListItem {Text = item.TargetName, Value = item.TargetId.ToString()});
            
            var model = new TaskDetailsViewModel
            {
                Project = projectManager.GetById(project),
                Task = taskManager.GetById(id),
                Notes = taskNoteManager.GetAll().Where(x => x.TaskId == id),
                Attachments = taskAttachmentManager.GetAll().Where(x => x.TaskId == id),
                Targets = taskTargetManager.GetAll().Where(x => x.TaskId == id).ToList(),
                TargetList = targets

            };
            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorTask"] = "Error loading task details!";

            _logger.LogError(e, "An error ocurred loading Task Workspace Details.Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    // GET: TaskController/Create
    public ActionResult Create(Guid project)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var result = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new TaskCreateViewModel
            {
                TargetId = e.Id,
                TargetName = e.Name
            }).ToList();

            var li = new List<SelectListItem>();

            foreach (var item in result)
                li.Add(new SelectListItem {Text = item.TargetName, Value = item.TargetId.ToString()});

            var model = new TaskCreateViewModel
            {
                Project = projectManager.GetById(project),
                TargetList = li
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorTask"] = "Error loading task form!";

            _logger.LogError(e, "An error ocurred loading Task Workspace create form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // POST: TaskController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Guid project, TaskCreateViewModel model)
    {
        try
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");

            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var task = new Task
            {
                Name = model.Name,
                Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description)),
                ProjectId = project,
                StartDate = model.StartDate.ToUniversalTime(),
                EndDate = model.EndDate.ToUniversalTime(),
                Status = model.Status,
                AsignedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            taskManager.Add(task);
            taskManager.Context.SaveChanges();
            
            if (model.SelectedTargets != null)
            {
                foreach (var tar in model.SelectedTargets)
                {
                    TaskTargets taskTargets = new TaskTargets
                    {
                        TaskId = task.Id,
                        TargetId = tar
                    };
                    taskTargetManager.Add(taskTargets);
                }

                taskTargetManager.Context.SaveChanges();
            }
            
            TempData["addedTask"] = "added";
            _logger.LogInformation("User: {0} Created a new Task on Project {1}", User.FindFirstValue(ClaimTypes.Name),
                project);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["errorAddTask"] = "Error creating task!";

            _logger.LogError(e, "An error ocurred adding a new Task Workspace.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Create");
        }
    }

    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult CreateProject(Guid project)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var targets = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new TaskCreateViewModel
            {
                TargetId = e.Id,
                TargetName = e.Name
            }).ToList();

            var li = new List<SelectListItem>();

            foreach (var item in targets)
                li.Add(new SelectListItem {Text = item.TargetName, Value = item.TargetId.ToString()});

            var users = projectUserManager.GetAll().Where(x => x.ProjectId == project).ToList();
            var li2 = new List<SelectListItem>();
            foreach (var item in users)
                li2.Add(new SelectListItem {Text = item.User.FullName, Value = item.User.Id.ToString()});


            var model = new TaskCreateViewModel
            {
                Project = projectManager.GetById(project),
                TargetList = li,
                UsersList = li2
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorTask"] = "Error loading task form!";

            _logger.LogError(e, "An error ocurred loading Task Workspace create project form.Project: {0} User: {1}",
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    // POST: TaskController/Create
    [Authorize(Roles = "Admin,SuperUser")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateProject(Guid project, TaskCreateViewModel model)
    {
        try
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");

            var task = new Task
            {
                Name = model.Name,
                Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description)),
                ProjectId = project,
                StartDate = model.StartDate.ToUniversalTime(),
                EndDate = model.EndDate.ToUniversalTime(),
                Status = model.Status,
                AsignedUserId = model.AsignedUserId,
                CreatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            taskManager.Add(task);
            taskManager.Context.SaveChanges();
            if (model.SelectedTargets != null)
            {
                foreach (var tar in model.SelectedTargets)
                {
                    TaskTargets taskTargets = new TaskTargets
                    {
                        TaskId = task.Id,
                        TargetId = tar
                    };
                    taskTargetManager.Add(taskTargets);
                }

                taskTargetManager.Context.SaveChanges();
            }
            
            TempData["addedTask"] = "added";
            _logger.LogInformation("User: {0} Created a new Task Project on Project {1}",
                User.FindFirstValue(ClaimTypes.Name), project);

            var projectRes = projectManager.GetById(project);
            var userRes = userManager.GetByUserId(task.AsignedUserId);
            var to = new List<EmailAddress>();
            to.Add(new EmailAddress
            {
                Address = userRes.Email,
                Name = userRes.FullName,
            });

            StreamReader sr =new StreamReader(_appEnvironment.WebRootPath + "/Resources/Email/AsignedTask.html");
            string s = sr.ReadToEnd();
            s = s.Replace("{UserName}", userRes.FullName).
                Replace("{Project}",projectRes.Name).
                Replace("{Task}",task.Name).
                Replace("{StartDate}", task.StartDate.ToShortDateString()).
                Replace("{EndDate}", task.EndDate.ToShortDateString()).
                Replace("{Description}",task.Description).
                Replace("{CervantesLink}",HttpContext.Request.Scheme.ToString() +"://" + HttpContext.Request.Host.ToString() + "/en/Workspace/"+project+"/Task/Details/"+task.Id);
            sr.Close();
                
            EmailMessage message = new EmailMessage
            {
                ToAddresses = to,
                Subject = "Cervantes - A new Task have been assigned to you",
                Content = s
            };
             
            email.Send(message);
            
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["errorAddTask"] = "Error creating task!";

            _logger.LogError(e, "An error ocurred adding a new Task Project Workspace on.Project: {0} User: {1}",
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("CreateProject");
        }
    }


    // GET: TaskController/Edit/5
    public ActionResult Edit(Guid project, Guid id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var result = taskManager.GetById(id);


            var target = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new TaskCreateViewModel
            {
                TargetId = e.Id,
                TargetName = e.Name
            }).ToList();

            var li = new List<SelectListItem>();

            foreach (var item in target)
                li.Add(new SelectListItem {Text = item.TargetName, Value = item.TargetId.ToString()});

            var model = new TaskCreateViewModel
            {
                Project = projectManager.GetById(project),
                Id = id,
                Name = result.Name,
                Description = result.Description,
                AsignedUserId = result.AsignedUserId,
                CreatedUserId = result.CreatedUserId,
                StartDate = result.StartDate.ToUniversalTime(),
                EndDate = result.EndDate.ToUniversalTime(),
                Status = result.Status,
                TargetList = li,
                ProjectId = project
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorTask"] = "Error loading task!";

            _logger.LogError(e, "An error ocurred loading Task Workspace edit form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    // POST: TaskController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Guid project, TaskCreateViewModel model, Guid id)
    {
        try
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");

            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var result = taskManager.GetById(id);
            result.Name = model.Name;
            result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
            result.EndDate = model.EndDate.ToUniversalTime();
            result.Status = model.Status;
            result.StartDate = model.StartDate.ToUniversalTime();

            taskManager.Context.SaveChanges();
            TempData["editedTask"] = "edited";
            _logger.LogInformation("User: {0} edited Task: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name),
                id, project);

            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["errorEditTask"] = "Error editing task!";

            _logger.LogError(e, "An error ocurred editing a Task Workspace on. Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Edit", new {id = id});
        }
    }
    
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult EditProject(Guid project, Guid id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            
            var result = taskManager.GetById(id);


            var targets = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new TaskCreateViewModel
            {
                TargetId = e.Id,
                TargetName = e.Name
            }).ToList();

            var li = new List<SelectListItem>();

            foreach (var item in targets)
                li.Add(new SelectListItem {Text = item.TargetName, Value = item.TargetId.ToString()});

            var users = projectUserManager.GetAll().Where(x => x.ProjectId == project).ToList();
            var li2 = new List<SelectListItem>();
            foreach (var item in users)
                li2.Add(new SelectListItem {Text = item.User.FullName, Value = item.User.Id.ToString()});

            var pro = projectManager.GetById(project);
            var model = new TaskCreateViewModel
            {
                Project = pro,
                Id = id,
                Name = result.Name,
                Description = result.Description,
                AsignedUserId = result.AsignedUserId,
                StartDate = result.StartDate.ToUniversalTime(),
                EndDate = result.EndDate.ToUniversalTime(),
                Status = result.Status,
                TargetList = li.ToList(),
                ProjectId = project,
                UsersList = li2.ToList()
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorEditTaskPro"] = "Error loading task details!";

            _logger.LogError(e, "An error ocurred loading Task Workspace edit PROJECT form.Project: {0} User: {1}",
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Project");
        }
    }

    
    // POST: TaskController/Edit/5
    [Authorize(Roles = "Admin,SuperUser")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditProject(Guid project, TaskCreateViewModel model, Guid id)
    {
        try
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");

            var result = taskManager.GetById(id);
            result.Name = model.Name;
            result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
            result.EndDate = model.EndDate.ToUniversalTime();
            result.Status = model.Status;
            result.StartDate = model.StartDate.ToUniversalTime();
            result.AsignedUserId = model.AsignedUserId;

            taskManager.Context.SaveChanges();
            TempData["edited"] = "edited";
            _logger.LogInformation("User: {0} edited Task Project: Task: {1} on Project {2}",
                User.FindFirstValue(ClaimTypes.Name), id, project);
            return RedirectToAction("Project");
        }
        catch (Exception e)
        {
            TempData["errorEditTask"] = "Error editing task!";

            _logger.LogError(e,
                "An error ocurred editing a Task project Workspace on. Task: {0} Project: {1} User: {2}", id, project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("EditProject", new {id= id});
        }
    }

    // GET: TaskController/Delete/5
    public ActionResult Delete(Guid project, Guid id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var result = taskManager.GetById(id);
            return View(result);
        }
        catch (Exception e)
        {
            TempData["errorTask"] = "Error loading task details!";

            _logger.LogError(e, "An error ocurred loading Task Workspace delete form. Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    // POST: TaskController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(Guid project, Guid id, IFormCollection collection)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var result = taskManager.GetById(id);
            if (result != null)
            {
                taskManager.Remove(result);
                taskManager.Context.SaveChanges();
            }

            TempData["deletedTask"] = "deleted";
            _logger.LogInformation("User: {0} deleted Task: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name),
                id, project);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["errorDeleteTask"] = "Error deleting task details!";

            _logger.LogError(e, "An error ocurred deleting a Task  Workspace on. Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult AddNote(Guid project, IFormCollection form)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            if (form != null)
            {
                var note = new TaskNote
                {
                    Name = form["noteName"],
                    Description = form["noteDescription"],
                    TaskId = Guid.Parse(form["task"]),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                taskNoteManager.Add(note);
                taskNoteManager.Context.SaveChanges();
                TempData["addedNote"] = "added";
                _logger.LogInformation("User: {0} added Task Note on Task: {1} on Project {2}",
                    User.FindFirstValue(ClaimTypes.Name), form["task"], project);
                return RedirectToAction("Details", "Task", new {project = project, id = form["task"]});
            }
            else
            {
                return RedirectToAction("Details", "Project", new {project = project, id = form["task"]});
            }
        }
        catch (Exception e)
        {
            TempData["errorAddNote"] = "Error adding task note!";

            _logger.LogError(e, "An error ocurred adding a Task Note Workspace on. Task: {0} Project: {1} User: {2}",
                form["task"], project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Project", new {project = project, id = form["task"]});
        }
    }

    [HttpPost]
    public IActionResult DeleteNote(Guid task, Guid project, Guid id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            if (id != null)
            {
                var result = taskNoteManager.GetById(id);

                taskNoteManager.Remove(result);
                taskNoteManager.Context.SaveChanges();
                TempData["deletedNote"] = "deleted";
                _logger.LogInformation("User: {0} deleted Task Note on Task: {1} on Project {2}",
                    User.FindFirstValue(ClaimTypes.Name), id, project);
                return RedirectToAction("Details", "Task", new {project = project, id = task});
            }
            else
            {
                return RedirectToAction("Details", "Task", new {project = project, id = task});
            }
        }
        catch (Exception e)
        {
            TempData["errorDeleteNote"] = "Error deleting task note!";

            _logger.LogError(e, "An error ocurred deleting a Task Note Workspace on. Task: {0} Project: {1} User: {2}",
                task, project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Task", new {project = project, id = task});
        }
    }


    [HttpPost]
    public IActionResult AddAttachment(Guid project, Guid task, IFormCollection form, IFormFile upload)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            if (form != null && upload != null)
            {
                var file = Request.Form.Files["upload"];
                var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Task/" + form["task"] + "/");
                var uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;

                if (Directory.Exists(uploads))
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
                else
                {
                    Directory.CreateDirectory(uploads);

                    using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }


                var note = new TaskAttachment
                {
                    Name = form["attachmentName"],
                    TaskId = Guid.Parse(form["task"]),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    FilePath = "/Attachments/Task/" + form["task"] + "/" + uniqueName
                };

                taskAttachmentManager.Add(note);
                taskAttachmentManager.Context.SaveChanges();
                TempData["addedAttachment"] = "added";
                _logger.LogInformation("User: {0} added Task Attachment on Task: {1} on Project {2}",
                    User.FindFirstValue(ClaimTypes.Name), task, project);
                return RedirectToAction("Details", "Task", new {project = project, id = form["task"]});
            }
            else
            {
                TempData["errorAttachment"] = "added";
                return RedirectToAction("Details", "Task", new {project = project, id = form["task"]});
            }
        }
        catch (Exception ex)
        {
            TempData["errorAddAttachemnt"] = "Error adding task attachment!";

            _logger.LogError(ex,
                "An error ocurred adding a Task Attachement Workspace on. Task: {0} Project: {1} User: {2}",
                form["task"], project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Task", new {project = project, id = form["task"]});
        }
    }


    [HttpPost]
    public IActionResult DeleteAttachment(Guid id, Guid project, Guid task)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            if (id != null)
            {
                var result = taskAttachmentManager.GetById(id);

                var pathFile = _appEnvironment.WebRootPath + result.FilePath;
                if (System.IO.File.Exists(pathFile)) System.IO.File.Delete(pathFile);

                taskAttachmentManager.Remove(result);
                taskAttachmentManager.Context.SaveChanges();
                TempData["deletedAttachment"] = "deleted";
                _logger.LogInformation("User: {0} deleted Task Attachment on Task: {1} on Project {2}",
                    User.FindFirstValue(ClaimTypes.Name), task, project);
                return RedirectToAction("Details", "Task", new {project = project, id = task});
            }
            else
            {
                return RedirectToAction("Details", "Task", new {project = project, id = task});
            }
        }
        catch (Exception ex)
        {
            TempData["errorDeleteAttachemnt"] = "Error deleting task attachment!";

            _logger.LogError(ex,
                "An error ocurred deleting a Task Attachement Workspace on. Task: {0} Project: {1} User: {2}", task,
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Task", new {project = project, id = task});
        }
    }
    
     public IActionResult DeleteTarget(Guid id, Guid project, Guid task)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            if (id != null)
            {
                var result = taskTargetManager.GetById(id);

                taskTargetManager.Remove(result);
                taskTargetManager.Context.SaveChanges();
                TempData["deletedTarget"] = "deleted";
                _logger.LogInformation("User: {0} Deleted task target: {1} on Vuln: {2}", User.FindFirstValue(ClaimTypes.Name),
                    result.Id, task);
                return RedirectToAction("Details", "Task", new {id = result.TaskId});
            }
            else
            {
                return RedirectToAction("Details", "Task", new {id = task});
            }
        }
        catch (Exception ex)
        {
            TempData["errorDeleting"] = "Error deleting Task target!";
            _logger.LogError(ex, "An error ocurred deleting a task target on Project: {1}. User: {2}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Task", new {id = task});
        }
    }
    
    [HttpPost]
    public IActionResult AddTarget(Guid project,IFormCollection form)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            if (form != null)
            {

               var result = taskTargetManager.GetAll().Where(x => x.TargetId == Guid.Parse(form["TargetId"]) && x.TaskId == Guid.Parse(form["taskId"])).FirstOrDefault();
                if (result != null)
                {
                    TempData["targetExists"] = "exists";
                    return RedirectToAction("Details", "Vuln", new {id = form["taskId"]});
                }
                
                var tar = new TaskTargets
                {
                    TargetId = Guid.Parse(form["TargetId"]),
                    TaskId = Guid.Parse(form["taskId"]),
                    
                };

                taskTargetManager.Add(tar);
                taskTargetManager.Context.SaveChanges();
                TempData["addedTarget"] = "added";
                _logger.LogInformation("User: {0} Added new target: {1} on Task: {2}",
                    User.FindFirstValue(ClaimTypes.Name), tar.Id, form["taskId"]);
                return RedirectToAction("Details", "Task", new {id = form["taskId"]});
            }
            else
            {
                return RedirectToAction("Details", "Task", new {id = form["taskId"]});
            }
        }
        catch (Exception ex)
        {
            TempData["errorAdding"] = "Error adding note vuln!";
            _logger.LogError(ex, "An error ocurred adding a target on Task: {1}. User: {2}", form["taskId"],
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Task", new {id = form["taskId"]});
        }
    }
}