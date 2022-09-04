using Cervantes.Contracts;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Cervantes.Web.Controllers;

[Authorize(Roles = "Admin,SuperUser,User")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IStringLocalizer<HomeController> _localizer;
    private IProjectManager projectManager = null;
    private IClientManager clientManager = null;
    private IVulnManager vulnManager = null;
    private ITaskManager taskManager = null;
    private IDocumentManager documentManager = null;
    private IProjectUserManager projectUserManager = null;

    public HomeController(ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer,
        IProjectManager projectManager, IClientManager clientManager, IVulnManager vulnManager,
        ITaskManager taskManager, IDocumentManager documentManager,
        IProjectUserManager projectUserManager)
    {
        _logger = logger;
        _localizer = localizer;
        this.projectManager = projectManager;
        this.clientManager = clientManager;
        this.vulnManager = vulnManager;
        this.taskManager = taskManager;
        this.documentManager = documentManager;
        this.projectUserManager = projectUserManager;
    }

    public IActionResult Index()
    {
        try
        {
            var model = new DashboardViewModel
            {
                ProjectNumber = projectManager.GetAll().Count(),
                VulnNumber = vulnManager.GetAll().Where(x => x.Template == false).Count(),
                ClientNumber = clientManager.GetAll().Count(),
                TasksNumber = taskManager.GetAll().Count(),
                ActiveProjects = projectManager.GetAll().Where(x => x.Status == CORE.ProjectStatus.Active)
                    .OrderByDescending(x => x.EndDate).Take(5),
                RecentClients = clientManager.GetAll()
                    .OrderByDescending(x => x.CreatedDate).Take(5),
                RecentDocuments = documentManager.GetAll()
                    .OrderByDescending(x => x.CreatedDate).Take(5),
                RecentVulns = vulnManager.GetAll().Where(x => x.Template == false)
                    .OrderByDescending(x => x.CreatedDate).ToList().Take(5),
                ProjectPercetagesActive =
                    projectManager.GetAll().Where(x => x.Status == CORE.ProjectStatus.Active).Count(),
                ProjectPercetagesArchived =
                    projectManager.GetAll().Where(x => x.Status == CORE.ProjectStatus.Archived).Count(),
                ProjectPercetagesWaiting =
                    projectManager.GetAll().Where(x => x.Status == CORE.ProjectStatus.Waiting).Count(),
                Open = vulnManager.GetAll().Where(x => x.Status == CORE.VulnStatus.Open).Count(),
                Confirmed = vulnManager.GetAll().Where(x => x.Status == CORE.VulnStatus.Confirmed).Count(),
                Accepted = vulnManager.GetAll().Where(x => x.Status == CORE.VulnStatus.Accepted).Count(),
                Resolved = vulnManager.GetAll().Where(x => x.Status == CORE.VulnStatus.Resolved).Count(),
                OutOfScope = vulnManager.GetAll().Where(x => x.Status == CORE.VulnStatus.OutOfScope).Count(),
                Invalid = vulnManager.GetAll().Where(x => x.Status == CORE.VulnStatus.Invalid).Count()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred loading Dashboard. User: {0}", User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }

    public IActionResult OnGetSetCultureCookie(string cltr, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cltr)),
            new CookieOptions {Expires = DateTimeOffset.UtcNow.AddYears(1)}
        );

        return LocalRedirect(returnUrl);
    }

    /*public IActionResult Workspaces()
    {
        try
        {
            if (User.FindFirstValue(ClaimTypes.Role) == "Admin" | User.FindFirstValue(ClaimTypes.Role) == "SuperUser")
            {
                var model = projectManager.GetAll();
                return View(model);
            }
            else
            {
                var projects = projectUserManager.GetAll().Where(x => x.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).Select(y => y.ProjectId);
                var model = projectManager.GetAll().Where(x => projects.Contains(x.Id));
                return View(model);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred loading MyWorkspaces. User: {0}", User.FindFirstValue(ClaimTypes.Name));
            return View();
        }
    }*/
}