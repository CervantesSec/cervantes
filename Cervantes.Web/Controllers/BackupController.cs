using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cervantes.Web.Controllers;
[Authorize(Roles = "Admin")]
public class BackupController : Controller
{
    private readonly ILogger<BackupController> _logger = null;
    private IClientManager clientManager = null;
    private IUserManager userManager = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private IProjectNoteManager projectNoteManager = null;
    private IProjectAttachmentManager projectAttachmentManager = null;
    private IDocumentManager documentManager = null;
    private INoteManager noteManager = null;
    private IOrganizationManager organizationManager = null;
    private IReportManager reportManager = null;
    private ITargetManager targetManager = null;
    private ITargetServicesManager targetServicesManager = null;
    private ITaskManager taskManager = null;
    private ITaskNoteManager taskNoteManager = null;
    private ITaskAttachmentManager taskAttachmentManager = null;
    private IVaultManager vaultManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private IVulnManager vulnManager = null;
    private IVulnNoteManager vulnNoteManager = null;
    private IVulnAttachmentManager vulnAttachmentManager = null;
    private readonly IHostingEnvironment _appEnvironment;

    public BackupController(IUserManager userManager,IClientManager clientManager, IProjectManager projectManager,
        IProjectUserManager projectUserManager, IProjectNoteManager projectNoteManager, IDocumentManager documentManager,
        INoteManager noteManager, IOrganizationManager organizationManager, IReportManager reportManager,
        ITargetManager targetManager, ITargetServicesManager targetServicesManager, ITaskManager taskManager,
        ITaskNoteManager taskNoteManager, ITaskAttachmentManager taskAttachmentManager, IVaultManager vaultManager,
        IVulnManager vulnManager, IVulnNoteManager vulnNoteManager, IVulnAttachmentManager vulnAttachmentManager, IVulnCategoryManager vulnCategoryManager,
        IProjectAttachmentManager projectAttachmentManager,ILogger<BackupController> logger,IHostingEnvironment _appEnvironment)
    {
        this.userManager = userManager;
        this.clientManager = clientManager;
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        this.projectNoteManager = projectNoteManager;
        this.projectAttachmentManager = projectAttachmentManager;
        this.documentManager = documentManager;
        this.noteManager = noteManager;
        this.organizationManager = organizationManager;
        this.reportManager = reportManager;
        this.targetManager = targetManager;
        this.targetServicesManager = targetServicesManager;
        this.taskManager = taskManager;
        this.taskNoteManager = taskNoteManager;
        this.taskAttachmentManager = taskAttachmentManager;
        this.vaultManager = vaultManager;
        this.vulnCategoryManager = vulnCategoryManager;
        this.vulnManager = vulnManager;
        this.vulnAttachmentManager = vulnAttachmentManager;
        this.vulnNoteManager = vulnNoteManager;
        _logger = logger;
        this._appEnvironment = _appEnvironment;

    }
    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult BackupData()
    {
        try
        {

            var model = new BackupViewModel
            {
                Users = userManager.GetAll().ToList(),
                Clients = clientManager.GetAll().Select(X => new Client
                {
                    Id = X.Id,
                    Name = X.Name,
                    Description = X.Description,
                    ContactEmail = X.ContactEmail,
                    ContactPhone = X.ContactPhone,
                    ContactName = X.ContactName,
                    CreatedDate = X.CreatedDate,
                    Url = X.Url,
                    UserId = X.UserId,
                    ImagePath = X.ImagePath,
                    
                }).ToList(),
                Projects = projectManager.GetAll().Select(x => new Project
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    Status = x.Status,
                    ClientId = x.ClientId,
                    ProjectType = x.ProjectType,
                    Template = x.Template,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    
                }).ToList(),
                ProjectUsers = projectUserManager.GetAll().Select( x => new ProjectUser
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId
                }).ToList(),
                ProjectAttachments = projectAttachmentManager.GetAll().Select(x => new ProjectAttachment
                {
                    Id = x.Id,
                    Name = x.Name,
                    FilePath = x.FilePath,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    
                    
                }).ToList(),
                ProjectNotes = projectNoteManager.GetAll().Select(x=> new ProjectNote
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    Visibility = x.Visibility,
                }).ToList(),
                Documents = documentManager.GetAll().Select(x => new Document
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    FilePath = x.FilePath,
                    CreatedDate = x.CreatedDate,
                    Visibility = x.Visibility,
                    UserId = x.UserId
                    
                }).ToList(),
                Notes = noteManager.GetAll().Select(x => new Note
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    CreatedDate = x.CreatedDate,
                    
                }).ToList(),
                Organization = organizationManager.GetById(1),
                Reports = reportManager.GetAll().Select(x => new Report
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    CreatedDate = x.CreatedDate,
                    Name = x.Name,
                    Description = x.Description,
                    Version = x.Version,
                    FilePath = x.FilePath,
                    Language = x.Language
                    
                }).ToList(),
                Targets = targetManager.GetAll().Select(x => new Target
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    Name = x.Name,
                    Description = x.Description,
                    Type = x.Type
                }).ToList(),
                TargetServices = targetServicesManager.GetAll().Select(x => new TargetServices
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    TargetId = x.TargetId,
                    Name = x.Name,
                    Description = x.Description,
                    Port = x.Port,
                    Version = x.Version,
                    Note = x.Note
                }).ToList(),
                Tasks = taskManager.GetAll().Select(X => new Task
                {
                    Id = X.Id,
                    Template = X.Template,
                    CreatedUserId = X.CreatedUserId,
                    AsignedUserId = X.AsignedUserId,
                    ProjectId = X.ProjectId,
                    StartDate = X.StartDate,
                    EndDate = X.EndDate,
                    Name = X.Name,
                    Description = X.Description,
                    TargetId = X.TargetId,
                    Status = X.Status
                }).ToList(),
                TaskAttachments = taskAttachmentManager.GetAll().Select(x => new TaskAttachment
                {
                    Id = x.Id,
                    Name = x.Name,
                    FilePath = x.FilePath,
                    UserId = x.UserId,
                    TaskId = x.TaskId
                }).ToList(),
                TaskNotes = taskNoteManager.GetAll().Select(x => new TaskNote
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    TaskId = x.TaskId,
                    Visibility = x.Visibility
                }).ToList(),
                Vaults = vaultManager.GetAll().Select(x => new Vault
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    Name = x.Name,
                    Type = x.Type,
                    Description = x.Description,
                    Value = x.Value,
                    CreatedDate = x.CreatedDate,
                    UserId = x.UserId
                }).ToList(),
                VulnCategories = vulnCategoryManager.GetAll().ToList(),
                Vulns = vulnManager.GetAll().Select(x => new Vuln
                {
                    Id = x.Id,
                    Template = x.Template,
                    Name = x.Name,
                    CreatedDate = x.CreatedDate,
                    UserId = x.UserId,
                    ProjectId = x.ProjectId,
                    TargetId = x.TargetId,
                    VulnCategoryId = x.VulnCategoryId,
                    Risk = x.Risk,
                    Status = x.Status,
                    cve = x.cve,
                    Description = x.Description,
                    ProofOfConcept = x.ProofOfConcept,
                    Impact = x.Impact,
                    CVSS3 = x.CVSS3,
                    CVSSVector = x.CVSSVector,
                    Remediation = x.Remediation,
                    RemediationComplexity = x.RemediationComplexity,
                    RemediationPriority = x.RemediationPriority
                }).ToList(),
                VulnAttachments = vulnAttachmentManager.GetAll().Select(x => new VulnAttachment
                {
                    Id = x.Id,
                    Name = x.Name,
                    FilePath = x.FilePath,
                    UserId = x.UserId,
                    VulnId = x.VulnId
                }).ToList(),
                VulnNotes = vulnNoteManager.GetAll().Select(x => new VulnNote
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    VulnId = x.VulnId
                }).ToList()



            };
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(model, options);
            
            string fileName = "BackupData_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            
            var fileBytes = Encoding.ASCII.GetBytes(jsonString);
            var mimeType = "application/json";
            return new FileContentResult(fileBytes, mimeType)
            {
                FileDownloadName = fileName
            };
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error backup data!";

            _logger.LogError(ex, "An error ocurred making backup data. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult BackupAttachments()
    {
        try
        {
            string startPath = Path.Combine(_appEnvironment.WebRootPath, "Attachments");
            string zipPath = Path.Combine(_appEnvironment.WebRootPath, "")+@"result"+DateTime.Now.ToString("yyyy-MM-dd")+".zip";

           ZipFile.CreateFromDirectory(startPath, zipPath);

           const string contentType ="application/zip";
           HttpContext.Response.ContentType = contentType;
           var result = new FileContentResult(System.IO.File.ReadAllBytes(zipPath), contentType)
           {
               FileDownloadName = "Attachments.zip"
           };

           return result;

        }
        catch (Exception ex)
        {
            TempData["error"] = "Error backup attachments!";

            _logger.LogError(ex, "An error ocurred making attachments backup Index. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    public IActionResult Restore()
    {
        try
        {
            return View("Restore");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error restore !";

            _logger.LogError(ex, "An error ocurred loading restore backup page. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult RestoreAttachments(IFormFile backup)
    {
        try
        {
            var file = Request.Form.Files["uploadAttachments"];
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Temp");
            string extractPath = Path.Combine(_appEnvironment.WebRootPath, "");
            var uniqueName = "Attachments.zip";

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

            ZipFile.ExtractToDirectory(uploads+"/"+uniqueName, extractPath,true);

            System.IO.File.Delete(Path.Combine(uploads, uniqueName));
            Directory.Delete(uploads);
            TempData["restoredAttachments"] = "restored";
            return View("Restore");


        }
        catch (Exception ex)
        {
            TempData["errorAttachments"] = "Error backup attachments!";

            _logger.LogError(ex, "An error ocurred making attachments backup Index. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View("Restore");
        }
    }
    
    [HttpPost]
    public IActionResult RestoreData()
    {
        try
        {
            var file = Request.Form.Files["uploadData"];
            
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Temp");
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

            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            
            
            using FileStream openStream = System.IO.File.OpenRead(Path.Combine(uploads, uniqueName));
            BackupViewModel backupViewModel = JsonSerializer.Deserialize<BackupViewModel>(openStream);

            if (backupViewModel.Users != null)
            {
                foreach (var user in backupViewModel.Users)
                {
                    if (user.Email == "admin@cervantes.local")
                    {

                        var result = userManager.GetByEmail("admin@cervantes.local");
                        userManager.Remove(result);
                        userManager.Context.SaveChanges();
                    }
                    userManager.Add(user);
                    userManager.Context.SaveChanges();
                }
            }

            if (backupViewModel.Clients != null)
            {
                foreach (var client in backupViewModel.Clients)
                {
                    clientManager.Add(client);
                    clientManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.Projects != null)
            {
                foreach (var project in backupViewModel.Projects)
                {
                    projectManager.Add(project);
                    projectManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.ProjectAttachments != null)
            {
                foreach (var attachment in backupViewModel.ProjectAttachments)
                {
                    projectAttachmentManager.Add(attachment);
                    projectAttachmentManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.ProjectNotes != null)
            {
                foreach (var note in backupViewModel.ProjectNotes)
                {
                    projectNoteManager.Add(note);
                    projectNoteManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.ProjectUsers != null)
            {
                foreach (var user in backupViewModel.ProjectUsers)
                {
                    projectUserManager.Add(user);
                    projectUserManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.Documents != null)
            {
                foreach (var doc in backupViewModel.Documents)
                {
                    documentManager.Add(doc);
                    documentManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.Notes != null)
            {
                foreach (var note in backupViewModel.Notes)
                {
                    noteManager.Add(note);
                    noteManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.Organization != null)
            {
                var org = organizationManager.GetById(1);
                org.Name = backupViewModel.Organization.Name;
                org.Description = backupViewModel.Organization.Description;
                org.Url = backupViewModel.Organization.Url;
                org.ContactEmail = backupViewModel.Organization.ContactEmail;
                org.ContactName = backupViewModel.Organization.ContactName;
                org.ContactPhone = backupViewModel.Organization.ContactPhone;
                org.ImagePath = backupViewModel.Organization.ImagePath;

                organizationManager.Context.SaveChanges();

            }
            
            if (backupViewModel.Reports != null)
            {
                foreach (var rep in backupViewModel.Reports)
                {
                    reportManager.Add(rep);
                    reportManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.Targets != null)
            {
                foreach (var target in backupViewModel.Targets)
                {
                    targetManager.Add(target);
                    targetManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.TargetServices != null)
            {
                foreach (var services in backupViewModel.TargetServices)
                {
                    targetServicesManager.Add(services);
                    targetServicesManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.Tasks != null)
            {
                foreach (var task in backupViewModel.Tasks)
                {
                    taskManager.Add(task);
                    taskManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.TaskAttachments != null)
            {
                foreach (var att in backupViewModel.TaskAttachments)
                {
                    taskAttachmentManager.Add(att);
                    taskAttachmentManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.TaskNotes != null)
            {
                foreach (var note in backupViewModel.TaskNotes)
                {
                    taskNoteManager.Add(note);
                    taskNoteManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.Vaults != null)
            {
                foreach (var vault in backupViewModel.Vaults)
                {
                    vaultManager.Add(vault);
                    vaultManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.VulnCategories != null)
            {
                foreach (var cat in backupViewModel.VulnCategories)
                {
                    var result = vulnCategoryManager.GetById(cat.Id);
                    if (result == null)
                    {
                        vulnCategoryManager.Add(cat);
                        vulnCategoryManager.Context.SaveChanges();
                    }
                   
                }
            }
            
            if (backupViewModel.Vulns != null)
            {
                foreach (var vulns in backupViewModel.Vulns)
                {
                    vulnManager.Add(vulns);
                    vulnManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.VulnAttachments != null)
            {
                foreach (var att in backupViewModel.VulnAttachments)
                {
                    vulnAttachmentManager.Add(att);
                    vulnAttachmentManager.Context.SaveChanges();
                }
            }
            
            if (backupViewModel.VulnNotes != null)
            {
                foreach (var note in backupViewModel.VulnNotes)
                {
                    vulnNoteManager.Add(note);
                    vulnNoteManager.Context.SaveChanges();
                }
            }

            System.IO.File.Delete(Path.Combine(uploads, uniqueName));
            Directory.Delete(uploads);
            TempData["restoredData"] = "restored";
            return View("Restore");
        }
        catch (Exception ex)
        {
            TempData["errorDataRestore"] = "Error backup attachments!";

            _logger.LogError(ex, "An error ocurred making attachments backup Index. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Restore");
        }
    }
    
}