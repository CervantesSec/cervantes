using System;
using System.Threading.Tasks;
using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

public record ApiKeyValidationResult(bool Success, string? UserId, ApiKey? ApiKey, string? FailureReason);

public interface IApiKeyManager : IGenericManager<ApiKey>
{
    Task<(string Plaintext, ApiKey Created)> GenerateAsync(string userId, string? name = null, DateTimeOffset? expiresAt = null);
    Task<ApiKeyValidationResult> ValidateAsync(string rawKey);
    Task<bool> RevokeAsync(Guid id);
    Task<(string Plaintext, ApiKey Rotated)?> RotateAsync(Guid id, DateTimeOffset? newExpiresAt = null);
}

