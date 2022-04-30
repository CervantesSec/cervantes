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

namespace Cervantes.Web.Areas.Workspace.Controllers
{
    [Area("Workspace")]
    public class TaskController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly ILogger<TaskController> _logger = null;
        IProjectManager projectManager = null;
        IProjectUserManager projectUserManager = null;
        ITaskManager taskManager = null;
        ITaskNoteManager taskNoteManager = null;
        ITaskAttachmentManager taskAttachmentManager = null;
        ITargetManager targetManager = null;

        public TaskController(IHostingEnvironment _appEnvironment, ITaskManager taskManager, IProjectManager projectManager, ITargetManager targetManager, ITaskNoteManager taskNoteManager, ITaskAttachmentManager taskAttachmentManager,
            IProjectUserManager projectUserManager, ILogger<TaskController> logger)
        {
            this.projectManager = projectManager;
            this.projectUserManager = projectUserManager;
            this.taskManager = taskManager;
            this.targetManager = targetManager;
            this.taskNoteManager = taskNoteManager;
            this.taskAttachmentManager = taskAttachmentManager;
            this._appEnvironment = _appEnvironment;
            _logger = logger;
        }

        public ActionResult Index(int project)
        {
            try
            {
                TaskViewModel model = new TaskViewModel
                {
                    Project = projectManager.GetById(project),
                    Tasks = taskManager.GetAll().Where(x => x.AsignedUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.ProjectId == project).ToList(),
                };
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading tasks!";

                _logger.LogError(e, "An error ocurred loading Task Workspace Index. Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }

        }

        public ActionResult Project(int project)
        {
            try
            {
                TaskViewModel model = new TaskViewModel
                {
                    Project = projectManager.GetById(project),
                    Tasks = taskManager.GetAll().Where(x => x.ProjectId == project),
                };
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading tasks!";

                _logger.LogError(e, "An error ocurred loading Task Workspace Project Index. Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }

        }

        // GET: TaskController/Details/5
        public ActionResult Details(int project, int id)
        {
            try
            {
                TaskDetailsViewModel model = new TaskDetailsViewModel
                {
                    Project = projectManager.GetById(project),
                    Task = taskManager.GetById(id),
                    Notes = taskNoteManager.GetAll().Where(x => x.TaskId == id),
                    Attachments = taskAttachmentManager.GetAll().Where(x => x.TaskId == id)

                };
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading task details!";

                _logger.LogError(e, "An error ocurred loading Task Workspace Details.Task: {0} Project: {1} User: {2}", id, project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }

        }

        // GET: TaskController/Create
        public ActionResult Create(int project)
        {
            try
            {

                var result = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new TaskCreateViewModel
                {
                    TargetId = e.Id,
                    TargetName = e.Name,
                }).ToList();

                var li = new List<SelectListItem>();

                foreach (var item in result)
                {
                    li.Add(new SelectListItem { Text = item.TargetName, Value = item.TargetId.ToString() });
                }

                var model = new TaskCreateViewModel
                {
                    Project = projectManager.GetById(project),
                    TargetList = li
                };

                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading task form!";

                _logger.LogError(e, "An error ocurred loading Task Workspace create form.Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }

        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int project, TaskCreateViewModel model)
        {
            try
            {
                Task task = new Task
                {
                    Name = model.Name,
                    Description = model.Description,
                    ProjectId = project,
                    TargetId = model.TargetId,
                    StartDate = model.StartDate.ToUniversalTime(),
                    EndDate = model.EndDate.ToUniversalTime(),
                    Status = model.Status,
                    AsignedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CreatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                };

                taskManager.Add(task);
                taskManager.Context.SaveChanges();
                TempData["added"] = "added";
                _logger.LogInformation("User: {0} Created a new Task on Project {1}", User.FindFirstValue(ClaimTypes.Name), project);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["error"] = "Error creating task!";

                _logger.LogError(e, "An error ocurred adding a new Task Workspace.Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        [Authorize(Roles = "Admin,SuperUser")]
        public ActionResult CreateProject(int project)
        {
            try
            {

                var targets = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new TaskCreateViewModel
                {
                    TargetId = e.Id,
                    TargetName = e.Name,
                }).ToList();

                var li = new List<SelectListItem>();

                foreach (var item in targets)
                {
                    li.Add(new SelectListItem { Text = item.TargetName, Value = item.TargetId.ToString() });
                }

                var users = projectUserManager.GetAll().Where(x => x.ProjectId == project).ToList();
                var li2 = new List<SelectListItem>();
                foreach (var item in users)
                {
                    li2.Add(new SelectListItem { Text = item.User.FullName, Value = item.User.Id.ToString() });
                }


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
                TempData["error"] = "Error loading task form!";

                _logger.LogError(e, "An error ocurred loading Task Workspace create project form.Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }

        }

        // POST: TaskController/Create
        [Authorize(Roles = "Admin,SuperUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProject(int project, TaskCreateViewModel model)
        {
            try
            {
                Task task = new Task
                {
                    Name = model.Name,
                    Description = model.Description,
                    ProjectId = project,
                    TargetId = model.TargetId,
                    StartDate = model.StartDate.ToUniversalTime(),
                    EndDate = model.EndDate.ToUniversalTime(),
                    Status = model.Status,
                    AsignedUserId = model.AsignedUserId,
                    CreatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                };

                taskManager.Add(task);
                taskManager.Context.SaveChanges();
                TempData["added"] = "added";
                _logger.LogInformation("User: {0} Created a new Task Project on Project {1}", User.FindFirstValue(ClaimTypes.Name), project);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["error"] = "Error creating task!";

                _logger.LogError(e, "An error ocurred adding a new Task Project Workspace on.Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }


        // GET: TaskController/Edit/5
        public ActionResult Edit(int project, int id)
        {
            try
            {
                var result = taskManager.GetById(id);


                var target = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new TaskCreateViewModel
                {
                    TargetId = e.Id,
                    TargetName = e.Name,
                }).ToList();

                var li = new List<SelectListItem>();

                foreach (var item in target)
                {
                    li.Add(new SelectListItem { Text = item.TargetName, Value = item.TargetId.ToString() });
                }

                TaskCreateViewModel model = new TaskCreateViewModel
                {
                    Project = projectManager.GetById(project),
                    Id = id,
                    Name = result.Name,
                    Description = result.Description,
                    TargetId = result.TargetId,
                    AsignedUserId = result.AsignedUserId,
                    CreatedUserId = result.CreatedUserId,
                    StartDate = result.StartDate.ToUniversalTime(),
                    EndDate = result.EndDate.ToUniversalTime(),
                    Status = result.Status,
                    TargetList = li,
                    ProjectId = project,

                };

                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading task!";

                _logger.LogError(e, "An error ocurred loading Task Workspace edit form.Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int project, TaskCreateViewModel model, int id)
        {
            try
            {
                var result = taskManager.GetById(id);
                result.Name = model.Name;
                result.Description = model.Description;
                result.TargetId = model.TargetId;
                result.EndDate = model.EndDate.ToUniversalTime();
                result.Status = model.Status;
                result.StartDate = model.StartDate.ToUniversalTime();

                taskManager.Context.SaveChanges();
                TempData["edited"] = "edited";
                _logger.LogInformation("User: {0} edited Task: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), id, project);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["error"] = "Error editing task!";

                _logger.LogError(e, "An error ocurred editing a Task Workspace on. Task: {0} Project: {1} User: {2}", id, project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        public ActionResult EditProject(int project, int id)
        {
            try
            {
                var result = taskManager.GetById(id);


                var targets = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new TaskCreateViewModel
                {
                    TargetId = e.Id,
                    TargetName = e.Name,
                }).ToList();

                var li = new List<SelectListItem>();

                foreach (var item in targets)
                {
                    li.Add(new SelectListItem { Text = item.TargetName, Value = item.TargetId.ToString() });
                }

                var users = projectUserManager.GetAll().Where(x => x.ProjectId == project);
                var li2 = new List<SelectListItem>();
                foreach (var item in users)
                {
                    li2.Add(new SelectListItem { Text = item.User.FullName, Value = item.User.Id.ToString() });
                }


                TaskCreateViewModel model = new TaskCreateViewModel
                {
                    Project = projectManager.GetById(project),
                    Id = id,
                    Name = result.Name,
                    Description = result.Description,
                    TargetId = result.TargetId,
                    AsignedUserId = result.AsignedUserId,
                    StartDate = result.StartDate.ToUniversalTime(),
                    EndDate = result.EndDate.ToUniversalTime(),
                    Status = result.Status,
                    TargetList = li,
                    ProjectId = project,
                    UsersList = li2,

                };

                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading task details!";

                _logger.LogError(e, "An error ocurred loading Task Workspace edit PROJECT form.Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProject(int project, TaskCreateViewModel model, int id)
        {
            try
            {
                var result = taskManager.GetById(id);
                result.Name = model.Name;
                result.Description = model.Description;
                result.TargetId = model.TargetId;
                result.EndDate = model.EndDate.ToUniversalTime();
                result.Status = model.Status;
                result.StartDate = model.StartDate.ToUniversalTime();
                result.AsignedUserId = model.AsignedUserId;

                taskManager.Context.SaveChanges();
                TempData["edited"] = "edited";
                _logger.LogInformation("User: {0} edited Task Project: Task: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), id, project);
                return RedirectToAction("Project");
            }
            catch (Exception e)
            {
                TempData["error"] = "Error editing task!";

                _logger.LogError(e, "An error ocurred editing a Task project Workspace on. Task: {0} Project: {1} User: {2}", id, project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        // GET: TaskController/Delete/5
        public ActionResult Delete(int project, int id)
        {
            try
            {
                var result = taskManager.GetById(id);
                return View(result);


            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading task details!";

                _logger.LogError(e, "An error ocurred loading Task Workspace delete form. Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View("Index");
            }
        }

        // POST: TaskController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int project, int id, IFormCollection collection)
        {
            try
            {
                var result = taskManager.GetById(id);
                if (result != null)
                {
                    taskManager.Remove(result);
                    taskManager.Context.SaveChanges();
                }

                TempData["deleted"] = "deleted";
                _logger.LogInformation("User: {0} deleted Task: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), id, project);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["error"] = "Error deleting task details!";

                _logger.LogError(e, "An error ocurred deleting a Task  Workspace on. Task: {0} Project: {1} User: {2}", id, project, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        [HttpPost]
        public IActionResult AddNote(int project, IFormCollection form)
        {
            try
            {
                if (form != null)
                {

                    TaskNote note = new TaskNote
                    {
                        Name = form["noteName"],
                        Description = form["noteDescription"],
                        TaskId = Int32.Parse(form["task"]),
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),

                    };

                    taskNoteManager.Add(note);
                    taskNoteManager.Context.SaveChanges();
                    TempData["addedNote"] = "added";
                    _logger.LogInformation("User: {0} added Task Note on Task: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), Int32.Parse(form["task"]), project);
                    return RedirectToAction("Details", "Task", new { project = project, id = Int32.Parse(form["task"]) });
                }
                else
                {
                    return RedirectToAction("Details", "Project", new { project = project, id = Int32.Parse(form["task"]) });
                }
            }
            catch (Exception e)
            {
                TempData["error"] = "Error adding task note!";

                _logger.LogError(e, "An error ocurred adding a Task Note Workspace on. Task: {0} Project: {1} User: {2}", Int32.Parse(form["task"]), project, User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Project", new { project = project, id = Int32.Parse(form["task"]) });
            }

        }

        [HttpPost]
        public IActionResult DeleteNote(int task, int project, int id)
        {
            try
            {
                if (id != 0)
                {
                    var result = taskNoteManager.GetById(id);

                    taskNoteManager.Remove(result);
                    taskNoteManager.Context.SaveChanges();
                    TempData["deletedNote"] = "deleted";
                    _logger.LogInformation("User: {0} deleted Task Note on Task: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), id, project);
                    return RedirectToAction("Details", "Task", new { project = project, id = task });
                }
                else
                {
                    return RedirectToAction("Details", "Task", new { project = project, id = task });
                }
            }
            catch (Exception e)
            {
                TempData["error"] = "Error deleting task note!";

                _logger.LogError(e, "An error ocurred deleting a Task Note Workspace on. Task: {0} Project: {1} User: {2}", task, project, User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Task", new { project = project, id = task });
            }

        }



        [HttpPost]
        public IActionResult AddAttachment(int project, int task, IFormCollection form, IFormFile upload)
        {
            try
            {
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



                    TaskAttachment note = new TaskAttachment
                    {
                        Name = form["attachmentName"],
                        TaskId = Int32.Parse(form["task"]),
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        FilePath = "/Attachments/Task/" + form["task"] + "/" + uniqueName,

                    };

                    taskAttachmentManager.Add(note);
                    taskAttachmentManager.Context.SaveChanges();
                    TempData["addedAttachment"] = "added";
                    _logger.LogInformation("User: {0} added Task Attachment on Task: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), task, project);
                    return RedirectToAction("Details", "Task", new { project = project, id = Int32.Parse(form["task"]) });
                }
                else
                {
                    TempData["errorAttachment"] = "added";
                    return RedirectToAction("Details", "Task", new { project = project, id = Int32.Parse(form["task"]) });
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error adding task attachment!";

                _logger.LogError(ex, "An error ocurred adding a Task Attachement Workspace on. Task: {0} Project: {1} User: {2}", Int32.Parse(form["task"]), project, User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Task", new { project = project, id = Int32.Parse(form["task"]) });
            }

        }


        [HttpPost]
        public IActionResult DeleteAttachment(int id, int project, int task)
        {
            try
            {
                if (id != 0)
                {
                    var result = taskAttachmentManager.GetById(id);

                    var pathFile = _appEnvironment.WebRootPath + result.FilePath;
                    if (System.IO.File.Exists(pathFile))
                    {
                        System.IO.File.Delete(pathFile);
                    }

                    taskAttachmentManager.Remove(result);
                    taskAttachmentManager.Context.SaveChanges();
                    TempData["deletedAttachment"] = "deleted";
                    _logger.LogInformation("User: {0} deleted Task Attachment on Task: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), task, project);
                    return RedirectToAction("Details", "Task", new { project = project, id = task });
                }
                else
                {
                    return RedirectToAction("Details", "Task", new { project = project, id = task });
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error deleting task attachment!";

                _logger.LogError(ex, "An error ocurred deleting a Task Attachement Workspace on. Task: {0} Project: {1} User: {2}", task, project, User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Task", new { project = project, id = task });
            }

        }

    }
}
