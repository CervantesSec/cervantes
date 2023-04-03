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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Cervantes.IFR.Jira;
using Cervantes.IFR.Parsers.CSV;
using CsvHelper;
using CsvHelper.Configuration;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using MimeDetective;

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
    private IJIraService jiraService = null;
    private IJiraManager jiraManager = null;
    private IJiraCommentManager jiraCommentManager = null;
    private IProjectAttachmentManager projectAttachmentManager = null;
    private ICsvParser csvParser = null;

    public VulnController(IVulnManager vulnManager, IProjectManager projectManager, ILogger<VulnController> logger,
        ITargetManager targetManager, IVulnTargetManager vulnTargetManager,
        IVulnCategoryManager vulnCategoryManager, IVulnNoteManager vulnNoteManager,
        IVulnAttachmentManager vulnAttachmentManager, IProjectUserManager projectUserManager, IHostingEnvironment _appEnvironment,
        IJIraService jiraService, IJiraManager jiraManager, IJiraCommentManager jiraCommentManager, IProjectAttachmentManager projectAttachmentManager,
        ICsvParser csvParser)
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
        this.jiraService = jiraService;
        this.jiraCommentManager = jiraCommentManager;
        this.jiraManager = jiraManager;
        this.projectAttachmentManager = projectAttachmentManager;
        this.csvParser = csvParser;
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
            _logger.LogError(e, "An error ocurred loading Vuln Workspace Index. Project: {1} User: {2}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return Redirect("/Home/Error");
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
            
            var jiraEnabled = jiraService.JiraEnabled();
            if (jiraEnabled == true)
            {
                var vuln = vulnManager.GetById(id);
                
                if (vuln.JiraCreated == true)
                {
                    var jira = jiraManager.GetByVulnId(id);
                    //jiraService.Issue(jira.JiraKey);
                    
                    var model = new VulnDetailsViewModel
                    {
                        Project = projectManager.GetById(project),
                        Vuln = vuln,
                        Notes = vulnNoteManager.GetAll().Where(x => x.VulnId == id),
                        Attachments = vulnAttachmentManager.GetAll().Where(x => x.VulnId == id),
                        Targets = vulnTargetManager.GetAll().Where(x => x.VulnId == id).ToList(),
                        TargetList = targets,
                        JiraEnabled = jiraService.JiraEnabled(),
                        Jira = jira,
                        JiraComments = jiraCommentManager.GetAll().Where(x => x.JiraId == jira.Id).ToList()
                
                    };
                    return View(model);
                }
                else
                {

                    var model = new VulnDetailsViewModel
                    {
                        Project = projectManager.GetById(project),
                        Vuln = vuln,
                        Notes = vulnNoteManager.GetAll().Where(x => x.VulnId == id),
                        Attachments = vulnAttachmentManager.GetAll().Where(x => x.VulnId == id),
                        Targets = vulnTargetManager.GetAll().Where(x => x.VulnId == id).ToList(),
                        TargetList = targets,
                        JiraEnabled = jiraService.JiraEnabled(),

                    };
                    return View(model);
                }
            }
            else
            {
                var model = new VulnDetailsViewModel
                {
                    Project = projectManager.GetById(project),
                    Vuln = vulnManager.GetById(id),
                    Notes = vulnNoteManager.GetAll().Where(x => x.VulnId == id),
                    Attachments = vulnAttachmentManager.GetAll().Where(x => x.VulnId == id),
                    Targets = vulnTargetManager.GetAll().Where(x => x.VulnId == id).ToList(),
                    TargetList = targets,
                    JiraEnabled = jiraService.JiraEnabled()
                };
                return View(model);
            }

            
        }
        catch (Exception e)
        {
            TempData["errorVuln"] = "Error loading vuln!";
            _logger.LogError(e, "An error ocurred loading Vuln Workspace Details.Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
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

            /*var result2 = vulnCategoryManager.GetAll().Select(e => new VulnCreateViewModel
            {
                VulnCategoryId = e.Id,
                VulnCategoryName = e.Name
            }).ToList();

            var vulnCat = new List<SelectListItem>();

            foreach (var item in result2)
                vulnCat.Add(new SelectListItem {Text = item.VulnCategoryName, Value = item.VulnCategoryId.ToString()});*/

            var model = new VulnCreateViewModel
            {
                TargetList = targets,
                //VulnCatList = vulnCat,
                Project = projectManager.GetById(project),
                VulnCategories = vulnCategoryManager.GetAll().ToList(),
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorVuln"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Task Workspace create form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
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
            
            var projectModel = projectManager.GetById(project);
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

                /*var result2 = vulnCategoryManager.GetAll().Select(e => new VulnCreateViewModel
                {
                    VulnCategoryId = e.Id,
                    VulnCategoryName = e.Name
                }).ToList();

                var vulnCat = new List<SelectListItem>();

                foreach (var item in result2)
                    vulnCat.Add(new SelectListItem {Text = item.VulnCategoryName, Value = item.VulnCategoryId.ToString()});*/
                
                model.TargetList = targets;
                //model.VulnCatList = vulnCat;
                model.Project = projectModel;
                model.VulnCategories = vulnCategoryManager.GetAll().ToList();
                return View(model);
            }

            var vulnNum = vulnManager.GetAll().Count(x => x.ProjectId == project && x.Template == false) + 1;
            switch (projectModel.Score)
            {
                case Score.CVSS:
                {
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
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        FindingId = projectModel.FindingsId+"-"+vulnNum.ToString("D2"),
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

                    TempData["addedVuln"] = "added";
                    _logger.LogInformation("User: {0} Created a new Vuln on Project {1}",
                        User.FindFirstValue(ClaimTypes.Name),
                        project);
                    return RedirectToAction(nameof(Index));
                }
                case Score.OWASP:
                {
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
                        ProofOfConcept = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ProofOfConcept)),
                        Remediation = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Remediation)),
                        RemediationComplexity = model.RemediationComplexity,
                        RemediationPriority = model.RemediationPriority,
                        CreatedDate = DateTime.Now.ToUniversalTime().AddDays(1),
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        OWASPRisk = model.OwaspRisk,
                        OWASPVector = model.OwaspVector,
                        OWASPLikehood = model.OwaspLikehood,
                        OWASPImpact = model.OwaspImpact,
                        FindingId = projectModel.FindingsId+"-"+vulnNum.ToString("D2"),
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

                    TempData["addedVuln"] = "added";
                    _logger.LogInformation("User: {0} Created a new Vuln on Project {1}",
                        User.FindFirstValue(ClaimTypes.Name),
                        project);
                    return RedirectToAction(nameof(Index));
                }
            }
            
            return RedirectToAction(nameof(Index));
        }
        catch(Exception e)
        {
            TempData["errorAddVuln"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Task Workspace create form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Create");
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

            var vulnCategories = vulnCategoryManager.GetAll().ToList();

            var projectResult = projectManager.GetById(project);
            var model = new VulnCreateViewModel
            {
                Name = vulnResult.Name,
                Project = projectResult,
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
                OwaspImpact = vulnResult.OWASPImpact,
                OwaspLikehood = vulnResult.OWASPLikehood,
                OwaspVector = vulnResult.OWASPVector,
                OwaspRisk = vulnResult.OWASPRisk,
                TargetList = targets,
                VulnCategories = vulnCategories,
                SelectedTargets = vulnTargetManager.GetAll().Where(x => x.VulnId == id).Select(e => e.Id).ToList()
            };

            switch (projectResult.Score)
            {
                case Score.OWASP:
                    return View("Edit",model);
                    break;
            }
            return View("Edit",model);
            
        }
        catch (Exception e)
        {
            TempData["errorVuln"] = "Error loading vuln!";
            _logger.LogError(e, "An error ocurred loading Vuln Workspace edit PROJECT form.Project: {0} User: {1}",
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
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
            var pro = projectManager.GetById(project);
            switch (pro.Score)
            {
                case Score.CVSS:
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
                    break;
                case Score.OWASP:
                    result.Name = model.Name;
                    result.Template = model.Template;
                    result.cve = model.cve;
                    result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                    result.VulnCategoryId = model.VulnCategoryId;
                    result.Risk = model.Risk;
                    result.Status = model.Status;
                    result.Impact = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Impact));
                    result.ProofOfConcept = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ProofOfConcept));
                    result.Remediation = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Remediation));
                    result.RemediationComplexity = model.RemediationComplexity;
                    result.RemediationPriority = model.RemediationPriority;
                    result.CreatedDate = result.CreatedDate;
                    result.OWASPVector = model.OwaspVector;
                    result.OWASPRisk = model.OwaspRisk;
                    result.OWASPImpact = model.OwaspImpact;
                    result.OWASPLikehood = model.OwaspLikehood;
                    break;
                
            }
            
            
            vulnManager.Context.SaveChanges();
            TempData["editedVuln"] = "edited";
            _logger.LogInformation("User: {0} edited Vuln: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name),
                id, project);
            if (result.JiraCreated == true)
            {
                var jira = jiraManager.GetByVulnId(result.Id);
                jiraService.UpdateIssue(jira.JiraKey); 
            }
            

            return RedirectToAction("Details", "Vuln", new {area="Workspace", project,id = id});

        }
        catch (Exception e)
        {
            TempData["errorEditingVuln"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred editing a Vuln Workspace on. Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Edit", "Vuln", new {area="Workspace", project,id = id});

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
            TempData["errorVuln"] = "User is not in the project";
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

            TempData["deletedVuln"] = "deleted";
            _logger.LogInformation("User: {0} deleted Vuln: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name),
                id, project);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["errorDeleteVuln"] = "Error deleting vulns!";
            _logger.LogError(e, "An error ocurred deleting a Vuln Workspace on. Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Delete", new {id = id});
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

            _logger.LogError(e, "An error ocurred loading Vuln Workspace Templates. Project: {1} User: {2}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return Redirect("/Home/Error");
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

            var categories = vulnCategoryManager.GetAll().ToList();


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
                VulnCategories = categories,
                OwaspVector = vulnResult.OWASPVector,
                OwaspImpact = vulnResult.OWASPImpact,
                OwaspLikehood = vulnResult.OWASPLikehood,
                OwaspRisk = vulnResult.OWASPRisk
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorVuln"] = "Error loading vuln!";
            _logger.LogError(e, "An error ocurred loading Vuln Workspace edit PROJECT form.Project: {0} User: {1}",
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Templates");
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
             var pro = projectManager.GetById(project);
            switch (pro.Score)
            {
                case Score.CVSS:
                    result.Id = new Guid();
                    result.Name = model.Name;
                    result.ProjectId = project;
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
                    var vulNum = vulnManager.GetAll().Count(x => x.ProjectId == project && x.Template == false) + 1;
                    result.FindingId = pro.FindingsId + "-" + vulNum.ToString("D2");
                    break;
                case Score.OWASP:
                    result.Id = new Guid();
                    result.Name = model.Name;
                    result.Template = model.Template;
                    result.cve = model.cve;
                    result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                    result.VulnCategoryId = model.VulnCategoryId;
                    result.Risk = model.Risk;
                    result.Status = model.Status;
                    result.Impact = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Impact));
                    result.ProofOfConcept = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ProofOfConcept));
                    result.Remediation = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Remediation));
                    result.RemediationComplexity = model.RemediationComplexity;
                    result.RemediationPriority = model.RemediationPriority;
                    result.CreatedDate = result.CreatedDate;
                    result.OWASPVector = model.OwaspVector;
                    result.OWASPRisk = model.OwaspRisk;
                    result.OWASPImpact = model.OwaspImpact;
                    result.OWASPLikehood = model.OwaspLikehood;
                    var vulNum2 = vulnManager.GetAll().Count(x => x.ProjectId == project && x.Template == false) + 1;
                    result.FindingId = pro.FindingsId + "-" + vulNum2.ToString("D2");
                    break;
                
            }

            vulnManager.Add(result);
            vulnManager.Context.SaveChanges();
            TempData["addedVuln"] = "edited";
            _logger.LogInformation("User: {0} added Vuln: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), id,
                project);

            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["errorAddVuln"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred editing a Vuln Workspace on. Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Template", new {id = id});
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

            var categories = vulnCategoryManager.GetAll().ToList();
            


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
                VulnCategories = categories,
                OwaspVector = vulnResult.OWASPVector,
                OwaspImpact = vulnResult.OWASPImpact,
                OwaspLikehood = vulnResult.OWASPLikehood,
                OwaspRisk = vulnResult.OWASPRisk
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorVuln"] = "Error loading vuln!";
            _logger.LogError(e, "An error ocurred loading Vuln Workspace edit PROJECT form.Project: {0} User: {1}",
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Templates");
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
              var pro = projectManager.GetById(project);
            switch (pro.Score)
            {
                case Score.CVSS:
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
                    break;
                case Score.OWASP:
                    result.Name = model.Name;
                    result.Template = model.Template;
                    result.cve = model.cve;
                    result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
                    result.VulnCategoryId = model.VulnCategoryId;
                    result.Risk = model.Risk;
                    result.Status = model.Status;
                    result.Impact = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Impact));
                    result.ProofOfConcept = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.ProofOfConcept));
                    result.Remediation = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Remediation));
                    result.RemediationComplexity = model.RemediationComplexity;
                    result.RemediationPriority = model.RemediationPriority;
                    result.CreatedDate = result.CreatedDate;
                    result.OWASPVector = model.OwaspVector;
                    result.OWASPRisk = model.OwaspRisk;
                    result.OWASPImpact = model.OwaspImpact;
                    result.OWASPLikehood = model.OwaspLikehood;
                    break;
                
            }
            
            vulnManager.Context.SaveChanges();
            TempData["editedVuln"] = "edited";
            _logger.LogInformation("User: {0} edited Vuln Template: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name), id,
                project);

            return RedirectToAction("Templates");
        }
        catch (Exception e)
        {
            TempData["errorEditedTemplate"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred editing a Vuln Workspace on. Task: {0} Project: {1} User: {2}", id,
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("TemplateEdit", new {id= id});
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
                TempData["addedVulnNote"] = "added";
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
            TempData["errorAddNote"] = "Error adding note vuln!";
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
                TempData["deletedVulnNote"] = "deleted";
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
            TempData["errorDeleteVulnNote"] = "Error deleting vuln note!";
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
                TempData["addedVulnAttachment"] = "added";
                _logger.LogInformation("User: {0} Added an attachment: {1} on Vuln: {2}",
                    User.FindFirstValue(ClaimTypes.Name), attachment.Name, form["vulnId"]);
                return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
            }
            else
            {
                TempData["errorVulnAttachment"] = "added";
                _logger.LogError("An error ocurred adding an Attachment on Vuln: {1}. User: {2}",
                    form["vulnId"], User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
            }
        }
        catch (Exception ex)
        {
            TempData["errorVulnAttachment"] = "Error adding attachment to vuln!";
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
                TempData["deletedVulnAttachment"] = "deleted";
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
            TempData["errorDeleteVulnAttachment"] = "Error deleting attachment from vuln!";
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
                TempData["deletedVulnTarget"] = "deleted";
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
            TempData["errorDeleteVulnTarget"] = "Error deleting vuln note!";
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

                foreach (var target in form["TargetId"])
                {

                    var result = vulnTargetManager.GetAll().Where(x =>
                            x.TargetId == Guid.Parse(target) && x.VulnId == Guid.Parse(form["vulnId"]))
                        .FirstOrDefault();
                    if (result != null)
                    {
                        TempData["targetExists"] = "exists";
                        return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
                    }

                    var tar = new VulnTargets
                    {
                        TargetId = Guid.Parse(target),
                        VulnId = Guid.Parse(form["vulnId"]),

                    };

                    vulnTargetManager.Add(tar);
                    vulnTargetManager.Context.SaveChanges();
                    _logger.LogInformation("User: {0} Added new target: {1} on Vuln: {2}",
                        User.FindFirstValue(ClaimTypes.Name), tar.Id, form["vulnId"]);
                }
                
                TempData["addedVulnTarget"] = "added";
                return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
            }
            else
            {
                return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
            }
        }
        catch (Exception ex)
        {
            TempData["errorAddVulnTarget"] = "Error adding note vuln!";
            _logger.LogError(ex, "An error ocurred adding a Note on Vuln: {1}. User: {2}", form["vulnId"],
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
        }
    }

    public IActionResult Import(Guid project)
    {
        var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null)
        {
            TempData["userProject"] = "User is not in the project";
            return RedirectToAction("Index", "Workspaces",new {area =""});
        }
        
        VulnImportViewModel model = new VulnImportViewModel()
        {
            Project = project
        };
        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Import(Guid project,VulnImportViewModel model)
    {
        try
        {
            var user = projectUserManager.VerifyUser(model.Project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                TempData["userProject"] = "User is not in the project";
                return RedirectToAction("Index", "Workspaces", new {area = ""});
            }

            var upload = Request.Form.Files["upload"];
            if (upload != null)
            {

                var file = upload;

                var Inspector = new ContentInspectorBuilder()
                {
                    Definitions = MimeDetective.Definitions.Default.FileTypes.Text.All()
                }.Build();

                var Results = Inspector.Inspect(file.OpenReadStream());

                if (Results.ByFileExtension().Length == 0 && Results.ByMimeType().Length == 0)
                {
                    TempData["fileNotPermitted"] = "User is not in the project";
                    VulnImportViewModel modelError = new VulnImportViewModel()
                    {
                        Project = project
                    };
                    return View("Import", modelError);
                }

                var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Project/" + project + "/");
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

                var attachment = new ProjectAttachment
                {
                    Name = "Vulnerabilities Upload "+model.Type.ToString(),
                    ProjectId = model.Project,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    FilePath = "/Attachments/Project/" + project + "/" + uniqueName
                };

                projectAttachmentManager.Add(attachment);
                projectAttachmentManager.Context.SaveChanges();
                var path = _appEnvironment.WebRootPath+ attachment.FilePath;

                switch (model.Type)
                {
                    case VulnImportType.CSV:
                        csvParser.Parse(project, User.FindFirstValue(ClaimTypes.NameIdentifier),
                                path);
                        TempData["fileImported"] = "file imported";
                        return RedirectToAction("Import", "Vuln", new {project = project});

                    
                }
                
               

            }

            return RedirectToAction("Import", "Vuln", new {project = project});
        }
        catch (Exception e)
        {
            
            TempData["errorImporting"] = "Error deleting service!";
            _logger.LogError(e, "An error ocurred importing Vulns: Project: {0} User: {1}", project,
               User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Import", "Vuln", new {project = project});
        }
    }
}