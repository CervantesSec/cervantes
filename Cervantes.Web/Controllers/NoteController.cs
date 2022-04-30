using Cervantes.Contracts;
using Cervantes.CORE;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace Cervantes.Web.Controllers
{
    public class NoteController : Controller
    {

        private readonly ILogger<NoteController> _logger = null;
        private readonly IHostingEnvironment _appEnvironment;
        INoteManager noteManager = null;
        public NoteController(INoteManager noteManager, ILogger<NoteController> logger)
        {
            this.noteManager = noteManager;
            _logger = logger;
        }

        // GET: DocumentController
        public ActionResult Index()
        {
            try
            {

                var model = noteManager.GetAll().Select(e => new CORE.Note
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    User = e.User,
                    UserId = e.UserId,
                    CreatedDate = e.CreatedDate.ToUniversalTime()

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
                TempData["error"] = "Error obtaining notes!";
                _logger.LogError(ex, "An error ocurred loading Note Index. User: {0}", User.FindFirstValue(ClaimTypes.Name));
                return View();
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
        public ActionResult Create(Note model, IFormFile upload)
        {
            try
            {

                Note note = new Note
                {
                    Name = model.Name,
                    Description = model.Description,
                    CreatedDate = DateTime.Now.ToUniversalTime(),
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                noteManager.AddAsync(note);
                noteManager.Context.SaveChanges();
                TempData["created"] = "created";
                _logger.LogInformation("User: {0} Created a new Note: {1}", User.FindFirstValue(ClaimTypes.Name), note.Name);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error loading note create form";
                _logger.LogError(ex, "An error ocurred adding a new Note. User: {0}", User.FindFirstValue(ClaimTypes.Name));
                return View("Index");

            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                //obtenemos la categoria a editar mediante su id
                var result = noteManager.GetById(id);

                Note note = new Note
                {
                    Name = result.Name,
                    Description = result.Description,
                };
                return View(note);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error loading edit form!";
                _logger.LogError(ex, "An error ocurred loading edit form on Note Id: {0}. User: {1}", id, User.FindFirstValue(ClaimTypes.Name));
                return View();

            }
        }

        // POST: DocumentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Document model)
        {
            try
            {
                var result = noteManager.GetById(id);
                if (result.UserId == User.FindFirstValue(ClaimTypes.Name))
                {
                    result.Name = model.Name;
                    result.Description = model.Description;

                    noteManager.Context.SaveChanges();
                    TempData["edited"] = "edited";
                    _logger.LogInformation("User: {0} edited Note: {1}", User.FindFirstValue(ClaimTypes.Name), result.Name);
                    return RedirectToAction("Details", new {id = result.Id});
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error editing note!";
                _logger.LogError(ex, "An error ocurred editing Note Id: {0}. User: {1}", id, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        // GET: DocumentController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var note = noteManager.GetById(id);
                if (note != null)
                {
                    Note note1 = new Note
                    {
                        Id = note.Id,
                        Name = note.Name,
                        Description = note.Description,
                        CreatedDate = note.CreatedDate,
                    };

                    return View(note1);
                }
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading note!";
                _logger.LogError(e, "An error ocurred loading delet form on Note Id: {0}. User: {1}", id, User.FindFirstValue(ClaimTypes.Name));
                Redirect("Error");
            }

            return View();
        }

        // POST: DocumentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var note = noteManager.GetById(id);
                if (note != null)
                {
                    if (note.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    {
                        noteManager.Remove(note);
                        noteManager.Context.SaveChanges();

                        TempData["deleted"] = "deleted";
                        _logger.LogInformation("User: {0} deleted Note: {1}", User.FindFirstValue(ClaimTypes.Name), note.Name);
                        return RedirectToAction("Index");
                    }

                }

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["error"] = "Error deleting note!";
                _logger.LogError(ex, "An error ocurred deleteing Note Id: {0}. User: {1}", id, User.FindFirstValue(ClaimTypes.Name));
                return View();
            }
        }

        public ActionResult Details(int id)
        {
            try
            {
                var result = noteManager.GetById(id);

                if (result != null)
                {
                    if (result.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    {
                        return View("Details", result);
                    }

                    return View("Index");
                }
                return View("Index");
            }
            catch (Exception e)
            {
                TempData["error"] = "Error loading note!";
                _logger.LogError(e, "An error ocurred obtaining Note Id: {0}. User: {1}", id, User.FindFirstValue(ClaimTypes.Name));
                return View("Index");
            }
            
        }
    }
}

