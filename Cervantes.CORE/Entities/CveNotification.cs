using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// CVE Notification Entity - Alerts generated by CVE subscriptions
/// </summary>
public class CveNotification
{
    /// <summary>
    /// Unique identifier for CVE Notification
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Associated CVE
    /// </summary>
    [ForeignKey("CveId")]
    [JsonIgnore]
    public virtual Cve Cve { get; set; }

    /// <summary>
    /// CVE identifier
    /// </summary>
    [Required]
    public Guid CveId { get; set; }

    /// <summary>
    /// Associated Subscription
    /// </summary>
    [ForeignKey("SubscriptionId")]
    [JsonIgnore]
    public virtual CveSubscription Subscription { get; set; }

    /// <summary>
    /// Subscription identifier
    /// </summary>
    [Required]
    public Guid SubscriptionId { get; set; }

    /// <summary>
    /// Notification title
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    /// <summary>
    /// Notification message
    /// </summary>
    [Required]
    public string Message { get; set; }

    /// <summary>
    /// Notification type (NewCve, UpdatedCve, CriticalCve, KevAdded)
    /// </summary>
    [StringLength(20)]
    public string NotificationType { get; set; }

    /// <summary>
    /// Notification priority (High, Medium, Low)
    /// </summary>
    [StringLength(20)]
    public string Priority { get; set; }

    /// <summary>
    /// Whether this notification has been read
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Whether this notification has been sent
    /// </summary>
    public bool IsSent { get; set; }

    /// <summary>
    /// Notification method used (Email, InApp, Webhook)
    /// </summary>
    [StringLength(20)]
    public string Method { get; set; }

    /// <summary>
    /// Notification status (Pending, Sent, Failed)
    /// </summary>
    [StringLength(20)]
    public string Status { get; set; }

    /// <summary>
    /// Error message if notification failed
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Next retry date
    /// </summary>
    public DateTime? NextRetryDate { get; set; }

    /// <summary>
    /// Notification creation date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Notification modified date
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// Date when notification was sent
    /// </summary>
    public DateTime? SentDate { get; set; }

    /// <summary>
    /// Date when notification was read
    /// </summary>
    public DateTime? ReadDate { get; set; }

    /// <summary>
    /// User who received the notification
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id of the user who received the notification
    /// </summary>
    public string UserId { get; set; }
}