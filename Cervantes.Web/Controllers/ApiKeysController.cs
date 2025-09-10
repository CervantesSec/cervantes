using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using AuthPermissions.AspNetCore;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
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

    public ApiKeysController(IApiKeyManager apiKeyManager)
    {
        _apiKeyManager = apiKeyManager;
    }

    public record CreateApiKeyRequest([Required] string UserId, string? Name, DateTimeOffset? ExpiresAt);
    public record CreateApiKeyResponse(string ApiKey, Guid Id, string Prefix, DateTimeOffset? ExpiresAt);

    // List API keys for a specific user (for internal use or admin APIs)
    [HttpGet("user/{userId}")]
    [Authorize]
    public IEnumerable<ApiKey> GetByUser(string userId)
    {
        return _apiKeyManager.Context.Set<ApiKey>()
            .Where(k => k.UserId == userId)
            .OrderByDescending(k => k.CreatedAt)
            .AsNoTracking()
            .ToArray();
    }

    [HttpPost]
    [HasPermission(Permissions.Admin)]
    public async Task<ActionResult<CreateApiKeyResponse>> Create([FromBody] CreateApiKeyRequest req)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (plaintext, created) = await _apiKeyManager.GenerateAsync(req.UserId, req.Name, req.ExpiresAt);
        return Ok(new CreateApiKeyResponse(plaintext, created.Id, created.KeyPrefix, created.ExpiresAt));
    }

    [HttpPost("{id:guid}/revoke")]
    [HasPermission(Permissions.Admin)]
    public async Task<IActionResult> Revoke(Guid id)
    {
        var ok = await _apiKeyManager.RevokeAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    public record RotateApiKeyRequest(DateTimeOffset? ExpiresAt);

    [HttpPost("{id:guid}/rotate")]
    [HasPermission(Permissions.Admin)]
    public async Task<ActionResult<CreateApiKeyResponse>> Rotate(Guid id, [FromBody] RotateApiKeyRequest? req)
    {
        var rotated = await _apiKeyManager.RotateAsync(id, req?.ExpiresAt);
        if (rotated == null) return NotFound();
        var (plaintext, key) = rotated.Value;
        return Ok(new CreateApiKeyResponse(plaintext, key.Id, key.KeyPrefix, key.ExpiresAt));
    }
}
