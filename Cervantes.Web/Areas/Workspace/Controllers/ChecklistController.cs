using System;
using System.Linq;
using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.Web.Areas.Workspace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cervantes.Web.Areas.Workspace.Controllers;

[Authorize(Roles = "Admin,SuperUser,User")]
[Area("Workspace")]
public class ChecklistController : Controller
{
    private readonly ILogger<ChecklistController> _logger = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private ITargetManager targetManager = null;
    private IWSTGManager wstgManager = null;
    
    public ChecklistController(IProjectManager projectManager, 
        IProjectUserManager projectUserManager, ITargetManager targetManager, ILogger<ChecklistController> logger, IWSTGManager wstgManager)
    {
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        this.targetManager = targetManager;
        _logger = logger;
        this.wstgManager = wstgManager;
    }
    // GET
    public IActionResult Index(Guid project)
    {
        var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null)
        {
            TempData["userProject"] = "User is not in the project";
            return RedirectToAction("Index", "Workspaces",new {area =""});
        }

        ChecklistViewModel model = new ChecklistViewModel
        {
            WSTG = wstgManager.GetAll().Where(x => x.ProjectId == project),
            Project = projectManager.GetById(project)

        };
        
        return View(model);
    }

    public IActionResult Wstg(Guid project)
    {
        var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null)
        {
            TempData["userProject"] = "User is not in the project";
            return RedirectToAction("Index", "Workspaces",new {area =""});
        }
        return View();
    }
}