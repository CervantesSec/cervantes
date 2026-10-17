using System.Security.Claims;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Helpers;
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
    private IProjectUserManager projectUserManager = null;
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
        this.projectUserManager = projectUserManager;
        _logger = logger;
        this.HttpContextAccessor = HttpContextAccessor;
        // MainLayout also renders for anonymous users, so the user may be missing here.
        aspNetUserId = HttpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
                // Search has no HTTP surface, so its body is the only gate: return only modules the user can read.
                bool Can(Permissions permission) => HttpContextAccessor.CurrentUserHas(permission);
                // The Vault module is limited to project members, so only search vault entries
                // of the projects the current user belongs to.
                var memberProjectIds = Can(Permissions.VaultRead)
                    ? projectUserManager.GetAll().Where(x => x.UserId == aspNetUserId).Select(x => x.ProjectId).ToList()
                    : new List<Guid>();
                var model = new SearchViewModel
                {
                    Users = Can(Permissions.UsersRead)
                        ? userManager.GetAll().Where(x => x.FullName.Contains(search) || x.Email.Contains(search) ||
                                                          x.Position.Contains(search) ||
                                                          x.Description.Contains(search)).ToList()
                        : Enumerable.Empty<ApplicationUser>(),
                    Clients = Can(Permissions.ClientsRead)
                        ? clientManager.GetAll().Where(x =>
                            x.Name.Contains(search) || x.Description.Contains(search) ||
                            x.ContactName.Contains(search) || x.ContactEmail.Contains(search)).ToList()
                        : Enumerable.Empty<Client>(),
                    Documents = Can(Permissions.DocumentsRead)
                        ? documentManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList()
                        : Enumerable.Empty<Document>(),
                    Projects = Can(Permissions.ProjectsRead)
                        ? projectManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList()
                        : Enumerable.Empty<Project>(),
                    Reports = Can(Permissions.ReportsRead)
                        ? reportManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search))
                        : Enumerable.Empty<Report>(),
                    Targets = Can(Permissions.TargetsRead)
                        ? targetManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList()
                        : Enumerable.Empty<Target>(),
                    TargetServices = Can(Permissions.TargetsServicesRead)
                        ? targetServicesManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList()
                        : Enumerable.Empty<TargetServices>(),
                    Tasks = Can(Permissions.TasksRead)
                        ? taskManager.GetAll().Where(x => x.Name.Contains(search) || x.Description.Contains(search)).ToList()
                        : Enumerable.Empty<CORE.Entities.Task>(),
                    Vaults = Can(Permissions.VaultRead)
                        ? vaultManager.GetAll().Where(x => memberProjectIds.Contains(x.ProjectId) &&
                                                           (x.Name.Contains(search) || x.Description.Contains(search))).ToList()
                        : Enumerable.Empty<Vault>(),
                    Vulns = Can(Permissions.VulnsRead)
                        ? vulnManager.GetAll().Where(x =>
                            x.Name.Contains(search) || x.Description.Contains(search) || x.Impact.Contains(search) ||
                            x.Remediation.Contains(search))
                        : Enumerable.Empty<Vuln>(),
                    VulnCategories = Can(Permissions.VulnCategoriesRead)
                        ? vulnCategoryManager.GetAll().Where(x => x.Name.Contains(search)).ToList()
                        : Enumerable.Empty<VulnCategory>(),
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