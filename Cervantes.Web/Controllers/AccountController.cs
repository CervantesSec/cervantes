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
    private readonly SignInManager<ApplicationUser> _signInManager;
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
            // HttpContext.User is still the principal the request came in with, not the user who just signed in.
            var user = await _signInManager.UserManager.FindByNameAsync(parameters.UserName);
            await auditManager.AddAsync(BuildAudit("Login", user?.Id));
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
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _signInManager.SignOutAsync();
        await auditManager.AddAsync(BuildAudit("Logout", userId));
        await auditManager.Context.SaveChangesAsync();
        return NoContent();
    }

    private Audit BuildAudit(string type, string? userId) => new Audit
    {
        UserId = userId,
        Type = type,
        TableName = "ApplicationUser",
        DateTime = DateTime.UtcNow,
        OldValues = "",
        NewValues = "",
        AffectedColumns = "",
        PrimaryKey = userId,
        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
        Browser = HttpContext.Request.Headers["User-Agent"].ToString(),
    };
}
