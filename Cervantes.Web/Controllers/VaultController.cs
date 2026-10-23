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
        this.HttpContextAccessor = HttpContextAccessor;
    }
    
    [HttpGet]
    [Route("Project/{id}")]
    [HasPermission(Permissions.VaultRead)]
    public ActionResult<IEnumerable<CORE.Entities.Vault>> GetByProject(Guid id)
    {
        var user = projectUserManager.VerifyUser(id, aspNetUserId);
        if (user == null)
        {
            _logger.LogWarning("Access denied to the vault of project {ProjectId}. User: {UserId}", id, aspNetUserId);
            return StatusCode(403, "You do not have permission to access this project");
        }

        // Wrapped explicitly: C# has no implicit conversion to ActionResult<T> from an interface type
        return new ActionResult<IEnumerable<CORE.Entities.Vault>>(vaultManager.GetAll().Where(x => x.ProjectId == id).ToArray());
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
        // [HasPermission] is not evaluated when a Blazor component calls this action in-process.
        if (!HttpContextAccessor.CurrentUserHas(Permissions.VaultAdd))
        {
            return StatusCode(403, "You do not have permission to perform this action");
        }

        try
        {
            if (ModelState.IsValid)
            {
                var user = projectUserManager.VerifyUser(model.ProjectId, aspNetUserId);
                if (user == null)
                {
                    _logger.LogWarning("Access denied adding a vault entry to project {ProjectId}. User: {UserId}", model.ProjectId, aspNetUserId);
                    return StatusCode(403, "You do not have permission to access this project");
                }
                
                
                var vault = new CORE.Entities.Vault();
                vault.Name = Sanitizer.Sanitize(model.Name);
                vault.Description = Sanitizer.Sanitize(model.Description);
                vault.CreatedDate = DateTime.Now.ToUniversalTime();
                vault.Type = model.Type;
                // Stored exactly as typed: shown as text in the vault dialog, HTML-encoded where it enters report HTML.
                vault.Value = model.Value;
                vault.ProjectId = model.ProjectId;
               vault.UserId = aspNetUserId;
               
                await vaultManager.AddAsync(vault);
                await vaultManager.Context.SaveChangesAsync();
                _logger.LogInformation("Vault added successfully. User: {0}",
                    aspNetUserId);
                return Created($"/api/Vault/{vault.Id}", vault);
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
        // [HasPermission] is not evaluated when a Blazor component calls this action in-process.
        if (!HttpContextAccessor.CurrentUserHas(Permissions.VaultEdit))
        {
            return StatusCode(403, "You do not have permission to edit vault entries");
        }

        try
        {
            if (ModelState.IsValid)
            {
                var vault = vaultManager.GetById(model.Id);
                if (vault == null)
                {
                    return NotFound($"Vault with ID {model.Id} not found");
                }

                // Authorize against the project stored in the database, never the one sent in the request.
                if (projectUserManager.VerifyUser(vault.ProjectId, aspNetUserId) == null)
                {
                    _logger.LogWarning("Access denied editing vault entry {VaultId}. User: {UserId}", vault.Id, aspNetUserId);
                    return StatusCode(403, "Access denied: User does not have permission to access this project");
                }

                // An edit never moves an entry to another project.
                if (model.ProjectId != vault.ProjectId)
                {
                    return BadRequest("A vault entry cannot be moved to another project");
                }

                vault.Name = Sanitizer.Sanitize(model.Name);
                vault.Description = Sanitizer.Sanitize(model.Description);
                vault.Type = model.Type;
                // Stored exactly as typed: shown as text in the vault dialog, HTML-encoded where it enters report HTML.
                vault.Value = model.Value;
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
        // [HasPermission] is not evaluated when a Blazor component calls this action in-process.
        if (!HttpContextAccessor.CurrentUserHas(Permissions.VaultDelete))
        {
            return StatusCode(403, "You do not have permission to delete vault entries");
        }

        try
        {
            var vault = vaultManager.GetById(vaultId);
            if (vault != null)
            {
                var user = projectUserManager.VerifyUser(vault.ProjectId, aspNetUserId);
                if (user == null)
                {
                    _logger.LogWarning("Access denied deleting vault entry {VaultId}. User: {UserId}", vault.Id, aspNetUserId);
                    return StatusCode(403, "You do not have permission to access this project");
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