using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// CVE Subscription Entity - User subscriptions to CVE alerts
/// </summary>
public class CveSubscription
{
    /// <summary>
    /// Unique identifier for CVE Subscription
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Subscription name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// Subscription description
    /// </summary>
    [StringLength(500)]
    public string Description { get; set; }

    /// <summary>
    /// Whether this subscription is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Vendor filter (optional)
    /// </summary>
    [StringLength(100)]
    public string Vendor { get; set; }

    /// <summary>
    /// Product filter (optional)
    /// </summary>
    [StringLength(100)]
    public string Product { get; set; }

    /// <summary>
    /// Keywords filter (JSON array)
    /// </summary>
    public string Keywords { get; set; }

    /// <summary>
    /// Minimum CVSS score threshold
    /// </summary>
    public double? MinCvssScore { get; set; }

    /// <summary>
    /// Maximum CVSS score threshold
    /// </summary>
    public double? MaxCvssScore { get; set; }

    /// <summary>
    /// Minimum EPSS score threshold
    /// </summary>
    public double? MinEpssScore { get; set; }

    /// <summary>
    /// Whether to only monitor CISA KEV listed CVEs
    /// </summary>
    public bool OnlyKnownExploited { get; set; }

    /// <summary>
    /// CWE filter (JSON array of CWE IDs)
    /// </summary>
    public string CweFilter { get; set; }

    /// <summary>
    /// Notification frequency (Immediate, Daily, Weekly)
    /// </summary>
    [StringLength(20)]
    public string NotificationFrequency { get; set; }

    /// <summary>
    /// Notification method (Email, InApp, Webhook)
    /// </summary>
    [StringLength(20)]
    public string NotificationMethod { get; set; }

    /// <summary>
    /// Webhook URL for notifications
    /// </summary>
    [StringLength(500)]
    public string WebhookUrl { get; set; }

    /// <summary>
    /// Associated Project (optional)
    /// </summary>
    [ForeignKey("ProjectId")]
    [JsonIgnore]
    public virtual Project Project { get; set; }

    /// <summary>
    /// Project identifier (optional)
    /// </summary>
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// Subscription creation date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Subscription modified date
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// User who created the subscription
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id of the user who created the subscription
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Associated notifications
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<CveNotification> Notifications { get; set; } = new List<CveNotification>();
}