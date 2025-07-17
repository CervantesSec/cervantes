using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger = null;
    private IHttpContextAccessor HttpContextAccessor;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private string aspNetUserId;
    private IAuditManager auditManager;
    
    public AccountController(ILogger<AccountController> logger, SignInManager<ApplicationUser> signInManager, IAuditManager auditManager)
    {
        _logger = logger;
        _signInManager = signInManager;
        this.auditManager = auditManager;
    }
    
    [HttpPost]
    [AllowAnonymous]
    [Route("Login")]
    public async Task<IActionResult> Login(LoginParameters parameters)
    {
        var result = await _signInManager.PasswordSignInAsync(parameters.UserName, parameters.Password, false, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in. User: {0}", parameters.UserName);
            var audit = new Audit
            {
                UserId = HttpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                Type = "Login",
                TableName = "ApplicationUser",
                DateTime = DateTime.Now.ToUniversalTime(),
                OldValues = "",
                NewValues = "",
                AffectedColumns = "",
                PrimaryKey = HttpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                IpAddress = HttpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Browser = HttpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString(),
                
            };
            await auditManager.AddAsync(audit);
            await auditManager.Context.SaveChangesAsync();
            return Ok();
        }
        else
        {
            return BadRequest("Invalid request");
        }
    }

    [Authorize]
    [HttpPost]
    [Route("Logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();
            var audit = new Audit
            {
                UserId = HttpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                Type = "Logout",
                TableName = "ApplicationUser",
                DateTime = DateTime.Now.ToUniversalTime(),
                OldValues = "",
                NewValues = "",
                AffectedColumns = "",
                PrimaryKey = HttpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                IpAddress = HttpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString(),
                Browser = HttpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString(),
                
            };
            await auditManager.AddAsync(audit);
            await auditManager.Context.SaveChangesAsync();
            return NoContent();
        }
        catch
        {
            return Unauthorized();
        }
        
    }
    
}