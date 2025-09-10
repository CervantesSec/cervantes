using System;
using System.ComponentModel.DataAnnotations;

namespace Cervantes.CORE.Entities;

public class ApiKey
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string KeyHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(24)]
    public string KeyPrefix { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Name { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset? LastUsedAt { get; set; }

    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTimeOffset.UtcNow;
}

