using Cervantes.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class LogController: ControllerBase
{
    private readonly ILogger<LogController> _logger = null;
    private ILogManager logManager = null;

    public LogController(ILogger<LogController> logger, ILogManager logManager)
    {
        _logger = logger;
        this.logManager = logManager;
    }
    
    [HttpGet]
    public IEnumerable<CORE.Entities.Log> GetAll()
    {
        try
        {
            IEnumerable<CORE.Entities.Log> model = logManager.GetAll().ToArray();
            return model;
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred getting the logs.");
            throw;
        }
        
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteAll()
    {
        try
        {
            var logs =  logManager.GetAll();
            foreach (var log in logs)
            {
                logManager.Remove(log);
            }
            _logger.LogInformation("Logs deleted successfully");
            await logManager.Context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"An error ocurred deleting the logs.");
            throw;
        }
        
    }
    
}