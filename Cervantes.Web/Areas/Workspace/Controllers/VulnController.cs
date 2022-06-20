using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Areas.Workspace.Models;
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
using Microsoft.AspNetCore.Authorization;

namespace Cervantes.Web.Areas.Workspace.Controllers;
[Authorize(Roles = "Admin,SuperUser,User")]
[Area("Workspace")]
public class VulnController : Controller
{
    private IVulnManager vulnManager = null;
    private IProjectManager projectManager = null;
    private ITargetManager targetManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private IVulnNoteManager vulnNoteManager = null;
    private IVulnAttachmentManager vulnAttachmentManager = null;
    private IProjectUserManager projectUserManager = null;
    private readonly IHostingEnvironment _appEnvironment;
    private readonly ILogger<VulnController> _logger = null;

    public VulnController(IVulnManager vulnManager, IProjectManager projectManager, ILogger<VulnController> logger,
        ITargetManager targetManager,
        IVulnCategoryManager vulnCategoryManager, IVulnNoteManager vulnNoteManager,
        IVulnAttachmentManager vulnAttachmentManager, IProjectUserManager projectUserManager, IHostingEnvironment _appEnvironment)
    {
        this.vulnManager = vulnManager;
        this.projectManager = projectManager;
        this.targetManager = targetManager;
        this.vulnCategoryManager = vulnCategoryManager;
        this.vulnAttachmentManager = vulnAttachmentManager;
        this.vulnNoteManager = vulnNoteManager;
        this.projectUserManager = projectUserManager;
        this._appEnvironment = _appEnvironment;
        _logger = logger;
    }

    // GET: VulnController
    public ActionResult Index(int project)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var model = new VulnViewModel
            {
                Project = projectManager.GetById(project),
                Vulns = vulnManager.GetAll().Where(x => x.ProjectId == project && x.Template == false).ToList()
            };
            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading vulns!";

