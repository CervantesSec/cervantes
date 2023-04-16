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
using System.Threading;
using System.Web;
using Cervantes.IFR.Jira;
using Cervantes.IFR.Parsers.Burp;
using Cervantes.IFR.Parsers.CSV;
using Cervantes.IFR.Parsers.Nessus;
using Cervantes.IFR.Parsers.Pwndoc;
using Cervantes.Web.Models;
using DocumentFormat.OpenXml.Math;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeDetective;

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
    private IProjectAttachmentManager projectAttachmentManager = null;
    private ICsvParser csvParser = null;
    private IPwndocParser pwndocParser = null;
    private IBurpParser burpParser = null;
    private INessusParser nessusParser = null;

    public VulnController(IVulnManager vulnManager, IProjectManager projectManager, ILogger<VulnController> logger,
        ITargetManager targetManager, IVulnTargetManager vulnTargetManager,
        IVulnCategoryManager vulnCategoryManager, IVulnNoteManager vulnNoteManager,
        IVulnAttachmentManager vulnAttachmentManager, IHostingEnvironment _appEnvironment, IJIraService jiraService,
        IJiraManager jiraManager, IJiraCommentManager jiraCommentManager, IProjectAttachmentManager projectAttachmentManager,
        ICsvParser csvParser, IPwndocParser pwndocParser, IBurpParser burpParser, INessusParser nessusParser )
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
        this.projectAttachmentManager = projectAttachmentManager;
        this.csvParser = csvParser;
        this.pwndocParser = pwndocParser;
        this.burpParser = burpParser;
        this.nessusParser = nessusParser;
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

                    };
                    return View(model);
                }
                
            }
            else
            {
                var vuln = vulnManager.GetById(id);
                var model = new VulnDetailsViewModel
                {
                    Vuln = vuln,
                    Notes = vulnNoteManager.GetAll().Where(x => x.VulnId == id),
                    Attachments = vulnAttachmentManager.GetAll().Where(x => x.VulnId == id),
                    Targets = vulnTargetManager.GetAll().Where(x => x.VulnId == id).ToList(),
                    JiraEnabled = jiraService.JiraEnabled(),
                };
                return View(model);
            }
            
        }
        catch (Exception e)
        {
            TempData["errorVulnDetails"] = "Error loading vuln!";
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

            var vulnCategories = vulnCategoryManager.GetAll().ToList();

            var model = new VulnCreateViewModel
            {
                Name = vulnResult.Name,
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
                VulnCategories = vulnCategories,
                OwaspVector = vulnResult.OWASPVector,
                OwaspImpact = vulnResult.OWASPImpact,
                OwaspLikehood = vulnResult.OWASPLikehood,
                OwaspRisk = vulnResult.OWASPRisk,
                Projects = projectManager.GetAll().ToList(),
                ProjectId = vulnResult.ProjectId
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorVulnDetails"] = "Error loading vuln!";
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
            sanitizer.AllowedTags.Add("pre");
            sanitizer.KeepChildNodes = true;
            var result = vulnManager.GetById(id);

            result.Name = model.Name;
            result.ProjectId = model.ProjectId;
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
            result.OWASPVector = model.OwaspVector;
            result.OWASPRisk = model.OwaspRisk;
            result.OWASPImpact = model.OwaspImpact;
            result.OWASPLikehood = model.OwaspLikehood;
                            

            vulnManager.Context.SaveChanges();
            TempData["editedVuln"] = "edited";
            _logger.LogInformation("User: {0} edited Vuln: {1}", User.FindFirstValue(ClaimTypes.Name), id);
            if (result.JiraCreated == true)
            {
                var jira = jiraManager.GetByVulnId(result.Id);
                jiraService.UpdateIssue(jira.JiraKey); 
            }

            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["errorEditVuln"] = "Error editing vuln!";
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
            TempData["errorVulnDetails"] = "Error loading vuln!";
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

            TempData["deletedVuln"] = "deleted";
            _logger.LogInformation("User: {0} deleted Vuln: {1}", User.FindFirstValue(ClaimTypes.Name), id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["errorDeletingVuln"] = "Error Creating vuln category!";
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
                Description = sanitizer.Sanitize(HttpUtility.HtmlDecode(model.Description)),
                Type = model.Type
            };
            vulnCategoryManager.Add(cat);
            vulnCategoryManager.Context.SaveChanges();
            TempData["createdVulnCategory"] = "deleted";
            return RedirectToAction("Categories");
        }
        catch (Exception e)
        {
            TempData["errorCreatingVulnCat"] = "Error deleting vulns!";
            _logger.LogError(e, "An error ocurred creating a Vuln category on. User: {0}", 
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("CreateCategory");
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
                Description = cat.Description,
                Type = cat.Type
            };
            return View(model);
        }
        catch (Exception ex)
        {
            TempData["errorVulnDetails"] = "Error deleting vulns!";
            _logger.LogError(ex, "An error ocurred loading Admin Delete Vuln Category. User: {0}, Vuln category: {1}",
                User.FindFirstValue(ClaimTypes.Name), id);
            return RedirectToAction("Index");
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
            result.Type = model.Type;
            vulnCategoryManager.Context.SaveChanges();
            _logger.LogInformation("User: {0} edited Vuln Category: {1}", User.FindFirstValue(ClaimTypes.Name), id);
            TempData["editedVulnCat"] = "Error deleting vulns!";
            return RedirectToAction(nameof(Categories));
        }
        catch (Exception ex)
        {
            TempData["errorEditedVulnCat"] = "Error deleting vulns!";
            _logger.LogError(ex, "An error ocurred loading Admin Delete Vuln Category. User: {0}, Vuln category: {1}",
                User.FindFirstValue(ClaimTypes.Name), id);
            return RedirectToAction("EditCategory", new {id = id});
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
            TempData["errorVulnDetails"] = "Error deleting vulns!";
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
            TempData["errorDeletingVulnCat"] = "Error deleting vulns!";
            _logger.LogError(ex, "An error ocurred loading Admin Delete Vuln Category. User: {0}, Vuln category: {1}",
                User.FindFirstValue(ClaimTypes.Name), id);
            return RedirectToAction("Delete", new{id = id});
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
            TempData["errorAddNoteVuln"] = "Error adding note vuln!";
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
            TempData["errorDeleteVulnAtt"] = "Error deleting attachment from vuln!";
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
            var vulnCategories = vulnCategoryManager.GetAll().ToList();
            
            var model = new VulnCreateViewModel
            {
                Name = vulnResult.Name,
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
                OwaspImpact = vulnResult.OWASPImpact,
                OwaspLikehood = vulnResult.OWASPLikehood,
                OwaspRisk = vulnResult.OWASPRisk,
                OwaspVector = vulnResult.OWASPVector,
                VulnCategories = vulnCategories
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorVulnDetails"] = "Error loading vuln!";
            _logger.LogError(e, "An error ocurred loading Vuln Workspace edit PROJECT form.Project: {0} User: {1}",
                project, User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Templates");
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

                    result.OWASPVector = model.OwaspVector;
                    result.OWASPRisk = model.OwaspRisk;
                    result.OWASPImpact = model.OwaspImpact;
                    result.OWASPLikehood = model.OwaspLikehood;
  
            vulnManager.Context.SaveChanges();
            TempData["editedVulnTemplate"] = "edited";
            _logger.LogInformation("User: {0} edited Vuln Template: {1}", User.FindFirstValue(ClaimTypes.Name), id
                );

            return RedirectToAction("Templates");
        }
        catch (Exception e)
        {
            TempData["errorEditVulnTemplate"] = "Error editing vuln!";
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
            var issue = jiraService.CreateIssue(vulnId, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (issue == false)
            {
                TempData["errorCreateJira"] = "Error editing vuln!";
                _logger.LogError( "An error ocurred creating Jira Issue on Vuln {0} User {1}", form["vulnId"],User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Vuln", new {id = vulnId});
            }
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
    
    public ActionResult Create()
    {
        try
        {

            
            var model = new VulnCreateViewModel
            {
                VulnCategories = vulnCategoryManager.GetAll().ToList(),
                Projects = projectManager.GetAll().ToList()
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["errorVuln"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Task Workspace create form.User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(VulnCreateViewModel model)
    {
        try
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedSchemes.Add("data");

            if (!ModelState.IsValid)
            {

                /*var result2 = vulnCategoryManager.GetAll().Select(e => new VulnCreateViewModel
                {
                    VulnCategoryId = e.Id,
                    VulnCategoryName = e.Name
                }).ToList();

                var vulnCat = new List<SelectListItem>();

                foreach (var item in result2)
                    vulnCat.Add(new SelectListItem {Text = item.VulnCategoryName, Value = item.VulnCategoryId.ToString()});*/
                
                model.VulnCategories = vulnCategoryManager.GetAll().ToList();
                return View(model);
            }

                    var vuln = new Vuln
                    {
                        Name = model.Name,
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
                        OWASPRisk = model.OwaspRisk,
                        OWASPVector = model.OwaspVector,
                        OWASPLikehood = model.OwaspLikehood,
                        OWASPImpact = model.OwaspImpact,
                    };
                    vulnManager.Add(vuln);
                    vulnManager.Context.SaveChanges();

            

                    TempData["addedVuln"] = "added";
                    _logger.LogInformation("User: {0} Created a new Vuln",
                        User.FindFirstValue(ClaimTypes.Name));
                    return RedirectToAction(nameof(Index));

        }
        catch(Exception e)
        {
            TempData["errorAddVuln"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Task Workspace create form.User: {0}", 
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Create");
        }
    }
    
    public IActionResult Import(Guid project)
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Import(VulnImportViewModel model)
    {
        try
        {
            
            var upload = Request.Form.Files["upload"];
            if (upload != null)
            {

                var file = upload;

                var Inspector = new ContentInspectorBuilder()
                {
                    Definitions = MimeDetective.Definitions.Default.FileTypes.Text.All()
                }.Build();

                var Results = Inspector.Inspect(file.OpenReadStream());

                /*if (Results.ByFileExtension().Length == 0 && Results.ByMimeType().Length == 0)
                {
                    TempData["fileNotPermitted"] = "User is not in the project";
                    return RedirectToAction("Import");
                }*/

                var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Imports");
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
                
                var path = Path.Combine(uploads, uniqueName);

                switch (model.Type)
                {
                    case VulnImportType.CSV:
                        csvParser.Parse(null, User.FindFirstValue(ClaimTypes.NameIdentifier),
                                path);
                        TempData["fileImported"] = "file imported";
                        return RedirectToAction("Import", "Vuln");
                    case VulnImportType.Pwndoc:
                        pwndocParser.Parse(null, User.FindFirstValue(ClaimTypes.NameIdentifier),path);
                        TempData["fileImported"] = "file imported";
                        return RedirectToAction("Import", "Vuln");
                    case VulnImportType.Burp:
                        burpParser.Parse(null, User.FindFirstValue(ClaimTypes.NameIdentifier),
                            path);
                        TempData["fileImported"] = "file imported";
                        return RedirectToAction("Import", "Vuln");
                    case VulnImportType.Nessus:
                        nessusParser.Parse(null, User.FindFirstValue(ClaimTypes.NameIdentifier),
                            path);
                        TempData["fileImported"] = "file imported";
                        return RedirectToAction("Import", "Vuln");

                    
                }
                
               

            }

            return RedirectToAction("Import", "Vuln");
        }
        catch (Exception e)
        {
            
            TempData["errorImporting"] = "Error deleting service!";
            _logger.LogError(e, "An error ocurred importing Vulns User: {0}", 
               User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Import", "Vuln");
        }
    }
}