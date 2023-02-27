using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.Web.Areas.Workspace.Models;
using Cervantes.Web.Areas.Workspace.Models.Wstg;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            WSTG = wstgManager.GetAll().Where(x => x.ProjectId == project).ToList(),
            Project = projectManager.GetById(project)

        };
        
        return View(model);
    }

    public IActionResult Wstg(Guid project, Guid id)
    {
        var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null)
        {
            TempData["userProject"] = "User is not in the project";
            return RedirectToAction("Index", "Workspaces",new {area =""});
        }

        var wstg = wstgManager.GetTargetId(project,id);
        WSTGInfo info = new WSTGInfo
        {
            Info01Note = wstg.Info01Note,
            Info01Status = wstg.Info01Status,
            Info02Note = wstg.Info02Note,
            Info02Status = wstg.Info02Status,
            Info03Note = wstg.Info03Note,
            Info03Status = wstg.Info03Status,
            Info04Note = wstg.Info04Note,
            Info04Status = wstg.Info04Status,
            Info05Note = wstg.Info05Note,
            Info05Status = wstg.Info05Status,
            Info06Note = wstg.Info06Note,
            Info06Status = wstg.Info06Status,
            Info07Note = wstg.Info07Note,
            Info07Status = wstg.Info07Status,
            Info08Note = wstg.Info08Note,
            Info08Status = wstg.Info08Status,
            Info09Note = wstg.Info09Note,
            Info09Status = wstg.Info09Status,
            Info10Note = wstg.Info10Note,
            Info10Status = wstg.Info10Status,
        };

        WSTGViewModel model = new WSTGViewModel
        {
            Project = projectManager.GetById(project),
            Info = info
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Wstg(Guid project, WSTGViewModel model)
    {
        var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null)
        {
            TempData["userProject"] = "User is not in the project";
            return RedirectToAction("Index", "Workspaces",new {area =""});
        }

        return RedirectToAction("Index");
    }

    public IActionResult Create(Guid project)
    {
        var user = projectUserManager.VerifyUser(project, User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (user == null)
        {
            TempData["userProject"] = "User is not in the project";
            return RedirectToAction("Index", "Workspaces",new {area =""});
        }

        try
        {
            var result = targetManager.GetAll().Where(x => x.ProjectId == project).Select(e => new VulnCreateViewModel
            {
                TargetId = e.Id,
                TargetName = e.Name
            }).ToList();

            var targets = new List<SelectListItem>();

            foreach (var item in result)
                targets.Add(new SelectListItem {Text = item.TargetName, Value = item.TargetId.ToString()});
            
            var model = new ChecklistCreateViewModel
            {
                TargetList = targets,
                Project = projectManager.GetById(project),
            };

            return View(model);
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Checklist Workspace create form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Guid project, ChecklistCreateViewModel model)
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

            switch (model.Type)
            {
                case ChecklistType.OWASPWSTG:
                    WSTG wstg = new WSTG
                    {
                        TargetId = model.TargetId,
                        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        ProjectId = project,
                        CreatedDate = DateTime.Now.ToUniversalTime()

                    };
                    wstgManager.Add(wstg);
                    wstgManager.Context.SaveChanges();
                    TempData["addedCheck"] = "Checklist added successfully!";

                    _logger.LogInformation("User: {0} added a new checklist on Project: {1}", User.FindFirstValue(ClaimTypes.Name),
                        project);
                    return RedirectToAction(nameof(Index));
                    break;
                case ChecklistType.OWASPMASVS:
                    break;
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["error"] = "Error loading create form!";
            _logger.LogError(e, "An error ocurred loading Checklist Workspace create form.Project: {0} User: {1}", project,
                User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }
}