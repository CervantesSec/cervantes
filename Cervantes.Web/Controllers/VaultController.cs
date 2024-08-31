using System.Security.Claims;
using System.Web;
using Cervantes.Contracts;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize (Roles = "Admin,SuperUser,User")]
public class VaultController : ControllerBase
{
    private IVaultManager vaultManager = null;
    private readonly IWebHostEnvironment env;
    private readonly ILogger<VaultController> _logger = null;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private IProjectUserManager projectUserManager;
    private Sanitizer Sanitizer;

    public VaultController(IVaultManager vaultManager,
        IWebHostEnvironment env, ILogger<VaultController> logger,IHttpContextAccessor HttpContextAccessor, 
        IProjectUserManager projectUserManager,Sanitizer Sanitizer)
    {
        this.vaultManager = vaultManager;
        this.env = env;
        _logger = logger;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.projectUserManager = projectUserManager;
        this.Sanitizer = Sanitizer;
    }
    
    [HttpGet]
    [Route("Project/{id}")]
    public IEnumerable<CORE.Entities.Vault> GetByProject(Guid id)
    {
        IEnumerable<CORE.Entities.Vault> model = vaultManager.GetAll().Where(x => x.ProjectId == id).ToArray();
        if (model != null)
        {
            var user = projectUserManager.VerifyUser(id, aspNetUserId);
            if (user == null)
            {
                return null;
            }
            
            return model;

        }
        return null;
    }
    
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] VaultCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return BadRequest();
                }
                
                
                var vault = new CORE.Entities.Vault();
                vault.Name = Sanitizer.Sanitize(model.Name);
                vault.Description = Sanitizer.Sanitize(model.Description);
                vault.CreatedDate = DateTime.Now.ToUniversalTime();
                vault.Type = model.Type;
                vault.Value = Sanitizer.Sanitize(model.Value);
                vault.ProjectId = model.ProjectId;
               vault.UserId = aspNetUserId;
               
                await vaultManager.AddAsync(vault);
                await vaultManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vault added successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }
            else
            {
                _logger.LogError("An error ocurred adding a Vault. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Vault. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> Edit([FromBody] VaultEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return BadRequest();
                }

                var vault = vaultManager.GetById(model.Id);
                vault.Name = Sanitizer.Sanitize(model.Name);
                vault.Description = Sanitizer.Sanitize(model.Description);
                vault.Type = model.Type;
                vault.Value = Sanitizer.Sanitize(model.Value);
                vault.ProjectId = model.ProjectId;
                await vaultManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vault edited successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }
            else
            {
                _logger.LogError("An error ocurred editing a Vault. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a Vault. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
    [HttpDelete]
    [Route("{vaultId}")]
    public async Task<IActionResult> Delete(Guid vaultId)
    {
        try
        {
            var vault = vaultManager.GetById(vaultId);
            if (vault != null)
            {
                var user = projectUserManager.VerifyUser(vault.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return BadRequest();
                }
                
                vaultManager.Remove(vault);
                await vaultManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vault deleted successfully. User: {0}",
                    aspNetUserId);
                return Ok();
            }
            else
            {
                _logger.LogError("An error ocurred deleting a Vault. User: {0}",
                    aspNetUserId);
                return BadRequest();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting a Vault. User: {0}",
                aspNetUserId);
            return BadRequest();
        }
    }
    
}