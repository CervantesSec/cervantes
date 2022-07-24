using Cervantes.Contracts;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Cervantes.CORE;
using Microsoft.AspNetCore.Authorization;

namespace Cervantes.Web.Controllers;
[Authorize(Roles = "Admin")]
public class OrganizationController : Controller
{
    private readonly ILogger<OrganizationController> _logger = null;
    private readonly IHostingEnvironment _appEnvironment;
    private IOrganizationManager organizationManager = null;

    /// <summary>
    /// Organization Controller Constructor
    /// </summary>
    public OrganizationController(IOrganizationManager organizationManager, IHostingEnvironment _appEnvironment,
        ILogger<OrganizationController> logger)
    {
        this.organizationManager = organizationManager;
        this._appEnvironment = _appEnvironment;
        _logger = logger;
    }

    /// <summary>
    /// Method show Organization Information
    /// </summary>
    /// <returns></returns>
    public ActionResult Index()
    {
        try
        {
            var org = organizationManager.GetAll().First();

            if (org != null)
            {
                var model = new Organization
                {
                    Id = org.Id,
                    Name = org.Name,
                    Description = org.Description,
                    ContactEmail = org.ContactEmail,
                    ContactName = org.ContactName,
                    ContactPhone = org.ContactPhone,
                    Url = org.Url
                };
                return View(model);
            }

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred loading Organization Index. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }


    /// <summary>
    /// Method save organization form
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model">OrganizationViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Index(Organization model)
    {
        try
        {
            var result = organizationManager.GetAll().First();
            result.Name = model.Name;
            result.Description = model.Description;
            result.ContactEmail = model.ContactEmail;
            result.ContactName = model.ContactName;
            result.ContactPhone = model.ContactPhone;
            result.Url = model.Url;
            if (Request.Form.Files["upload"] != null)
            {
                var file = Request.Form.Files["upload"];
                var uploads = Path.Combine(_appEnvironment.WebRootPath, "Attachments/Images/Avatars");
                var uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
                using (var fileStream = new FileStream(Path.Combine(uploads, uniqueName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                result.ImagePath = "/Attachments/Images/Organization/" + uniqueName;
            }


            organizationManager.Context.SaveChanges();
            TempData["edited"] = "edited";
            _logger.LogInformation("User: {0} Edited Organization", User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred saving editing Organization form. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    /// <summary>
    /// Method delete organization logo
    /// </summary>
    /// <param name="id">Organization Id</param>
    /// <returns></returns>
    public ActionResult DeleteLogo(int id)
    {
        try
        {
            var org = organizationManager.GetAll().First();
            var pathFile = _appEnvironment.WebRootPath + org.ImagePath;
            if (System.IO.File.Exists(pathFile)) System.IO.File.Delete(pathFile);


            org.ImagePath = null;
            organizationManager.Context.SaveChanges();

            TempData["avatar_deleted"] = "avatar deleted";
            _logger.LogInformation("User: {0} Organization Logo Deleted", User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index", "Organization", new {id = id});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred deleting Organization Logo. User: {0}",
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index", "Organization", new {id = id});
        }
    }
}