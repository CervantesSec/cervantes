using System.Security.Claims;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;

public class SearchController: ControllerBase
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
    private string aspNetUserId;
    private IHttpContextAccessor HttpContextAccessor;

    public SearchController(IUserManager userManager, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> usrManager,IClientManager clientManager, IProjectManager projectManager,
        IProjectUserManager projectUserManager, IProjectNoteManager projectNoteManager, IDocumentManager documentManager,
        IReportManager reportManager, ITargetManager targetManager, ITargetServicesManager targetServicesManager, ITaskManager taskManager,IVaultManager vaultManager,
        IVulnManager vulnManager, IVulnCategoryManager vulnCategoryManager, ILogger<SearchController> logger,IHttpContextAccessor HttpContextAccessor)
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
    }

    public SearchViewModel Search(string search)
    {
        try
        {
            if (search != null)
            {
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedSchemes.Add("data");
                search = sanitizer.Sanitize(HttpUtility.HtmlDecode(search));
                var model = new SearchViewModel
                {
                    Users = userManager.GetAll().Where(x => x.FullName.Contains(search) || x.Email.Contains(search) ||
                                                            x.Position.Contains(search) ||
                                                            x.Description.Contains(search)).ToList(),
                    Clients = clientManager.GetAll().Where(x =>
                        x.Name.Contains(search) || x.Description.Contains(search) ||
                        x.ContactName.Contains(search) || x.ContactEmail.Contains(search)).ToList(),
                    Documents = documentManager.GetAll()
                        .Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList(),
                    Projects = projectManager.GetAll()
                        .Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList(),
                    Reports = reportManager.GetAll()
                        .Where(x => x.Name.Contains(search) || x.Description.Contains(search)),
                    Targets = targetManager.GetAll()
                        .Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList(),
                    TargetServices = targetServicesManager.GetAll()
                        .Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList(),
                    Tasks = taskManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search))
                        .ToList(),
                    Vaults = vaultManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search))
                        .ToList(),
                    Vulns = vulnManager.GetAll().Where(x =>
                        x.Name.Contains(search) || x.Description.Contains(search) || x.Impact.Contains(search) ||
                        x.Remediation.Contains(search)),
                    VulnCategories = vulnCategoryManager.GetAll().Where(x => x.Name.Contains(search)).ToList(),

                };

                return model;

            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "An error ocurred using search form. User:");
            throw;
        }

    }
}