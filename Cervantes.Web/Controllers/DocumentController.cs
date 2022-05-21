using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace Cervantes.Web.Controllers;
[Authorize(Roles = "Admin,SuperUser,User")]
public class DocumentController : Controller
{
    private readonly ILogger<DocumentController> _logger = null;
    private readonly IHostingEnvironment _appEnvironment;
    private IDocumentManager documentManager = null;

    public DocumentController(IDocumentManager documentManager, ILogger<DocumentController> logger,
        IHostingEnvironment _appEnvironment)
    {
        this.documentManager = documentManager;
        this._appEnvironment = _appEnvironment;
        _logger = logger;
    }

    // GET: DocumentController
    public ActionResult Index()
    {
        try
        {
            var model = documentManager.GetAll().Select(e => new Document
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                FilePath = e.FilePath,
                User = e.User,
                UserId = e.UserId
            });

            if (model != null)
            {
                return View(model);
            }
            else
            {
                TempData["empty"] = "No clients introduced";
                return View();
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading documents!";

            _logger.LogError(ex, "An error ocurred loading Document Index. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    public IActionResult Details(int id)
    {
        try
        {
            var doc = documentManager.GetById(id);
            var model = new Document
            {
                Name = doc.Name,
                Description = doc.Description,
                UserId = doc.UserId,
                User = doc.User,
                FilePath = doc.FilePath,
                Visibility = doc.Visibility,
                CreatedDate = doc.CreatedDate.ToUniversalTime()
            };
            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading document!";

            _logger.LogError(e, "An error ocurred loading Document Details. User: {0}. Document: {1}",
                User.FindFirstValue(ClaimTypes.Name), id);
            return RedirectToAction("Index");
        }
    }

    // GET: DocumentController/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: DocumentController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult Create(DocumentViewModel model, IFormFile Upload)
    {
        try
        {
            var file = Upload;

            if (file != null)
            {
                var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Documents");
                var uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
                using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                var doc = new Document
                {
                    Name = model.Name,
                    Description = model.Description,
                    FilePath = "/Attachments/Documents/" + uniqueName,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Visibility = Visibility.Public
                };
                documentManager.AddAsync(doc);
                documentManager.Context.SaveChanges();
                TempData["created"] = "created";
                _logger.LogInformation("User: {0} Created a new Document: {1}", User.FindFirstValue(ClaimTypes.Name),
                    doc.Name);
                return RedirectToAction("Details", "Document", new {id = doc.Id});
            }
            else
            {
                /*Document doc = new Document
                {
                    Name = model.Name,
                    Description = model.Description,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Visibility = Visibility.Public
                };
                documentManager.AddAsync(doc);
                documentManager.Context.SaveChanges();
                TempData["created"] = "created";
                _logger.LogInformation("User: {0} Created a new Document: {1}", User.FindFirstValue(ClaimTypes.Name), doc.Name);*/
                return View(model);
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error creating document!";

            _logger.LogError(ex, "An error ocurred adding a new Document. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View("Index");
        }
    }

    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult Edit(int id)
    {
        try
        {
            //obtenemos la categoria a editar mediante su id
            var result = documentManager.GetById(id);

            var doc = new Document
            {
                Name = result.Name,
                Description = result.Description
            };
            return View(doc);
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading document!";

            _logger.LogError(ex, "An error ocurred loading edit form on Document Id: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // POST: DocumentController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult Edit(int id, Document model)
    {
        try
        {
            var result = documentManager.GetById(id);
            result.Name = model.Name;
            result.Description = model.Description;

            documentManager.Context.SaveChanges();
            TempData["edited"] = "edited";
            _logger.LogInformation("User: {0} edited Document: {1}", User.FindFirstValue(ClaimTypes.Name), result.Name);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error editing document!";

            _logger.LogError(ex, "An error ocurred editing Document Id: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    // GET: DocumentController/Delete/5
    public ActionResult Delete(int id)
    {
        try
        {
            var doc = documentManager.GetById(id);
            if (doc != null)
            {
                var document = new Document
                {
                    Id = doc.Id,
                    Name = doc.Name,
                    Description = doc.Description
                };

                return View(document);
            }
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading document!";

            _logger.LogError(e, "An error ocurred loading delet form on Document Id: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            Redirect("Error");
        }

        return View();
    }

    // POST: DocumentController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
            var doc = documentManager.GetById(id);
            if (doc != null)
            {
                documentManager.Remove(doc);
                documentManager.Context.SaveChanges();
            }

            TempData["deleted"] = "deleted";
            _logger.LogInformation("User: {0} deleted document: {1}", User.FindFirstValue(ClaimTypes.Name), doc.Name);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error deleting document!";
            _logger.LogError(ex, "An error ocurred deleteing Document Id: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }
}