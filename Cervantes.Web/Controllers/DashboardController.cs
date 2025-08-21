using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using TaskStatus = Cervantes.CORE.Entities.TaskStatus;

namespace Cervantes.Web.Controllers;

[Authorize]
public class DashboardController: ControllerBase
{
    private readonly ILogger<DashboardController> _logger;
    private IProjectManager projectManager = null;
    private IClientManager clientManager = null;
    private IVulnManager vulnManager = null;
    private ITaskManager taskManager = null;
    private IDocumentManager documentManager = null;
    private IUserManager userManager = null;
    private IReportManager reportManager = null;
    private INoteManager noteManager = null;

    public DashboardController(ILogger<DashboardController> logger,
        IProjectManager projectManager, IClientManager clientManager, IVulnManager vulnManager,
        ITaskManager taskManager, IDocumentManager documentManager,
        IUserManager userManager, IReportManager reportManager, INoteManager noteManager)
    {
        _logger = logger;
        this.projectManager = projectManager;
        this.clientManager = clientManager;
        this.vulnManager = vulnManager;
        this.taskManager = taskManager;
        this.documentManager = documentManager;
        this.userManager = userManager;
        this.reportManager = reportManager;
        this.noteManager = noteManager;
    }
    
    public async Task<IActionResult> Get()
    {
        var model = new DashboardViewModel();
        model.Projects = projectManager.GetAll().Where(x => x.Status == CORE.Entities.ProjectStatus.Active).Include(x => x.Client)
            .OrderByDescending(x => x.EndDate).Take(5).ToList();
        model.Clients = clientManager.GetAll()
            .OrderByDescending(x => x.CreatedDate).Take(5).ToList();
        model.Vulns = vulnManager.GetAll().Where(x => x.Template == false)
            .OrderByDescending(x => x.CreatedDate).ToList().Take(5).ToList();
        model.Documents = documentManager.GetAll()
            .OrderByDescending(x => x.CreatedDate).Take(5).ToList();
        
        model.TotalProjects = projectManager.GetAll().Count(x => x.Template == false);
        model.TotalClients = clientManager.GetAll().Count();
        model.TotalVulns = vulnManager.GetAll().Count();
        model.TotalDocuments = documentManager.GetAll().Count();
        model.TotalTasks = taskManager.GetAll().Count();
        model.TotalUsers = userManager.GetAll().Count();
        model.TotalReports = reportManager.GetAll().Count();
        model.TotalNotes = noteManager.GetAll().Count();
        
        model.ProjectWaiting = projectManager.GetAll().Count(x => x.Status == ProjectStatus.Waiting);
        model.ProjectActive = projectManager.GetAll().Count(x => x.Status == ProjectStatus.Active);
        model.ProjectArchived = projectManager.GetAll().Count(x => x.Status == ProjectStatus.Archived);
        
        model.VulnsOpen = vulnManager.GetAll().Count(x => x.Status == VulnStatus.Open);
        model.VulnsConfirmed = vulnManager.GetAll().Count(x => x.Status == VulnStatus.Confirmed);
        model.VulnsAccepted = vulnManager.GetAll().Count(x => x.Status == VulnStatus.Accepted);
        model.VulnsResolved = vulnManager.GetAll().Count(x => x.Status == VulnStatus.Resolved);
        model.VulnsOut = vulnManager.GetAll().Count(x => x.Status == VulnStatus.OutOfScope);
        model.VulnsInvalid = vulnManager.GetAll().Count(x => x.Status == VulnStatus.Invalid);
        
        model.VulnsInfo = vulnManager.GetAll().Count(x => x.Risk == VulnRisk.Info);
        model.VulnsLow = vulnManager.GetAll().Count(x => x.Risk == VulnRisk.Low);
        model.VulnsMedium = vulnManager.GetAll().Count(x => x.Risk == VulnRisk.Medium);
        model.VulnsHigh = vulnManager.GetAll().Count(x => x.Risk == VulnRisk.High);
        model.VulnsCritical = vulnManager.GetAll().Count(x => x.Risk == VulnRisk.Critical);

        model.TasksBacklog = taskManager.GetAll().Count(x => x.Status == TaskStatus.Backlog);
        model.TasksToDo = taskManager.GetAll().Count(x => x.Status == TaskStatus.ToDo);
        model.TasksInProgress = taskManager.GetAll().Count(x => x.Status == TaskStatus.InProgress);
        model.TasksBlocked = taskManager.GetAll().Count(x => x.Status == TaskStatus.Blocked);
        model.TasksDone = taskManager.GetAll().Count(x => x.Status == TaskStatus.Done);
        
        
        return Ok(model);
    }
}