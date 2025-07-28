namespace Cervantes.CORE.Entities;

/// <summary>
/// Represents a synchronization source for CVE data
/// </summary>
public class CveSyncSource
{
    /// <summary>
    /// Unique identifier for the sync source
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the sync source (e.g., "NVD", "MITRE", "RedHat")
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// URL or endpoint for the sync source
    /// </summary>
    public string Url { get; set; } = "";

    /// <summary>
    /// Description of the sync source
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Whether this sync source is currently enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// API key or authentication token (if required)
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Rate limit in requests per minute
    /// </summary>
    public int RateLimitPerMinute { get; set; } = 60;

    /// <summary>
    /// Last successful synchronization timestamp
    /// </summary>
    public DateTime? LastSyncDate { get; set; }

    /// <summary>
    /// Last sync status
    /// </summary>
    public string LastSyncStatus { get; set; } = "Never";

    /// <summary>
    /// Error message from last sync (if any)
    /// </summary>
    public string? LastSyncError { get; set; }

    /// <summary>
    /// Number of CVEs synchronized from this source
    /// </summary>
    public int SyncedCveCount { get; set; } = 0;

    /// <summary>
    /// When this sync source was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// When this sync source was last modified
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// User who created this sync source
    /// </summary>
    public string? UserId { get; set; }
}