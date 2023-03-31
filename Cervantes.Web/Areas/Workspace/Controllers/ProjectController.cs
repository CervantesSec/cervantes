using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.IFR.Email;
using Cervantes.Web.Areas.Workspace.Models;
using Cervantes.Web.Models;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Cervantes.Web.Areas.Workspace.Controllers;
[Authorize(Roles = "Admin,SuperUser,User")]
[Area("Workspace")]
public class ProjectController: Controller
{
    private readonly ILogger<ProjectController> _logger = null;
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
    private IReportManager reportManager = null;
    private IReportTemplateManager reportTemplateManager = null;
    private IEmailService email = null;

    /// <summary>
    /// ProjectController Constructor
    /// </summary>
    /// <param name="projectManager">ProjectManager</param>
    /// <param name="clientManager">ClientManager</param>
    public ProjectController(IProjectManager projectManager, IClientManager clientManager,
        IProjectUserManager projectUserManager, IProjectNoteManager projectNoteManager,
        IProjectAttachmentManager projectAttachmentManager, ITargetManager targetManager, ITaskManager taskManager,
        IUserManager userManager, IVulnManager vulnManager, IHostingEnvironment _appEnvironment,
        ILogger<ProjectController> logger, IReportManager reportManager, IReportTemplateManager reportTemplateManager,IEmailService email)
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
        this._appEnvironment = _appEnvironment;
        _logger = logger;
        this.reportManager = reportManager;
        this.reportTemplateManager = reportTemplateManager;
        this.email = email;
    }

    public IActionResult ExecutiveSummary(Guid project)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces", new {area = ""});
            }

            var pro = projectManager.GetById(project);
            var model = new ExecutiveSummaryViewModel
            {
                Project = pro,
                ExecutiveSummary = pro.ExecutiveSummary
            };
            

            return View(model);
        }
        catch (Exception ex)
        {
            TempData["error"] = "loading project";
            _logger.LogError(ex, "An error ocurred loading Project Executive Summary: {0}. User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
        
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExecutiveSummary(Guid project, ExecutiveSummaryViewModel model)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces", new {area = ""});
            }

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");
            
            var pro = projectManager.GetById(project);
            pro.ExecutiveSummary = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ExecutiveSummary));

            projectManager.Context.SaveChanges();

            return RedirectToAction("ExecutiveSummary", "Project", new {project = project});
        }
        catch (Exception ex)
        {
            TempData["error"] = "loading project";
            _logger.LogError(ex, "An error ocurred loading Project Executive Summary: {0}. User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
        
    }
    
     /// <summary>
    /// Methos show project details
    /// </summary>
    /// <param name="id">Project Id</param>
    /// <returns></returns>
    [Authorize(Roles = "Admin,SuperUser,User,Client")]
    public ActionResult Details(Guid project)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            
            var projectDet = projectManager.GetById(project);
            
           
            
            if (projectDet != null)
            {
                
                
                var result = userManager.GetAll().Select(e => new ApplicationUser
                {
                    Id = e.Id,
                    FullName = e.FullName
                }).ToList();

                var users = new List<SelectListItem>();

                foreach (var item in result)
                    users.Add(new SelectListItem {Text = item.FullName, Value = item.Id.ToString()});
                
                var repTemplates2 = reportTemplateManager.GetAll().Where(x => x.Language == projectDet.Language).Select(e => new ReportTemplate
                {
                    Id = e.Id,
                    Name = e.Name,
                }).ToList();

                var liRep2 = new List<SelectListItem>();

                foreach (var item in repTemplates2) liRep2.Add(new SelectListItem {Text = item.Name, Value = item.Id.ToString()});

                var model = new ProjectDetailsViewModel
                {
                    Project = projectDet,
                    ProjectUsers = projectUserManager.GetAll().Where(x => x.ProjectId == project).ToList(),
                    ProjectNotes = projectNoteManager.GetAll().Where(x => x.ProjectId == project),
                    ProjectAttachments = projectAttachmentManager.GetAll().Where(x => x.ProjectId == project),
                    Targets = targetManager.GetAll().Where(x => x.ProjectId == project),
                    Tasks = taskManager.GetAll().Where(x => x.ProjectId == project),
                    Users = users.ToList(),
                    Vulns = vulnManager.GetAll().Where(x => x.ProjectId == project).ToList(),
                    Reports = reportManager.GetAll().Where(x => x.ProjectId == project).ToList(),
                    ReportTemplates = liRep2.ToList()
                };
                return View(model);
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "loading project";
            _logger.LogError(ex, "An error ocurred loading Project: {0}. User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }
     
     /// <summary>
    /// Method Add user to project
    /// </summary>
    /// <param name="project">Project Id</param>
    /// <param name="user">User Id</param>
    /// <returns></returns>
    [Authorize(Roles = "Admin,SuperUser")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddMember(Guid project, string users)
    {
        try
        {
            if (project != null && users != null)
            {
                var result = projectUserManager.GetAll().Where(x => x.UserId == users && x.ProjectId == project);

                if (result.FirstOrDefault() == null)
                {
                    var user = new ProjectUser
                    {
                        ProjectId = project,
                        UserId = users
                    };
                    projectUserManager.Add(user);
                    projectUserManager.Context.SaveChanges();
                    TempData["addedMember"] = "added";
                    _logger.LogInformation("User: {0} Added new member Member on Project: {2}",
                        User.FindFirstValue(ClaimTypes.Name), project);

                    var userRes = userManager.GetByUserId(user.UserId);
                    var projectRes = projectManager.GetById(user.ProjectId);
                    
                    var to = new List<EmailAddress>();
                    to.Add(new EmailAddress
                    {
                        Address = userRes.Email,
                        Name = userRes.FullName,
                    });

                    StreamReader sr =new StreamReader(_appEnvironment.WebRootPath + "/Resources/Email/AddedToProject.html");
                    string s = sr.ReadToEnd();
                    s = s.Replace("{UserName}", userRes.FullName).
                        Replace("{Project}",projectRes.Name).
                        Replace("{StartDate}", projectRes.StartDate.ToShortDateString()).
                        Replace("{EndDate}", projectRes.EndDate.ToShortDateString()).
                        Replace("{Client}",projectRes.Client.Name).
                        Replace("{CervantesLink}",HttpContext.Request.Scheme.ToString() +"://" + HttpContext.Request.Host.ToString() + "/en/Project/Details/"+project);
                    sr.Close();
                
                    EmailMessage message = new EmailMessage
                    {
                        ToAddresses = to,
                        Subject = "Cervantes - You have been added to a Project",
                        Content = s
                    };
             
                    email.Send(message);
                    return RedirectToAction("Details", "Project", new {id = project});
                }
                else
                {
                    TempData["existsMember"] = "exists";
                    return RedirectToAction("Details", "Project", new {id = project});
                }
            }
            else
            {
                return RedirectToAction("Details", "Project", new {id = project});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "adding member to ";
            _logger.LogError(ex, "An error ocurred adding a new Memeber on Project: {0}. User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Project", new {id = project});
        }
    }

    /// <summary>
    /// Method delete user from project
    /// </summary>
    /// <param name="project">Project Id</param>
    /// <param name="user">User Id</param>
    /// <returns></returns>
    [Authorize(Roles = "Admin,SuperUser")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteMember(Guid project, string user)
    {
        try
        {
            var result = projectUserManager.GetAll().Where(x => x.UserId == user && x.ProjectId == project);
            if (result.FirstOrDefault() != null)
            {
                projectUserManager.Remove(result.FirstOrDefault());
                projectUserManager.Context.SaveChanges();
                TempData["deletedMember"] = "deleted";
                return RedirectToAction("Details", "Project", new {id = project});
            }

            return RedirectToAction("Details", "Project", new {id = project});
        }
        catch (Exception ex)
        {
            TempData["error"] = "deleting memeber from ";
            _logger.LogError(ex, "An error ocurred deleteing a Memeber on Project: {0}. User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Project", new {id = project});
        }
    }

    [Authorize(Roles = "Admin,SuperUser")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTarget(IFormCollection form)
    {
        try
        {
            if (form != null)
            {
                var target = new Target
                {
                    Name = form["name"],
                    Description = form["description"],
                    ProjectId = Guid.Parse(form["project"]),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Type = (TargetType) Enum.ToObject(typeof(TargetType), int.Parse(form["TargetType"]))
                };

                targetManager.Add(target);
                targetManager.Context.SaveChanges();
                TempData["addedTarget"] = "added";
                _logger.LogInformation("User: {0} Added new target: {1} on Project: {2}",
                    User.FindFirstValue(ClaimTypes.Name), target.Name, form["project"]);
                return RedirectToAction("Details", "Project", new {id = form["project"]});
            }
            else
            {
                return RedirectToAction("Details", "Project", new {id = form["project"]});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "adding target to ";
            _logger.LogError(ex, "An error ocurred adding a new Target on Project: {0}. User: {1}",
                form["project"], User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Project", new {id = form["project"]});
        }
    }

    [Authorize(Roles = "Admin,SuperUser")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteTarget(Guid target, Guid project)
    {
        try
        {
            if (target != null)
            {
                var result = targetManager.GetById(target);

                targetManager.Remove(result);
                targetManager.Context.SaveChanges();
                TempData["deletedTarget"] = "deleted";
                return RedirectToAction("Details", "Project", new {id = project});
            }
            else
            {
                return RedirectToAction("Details", "Project", new {id = project});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "deleting target from ";
            _logger.LogError(ex, "An error ocurred deleting a Target: {0} on Project: {1}. User: {2}", project, target,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Project", new {id = project});
        }
    }

    [Authorize(Roles = "Admin,SuperUser")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddNote(IFormCollection form)
    {
        try
        {
            if (form != null)
            {
                var note = new ProjectNote
                {
                    Name = form["noteName"],
                    Description = form["noteDescription"],
                    ProjectId = Guid.Parse(form["project"]),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Visibility = (Visibility) Enum.ToObject(typeof(Visibility), int.Parse(form["Visibility"]))
                };

                projectNoteManager.Add(note);
                projectNoteManager.Context.SaveChanges();
                TempData["addedNote"] = "added";
                _logger.LogInformation("User: {0} Added new note: {1} on Project: {2}",
                    User.FindFirstValue(ClaimTypes.Name), note.Name, form["project"]);
                return RedirectToAction("Details", "Project", new {id = form["project"]});
            }
            else
            {
                return RedirectToAction("Details", "Project", new {id = form["project"]});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "adding note to ";
            _logger.LogError(ex, "An error ocurred adding a Note on Project: {1}. User: {2}",
                form["project"], User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Project", new {id = form["project"]});
        }
    }

    [Authorize(Roles = "Admin,SuperUser")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteNote(Guid id, Guid project)
    {
        try
        {
            if (id != null)
            {
                var result = projectNoteManager.GetById(id);

                projectNoteManager.Remove(result);
                projectNoteManager.Context.SaveChanges();
                TempData["deletedNote"] = "deleted";
                _logger.LogInformation("User: {0} Deleted note: {1} on Project: {2}",
                    User.FindFirstValue(ClaimTypes.Name), result.Name, project);
                return RedirectToAction("Details", "Project", new {id = project});
            }
            else
            {
                return RedirectToAction("Details", "Project", new {id = project});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "deleting note from ";
            _logger.LogError(ex, "An error ocurred deleting a Note on Project: {1}. User: {2}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Project", new {id = project});
        }
    }


    [Authorize(Roles = "Admin,SuperUser")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddAttachment(IFormCollection form, IFormFile upload)
    {
        try
        {
            if (form != null && upload != null)
            {
                var file = Request.Form.Files["upload"];
                var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Project/" + form["project"] + "/");
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


                var note = new ProjectAttachment
                {
                    Name = form["attachmentName"],
                    ProjectId = Guid.Parse(form["project"]),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    FilePath = "/Attachments/Project/" + form["project"] + "/" + uniqueName
                };

                projectAttachmentManager.Add(note);
                projectAttachmentManager.Context.SaveChanges();
                TempData["addedAttachment"] = "added";
                _logger.LogInformation("User: {0} Added an attachment: {1} on Project: {2}",
                    User.FindFirstValue(ClaimTypes.Name), note.Name, form["project"]);
                return RedirectToAction("Details", "Project", new {id = form["project"]});
            }
            else
            {
                TempData["errorAttachment"] = "added";
                _logger.LogError("An error ocurred adding an Attachment on Project: {1}. User: {2}",
                    form["project"], User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Project", new {id = form["project"]});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "adding attachment to ";
            _logger.LogError(ex, "An error ocurred adding an Attachment on Project: {1}. User: {2}",
                form["project"], User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Project", new {id = form["project"]});
        }
    }

    [Authorize(Roles = "Admin,SuperUser")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteAttachment(Guid id, Guid project)
    {
        try
        {
            if (id != null)
            {
                var result = projectAttachmentManager.GetById(id);

                var pathFile = _appEnvironment.WebRootPath + result.FilePath;
                if (System.IO.File.Exists(pathFile)) System.IO.File.Delete(pathFile);

                projectAttachmentManager.Remove(result);
                projectAttachmentManager.Context.SaveChanges();
                TempData["deletedAttachment"] = "deleted";
                _logger.LogInformation("User: {0} Deleted an attachment: {1} on Project: {2}",
                    User.FindFirstValue(ClaimTypes.Name), result.Name, project);
                return RedirectToAction("Details", "Project", new {id = project});
            }
            else
            {
                _logger.LogError("An error ocurred deleting an Attachment on Project: {1}. User: {2}", project,
                    User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Project", new {id = project});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "deleting attachment from ";
            _logger.LogError(ex, "An error ocurred deleting an Attachment on Project: {1}. User: {2}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Project", new {id = project});
        }
    }

    [Authorize(Roles = "Admin,SuperUser,User,Client")]
    public ActionResult Download(Guid id)
    {
        var attachment = projectAttachmentManager.GetById(id);

        var filePath = Path.Combine(_appEnvironment.WebRootPath, attachment.FilePath);

        var fileBytes = System.IO.File.ReadAllBytes(filePath);

        return File(fileBytes, attachment.Name);
    }
}