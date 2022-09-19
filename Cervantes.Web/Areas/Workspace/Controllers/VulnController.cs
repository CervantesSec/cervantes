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
using System.Web;
using Ganss.XSS;
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
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectUserManager projectUserManager = null;
    private readonly IHostingEnvironment _appEnvironment;
    private readonly ILogger<VulnController> _logger = null;

    public VulnController(IVulnManager vulnManager, IProjectManager projectManager, ILogger<VulnController> logger,
        ITargetManager targetManager, IVulnTargetManager vulnTargetManager,
        IVulnCategoryManager vulnCategoryManager, IVulnNoteManager vulnNoteManager,
        IVulnAttachmentManager vulnAttachmentManager, IProjectUserManager projectUserManager, IHostingEnvironment _appEnvironment)
    {
        this.vulnManager = vulnManager;
        this.projectManager = projectManager;
        this.targetManager = targetManager;
        this.vulnCategoryManager = vulnCategoryManager;
        this.vulnAttachmentManager = vulnAttachmentManager;
        this.vulnNoteManager = vulnNoteManager;
        this.vulnTargetManager = vulnTargetManager;
        this.projectUserManager = projectUserManager;
        this._appEnvironment = _appEnvironment;
        _logger = logger;
    }

    // GET: VulnController
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
            
            var model = new VulnDetailsViewModel
            {
                Project = projectManager.GetById(project),
                Vuln = vulnManager.GetById(id),
                Notes = vulnNoteManager.GetAll().Where(x => x.VulnId == id),
                Attachments = vulnAttachmentManager.GetAll().Where(x => x.VulnId == id),
                Targets = vulnTargetManager.GetAll().Where(x => x.VulnId == id).ToList(),
                TargetList = targets
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
                VulnCatList = vulnCat,
                Project = projectManager.GetById(project)
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
    public ActionResult Create(Guid project, VulnCreateViewModel model)
    {
        try
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");
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
                model.Project = projectManager.GetById(project);
                return View(model);
            }
            

                var vuln = new Vuln
                {
                    Name = model.Name,
                    ProjectId = project,
                    Template = model.Template,
                    cve = model.cve,
                    Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description)),
                    VulnCategoryId = model.VulnCategoryId,
                    Risk = model.Risk,
                    Status = model.Status,
                    Impact = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Impact)),
                    CVSS3 = model.CVSS3,
                    CVSSVector = model.CVSSVector,
                    ProofOfConcept = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ProofOfConcept)),
                    Remediation = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Remediation)),
                    RemediationComplexity = model.RemediationComplexity,
                    RemediationPriority = model.RemediationPriority,
                    CreatedDate = DateTime.Now.ToUniversalTime().AddDays(1),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                vulnManager.Add(vuln);
                vulnManager.Context.SaveChanges();

                if (model.SelectedTargets != null)
                {
                    foreach (var tar in model.SelectedTargets)
                    {
                        VulnTargets vulnTargets = new VulnTargets
                        {
                            VulnId = vuln.Id,
                            TargetId = tar
                        };
                        vulnTargetManager.Add(vulnTargets);
                    }

                    vulnTargetManager.Context.SaveChanges();
                }
                
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
                CVSS3 = vulnResult.CVSS3,
                CVSSVector = vulnResult.CVSSVector,
                ProofOfConcept = vulnResult.ProofOfConcept,
                Remediation = vulnResult.Remediation,
                RemediationComplexity = vulnResult.RemediationComplexity,
                RemediationPriority = vulnResult.RemediationPriority,
                CreatedDate = vulnResult.CreatedDate,
                UserId = vulnResult.UserId,
                TargetList = targets,
                VulnCatList = vulnCat,
                SelectedTargets = vulnTargetManager.GetAll().Where(x => x.VulnId == id).Select(e => e.Id).ToList()
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
    public ActionResult Edit(Guid project, VulnCreateViewModel model, Guid id)
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
            var result = vulnManager.GetById(id);
            result.Name = model.Name;
            result.Template = model.Template;
            result.cve = model.cve;
            result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
            result.VulnCategoryId = model.VulnCategoryId;
            result.Risk = model.Risk;
            result.Status = model.Status;
            result.Impact = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Impact));
            result.CVSS3 = model.CVSS3;
            result.CVSSVector = model.CVSSVector;
            result.ProofOfConcept = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ProofOfConcept));
            result.Remediation = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Remediation));
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

    public ActionResult Templates(Guid project)
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

    public ActionResult Template(Guid project, Guid id)
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
    public ActionResult Template(Guid project, VulnCreateViewModel model, Guid id)
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
            var result = vulnManager.GetById(id);
            result.Name = model.Name;
            result.Template = model.Template;
            result.cve = model.cve;
            result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
            result.VulnCategoryId = model.VulnCategoryId;
            result.Risk = model.Risk;
            result.Status = model.Status;
            result.Impact = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Impact));
            result.CVSS3 = model.CVSS3;
            result.CVSSVector = model.CVSSVector;
            result.ProofOfConcept = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ProofOfConcept));
            result.Remediation = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Remediation));
            result.RemediationComplexity = model.RemediationComplexity;
            result.RemediationPriority = model.RemediationPriority;
            result.ProjectId = project;
            result.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            result.CreatedDate = DateTime.Now.ToUniversalTime().AddDays(1);

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
    
        public ActionResult TemplateEdit(Guid project, Guid id)
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
    public ActionResult TemplateEdit(Guid project, VulnCreateViewModel model, Guid id)
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
            var result = vulnManager.GetById(id);
            result.Name = model.Name;
            result.Template = model.Template;
            result.cve = model.cve;
            result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
            result.VulnCategoryId = model.VulnCategoryId;
            result.Risk = model.Risk;
            result.Status = model.Status;
            result.Impact = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Impact));
            result.CVSS3 = model.CVSS3;
            result.CVSSVector = model.CVSSVector;
            result.ProofOfConcept = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ProofOfConcept));
            result.Remediation = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Remediation));
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
    public IActionResult AddNote(Guid project,IFormCollection form)
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
                    VulnId = Guid.Parse(form["vulnId"])
                };

                vulnNoteManager.Add(note);
                vulnNoteManager.Context.SaveChanges();
                TempData["addedNote"] = "added";
                _logger.LogInformation("User: {0} Added new note: {1} on Vuln: {2}",
                    User.FindFirstValue(ClaimTypes.Name), note.Name, form["vulnId"]);
                return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
            }
            else
            {
                return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error adding note vuln!";
            _logger.LogError(ex, "An error ocurred adding a Note on Vuln: {1}. User: {2}", form["vulnId"],
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
        }
    }


    [HttpPost]
    public IActionResult DeleteNote(Guid id, Guid project, Guid vuln)
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
    public IActionResult AddAttachment(Guid project, IFormCollection form, IFormFile upload)
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
                    VulnId = Guid.Parse(form["vulnId"]),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    FilePath = "/Attachments/Project/" + form["vulnId"] + "/" + uniqueName
                };

                vulnAttachmentManager.Add(attachment);
                vulnAttachmentManager.Context.SaveChanges();
                TempData["addedAttachment"] = "added";
                _logger.LogInformation("User: {0} Added an attachment: {1} on Vuln: {2}",
                    User.FindFirstValue(ClaimTypes.Name), attachment.Name, form["vulnId"]);
                return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
            }
            else
            {
                TempData["errorAttachment"] = "added";
                _logger.LogError("An error ocurred adding an Attachment on Vuln: {1}. User: {2}",
                    form["vulnId"], User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error adding attachment to vuln!";
            _logger.LogError(ex, "An error ocurred adding an Attachment on Vuln: {1}. User: {2}",
                form["project"], User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
        }
    }

    [HttpPost]
    public IActionResult DeleteAttachment(Guid id, Guid project, Guid vuln)
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
    
    public IActionResult DeleteTarget(Guid id, Guid project, Guid vuln)
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
                var result = vulnTargetManager.GetById(id);

                vulnTargetManager.Remove(result);
                vulnTargetManager.Context.SaveChanges();
                TempData["deletedNote"] = "deleted";
                _logger.LogInformation("User: {0} Deleted vuln target: {1} on Vuln: {2}", User.FindFirstValue(ClaimTypes.Name),
                    result.Id, vuln);
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

               var result = vulnTargetManager.GetAll().Where(x => x.TargetId == Guid.Parse(form["TargetId"]) && x.VulnId == Guid.Parse(form["vulnId"])).FirstOrDefault();
                if (result != null)
                {
                    TempData["targetExists"] = "exists";
                    return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
                }
                
                var tar = new VulnTargets
                {
                    TargetId = Guid.Parse(form["TargetId"]),
                    VulnId = Guid.Parse(form["vulnId"]),
                    
                };

                vulnTargetManager.Add(tar);
                vulnTargetManager.Context.SaveChanges();
                TempData["addedTarget"] = "added";
                _logger.LogInformation("User: {0} Added new target: {1} on Vuln: {2}",
                    User.FindFirstValue(ClaimTypes.Name), tar.Id, form["vulnId"]);
                return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
            }
            else
            {
                return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error adding note vuln!";
            _logger.LogError(ex, "An error ocurred adding a Note on Vuln: {1}. User: {2}", form["vulnId"],
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
        }
    }
}