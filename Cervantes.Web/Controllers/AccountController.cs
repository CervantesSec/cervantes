using System.Security.Claims;
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
    
    public AccountController(ILogger<AccountController> logger, SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _signInManager = signInManager;

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
            return NoContent();
        }
        catch
        {
            return Unauthorized();
        }
        
    }
    
}