using System;
using System.Linq;
using System.Security.Claims;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Cervantes.Contracts;
using Cervantes.CORE;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Cervantes.Web.Controllers;

public class SearchController : Controller
{
     private readonly ILogger<SearchController> _logger = null;
    private IClientManager clientManager = null;
    private IUserManager userManager = null;
    private Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager = null;
    private IProjectManager projectManager = null;
    private IDocumentManager documentManager = null;
    private IReportManager reportManager = null;
    private ITargetManager targetManager = null;
    private ITargetServicesManager targetServicesManager = null;
    private ITaskManager taskManager = null;
    private IVaultManager vaultManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private IVulnManager vulnManager = null;
    private readonly IHostingEnvironment _appEnvironment;

    public SearchController(IUserManager userManager, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> usrManager,IClientManager clientManager, IProjectManager projectManager,
        IProjectUserManager projectUserManager, IProjectNoteManager projectNoteManager, IDocumentManager documentManager,
        IReportManager reportManager, ITargetManager targetManager, ITargetServicesManager targetServicesManager, ITaskManager taskManager,IVaultManager vaultManager,
        IVulnManager vulnManager, IVulnCategoryManager vulnCategoryManager, ILogger<SearchController> logger, IHostingEnvironment _appEnvironment)
    {
        this.userManager = userManager;
        _userManager = usrManager;
        this.clientManager = clientManager;
        this.projectManager = projectManager;
        this.documentManager = documentManager;
        this.reportManager = reportManager;
        this.targetManager = targetManager;
        this.targetServicesManager = targetServicesManager;
        this.taskManager = taskManager;
        this.vaultManager = vaultManager;
        this.vulnCategoryManager = vulnCategoryManager;
        this.vulnManager = vulnManager;
        _logger = logger;
        this._appEnvironment = _appEnvironment;

    }
    // GET
    public IActionResult Index(SearchViewModel model)
    {
        if (model != null)
        {
            return View(model);
        }
        else
        {
            return View();
        }
        
    }

    [HttpPost]
    public IActionResult Search(IFormCollection form)
    {
        try
        {
            string search = form["search"];
            if (search != null)
            {
                var model = new SearchViewModel
                {
                    Search = search,
                    Users = userManager.GetAll().Where(x => x.FullName.Contains(search) || x.Email.Contains(search) ||
                                                            x.Position.Contains(search) || x.Description.Contains(search)).ToList(),
                    Clients = clientManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search) ||
                                                                x.ContactName.Contains(search) || x.ContactEmail.Contains(search)).ToList(),
                    Documents = documentManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList(),
                    Projects = projectManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList(),
                    Reports = reportManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)),
                    Targets = targetManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList(),
                    TargetServices = targetServicesManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList(),
                    Tasks = taskManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList(),
                    Vaults = vaultManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList(),
                    Vulns = vulnManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search) || x.Impact.Contains(search) || x.Remediation.Contains(search)),
                    VulnCategories = vulnCategoryManager.GetAll().Where(x => x.Name.Contains(search)).ToList(),
                    
                };

                return View("Index", model);

            }
            else
            {
                return View("Index");
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = "Error doing search!";

            _logger.LogError(ex, "An error ocurred using search form. User: {1}", 
                User.FindFirstValue(ClaimTypes.Name));
            return View("Index");
        }
        
    }
}