            _logger.LogError(e, "An error ocurred loading Vuln Workspace Index. Project: {1} User: {2}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // GET: VulnController/Details/5
    public ActionResult Details(int project, int id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var model = new VulnDetailsViewModel
            {
                Project = projectManager.GetById(project),
                Vuln = vulnManager.GetById(id),
                Notes = vulnNoteManager.GetAll().Where(x => x.VulnId == id),
                Attachments = vulnAttachmentManager.GetAll().Where(x => x.VulnId == id)
            };
            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading vuln!";
            _logger.LogError(e, "An error ocurred loading Vuln Workspace Details.Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // GET: VulnController/Create
    public ActionResult Create(int project)
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

            var result2 = vulnCategoryManager.GetAll().Select(e => new VulnCreateViewModel
            {
                VulnCategoryId = e.Id,
                VulnCategoryName = e.Name
            }).ToList();

            var vulnCat = new List<SelectListItem>();

            foreach (var item in result2)
                vulnCat.Add(new SelectListItem {Text = item.VulnCategoryName, Value = item.VulnCategoryId.ToString()});

            var model = new VulnCreateViewModel
            {
                TargetList = targets,
                VulnCatList = vulnCat
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Task Workspace create form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // POST: VulnController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(int project, VulnCreateViewModel model)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces", new {area = ""});
            }
            
            if (!ModelState.IsValid)
            {
                var result = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new VulnCreateViewModel
                {
                    TargetId = e.Id,
                    TargetName = e.Name
                }).ToList();

                var targets = new List<SelectListItem>();

                foreach (var item in result)
                    targets.Add(new SelectListItem {Text = item.TargetName, Value = item.TargetId.ToString()});

                var result2 = vulnCategoryManager.GetAll().Select(e => new VulnCreateViewModel
                {
                    VulnCategoryId = e.Id,
                    VulnCategoryName = e.Name
                }).ToList();

                var vulnCat = new List<SelectListItem>();

                foreach (var item in result2)
                    vulnCat.Add(new SelectListItem {Text = item.VulnCategoryName, Value = item.VulnCategoryId.ToString()});
                
                model.TargetList = targets;
                model.VulnCatList = vulnCat;
                return View(model);
            }
            

                var vuln = new Vuln
                {
                    Name = model.Name,
                    ProjectId = project,
                    Template = model.Template,
                    cve = model.cve,
                    Description = model.Description,
                    VulnCategoryId = model.VulnCategoryId,
                    Risk = model.Risk,
                    Status = model.Status,
                    Impact = model.Impact,
                    TargetId = model.TargetId.GetHashCode(),
                    CVSS3 = model.CVSS3,
                    CVSSVector = model.CVSSVector,
                    ProofOfConcept = model.ProofOfConcept,
                    Remediation = model.Remediation,
                    RemediationComplexity = model.RemediationComplexity,
                    RemediationPriority = model.RemediationPriority,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                vulnManager.Add(vuln);
                vulnManager.Context.SaveChanges();
                TempData["added"] = "added";
                _logger.LogInformation("User: {0} Created a new Vuln on Project {1}",
                    User.FindFirstValue(ClaimTypes.Name),
                    project);
                return RedirectToAction(nameof(Index));
            

        }
        catch
        {
            return View();
        }
    }

    // GET: VulnController/Edit/5
    public ActionResult Edit(int project, int id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var vulnResult = vulnManager.GetById(id);


            var result = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new VulnCreateViewModel
            {
                TargetId = e.Id,
                TargetName = e.Name
            }).ToList();

            var targets = new List<SelectListItem>();

            foreach (var item in result)
                targets.Add(new SelectListItem {Text = item.TargetName, Value = item.TargetId.ToString()});

            var result2 = vulnCategoryManager.GetAll().Select(e => new VulnCreateViewModel
            {
                VulnCategoryId = e.Id,
                VulnCategoryName = e.Name
            }).ToList();

            var vulnCat = new List<SelectListItem>();

            foreach (var item in result2)
                vulnCat.Add(new SelectListItem {Text = item.VulnCategoryName, Value = item.VulnCategoryId.ToString()});


            var model = new VulnCreateViewModel
            {
                Name = vulnResult.Name,
                Project = projectManager.GetById(project),
                ProjectId = project,
                Template = vulnResult.Template,
                cve = vulnResult.cve,
                Description = vulnResult.Description,
                VulnCategoryId = vulnResult.VulnCategoryId,
                Risk = vulnResult.Risk,
                Status = vulnResult.Status,
                Impact = vulnResult.Impact,
                TargetId = vulnResult.TargetId,
                CVSS3 = vulnResult.CVSS3,
                CVSSVector = vulnResult.CVSSVector,
                ProofOfConcept = vulnResult.ProofOfConcept,
                Remediation = vulnResult.Remediation,
                RemediationComplexity = vulnResult.RemediationComplexity,
                RemediationPriority = vulnResult.RemediationPriority,
                CreatedDate = vulnResult.CreatedDate,
                UserId = vulnResult.UserId,
                TargetList = targets,
                VulnCatList = vulnCat
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading vuln!";
            _logger.LogError(e, "An error ocurred loading Vuln Workspace edit PROJECT form.Project: {0} User: {1}",
                project, User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // POST: VulnController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int project, VulnCreateViewModel model, int id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var result = vulnManager.GetById(id);
            result.Name = model.Name;
            result.Template = model.Template;
            result.cve = model.cve;
            result.Description = model.Description;
            result.VulnCategoryId = model.VulnCategoryId;
            result.Risk = model.Risk;
            result.Status = model.Status;
            result.Impact = model.Impact;
            result.TargetId = model.TargetId;
            result.CVSS3 = model.CVSS3;
            result.CVSSVector = model.CVSSVector;
            result.ProofOfConcept = model.ProofOfConcept;
            result.Remediation = model.Remediation;
            result.RemediationComplexity = model.RemediationComplexity;
            result.RemediationPriority = model.RemediationPriority;
            result.CreatedDate = result.CreatedDate;


            vulnManager.Context.SaveChanges();
            TempData["edited"] = "edited";
            _logger.LogInformation("User: {0} edited Vuln: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name),
                id, project);

            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["error"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred editing a Vuln Workspace on. Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // GET: VulnController/Delete/5
    public ActionResult Delete(int project, int id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var result = vulnManager.GetById(id);
            return View(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred loading Vuln Workspace delete form. Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View("Index");
        }
    }

    // POST: VulnController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int project, int id, IFormCollection collection)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var result = vulnManager.GetById(id);
            if (result != null)
            {
                vulnManager.Remove(result);
                vulnManager.Context.SaveChanges();
            }

            TempData["deleted"] = "deleted";
            _logger.LogInformation("User: {0} deleted Vuln: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name),
                id, project);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["error"] = "Error deleting vulns!";
            _logger.LogError(e, "An error ocurred deleting a Vuln Workspace on. Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    public ActionResult Templates(int project)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var model = new VulnViewModel
            {
                Project = projectManager.GetById(project),
                Vulns = vulnManager.GetAll().Where(x => x.Template == true).ToList()
            };
            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading vulns!";

            _logger.LogError(e, "An error ocurred loading Vuln Workspace Templates. Project: {1} User: {2}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    public ActionResult Template(int project, int id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var vulnResult = vulnManager.GetById(id);


            var result = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new VulnCreateViewModel
            {
                TargetId = e.Id,
                TargetName = e.Name
            }).ToList();

            var targets = new List<SelectListItem>();

            foreach (var item in result)
                targets.Add(new SelectListItem {Text = item.TargetName, Value = item.TargetId.ToString()});

            var result2 = vulnCategoryManager.GetAll().Select(e => new VulnCreateViewModel
            {
                VulnCategoryId = e.Id,
                VulnCategoryName = e.Name
            }).ToList();

            var vulnCat = new List<SelectListItem>();

            foreach (var item in result2)
                vulnCat.Add(new SelectListItem {Text = item.VulnCategoryName, Value = item.VulnCategoryId.ToString()});


            var model = new VulnCreateViewModel
            {
                Name = vulnResult.Name,
                Project = projectManager.GetById(project),
                ProjectId = project,
                Template = false,
                cve = vulnResult.cve,
                Description = vulnResult.Description,
                VulnCategoryId = vulnResult.VulnCategoryId,
                Risk = vulnResult.Risk,
                Status = vulnResult.Status,
                Impact = vulnResult.Impact,
                TargetId = vulnResult.TargetId,
                CVSS3 = vulnResult.CVSS3,
                CVSSVector = vulnResult.CVSSVector,
                ProofOfConcept = vulnResult.ProofOfConcept,
                Remediation = vulnResult.Remediation,
                RemediationComplexity = vulnResult.RemediationComplexity,
                RemediationPriority = vulnResult.RemediationPriority,
                CreatedDate = vulnResult.CreatedDate,
                UserId = vulnResult.UserId,
                TargetList = targets,
                VulnCatList = vulnCat
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading vuln!";
            _logger.LogError(e, "An error ocurred loading Vuln Workspace edit PROJECT form.Project: {0} User: {1}",
                project, User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // POST: VulnController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Template(int project, VulnCreateViewModel model, int id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var result = vulnManager.GetById(id);
            result.Id = 0;
            result.Name = model.Name;
            result.Template = model.Template;
            result.cve = model.cve;
            result.Description = model.Description;
            result.VulnCategoryId = model.VulnCategoryId;
            result.Risk = model.Risk;
            result.Status = model.Status;
            result.Impact = model.Impact;
            result.TargetId = model.TargetId;
            result.CVSS3 = model.CVSS3;
            result.CVSSVector = model.CVSSVector;
            result.ProofOfConcept = model.ProofOfConcept;
            result.Remediation = model.Remediation;
            result.RemediationComplexity = model.RemediationComplexity;
            result.RemediationPriority = model.RemediationPriority;
            result.ProjectId = project;
            result.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            vulnManager.Add(result);
            vulnManager.Context.SaveChanges();
            TempData["edited"] = "edited";
            _logger.LogInformation("User: {0} added Vuln: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), id,
                project);

            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["error"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred editing a Vuln Workspace on. Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }
    
        public ActionResult TemplateEdit(int project, int id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var vulnResult = vulnManager.GetById(id);


            var result = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new VulnCreateViewModel
            {
                TargetId = e.Id,
                TargetName = e.Name
            }).ToList();

            var targets = new List<SelectListItem>();

            foreach (var item in result)
                targets.Add(new SelectListItem {Text = item.TargetName, Value = item.TargetId.ToString()});

            var result2 = vulnCategoryManager.GetAll().Select(e => new VulnCreateViewModel
            {
                VulnCategoryId = e.Id,
                VulnCategoryName = e.Name
            }).ToList();

            var vulnCat = new List<SelectListItem>();

            foreach (var item in result2)
                vulnCat.Add(new SelectListItem {Text = item.VulnCategoryName, Value = item.VulnCategoryId.ToString()});


            var model = new VulnCreateViewModel
            {
                Name = vulnResult.Name,
                Project = projectManager.GetById(project),
                ProjectId = project,
                Template = true,
                cve = vulnResult.cve,
                Description = vulnResult.Description,
                VulnCategoryId = vulnResult.VulnCategoryId,
                Risk = vulnResult.Risk,
                Status = vulnResult.Status,
                Impact = vulnResult.Impact,
                TargetId = vulnResult.TargetId,
                CVSS3 = vulnResult.CVSS3,
                CVSSVector = vulnResult.CVSSVector,
                ProofOfConcept = vulnResult.ProofOfConcept,
                Remediation = vulnResult.Remediation,
                RemediationComplexity = vulnResult.RemediationComplexity,
                RemediationPriority = vulnResult.RemediationPriority,
                CreatedDate = vulnResult.CreatedDate,
                UserId = vulnResult.UserId,
                TargetList = targets,
                VulnCatList = vulnCat
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading vuln!";
            _logger.LogError(e, "An error ocurred loading Vuln Workspace edit PROJECT form.Project: {0} User: {1}",
                project, User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }
        
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult TemplateEdit(int project, VulnCreateViewModel model, int id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            var result = vulnManager.GetById(id);
            result.Id = model.Id;
            result.Name = model.Name;
            result.Template = model.Template;
            result.cve = model.cve;
            result.Description = model.Description;
            result.VulnCategoryId = model.VulnCategoryId;
            result.Risk = model.Risk;
            result.Status = model.Status;
            result.Impact = model.Impact;
            result.TargetId = model.TargetId;
            result.CVSS3 = model.CVSS3;
            result.CVSSVector = model.CVSSVector;
            result.ProofOfConcept = model.ProofOfConcept;
            result.Remediation = model.Remediation;
            result.RemediationComplexity = model.RemediationComplexity;
            result.RemediationPriority = model.RemediationPriority;
            result.ProjectId = project;
            result.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            vulnManager.Context.SaveChanges();
            TempData["edited"] = "edited";
            _logger.LogInformation("User: {0} edited Vuln Template: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), id,
                project);

            return RedirectToAction("Templates");
        }
        catch (Exception e)
        {
            TempData["error"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred editing a Vuln Workspace on. Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return View("Templates");
        }
    }

    [HttpPost]
    public IActionResult AddNote(int project,IFormCollection form)
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
                var note = new VulnNote
                {
                    Name = form["noteName"],
                    Description = form["noteDescription"],
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Visibility = (Visibility) Enum.ToObject(typeof(Visibility), int.Parse(form["Visibility"])),
                    VulnId = int.Parse(form["vulnId"])
                };

                vulnNoteManager.Add(note);
                vulnNoteManager.Context.SaveChanges();
                TempData["addedNote"] = "added";
                _logger.LogInformation("User: {0} Added new note: {1} on Vuln: {2}",
                    User.FindFirstValue(ClaimTypes.Name), note.Name, int.Parse(form["vulnId"]));
                return RedirectToAction("Details", "Vuln", new {id = int.Parse(form["vulnId"])});
            }
            else
            {
                return RedirectToAction("Details", "Vuln", new {id = int.Parse(form["vulnId"])});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error adding note vuln!";
            _logger.LogError(ex, "An error ocurred adding a Note on Vuln: {1}. User: {2}", int.Parse(form["vulnId"]),
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Vuln", new {id = int.Parse(form["vulnId"])});
        }
    }


    [HttpPost]
    public IActionResult DeleteNote(int id, int project, int vuln)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            if (id != 0)
            {
                var result = vulnNoteManager.GetById(id);

                vulnNoteManager.Remove(result);
                vulnNoteManager.Context.SaveChanges();
                TempData["deletedNote"] = "deleted";
                _logger.LogInformation("User: {0} Deleted note: {1} on Vuln: {2}", User.FindFirstValue(ClaimTypes.Name),
                    result.Name, vuln);
                return RedirectToAction("Details", "Vuln", new {id = result.VulnId});
            }
            else
            {
                return RedirectToAction("Details", "Vuln", new {id = vuln});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error deleting vuln note!";
            _logger.LogError(ex, "An error ocurred deleting a Note on Project: {1}. User: {2}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Vuln", new {id = vuln});
        }
    }


    [HttpPost]
    public IActionResult AddAttachment(int project, IFormCollection form, IFormFile upload)
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
                var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Vuln/" + form["vulnId"] + "/");
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


                var attachment = new VulnAttachment
                {
                    Name = form["attachmentName"],
                    VulnId = int.Parse(form["vulnId"]),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    FilePath = "/Attachments/Project/" + form["vulnId"] + "/" + uniqueName
                };

                vulnAttachmentManager.Add(attachment);
                vulnAttachmentManager.Context.SaveChanges();
                TempData["addedAttachment"] = "added";
                _logger.LogInformation("User: {0} Added an attachment: {1} on Vuln: {2}",
                    User.FindFirstValue(ClaimTypes.Name), attachment.Name, int.Parse(form["vulnId"]));
                return RedirectToAction("Details", "Vuln", new {id = int.Parse(form["vulnId"])});
            }
            else
            {
                TempData["errorAttachment"] = "added";
                _logger.LogError("An error ocurred adding an Attachment on Vuln: {1}. User: {2}",
                    int.Parse(form["vulnId"]), User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Vuln", new {id = int.Parse(form["vulnId"])});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error adding attachment to vuln!";
            _logger.LogError(ex, "An error ocurred adding an Attachment on Vuln: {1}. User: {2}",
                int.Parse(form["project"]), User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Vuln", new {id = int.Parse(form["vulnId"])});
        }
    }

    [HttpPost]
    public IActionResult DeleteAttachment(int id, int project, int vuln)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            if (id != 0)
            {
                var result = vulnAttachmentManager.GetById(id);

                var pathFile = _appEnvironment.WebRootPath + result.FilePath;
                if (System.IO.File.Exists(pathFile)) System.IO.File.Delete(pathFile);

                vulnAttachmentManager.Remove(result);
                vulnAttachmentManager.Context.SaveChanges();
                TempData["deletedAttachment"] = "deleted";
                _logger.LogInformation("User: {0} Deleted an attachment: {1} on Vuln: {2}",
                    User.FindFirstValue(ClaimTypes.Name), result.Name, vuln);
                return RedirectToAction("Details", "Vuln", new {id = vuln});
            }
            else
            {
                _logger.LogError("An error ocurred deleting an Attachment on Vuln: {1}. User: {2}", vuln,
                    User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Vuln", new {id = vuln});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error deleting attachment from vuln!";
            _logger.LogError(ex, "An error ocurred deleting an Attachment on Vuln: {1}. User: {2}", vuln,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Vuln", new {id = vuln});
        }
    }
}