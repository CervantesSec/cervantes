using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// CVE Source Entity - Configuration for CVE data sources
/// </summary>
public class CveSource
{
    /// <summary>
    /// Unique identifier for CVE Source
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Source name (e.g., "NVD", "MITRE", "RedHat")
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// Source description
    /// </summary>
    [StringLength(500)]
    public string Description { get; set; }

    /// <summary>
    /// Source API endpoint URL
    /// </summary>
    [Required]
    [StringLength(500)]
    public string ApiUrl { get; set; }

    /// <summary>
    /// API key for authentication
    /// </summary>
    [StringLength(200)]
    public string ApiKey { get; set; }

    /// <summary>
    /// Whether this source is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether this source is the primary source
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Sync interval in minutes
    /// </summary>
    public int SyncIntervalMinutes { get; set; }

    /// <summary>
    /// Last successful sync date
    /// </summary>
    public DateTime? LastSyncDate { get; set; }

    /// <summary>
    /// Last sync status (Success, Failed, InProgress)
    /// </summary>
    [StringLength(20)]
    public string LastSyncStatus { get; set; }

    /// <summary>
    /// Last sync error message
    /// </summary>
    public string LastSyncError { get; set; }

    /// <summary>
    /// Number of CVEs synced in last sync
    /// </summary>
    public int LastSyncCount { get; set; }

    /// <summary>
    /// Rate limit requests per minute
    /// </summary>
    public int RateLimitPerMinute { get; set; }

    /// <summary>
    /// Source priority (lower number = higher priority)
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; }

    /// <summary>
    /// Maximum retry attempts
    /// </summary>
    public int MaxRetries { get; set; }

    /// <summary>
    /// Additional configuration as JSON
    /// </summary>
    public string Configuration { get; set; }

    /// <summary>
    /// Source creation date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Source modified date
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// User who created the source
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id of the user who created the source
    /// </summary>
    public string UserId { get; set; }
}