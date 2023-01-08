using Cervantes.Contracts;
using Cervantes.CORE;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Cervantes.IFR.Jira;
using Cervantes.Web.Models;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cervantes.Web.Controllers;
[Authorize(Roles = "Admin,SuperUser,User")]
public class VulnController : Controller
{
    private IVulnManager vulnManager = null;
    private IProjectManager projectManager = null;
    private ITargetManager targetManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private IVulnNoteManager vulnNoteManager = null;
    private IVulnAttachmentManager vulnAttachmentManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private readonly IHostingEnvironment _appEnvironment;
    private readonly ILogger<VulnController> _logger = null;
    private IJIraService jiraService = null;
    private IJiraManager jiraManager = null;
    private IJiraCommentManager jiraCommentManager = null;

    public VulnController(IVulnManager vulnManager, IProjectManager projectManager, ILogger<VulnController> logger,
        ITargetManager targetManager, IVulnTargetManager vulnTargetManager,
        IVulnCategoryManager vulnCategoryManager, IVulnNoteManager vulnNoteManager,
        IVulnAttachmentManager vulnAttachmentManager, IHostingEnvironment _appEnvironment, IJIraService jiraService,
        IJiraManager jiraManager, IJiraCommentManager jiraCommentManager)
    {
        this.vulnManager = vulnManager;
        this.projectManager = projectManager;
        this.targetManager = targetManager;
        this.vulnCategoryManager = vulnCategoryManager;
        this.vulnAttachmentManager = vulnAttachmentManager;
        this.vulnNoteManager = vulnNoteManager;
        this.vulnTargetManager = vulnTargetManager;
        this._appEnvironment = _appEnvironment;
        _logger = logger;
        this.jiraService = jiraService;
        this.jiraManager = jiraManager;
        this.jiraCommentManager = jiraCommentManager;
    }

