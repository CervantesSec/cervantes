﻿using Cervantes.Contracts;
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
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cervantes.Web.Controllers
{
    public class VulnController : Controller
    {

        IVulnManager vulnManager = null;
        IProjectManager projectManager = null;
        ITargetManager targetManager = null;
        IVulnCategoryManager vulnCategoryManager = null;
        IVulnNoteManager vulnNoteManager = null;
        IVulnAttachmentManager vulnAttachmentManager = null;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly ILogger<VulnController> _logger = null;

        public VulnController(IVulnManager vulnManager, IProjectManager projectManager, ILogger<VulnController> logger, ITargetManager targetManager,
            IVulnCategoryManager vulnCategoryManager, IVulnNoteManager vulnNoteManager, IVulnAttachmentManager vulnAttachmentManager, IHostingEnvironment _appEnvironment)
        {
            this.vulnManager = vulnManager;
            this.projectManager = projectManager;
            this.targetManager = targetManager;
            this.vulnCategoryManager = vulnCategoryManager;
            this.vulnAttachmentManager = vulnAttachmentManager;
            this.vulnNoteManager = vulnNoteManager;
            this._appEnvironment = _appEnvironment;
            _logger = logger;
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
                _logger.LogError(ex, "An error ocurred loading Vulnerabilities. User: {0}", User.FindFirstValue(ClaimTypes.Name));
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
                _logger.LogError(ex, "An error ocurred loading Admin Vuln Categories. User: {0}", User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }


        // GET: VulnController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                VulnDetailsViewModel model = new VulnDetailsViewModel
                {
                    Vuln = vulnManager.GetById(id),
                    Notes = vulnNoteManager.GetAll().Where(x => x.VulnId == id),
                    Attachments = vulnAttachmentManager.GetAll().Where(x => x.VulnId == id)
                };
                return View(model);
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading vuln!";
                _logger.LogError(e, "An error ocurred loading Vuln Details. Vuln: {0} User: {1}", id,  User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }
        
        // GET: VulnController/Edit/5
        public ActionResult Edit(int id)
        {
             try
            {
                var vulnResult = vulnManager.GetById(id);


                var result = targetManager.GetAll().Where(x => x.ProjectId == vulnResult.ProjectId).Select(e =>
                    new VulnCreateViewModel
                {
                    TargetId = e.Id,
                    TargetName = e.Name,
                }).ToList();

                var targets = new List<SelectListItem>();

                foreach (var item in result)
                {
                    targets.Add(new SelectListItem { Text = item.TargetName, Value = item.TargetId.ToString() });
                }

                var result2 = vulnCategoryManager.GetAll().Select(e => new VulnCreateViewModel
                {
                    VulnCategoryId = e.Id,
                    VulnCategoryName = e.Name,
                }).ToList();

                var vulnCat = new List<SelectListItem>();

                foreach (var item in result2)
                {
                    vulnCat.Add(new SelectListItem { Text = item.VulnCategoryName, Value = item.VulnCategoryId.ToString() });
                }


                VulnCreateViewModel model = new VulnCreateViewModel
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
                _logger.LogError(e, "An error ocurred loading Vuln edit form. Vuln {0} User: {1}", id, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        // POST: VulnController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, VulnCreateViewModel model)
        {
            try
            {
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


                vulnManager.Context.SaveChanges();
                TempData["edited"] = "edited";
                _logger.LogInformation("User: {0} edited Vuln: {1}", User.FindFirstValue(ClaimTypes.Name), id);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["error"] = "Error editing vuln!";
                _logger.LogError(e, "An error ocurred editing a Vuln . Vuln: {0} User: {1}", id,  User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        // GET: VulnController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var result = vulnManager.GetById(id);
                return View(result);


            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error ocurred loading Vuln delete form. Vuln: {0} User: {1}",  id,User.FindFirstValue(ClaimTypes.Name));
                return View("Index");
            }
        }

        // POST: VulnController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
                TempData["error"] = "Error deleting vulns!";
                _logger.LogError(e, "An error ocurred deleting a Vuln Workspace on. Vuln: {0}  User: {1}", id,  User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }


        public ActionResult CreateCategory()
        {
            return View();
        }

        // POST: VulnController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VulnController/Edit/5
        public ActionResult EditCategory(int id)
        {
            try
            {
                var model = vulnCategoryManager.GetById(id);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error ocurred loading Admin Delete Vuln Category. User: {0}, Vuln category: {1}", User.FindFirstValue(ClaimTypes.Name), id);
                return View();
            }
        }

        // POST: VulnController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(int id, VulnCategory model)
        {
            try
            {
                var result = vulnCategoryManager.GetById(id);
                result.Name = model.Name;
                result.Description = model.Description;
                vulnCategoryManager.Context.SaveChanges();
                _logger.LogInformation("User: {0} edited Vuln Category: {1}", User.FindFirstValue(ClaimTypes.Name), id);
                return RedirectToAction(nameof(Categories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error ocurred loading Admin Delete Vuln Category. User: {0}, Vuln category: {1}", User.FindFirstValue(ClaimTypes.Name), id);
                return View();
            }
        }

        // GET: VulnController/Delete/5
        public ActionResult DeleteCategory(int id)
        {

            try
            {
                var model = vulnCategoryManager.GetById(id);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error ocurred loading Admin Delete Vuln Category. User: {0}, Vuln category: {1}", User.FindFirstValue(ClaimTypes.Name), id);
                return View();
            }
        }

        // POST: VulnController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategory(int id, IFormCollection collection)
        {
            try
            {
                var result = vulnCategoryManager.GetById(id);

                if (result != null)
                {
                    vulnCategoryManager.Remove(result);
                    vulnCategoryManager.Context.SaveChanges();
                    _logger.LogInformation("User: {0} deleted Delete Vuln Category: {1}", User.FindFirstValue(ClaimTypes.Name), id);
                    return RedirectToAction(nameof(Categories));
                }

                return RedirectToAction(nameof(Categories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error ocurred loading Admin Delete Vuln Category. User: {0}, Vuln category: {1}", User.FindFirstValue(ClaimTypes.Name), id);
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

                    VulnNote note = new VulnNote
                    {
                        Name = form["noteName"],
                        Description = form["noteDescription"],
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        Visibility = (Visibility)Enum.ToObject(typeof(Visibility), Int32.Parse(form["Visibility"])),
                        VulnId = Int32.Parse(form["vulnId"])

                    };

                    vulnNoteManager.Add(note);
                    vulnNoteManager.Context.SaveChanges();
                    TempData["addedNote"] = "added";
                    _logger.LogInformation("User: {0} Added new note: {1} on Vuln: {2}", User.FindFirstValue(ClaimTypes.Name), note.Name, Int32.Parse(form["vulnId"]));
                    return RedirectToAction("Details", "Vuln", new { id = Int32.Parse(form["vulnId"]) });
                }
                else
                {
                    return RedirectToAction("Details", "Vuln", new { id = Int32.Parse(form["vulnId"]) });
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error adding note vuln!";
                _logger.LogError(ex, "An error ocurred adding a Note on Vuln: {1}. User: {2}", Int32.Parse(form["vulnId"]), User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Vuln", new { id = Int32.Parse(form["vulnId"]) });
            }

        }


        [HttpPost]
        public IActionResult DeleteNote(int id, int project, int vuln)
        {
            try
            {
                if (id != 0)
                {
                    var result = vulnNoteManager.GetById(id);

                    vulnNoteManager.Remove(result);
                    vulnNoteManager.Context.SaveChanges();
                    TempData["deletedNote"] = "deleted";
                    _logger.LogInformation("User: {0} Deleted note: {1} on Vuln: {2}", User.FindFirstValue(ClaimTypes.Name), result.Name, vuln);
                    return RedirectToAction("Details", "Vuln", new { id = result.VulnId });
                }
                else
                {
                    return RedirectToAction("Details", "Vuln", new { id = vuln });
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error deleting vuln note!";
                _logger.LogError(ex, "An error ocurred deleting a Note on Project: {1}. User: {2}", project, User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Vuln", new { id = vuln });

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



                    VulnAttachment attachment = new VulnAttachment
                    {
                        Name = form["attachmentName"],
                        VulnId = Int32.Parse(form["vulnId"]),
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        FilePath = "/Attachments/Project/" + form["vulnId"] + "/" + uniqueName,

                    };

                    vulnAttachmentManager.Add(attachment);
                    vulnAttachmentManager.Context.SaveChanges();
                    TempData["addedAttachment"] = "added";
                    _logger.LogInformation("User: {0} Added an attachment: {1} on Vuln: {2}", User.FindFirstValue(ClaimTypes.Name), attachment.Name, Int32.Parse(form["vulnId"]));
                    return RedirectToAction("Details", "Vuln", new { id = Int32.Parse(form["vulnId"]) });
                }
                else
                {
                    TempData["errorAttachment"] = "added";
                    _logger.LogError("An error ocurred adding an Attachment on Vuln: {1}. User: {2}", Int32.Parse(form["vulnId"]), User.FindFirstValue(ClaimTypes.Name));
                    return RedirectToAction("Details", "Vuln", new { id = Int32.Parse(form["vulnId"]) });
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error adding attachment to vuln!";
                _logger.LogError(ex, "An error ocurred adding an Attachment on Vuln: {1}. User: {2}", Int32.Parse(form["project"]), User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Vuln", new { id = Int32.Parse(form["vulnId"]) });
            }

        }

        [HttpPost]
        public IActionResult DeleteAttachment(int id, int project, int vuln)
        {
            try
            {
                if (id != 0)
                {
                    var result = vulnAttachmentManager.GetById(id);

                    var pathFile = _appEnvironment.WebRootPath + result.FilePath;
                    if (System.IO.File.Exists(pathFile))
                    {
                        System.IO.File.Delete(pathFile);
                    }

                    vulnAttachmentManager.Remove(result);
                    vulnAttachmentManager.Context.SaveChanges();
                    TempData["deletedAttachment"] = "deleted";
                    _logger.LogInformation("User: {0} Deleted an attachment: {1} on Vuln: {2}", User.FindFirstValue(ClaimTypes.Name), result.Name, vuln);
                    return RedirectToAction("Details", "Vuln", new { id = vuln });
                }
                else
                {
                    _logger.LogError("An error ocurred deleting an Attachment on Vuln: {1}. User: {2}", vuln, User.FindFirstValue(ClaimTypes.Name));
                    return RedirectToAction("Details", "Vuln", new { id = vuln });
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error deleting attachment from vuln!";
                _logger.LogError(ex, "An error ocurred deleting an Attachment on Vuln: {1}. User: {2}", vuln, User.FindFirstValue(ClaimTypes.Name));
                return RedirectToAction("Details", "Vuln", new { id = vuln });


            }

        }
    }
}
