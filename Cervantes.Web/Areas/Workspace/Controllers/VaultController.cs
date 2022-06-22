using System;
using System.Linq;
using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Areas.Workspace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cervantes.Web.Areas.Workspace.Controllers;
[Authorize(Roles = "Admin,SuperUser,User")]
[Area("Workspace")]
public class VaultController : Controller
{
    private IVaultManager vaultManager = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private readonly ILogger<VaultController> _logger = null;

    public VaultController(IVaultManager vaultManager, IProjectManager projectManager, IProjectUserManager projectUserManager, ILogger<VaultController> logger)
    {
        this.vaultManager = vaultManager;
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        _logger = logger;
    }
    // GET
    public IActionResult Index(int project)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }
            
            var model = new VaultIndexViewModel
            {
                Vaults = vaultManager.GetAll().Where(x => x.ProjectId == project).ToList(),
                Project = projectManager.GetById(project)
            };
            
            return View("Index",model);
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading vault index!";
            _logger.LogError(ex, "An error ocurred loading Vault. Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    public IActionResult Create(int project)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

            VaultViewModel model = new VaultViewModel
            {
                Project = projectManager.GetById(project)
            };
            
            return View("Create",model);
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(ex, "An error ocurred loading Vault create form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View("Index");
        }
    }

    [HttpPost]
    public IActionResult Create(int project,VaultViewModel model)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

            var vault = new Vault
            {
                Name = model.Name,
                Description = model.Description,
                Type = model.Type,
                Value = model.Value,
                ProjectId = project,
                CreatedDate = DateTime.Now.ToUniversalTime(),
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)

            };

            vaultManager.Add(vault);
            vaultManager.Context.SaveChanges();
            TempData["addedTarget"] = "Vault added successfully!";

            _logger.LogInformation("User: {0} added a new target on Project: {1}", User.FindFirstValue(ClaimTypes.Name),
                project);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(ex, "An error ocurred loading Vault creating vault.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View("Index");
        }
    }

    public IActionResult Edit(int project, int id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

            var result = vaultManager.GetById(id);

            var model = new VaultViewModel
            {
                Name = result.Name,
                Description = result.Description,
                Value = result.Value,
                Type = result.Type,
                Project = result.Project,
                ProjectId = result.ProjectId,
                UserId = result.UserId,
                CreatedDate = result.CreatedDate,
            };

            return View("Edit",model);
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(ex, "An error ocurred loading Vault editing form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View("Index");
        }
    }
    
    [HttpPost]
    public IActionResult Edit(int project, int id, VaultViewModel model)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

            var vault = vaultManager.GetById(id);
            vault.Name = model.Name;
            vault.Description = model.Description;
            vault.Type = model.Type;
            vault.Value = model.Value;

            vaultManager.Context.SaveChanges();
            TempData["edited"] = "edited";
            _logger.LogInformation("User: {0} edited Vault: {1} on Project {2}", User.FindFirstValue(ClaimTypes.Name),
                id, project);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(ex, "An error ocurred loading Vault editing form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return RedirectToAction("Index");
        }
    }

    public IActionResult Delete(int project, int id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

            var model = vaultManager.GetById(id);
            

            return View("Delete",model);
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading delete form!";
            _logger.LogError(ex, "An error ocurred loading Vault editing form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View("Index");
        }
    }
    
    [HttpPost]
    public IActionResult Delete(int project, int id,IFormCollection form)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

            var result = vaultManager.GetById(id);
            if (result != null)
            {
                vaultManager.Remove(result);
                vaultManager.Context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading delete form!";
            _logger.LogError(ex, "An error ocurred loading Vault editing form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View("Index");
        }
    }

    public IActionResult Details(int project, int id)
    {
        try
        {
            var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return RedirectToAction("Index", "Workspaces",new {area =""});
            }

            var result = vaultManager.GetById(id);
            var model = new VaultViewModel
            {
                Id = result.Id,
                Name = result.Name,
                Description = result.Description,
                Value = result.Value,
                Type = result.Type,
                Project = result.Project,
                ProjectId = result.ProjectId,
                User = result.User,
                UserId = result.UserId,
                CreatedDate = result.CreatedDate,
            };
            

            return View("Details",model);
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error loading delete form!";
            _logger.LogError(ex, "An error ocurred loading Vault details.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View("Index");
        }
    }
}