using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;

public class AuditController: ControllerBase
{
    private IAuditManager auditManager = null;
    
    public AuditController(IAuditManager auditManager)
    {
        this.auditManager = auditManager;
    }
    
    [HttpGet]
    public IEnumerable<Audit> Get()
    {
        try
        {
            var model = auditManager.GetAll().ToArray();
            return model;
        }
        catch (Exception e)
        {
            throw;
        }
        
    }
    
}