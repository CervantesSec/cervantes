using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web.Controllers;

public class WorkspacesController : ControllerBase
{
    private readonly ILogger<WorkspacesController> _logger = null;
    private IProjectManager projectManager = null;
    private IProjectUserManager projectUserManager = null;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private string aspNetUserRole;
    public WorkspacesController(IProjectManager projectManager, IProjectUserManager projectUserManager,
        ILogger<WorkspacesController> logger,IHttpContextAccessor HttpContextAccessor)
    {
        this.projectManager = projectManager;
        this.projectUserManager = projectUserManager;
        _logger = logger;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //aspNetUserRole = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;

    }
    
    [HttpGet]
    public IEnumerable<CORE.Entities.Project> Get()
    {
        try
        {
         
                var projects = projectUserManager.GetAll().Where(x => x.UserId == aspNetUserId).Select(y => y.ProjectId).ToList();
                var model = projectManager.GetAll().Where(x => projects.Contains(x.Id) && x.Status == ProjectStatus.Active).Include(x => x.Client).ToList();
                return model;
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred getting user workspaces. User: {0}",
                aspNetUserId);
            throw;
        }
       
    }
    
}