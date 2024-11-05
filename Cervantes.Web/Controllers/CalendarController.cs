using System.Security.Claims;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;
[Authorize]
public class CalendarController: ControllerBase
{
    private readonly ILogger<CalendarController> _logger = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private ITaskManager taskManager = null;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;

    /// <summary>
    /// Calendar Controller Constructor
    /// </summary>
    public CalendarController( IProjectManager projectManager, ITaskManager taskManager,IProjectUserManager projectUserManager, 
        ILogger<CalendarController> logger,IHttpContextAccessor HttpContextAccessor)
    {
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        this.taskManager = taskManager;
        _logger = logger;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

    }
    
    [HasPermission(Permissions.CalendarRead)]
    public CalendarViewModel Get()
    {
        try
        {
            var model = new CalendarViewModel();
            var ids = projectUserManager.GetAll().Where(x => x.UserId == aspNetUserId).Select(x => x.ProjectId).ToList();
            model.Projects = projectManager.GetAll().Where(x => ids.Contains(x.Id)).ToList();
            model.Tasks = taskManager.GetAll().Where(x => x.AsignedUserId == aspNetUserId).ToList();
        
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred getting the calendar information. User: {0}",
                aspNetUserId);
            throw;
        }
        
    }
}