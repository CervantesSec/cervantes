using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

/// <summary>
/// Interface for CVE Notification Manager
/// </summary>
public interface ICveNotificationManager : IGenericManager<CveNotification>
{
    /// <summary>
    /// Get notifications by user ID
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of CVE notifications</returns>
    Task<List<CveNotification>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Get unread notifications by user ID
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of unread CVE notifications</returns>
    Task<List<CveNotification>> GetUnreadByUserIdAsync(string userId);

    /// <summary>
    /// Get notifications by subscription ID
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <returns>List of CVE notifications</returns>
    Task<List<CveNotification>> GetBySubscriptionIdAsync(Guid subscriptionId);

    /// <summary>
    /// Get notifications by CVE ID
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <returns>List of CVE notifications</returns>
    Task<List<CveNotification>> GetByCveIdAsync(Guid cveId);

    /// <summary>
    /// Get notifications by priority
    /// </summary>
    /// <param name="priority">Notification priority</param>
    /// <returns>List of CVE notifications</returns>
    Task<List<CveNotification>> GetByPriorityAsync(string priority);

    /// <summary>
    /// Get notifications by status
    /// </summary>
    /// <param name="status">Notification status</param>
    /// <returns>List of CVE notifications</returns>
    Task<List<CveNotification>> GetByStatusAsync(string status);

    /// <summary>
    /// Get pending notifications
    /// </summary>
    /// <returns>List of pending CVE notifications</returns>
    Task<List<CveNotification>> GetPendingNotificationsAsync();

    /// <summary>
    /// Get failed notifications
    /// </summary>
    /// <returns>List of failed CVE notifications</returns>
    Task<List<CveNotification>> GetFailedNotificationsAsync();

    /// <summary>
    /// Get notifications requiring retry
    /// </summary>
    /// <returns>List of CVE notifications requiring retry</returns>
    Task<List<CveNotification>> GetRetryNotificationsAsync();

    /// <summary>
    /// Create notification for CVE and subscription
    /// </summary>
    /// <param name="cve">CVE entity</param>
    /// <param name="subscription">CVE subscription</param>
    /// <param name="notificationType">Notification type</param>
    /// <param name="priority">Notification priority</param>
    /// <returns>Created notification</returns>
    Task<CveNotification> CreateNotificationAsync(Cve cve, CveSubscription subscription, string notificationType, string priority = "Medium");

    /// <summary>
    /// Mark notification as read
    /// </summary>
    /// <param name="notificationId">Notification identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    Task<bool> MarkAsReadAsync(Guid notificationId, string userId);

    /// <summary>
    /// Mark all notifications as read for user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>Number of notifications marked as read</returns>
    Task<int> MarkAllAsReadAsync(string userId);

    /// <summary>
    /// Mark notification as sent
    /// </summary>
    /// <param name="notificationId">Notification identifier</param>
    /// <returns>True if successful</returns>
    Task<bool> MarkAsSentAsync(Guid notificationId);

    /// <summary>
    /// Mark notification as failed
    /// </summary>
    /// <param name="notificationId">Notification identifier</param>
    /// <param name="errorMessage">Error message</param>
    /// <returns>True if successful</returns>
    Task<bool> MarkAsFailedAsync(Guid notificationId, string errorMessage);

    /// <summary>
    /// Schedule notification for retry
    /// </summary>
    /// <param name="notificationId">Notification identifier</param>
    /// <param name="nextRetryDate">Next retry date</param>
    /// <returns>True if successful</returns>
    Task<bool> ScheduleRetryAsync(Guid notificationId, DateTime nextRetryDate);

    /// <summary>
    /// Process notification queue
    /// </summary>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result</returns>
    Task<CveNotificationProcessingResult> ProcessNotificationQueueAsync(int batchSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send notification
    /// </summary>
    /// <param name="notification">CVE notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful</returns>
    Task<bool> SendNotificationAsync(CveNotification notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get notification statistics
    /// </summary>
    /// <param name="userId">User identifier (optional)</param>
    /// <returns>Notification statistics</returns>
    Task<CveNotificationStatistics> GetStatisticsAsync(string userId = null);

    /// <summary>
    /// Get user notification preferences
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>User notification preferences</returns>
    Task<CveUserNotificationPreferences> GetUserPreferencesAsync(string userId);

    /// <summary>
    /// Update user notification preferences
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="preferences">User notification preferences</param>
    /// <returns>True if successful</returns>
    Task<bool> UpdateUserPreferencesAsync(string userId, CveUserNotificationPreferences preferences);

    /// <summary>
    /// Clean up old notifications
    /// </summary>
    /// <param name="retentionDays">Retention period in days</param>
    /// <returns>Number of notifications deleted</returns>
    Task<int> CleanupOldNotificationsAsync(int retentionDays = 90);

    /// <summary>
    /// Get notification delivery history
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="days">Number of days to look back</param>
    /// <returns>Notification delivery history</returns>
    Task<List<CveNotificationDeliveryHistory>> GetDeliveryHistoryAsync(string userId, int days = 30);

    /// <summary>
    /// Delete multiple notifications by their IDs
    /// </summary>
    /// <param name="notificationIds">List of notification IDs to delete</param>
    /// <returns>Number of notifications deleted</returns>
    Task<int> DeleteMultipleAsync(List<Guid> notificationIds);
}

/// <summary>
/// CVE notification processing result model
/// </summary>
public class CveNotificationProcessingResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public int ProcessedCount { get; set; }
    public int SentCount { get; set; }
    public int FailedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// CVE notification statistics model
/// </summary>
public class CveNotificationStatistics
{
    public int TotalNotifications { get; set; }
    public int UnreadNotifications { get; set; }
    public int SentNotifications { get; set; }
    public int FailedNotifications { get; set; }
    public int PendingNotifications { get; set; }
    public int NotificationsToday { get; set; }
    public int NotificationsThisWeek { get; set; }
    public int NotificationsThisMonth { get; set; }
    public Dictionary<string, int> NotificationsByType { get; set; } = new();
    public Dictionary<string, int> NotificationsByPriority { get; set; } = new();
    public Dictionary<string, int> NotificationsByMethod { get; set; } = new();
    public DateTime LastNotificationDate { get; set; }
}

/// <summary>
/// CVE user notification preferences model
/// </summary>
public class CveUserNotificationPreferences
{
    public string UserId { get; set; }
    public bool EmailNotifications { get; set; } = true;
    public bool InAppNotifications { get; set; } = true;
    public bool WebhookNotifications { get; set; } = false;
    public string EmailFrequency { get; set; } = "Immediate"; // Immediate, Daily, Weekly
    public bool CriticalOnly { get; set; } = false;
    public bool HighAndAbove { get; set; } = false;
    public bool KnownExploitedOnly { get; set; } = false;
    public List<string> NotificationTypes { get; set; } = new(); // NewCve, UpdatedCve, CriticalCve, KevAdded
    public string QuietHoursStart { get; set; } = "22:00";
    public string QuietHoursEnd { get; set; } = "08:00";
    public string Timezone { get; set; } = "UTC";
    public bool WeekendNotifications { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}

/// <summary>
/// CVE notification delivery history model
/// </summary>
public class CveNotificationDeliveryHistory
{
    public Guid NotificationId { get; set; }
    public string CveId { get; set; }
    public string Title { get; set; }
    public string NotificationType { get; set; }
    public string Priority { get; set; }
    public string Method { get; set; }
    public string Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? SentDate { get; set; }
    public DateTime? ReadDate { get; set; }
    public string ErrorMessage { get; set; }
}