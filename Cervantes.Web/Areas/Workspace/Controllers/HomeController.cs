using Cervantes.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Localization;
using DashboardViewModel = Cervantes.Web.Areas.Workspace.Models.DashboardViewModel;

namespace Cervantes.Web.Areas.Workspace.Controllers
{
    [Area("Workspace")]
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger = null;
        IProjectManager projectManager = null;
        IClientManager clientManager = null;
        IVulnManager vulnManager = null;
        ITaskManager taskManager = null;
        IDocumentManager documentManager = null;
        IProjectUserManager projectUserManager = null;
        ITargetManager targetManager = null;
        IProjectNoteManager projectNoteManager = null;
        IProjectAttachmentManager projectAttachmentManager = null;

        public HomeController(IProjectManager projectManager, IClientManager clientManager, IVulnManager vulnManager, ITaskManager taskManager, IDocumentManager documentManager,
            IProjectUserManager projectUserManager, ITargetManager targetManager, IProjectNoteManager projectNoteManager, IProjectAttachmentManager projectAttachmentManager,
            ILogger<HomeController> logger)
        {
            this.projectManager = projectManager;
            this.clientManager = clientManager;
            this.vulnManager = vulnManager;
            this.taskManager = taskManager;
            this.documentManager = documentManager;
            this.projectUserManager = projectUserManager;
            this.targetManager = targetManager;
            this.projectNoteManager = projectNoteManager;
            this.projectAttachmentManager = projectAttachmentManager;
            _logger = logger;
        }

        public ActionResult Index(int project)
        {
            try
            {
                DashboardViewModel model = new DashboardViewModel
                {
                    Project = projectManager.GetById(project),
                    Members = projectUserManager.GetAll().Where(x => x.ProjectId == project).ToList(),
                    Vulns = vulnManager.GetAll().Where(x => x.ProjectId == project).OrderByDescending(x => x.CreatedDate).Take(5),
                    Tasks = taskManager.GetAll().Where(x => x.AsignedUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.ProjectId == project).Take(5).OrderBy(x => x.StartDate),
                    Targets = targetManager.GetAll().Where(x => x.ProjectId == project).Take(5).OrderByDescending(x => x.Id),
                    Notes = projectNoteManager.GetAll().Where(x => x.ProjectId == project).Take(5).OrderByDescending(x => x.Id),
                    VulnNumber = vulnManager.GetAll().Where(x => x.ProjectId == project).Count(),
                    TasksNumber = taskManager.GetAll().Where(x => x.ProjectId == project).Count(),
                    TargetsNumber = targetManager.GetAll().Where(x => x.ProjectId == project).Count(),
                    MembersNumber = projectUserManager.GetAll().Where(x => x.ProjectId == project).Count(),
                    NotesNumber = projectNoteManager.GetAll().Where(x => x.ProjectId == project).Count(),
                    AttachmentsNumber = projectAttachmentManager.GetAll().Where(x => x.ProjectId == project).Count(),
                    VulnInfo = vulnManager.GetAll().Where(x => x.ProjectId == project && x.Risk == CORE.VulnRisk.Info).Count(),
                    VulnLow = vulnManager.GetAll().Where(x => x.ProjectId == project && x.Risk == CORE.VulnRisk.Low).Count(),
                    VulnMedium = vulnManager.GetAll().Where(x => x.ProjectId == project && x.Risk == CORE.VulnRisk.Medium).Count(),
                    VulnHigh = vulnManager.GetAll().Where(x => x.ProjectId == project && x.Risk == CORE.VulnRisk.High).Count(),
                    VulnCritical = vulnManager.GetAll().Where(x => x.ProjectId == project && x.Risk == CORE.VulnRisk.Critical).Count()
                };
                return View(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error ocurred loading Home Workspace Dashboard. Project: {0} User: {1}", project, User.FindFirstValue(ClaimTypes.Name));
                return View("Error");
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult OnGetSetCultureCookie(string cltr, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cltr)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
        
        

    }
}
