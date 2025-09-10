using System.Security.Cryptography;
using System.Text;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cervantes.Application;

public class ApiKeyManager : GenericManager<ApiKey>, IApiKeyManager
{
    private readonly ApiKeyOptions _options;

    public ApiKeyManager(IApplicationDbContext context, IOptions<ApiKeyOptions> options) : base(context)
    {
        _options = options.Value;
    }

    public async Task<(string Plaintext, ApiKey Created)> GenerateAsync(string userId, string? name = null, DateTimeOffset? expiresAt = null)
    {
        var prefix = GeneratePrefix(8);
        var secret = GenerateSecret(32);
        var plaintext = $"ck_{prefix}.{secret}";
        var hash = ComputeHash(secret);

        var entity = new ApiKey
        {
            Id = Guid.NewGuid(),
            KeyHash = hash,
            KeyPrefix = prefix,
            Name = name,
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = (expiresAt?.ToUniversalTime()) ?? DateTimeOffset.UtcNow.AddDays(_options.DefaultExpiryDays)
        };

        await Context.Set<ApiKey>().AddAsync(entity);
        await Context.SaveChangesAsync();
        return (plaintext, entity);
    }

    public async Task<ApiKeyValidationResult> ValidateAsync(string rawKey)
    {
        if (string.IsNullOrWhiteSpace(rawKey))
            return new ApiKeyValidationResult(false, null, null, "Empty");

        if (!TryParse(rawKey, out var prefix, out var secret))
            return new ApiKeyValidationResult(false, null, null, "InvalidFormat");

        var hash = ComputeHash(secret);
        var key = await Context.Set<ApiKey>()
            .FirstOrDefaultAsync(k => k.KeyHash == hash);

        if (key == null)
            return new ApiKeyValidationResult(false, null, null, "NotFound");
        if (key.IsRevoked)
            return new ApiKeyValidationResult(false, null, key, "Revoked");
        if (key.IsExpired)
            return new ApiKeyValidationResult(false, null, key, "Expired");

        key.LastUsedAt = DateTimeOffset.UtcNow;
        await Context.SaveChangesAsync();
        return new ApiKeyValidationResult(true, key.UserId, key, null);
    }

    public async Task<bool> RevokeAsync(Guid id)
    {
        var key = await Context.Set<ApiKey>().FirstOrDefaultAsync(k => k.Id == id);
        if (key == null) return false;
        if (key.IsRevoked) return true;
        key.RevokedAt = DateTimeOffset.UtcNow;
        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<(string Plaintext, ApiKey Rotated)?> RotateAsync(Guid id, DateTimeOffset? newExpiresAt = null)
    {
        var oldKey = await Context.Set<ApiKey>().FirstOrDefaultAsync(k => k.Id == id);
        if (oldKey == null) return null;

        // Revoke the old key
        if (!oldKey.IsRevoked)
            oldKey.RevokedAt = DateTimeOffset.UtcNow;

        // Create a new key for the same user (rotate)
        var prefix = GeneratePrefix(8);
        var secret = GenerateSecret(32);
        var plaintext = $"ck_{prefix}.{secret}";
        var newKey = new ApiKey
        {
            Id = Guid.NewGuid(),
            KeyPrefix = prefix,
            KeyHash = ComputeHash(secret),
            Name = oldKey.Name,
            UserId = oldKey.UserId,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = (newExpiresAt?.ToUniversalTime()) ?? DateTimeOffset.UtcNow.AddDays(_options.DefaultExpiryDays),
            RevokedAt = null,
            LastUsedAt = null
        };

        await Context.Set<ApiKey>().AddAsync(newKey);
        await Context.SaveChangesAsync();
        return (plaintext, newKey);
    }

    private string GeneratePrefix(int length)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        return ToBase32(bytes).ToLowerInvariant().Substring(0, length);
    }

    private string GenerateSecret(int length)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        return ToBase64Url(bytes);
    }

    private string ComputeHash(string secret)
    {
        if (string.IsNullOrEmpty(_options.Pepper))
        {
            // Fallback to SHA256 without pepper (dev only)
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(secret)));
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.Pepper));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(secret)));
    }

    private static bool TryParse(string rawKey, out string prefix, out string secret)
    {
        prefix = secret = string.Empty;
        var trimmed = rawKey.Trim();
        if (trimmed.StartsWith("ck_", StringComparison.OrdinalIgnoreCase))
        {
            var idx = trimmed.IndexOf('.')
;            if (idx > 3 && idx < trimmed.Length - 1)
            {
                prefix = trimmed.Substring(3, idx - 3);
                secret = trimmed[(idx + 1)..];
                return true;
            }
        }
        return false;
    }

    private static string ToBase64Url(byte[] data)
    {
        return Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string ToBase32(byte[] data)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var output = new StringBuilder();
        int bitBuffer = 0, bitBufferLength = 0;
        foreach (var b in data)
        {
            bitBuffer = (bitBuffer << 8) | b;
            bitBufferLength += 8;
            while (bitBufferLength >= 5)
            {
                int index = (bitBuffer >> (bitBufferLength - 5)) & 0x1F;
                bitBufferLength -= 5;
                output.Append(alphabet[index]);
            }
        }
        if (bitBufferLength > 0)
        {
            int index = (bitBuffer << (5 - bitBufferLength)) & 0x1F;
            output.Append(alphabet[index]);
        }
        return output.ToString();
    }
}
