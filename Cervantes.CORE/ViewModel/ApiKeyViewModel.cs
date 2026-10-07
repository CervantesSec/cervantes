namespace Cervantes.CORE.ViewModel;

public class ApiKeyViewModel
{
    public Guid Id { get; set; }
    public string KeyPrefix { get; set; } = string.Empty;
    public string? Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset? LastUsedAt { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsExpired { get; set; }
}
