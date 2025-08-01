using System.Security.Claims;
using System.Web;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Helpers;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    [HasPermission(Permissions.VaultRead)]
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
    
    [NonAction]
    public CORE.Entities.Vault GetByVaultId(Guid vaultId)
    {
        return vaultManager.GetById(vaultId);
    }
    
    [HttpPost]
    [HasPermission(Permissions.VaultAdd)]
    public async Task<IActionResult> Add([FromBody] VaultCreateViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return BadRequest("Access denied: User does not have permission to access this project");
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
                return CreatedAtAction(nameof(GetByVaultId), new { vaultId = vault.Id }, vault);
            }
            else
            {
                _logger.LogError("Validation failed when adding a Vault. User: {0}",
                    aspNetUserId);
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(new { message = "Validation failed", errors });
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred adding a Vault. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while creating the vault entry. Please try again later.");
        }
    }
    
    [HttpPut]
    [HasPermission(Permissions.VaultEdit)]
    public async Task<IActionResult> Edit([FromBody] VaultEditViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.ProjectId, aspNetUserId);
                if (user == null)
                {
                    return BadRequest("Access denied: User does not have permission to access this project");
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
                return NoContent();
            }
            else
            {
                _logger.LogError("Validation failed when editing a Vault. User: {0}",
                    aspNetUserId);
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(new { message = "Validation failed", errors });
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred editing a Vault. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while updating the vault entry. Please try again later.");
        }
    }
    
    [HttpDelete]
    [Route("{vaultId}")]
    [HasPermission(Permissions.VaultDelete)]
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
                    return BadRequest("Access denied: User does not have permission to access this project");
                }
                
                vaultManager.Remove(vault);
                await vaultManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vault deleted successfully. User: {0}",
                    aspNetUserId);
                return NoContent();
            }
            else
            {
                _logger.LogError("Vault not found. User: {0}",
                    aspNetUserId);
                return NotFound($"Vault with ID {vaultId} not found");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error ocurred deleting a Vault. User: {0}",
                aspNetUserId);
            return StatusCode(500, "An error occurred while deleting the vault entry. Please try again later.");
        }
    }
    
}