    // GET: VulnController
    public ActionResult Index()
    {
        try
        {
            var model = vulnManager.GetAll().ToList();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred loading Vulnerabilities. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    public ActionResult Categories()
    {
        try
        {
            var model = vulnCategoryManager.GetAll();
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred loading Admin Vuln Categories. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }


    // GET: VulnController/Details/5
    public ActionResult Details(Guid id)
    {
        try
        {
            
            var jiraEnabled = jiraService.JiraEnabled();
            if (jiraEnabled == true)
            {
                var vuln = vulnManager.GetById(id);
                var project = projectManager.GetById(vuln.ProjectId);
                
                if (vuln.JiraCreated == true)
                {
                    var jira = jiraManager.GetByVulnId(id);
                    //jiraService.Issue(jira.JiraKey);
                    
                    var model = new VulnDetailsViewModel
                    {
                        Vuln = vuln,
                        Notes = vulnNoteManager.GetAll().Where(x => x.VulnId == id),
                        Attachments = vulnAttachmentManager.GetAll().Where(x => x.VulnId == id),
                        Targets = vulnTargetManager.GetAll().Where(x => x.VulnId == id).ToList(),
                        JiraEnabled = jiraService.JiraEnabled(),
                        Jira = jira,
                        JiraComments = jiraCommentManager.GetAll().Where(x => x.JiraId == jira.Id).ToList(),
                        Project = project
                
                    };
                    return View(model);
                }
                else
                {

                    var model = new VulnDetailsViewModel
                    {
                        Vuln = vuln,
                        Notes = vulnNoteManager.GetAll().Where(x => x.VulnId == id),
                        Attachments = vulnAttachmentManager.GetAll().Where(x => x.VulnId == id),
                        Targets = vulnTargetManager.GetAll().Where(x => x.VulnId == id).ToList(),
                        JiraEnabled = jiraService.JiraEnabled(),
                        Project = project

                    };
                    return View(model);
                }
                
            }
            else
            {
                var vuln = vulnManager.GetById(id);
                var project = projectManager.GetById(vuln.ProjectId);
                var model = new VulnDetailsViewModel
                {
                    Vuln = vuln,
                    Notes = vulnNoteManager.GetAll().Where(x => x.VulnId == id),
                    Attachments = vulnAttachmentManager.GetAll().Where(x => x.VulnId == id),
                    Targets = vulnTargetManager.GetAll().Where(x => x.VulnId == id).ToList(),
                    JiraEnabled = jiraService.JiraEnabled(),
                    Project = project
                };
                return View(model);
            }
            
        }
        catch (Exception e)
        {
            TempData["errorDetails"] = "Error loading vuln!";
            _logger.LogError(e, "An error ocurred loading Vuln Details. Vuln: {0} User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index","Vuln");
        }
    }

    // GET: VulnController/Edit/5
    public ActionResult Edit(Guid id)
    {
        try
        {
            var vulnResult = vulnManager.GetById(id);


            var result = targetManager.GetAll().Where(x => x.ProjectId == vulnResult.ProjectId).Select(e =>
                new VulnCreateViewModel
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
                ProjectId = vulnResult.ProjectId,
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
                OwaspVector = vulnResult.OWASPVector,
                OwaspImpact = vulnResult.OWASPImpact,
                OwaspLikehood = vulnResult.OWASPLikehood,
                OwaspRisk = vulnResult.OWASPRisk,
                Project = projectManager.GetById(vulnResult.ProjectId)
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading vuln!";
            _logger.LogError(e, "An error ocurred loading Vuln edit form. Vuln {0} User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // POST: VulnController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Guid id, VulnCreateViewModel model)
    {
        try
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");

            var result = vulnManager.GetById(id);
            switch (result.Project.Score)
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
            TempData["edited"] = "edited";
            _logger.LogInformation("User: {0} edited Vuln: {1}", User.FindFirstValue(ClaimTypes.Name), id);

            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["error"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred editing a Vuln . Vuln: {0} User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // GET: VulnController/Delete/5
    public ActionResult Delete(Guid id)
    {
        try
        {
            var result = vulnManager.GetById(id);
            return View(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred loading Vuln delete form. Vuln: {0} User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return View("Index");
        }
    }

    // POST: VulnController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(Guid id, IFormCollection collection)
    {
        try
        {
            var result = vulnManager.GetById(id);
            if (result != null)
            {
                vulnManager.Remove(result);
                vulnManager.Context.SaveChanges();
            }

            TempData["deleted"] = "deleted";
            _logger.LogInformation("User: {0} deleted Vuln: {1}", User.FindFirstValue(ClaimTypes.Name), id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["error"] = "Error Creating vuln category!";
            _logger.LogError(e, "An error ocurred creating a Vuln category on. User: {0}", 
                User.FindFirstValue(ClaimTypes.Name));
            return View("Categories");
        }
    }

    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult CreateCategory()
    {
        return View();
    }

    // POST: VulnController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult CreateCategory(VulnCategoryViewModel model)
    {
        try
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");

            VulnCategory cat = new VulnCategory
            {
                Name = model.Name,
                Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description))
            };
            vulnCategoryManager.Add(cat);
            vulnCategoryManager.Context.SaveChanges();
            return RedirectToAction("Categories");
        }
        catch (Exception e)
        {
            TempData["error"] = "Error deleting vulns!";
            _logger.LogError(e, "An error ocurred creating a Vuln category on. User: {0}", 
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // GET: VulnController/Edit/5
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult EditCategory(Guid id)
    {
        try
        {
            var cat = vulnCategoryManager.GetById(id);
            VulnCategoryViewModel model = new VulnCategoryViewModel
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description
            };
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred loading Admin Delete Vuln Category. User: {0}, Vuln category: {1}",
                User.FindFirstValue(ClaimTypes.Name), id);
            return View();
        }
    }

    // POST: VulnController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult EditCategory(Guid id, VulnCategoryViewModel model)
    {
        try
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");

            var result = vulnCategoryManager.GetById(id);
            result.Name = model.Name;
            result.Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description));
            vulnCategoryManager.Context.SaveChanges();
            _logger.LogInformation("User: {0} edited Vuln Category: {1}", User.FindFirstValue(ClaimTypes.Name), id);
            return RedirectToAction(nameof(Categories));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred loading Admin Delete Vuln Category. User: {0}, Vuln category: {1}",
                User.FindFirstValue(ClaimTypes.Name), id);
            return View();
        }
    }

    // GET: VulnController/Delete/5
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult DeleteCategory(Guid id)
    {
        try
        {
            var model = vulnCategoryManager.GetById(id);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred loading Admin Delete Vuln Category. User: {0}, Vuln category: {1}",
                User.FindFirstValue(ClaimTypes.Name), id);
            return View();
        }
    }

    // POST: VulnController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult DeleteCategory(Guid id, IFormCollection collection)
    {
        try
        {
            var result = vulnCategoryManager.GetById(id);

            if (result != null)
            {
                vulnCategoryManager.Remove(result);
                vulnCategoryManager.Context.SaveChanges();
                _logger.LogInformation("User: {0} deleted Delete Vuln Category: {1}",
                    User.FindFirstValue(ClaimTypes.Name), id);
                return RedirectToAction(nameof(Categories));
            }

            return RedirectToAction(nameof(Categories));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred loading Admin Delete Vuln Category. User: {0}, Vuln category: {1}",
                User.FindFirstValue(ClaimTypes.Name), id);
            return View();
        }
    }

    [HttpPost]
    public IActionResult AddNote(IFormCollection form)
    {
        try
        {
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
    public IActionResult AddAttachment(IFormCollection form, IFormFile upload)
    {
        try
        {
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
    
    public ActionResult Templates(Guid project)
    {
        try
        {

            var model = vulnManager.GetAll().Where(x => x.Template == true).ToList();
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
    
     public ActionResult TemplateEdit(Guid project, Guid id)
    {
        try
        {

            var vulnResult = vulnManager.GetById(id);
            ;
            var result2 = vulnCategoryManager.GetAll().Select(e => new VulnCreateViewModel
            {
                VulnCategoryId = e.Id,
                VulnCategoryName = e.Name
            }).ToList();

            var vulnCat = new List<SelectListItem>();

            foreach (var item in result2)
                vulnCat.Add(new SelectListItem {Text = item.VulnCategoryName, Value = item.VulnCategoryId.ToString()});

            var pro = projectManager.GetById(vulnResult.ProjectId);

            var model = new VulnCreateViewModel
            {
                Name = vulnResult.Name,
                Project = pro,
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
                VulnCatList = vulnCat,
                OwaspImpact = vulnResult.OWASPImpact,
                OwaspLikehood = vulnResult.OWASPLikehood,
                OwaspRisk = vulnResult.OWASPRisk,
                OwaspVector = vulnResult.OWASPVector
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
    public ActionResult TemplateEdit(string id,VulnCreateViewModel model)
    {
        try
        {

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");

            var result = vulnManager.GetById(model.Id);
            switch (result.Project.Score)
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
            TempData["edited"] = "edited";
            _logger.LogInformation("User: {0} edited Vuln Template: {1}", User.FindFirstValue(ClaimTypes.Name), id
                );

            return RedirectToAction("Templates");
        }
        catch (Exception e)
        {
            TempData["error"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred editing a Vuln Workspace on. Task: {0} Project: {1} User: {2}", id,
              User.FindFirstValue(ClaimTypes.Name));
            return View("Templates");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateIssueJira(IFormCollection form)
    {
        try
        {
            var vulnId = Guid.Parse(form["vulnId"]);
            jiraService.CreateIssue(vulnId, User.FindFirstValue(ClaimTypes.NameIdentifier));
            var vuln = vulnManager.GetById(vulnId);
            vuln.JiraCreated = true;
            vulnManager.Context.SaveChanges();
            return RedirectToAction("Details", "Vuln", new {id = vulnId});
        }
        catch (Exception e)
        {
            var vuln = vulnManager.GetById(Guid.Parse(form["vulnId"]));
            vuln.JiraCreated = false;
            vulnManager.Context.SaveChanges();
            TempData["errorCreateJira"] = "Error editing vuln!";
            _logger.LogError(e, "An error ocurred creating Jira Issue on Vuln {0} User {1}", form["vulnId"],
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Details", "Vuln", new {id = form["vulnId"]});
        }
        

    }
}