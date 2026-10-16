using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using AuthPermissions.AspNetCore;
using AuthPermissions.BaseCode.PermissionsCode;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.Email;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cervantes.Web.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyManager _apiKeyManager;
    private readonly IHttpContextAccessor HttpContextAccessor;
    private readonly IEmailService _emailService;
    private readonly string? aspNetUserId;

    public ApiKeysController(IApiKeyManager apiKeyManager, IHttpContextAccessor HttpContextAccessor,
        IEmailService emailService)
    {
        _apiKeyManager = apiKeyManager;
        this.HttpContextAccessor = HttpContextAccessor;
        _emailService = emailService;
        // ControllerBase.User is not populated when Blazor components call this controller in-process,
        // so resolve the current user from the accessor.
        aspNetUserId = HttpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public record CreateApiKeyRequest([Required] string UserId, string? Name, DateTimeOffset? ExpiresAt);
    public record CreateApiKeyResponse(string ApiKey, Guid Id, string Prefix, DateTimeOffset? ExpiresAt);

    private bool IsAdmin() =>
        HttpContextAccessor.HttpContext?.User.HasPermission(Permissions.Admin) == true;

    // A user manages only their own keys; admins manage anyone's.
    private bool CanManage(string ownerUserId) =>
        (aspNetUserId != null && ownerUserId == aspNetUserId) || IsAdmin();

    // List API keys for a specific user (the caller's own keys, or any user's keys for admins)
    [HttpGet("user/{userId}")]
    public ActionResult<IEnumerable<ApiKeyViewModel>> GetByUser(string userId)
    {
        if (!CanManage(userId)) return Forbid();

        var now = DateTimeOffset.UtcNow;
        var keys = _apiKeyManager.Context.Set<ApiKey>()
            .Where(k => k.UserId == userId)
            .OrderByDescending(k => k.CreatedAt)
            .AsNoTracking()
            .Select(k => new ApiKeyViewModel
            {
                Id = k.Id,
                KeyPrefix = k.KeyPrefix,
                Name = k.Name,
                CreatedAt = k.CreatedAt,
                ExpiresAt = k.ExpiresAt,
                RevokedAt = k.RevokedAt,
                LastUsedAt = k.LastUsedAt,
                IsRevoked = k.RevokedAt != null,
                IsExpired = k.ExpiresAt != null && k.ExpiresAt <= now
            })
            .ToArray();
        return Ok(keys);
    }

    [HttpPost]
    [HasPermission(Permissions.Admin)]
    public async Task<ActionResult<CreateApiKeyResponse>> Create([FromBody] CreateApiKeyRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!CanManage(req.UserId)) return Forbid();
        var (plaintext, created) = await _apiKeyManager.GenerateAsync(req.UserId, req.Name, req.ExpiresAt);
        // Tell the key's owner. Metadata only: the plaintext key is never handed to the job.
        if (_emailService.IsEnabled())
        {
            BackgroundJob.Enqueue(() => _emailService.SendApiKeyCreated(
                created.UserId, created.Name, created.KeyPrefix, created.ExpiresAt));
        }
        return Ok(new CreateApiKeyResponse(plaintext, created.Id, created.KeyPrefix, created.ExpiresAt));
    }

    [HttpPost("{id:guid}/revoke")]
    [HasPermission(Permissions.Admin)]
    public async Task<IActionResult> Revoke(Guid id)
    {
        var key = _apiKeyManager.GetById(id);
        if (key == null) return NotFound();
        if (!CanManage(key.UserId)) return Forbid();

        var ok = await _apiKeyManager.RevokeAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    public record RotateApiKeyRequest(DateTimeOffset? ExpiresAt);

    [HttpPost("{id:guid}/rotate")]
    [HasPermission(Permissions.Admin)]
    public async Task<ActionResult<CreateApiKeyResponse>> Rotate(Guid id, [FromBody] RotateApiKeyRequest? req)
    {
        var key = _apiKeyManager.GetById(id);
        if (key == null) return NotFound();
        if (!CanManage(key.UserId)) return Forbid();

        var rotated = await _apiKeyManager.RotateAsync(id, req?.ExpiresAt);
        if (rotated == null) return NotFound();
        var (plaintext, newKey) = rotated.Value;
        // Rotation mints a new key, so the owner gets the same notice. Metadata only.
        if (_emailService.IsEnabled())
        {
            BackgroundJob.Enqueue(() => _emailService.SendApiKeyCreated(
                newKey.UserId, newKey.Name, newKey.KeyPrefix, newKey.ExpiresAt));
        }
        return Ok(new CreateApiKeyResponse(plaintext, newKey.Id, newKey.KeyPrefix, newKey.ExpiresAt));
    }
}
