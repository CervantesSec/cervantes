using System;
using System.Linq;
using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cervantes.Web.Controllers;

public class CalendarController : Controller
{
    private readonly ILogger<ClientController> _logger = null;
    private IProjectManager projectManager = null;
    private ITaskManager taskManager = null;

    /// <summary>
    /// Calendar Controller Constructor
    /// </summary>
    public CalendarController( IProjectManager projectManager, ITaskManager taskManager, ILogger<ClientController> logger)
    {
        this.projectManager = projectManager;
        this.taskManager = taskManager;
        _logger = logger;
    }
    
    public IActionResult Index()
    {
        var projects = projectManager.GetAll();
        CalendarViewModel model = new CalendarViewModel
        {
            Projects = projects,
            Tasks = taskManager.GetAll().Where(x => x.AsignedUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
        };
        return View(model);
    }
}