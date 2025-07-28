using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Mvc;
using global::AuthPermissions.AspNetCore;
using Cervantes.CORE;
using Microsoft.AspNetCore.Authorization;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditController: ControllerBase
{
    private IAuditManager auditManager = null;
    
    public AuditController(IAuditManager auditManager)
    {
        this.auditManager = auditManager;
    }
    
    [HttpGet]
    [HasPermission(Permissions.LogsRead)]
    public IEnumerable<Audit> Get()
    {
        try
        {
            var model = auditManager.GetAll().OrderByDescending(x => x.Id).Take(1000).ToArray();
            return model;
        }
        catch (Exception e)
        {
            throw;
        }
        
    }
    
    [HttpGet("paged")]
    [HasPermission(Permissions.LogsRead)]
    public IEnumerable<Audit> GetPaged([FromQuery] int page = 0, [FromQuery] int pageSize = 100, [FromQuery] string? search = null)
    {
        try
        {
            var query = auditManager.GetAll().OrderByDescending(x => x.Id);
            
            if (!string.IsNullOrEmpty(search))
            {
                query = (IOrderedQueryable<Audit>)query.Where(x => 
                    x.Type.Contains(search) ||
                    x.UserId.Contains(search) ||
                    x.TableName.Contains(search) ||
                    x.Id.ToString().Contains(search));
            }
            
            var model = query.Skip(page * pageSize).Take(pageSize).ToArray();
            return model;
        }
        catch (Exception e)
        {
            throw;
        }
    }
    
    [HttpGet("count")]
    [HasPermission(Permissions.LogsRead)]
    public int GetCount([FromQuery] string? search = null)
    {
        try
        {
            var query = auditManager.GetAll();
            
            if (!string.IsNullOrEmpty(search))
            {
                query = (IOrderedQueryable<Audit>)query.Where(x => 
                    x.Type.Contains(search) ||
                    x.UserId.Contains(search) ||
                    x.TableName.Contains(search) ||
                    x.Id.ToString().Contains(search));
            }
            
            return query.Count();
        }
        catch (Exception e)
        {
            throw;
        }
    }
    
}