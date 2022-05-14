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

public class ClientController : Controller
{
    private readonly ILogger<ClientController> _logger = null;
    private readonly IHostingEnvironment _appEnvironment;
    private IClientManager clientManager = null;
    private IProjectManager projectManager = null;
    private IUserManager userManager = null;

    /// <summary>
    /// Client Controller Constructor
    /// </summary>
    public ClientController(IClientManager clientManager, IUserManager userManager, IProjectManager projectManager,
        IHostingEnvironment _appEnvironment, ILogger<ClientController> logger)
    {
        this.clientManager = clientManager;
        this.projectManager = projectManager;
        this.userManager = userManager;
        this._appEnvironment = _appEnvironment;
        _logger = logger;
    }


    /// <summary>
    /// Method Index shows all clients
    /// </summary>
    /// <returns>All Clients</returns>
    public ActionResult Index()
    {
        try
        {
            var model = clientManager.GetAll().Select(e => new Client
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                ContactEmail = e.ContactEmail,
                ContactName = e.ContactName,
                ContactPhone = e.ContactPhone,
                Url = e.Url
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
            TempData["error"] = "Error loading clients!";

            _logger.LogError(ex, "An error ocurred loading Client Index. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    /// <summary>
    /// Show Details of a client
    /// </summary>
    /// <param name="id">Client Id</param>
    /// <returns>Client</returns>
    public ActionResult Details(int id)
    {
        try
        {
            var client = clientManager.GetById(id);
            if (client != null)
            {
                var model = new ClientDetailsViewModel
                {
                    Client = client,
                    Project = projectManager.GetAll().Where(x => x.ClientId == client.Id)
                };
                return View(model);
            }
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading client!";

            _logger.LogError(e, "An error ocurred loading Client Details. Client: {0} User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            Redirect("Error");
        }

        return View();
    }

    /// <summary>
    /// Method Create show creation form
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Method Save Post Create form
    /// </summary>
    /// <param name="model">ClientViewModel</param>
    /// <param name="upload">File</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult Create(ClientViewModel model, IFormFile upload)
    {
        try
        {
            var file = Request.Form.Files["upload"];
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Clients");
            var uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
            using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            var client = new Client
            {
                Name = model.Name,
                Description = model.Description,
                ContactPhone = model.ContactPhone,
                ContactName = model.ContactName,
                ContactEmail = model.ContactEmail,
                Url = model.Url,
                ImagePath = "/Attachments/Images/Clients/" + uniqueName,
                CreatedDate = DateTime.UtcNow,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            clientManager.AddAsync(client);
            clientManager.Context.SaveChanges();
            TempData["created"] = "created";
            _logger.LogInformation("User: {0} Created a new client: {1}", User.FindFirstValue(ClaimTypes.Name),
                client.Name);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error creating client!";

            _logger.LogError(ex, "An error ocurred adding a new Client. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    /// <summary>
    /// Methos shwo edit form
    /// </summary>
    /// <param name="id">Client Id</param>
    /// <returns></returns>
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult Edit(int id)
    {
        try
        {
            //obtenemos la categoria a editar mediante su id
            var result = clientManager.GetById(id);

            var client = new ClientViewModel()
            {
                Name = result.Name,
                Description = result.Description,
                ContactEmail = result.ContactEmail,
                ContactName = result.ContactName,
                ContactPhone = result.ContactPhone,
                Url = result.Url
            };
            return View(client);
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading client!";

            _logger.LogError(ex, "An error ocurred loading edit form on Client Id: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    /// <summary>
    /// Method save post edit form
    /// </summary>
    /// <param name="id">Client Id</param>
    /// <param name="model">Client</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult Edit(int id, ClientViewModel model)
    {
        try
        {
            var result = clientManager.GetById(id);
            result.Name = model.Name;
            result.Description = model.Description;
            result.ContactEmail = model.ContactEmail;
            result.ContactName = model.ContactName;
            result.ContactPhone = model.ContactPhone;
            result.Url = model.Url;

            clientManager.Context.SaveChanges();
            TempData["edited"] = "edited";
            _logger.LogInformation("User: {0} edited client: {1}", User.FindFirstValue(ClaimTypes.Name), result.Name);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error editing client!";

            _logger.LogError(ex, "An error ocurred editing Client Id: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    /// <summary>
    /// Methos show delete page
    /// </summary>
    /// <param name="id">Client Id</param>
    /// <returns></returns>
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult Delete(int id)
    {
        try
        {
            var client = clientManager.GetById(id);
            if (client != null)
            {
                var model = new Client
                {
                    Id = client.Id,
                    Name = client.Name,
                    Description = client.Description,
                    ContactEmail = client.ContactEmail,
                    ContactName = client.ContactName,
                    ContactPhone = client.ContactPhone,
                    Url = client.Url
                };

                return View(model);
            }
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading client!";

            _logger.LogError(e, "An error ocurred loading delet form on Client Id: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            Redirect("Error");
        }

        return View();
    }

    /// <summary>
    /// Method confirms remove client
    /// </summary>
    /// <param name="id">Client Id</param>
    /// <param name="collection"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperUser")]
    public ActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
            var client = clientManager.GetById(id);
            if (client != null)
            {
                clientManager.Remove(client);
                clientManager.Context.SaveChanges();
            }

            TempData["deleted"] = "deleted";
            _logger.LogInformation("User: {0} deleted client: {1}", User.FindFirstValue(ClaimTypes.Name), client.Name);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error deleting client!";

            _logger.LogError(ex, "An error ocurred deleteing Client Id: {0}. User: {1}", id,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }
}