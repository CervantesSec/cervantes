using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Cervantes.Application;

/// <summary>
/// CVE Notification Manager implementation
/// </summary>
public class CveNotificationManager : GenericManager<CveNotification>, ICveNotificationManager
{
    public CveNotificationManager(IApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get notifications by user ID
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of CVE notifications</returns>
    public async Task<List<CveNotification>> GetByUserIdAsync(string userId)
    {
        return await Context.Set<CveNotification>()
            .Where(n => n.UserId == userId)
            .Include(n => n.Cve)
            .Include(n => n.Subscription)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get unread notifications by user ID
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of unread CVE notifications</returns>
    public async Task<List<CveNotification>> GetUnreadByUserIdAsync(string userId)
    {
        return await Context.Set<CveNotification>()
            .Where(n => n.UserId == userId && !n.IsRead)
            .Include(n => n.Cve)
            .Include(n => n.Subscription)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get notifications by subscription ID
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <returns>List of CVE notifications</returns>
    public async Task<List<CveNotification>> GetBySubscriptionIdAsync(Guid subscriptionId)
    {
        return await Context.Set<CveNotification>()
            .Where(n => n.SubscriptionId == subscriptionId)
            .Include(n => n.Cve)
            .Include(n => n.User)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get notifications by CVE ID
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <returns>List of CVE notifications</returns>
    public async Task<List<CveNotification>> GetByCveIdAsync(Guid cveId)
    {
        return await Context.Set<CveNotification>()
            .Where(n => n.CveId == cveId)
            .Include(n => n.Subscription)
            .Include(n => n.User)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get notifications by priority
    /// </summary>
    /// <param name="priority">Notification priority</param>
    /// <returns>List of CVE notifications</returns>
    public async Task<List<CveNotification>> GetByPriorityAsync(string priority)
    {
        return await Context.Set<CveNotification>()
            .Where(n => n.Priority == priority)
            .Include(n => n.Cve)
            .Include(n => n.Subscription)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get notifications by status
    /// </summary>
    /// <param name="status">Notification status</param>
    /// <returns>List of CVE notifications</returns>
    public async Task<List<CveNotification>> GetByStatusAsync(string status)
    {
        return await Context.Set<CveNotification>()
            .Where(n => n.Status == status)
            .Include(n => n.Cve)
            .Include(n => n.Subscription)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get pending notifications
    /// </summary>
    /// <returns>List of pending CVE notifications</returns>
    public async Task<List<CveNotification>> GetPendingNotificationsAsync()
    {
        return await Context.Set<CveNotification>()
            .Where(n => n.Status == "Pending" && !n.IsSent)
            .Include(n => n.Cve)
            .Include(n => n.Subscription)
            .Include(n => n.User)
            .OrderBy(n => n.CreatedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get failed notifications
    /// </summary>
    /// <returns>List of failed CVE notifications</returns>
    public async Task<List<CveNotification>> GetFailedNotificationsAsync()
    {
        return await Context.Set<CveNotification>()
            .Where(n => n.Status == "Failed")
            .Include(n => n.Cve)
            .Include(n => n.Subscription)
            .Include(n => n.User)
            .OrderByDescending(n => n.ModifiedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get notifications requiring retry
    /// </summary>
    /// <returns>List of CVE notifications requiring retry</returns>
    public async Task<List<CveNotification>> GetRetryNotificationsAsync()
    {
        return await Context.Set<CveNotification>()
            .Where(n => n.Status == "Failed" && 
                       n.NextRetryDate.HasValue && 
                       n.NextRetryDate <= DateTime.UtcNow &&
                       n.RetryCount < 3)
            .Include(n => n.Cve)
            .Include(n => n.Subscription)
            .Include(n => n.User)
            .OrderBy(n => n.NextRetryDate)
            .ToListAsync();
    }

    /// <summary>
    /// Create notification for CVE and subscription
    /// </summary>
    /// <param name="cve">CVE entity</param>
    /// <param name="subscription">CVE subscription</param>
    /// <param name="notificationType">Notification type</param>
    /// <param name="priority">Notification priority</param>
    /// <returns>Created notification</returns>
    public async Task<CveNotification> CreateNotificationAsync(Cve cve, CveSubscription subscription, string notificationType, string priority = "Medium")
    {
        var notification = new CveNotification
        {
            Id = Guid.NewGuid(),
            CveId = cve.Id,
            SubscriptionId = subscription.Id,
            UserId = subscription.UserId,
            Title = GenerateNotificationTitle(cve, notificationType),
            Message = GenerateNotificationMessage(cve, notificationType, subscription),
            NotificationType = notificationType,
            Priority = priority,
            Method = subscription.NotificationMethod,
            Status = "Pending",
            IsRead = false,
            IsSent = false,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        return await AddAsync(notification);
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    /// <param name="notificationId">Notification identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    public async Task<bool> MarkAsReadAsync(Guid notificationId, string userId)
    {
        var notification = await Context.Set<CveNotification>()
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification == null)
        {
            return false;
        }

        notification.IsRead = true;
        notification.ReadDate = DateTime.UtcNow;
        notification.ModifiedDate = DateTime.UtcNow;

        await Context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Mark all notifications as read for user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>Number of notifications marked as read</returns>
    public async Task<int> MarkAllAsReadAsync(string userId)
    {
        var notifications = await Context.Set<CveNotification>()
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        var count = notifications.Count;
        var now = DateTime.UtcNow;

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadDate = now;
            notification.ModifiedDate = now;
        }

        await Context.SaveChangesAsync();
        return count;
    }

    /// <summary>
    /// Mark notification as sent
    /// </summary>
    /// <param name="notificationId">Notification identifier</param>
    /// <returns>True if successful</returns>
    public async Task<bool> MarkAsSentAsync(Guid notificationId)
    {
        var notification = await Context.Set<CveNotification>().FindAsync(notificationId);
        if (notification == null)
        {
            return false;
        }

        notification.IsSent = true;
        notification.SentDate = DateTime.UtcNow;
        notification.Status = "Sent";
        notification.ModifiedDate = DateTime.UtcNow;

        await Context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Mark notification as failed
    /// </summary>
    /// <param name="notificationId">Notification identifier</param>
    /// <param name="errorMessage">Error message</param>
    /// <returns>True if successful</returns>
    public async Task<bool> MarkAsFailedAsync(Guid notificationId, string errorMessage)
    {
        var notification = await Context.Set<CveNotification>().FindAsync(notificationId);
        if (notification == null)
        {
            return false;
        }

        notification.Status = "Failed";
        notification.ErrorMessage = errorMessage;
        notification.RetryCount++;
        notification.ModifiedDate = DateTime.UtcNow;

        await Context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Schedule notification for retry
    /// </summary>
    /// <param name="notificationId">Notification identifier</param>
    /// <param name="nextRetryDate">Next retry date</param>
    /// <returns>True if successful</returns>
    public async Task<bool> ScheduleRetryAsync(Guid notificationId, DateTime nextRetryDate)
    {
        var notification = await Context.Set<CveNotification>().FindAsync(notificationId);
        if (notification == null)
        {
            return false;
        }

        notification.NextRetryDate = nextRetryDate;
        notification.Status = "Pending";
        notification.ModifiedDate = DateTime.UtcNow;

        await Context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Process notification queue
    /// </summary>
    /// <param name="batchSize">Batch size for processing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result</returns>
    public async Task<CveNotificationProcessingResult> ProcessNotificationQueueAsync(int batchSize = 50, CancellationToken cancellationToken = default)
    {
        var result = new CveNotificationProcessingResult { IsSuccess = true };

        try
        {
            var pendingNotifications = await Context.Set<CveNotification>()
                .Where(n => n.Status == "Pending" && !n.IsSent)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            result.ProcessedCount = pendingNotifications.Count;

            foreach (var notification in pendingNotifications)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    var sent = await SendNotificationAsync(notification, cancellationToken);
                    if (sent)
                    {
                        result.SentCount++;
                    }
                    else
                    {
                        result.FailedCount++;
                    }
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"Failed to send notification {notification.Id}: {ex.Message}");
                    await MarkAsFailedAsync(notification.Id, ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// Send notification
    /// </summary>
    /// <param name="notification">CVE notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if successful</returns>
    public async Task<bool> SendNotificationAsync(CveNotification notification, CancellationToken cancellationToken = default)
    {
        try
        {
            // This is a placeholder - actual implementation would depend on the notification method
            // For now, we'll just mark as sent for email and in-app notifications
            switch (notification.Method)
            {
                case "Email":
                    // TODO: Implement email sending
                    await MarkAsSentAsync(notification.Id);
                    return true;

                case "InApp":
                    // In-app notifications are already created, just mark as sent
                    await MarkAsSentAsync(notification.Id);
                    return true;

                case "Webhook":
                    // TODO: Implement webhook sending
                    await MarkAsSentAsync(notification.Id);
                    return true;

                default:
                    await MarkAsFailedAsync(notification.Id, "Unknown notification method");
                    return false;
            }
        }
        catch (Exception ex)
        {
            await MarkAsFailedAsync(notification.Id, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Get notification statistics
    /// </summary>
    /// <param name="userId">User identifier (optional)</param>
    /// <returns>Notification statistics</returns>
    public async Task<CveNotificationStatistics> GetStatisticsAsync(string userId = null)
    {
        var query = Context.Set<CveNotification>().AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(n => n.UserId == userId);
        }

        var notifications = await query.ToListAsync();
        var today = DateTime.UtcNow.Date;

        var statistics = new CveNotificationStatistics
        {
            TotalNotifications = notifications.Count,
            UnreadNotifications = notifications.Count(n => !n.IsRead),
            SentNotifications = notifications.Count(n => n.IsSent),
            FailedNotifications = notifications.Count(n => n.Status == "Failed"),
            PendingNotifications = notifications.Count(n => n.Status == "Pending"),
            NotificationsToday = notifications.Count(n => n.CreatedDate.Date == today),
            NotificationsThisWeek = notifications.Count(n => n.CreatedDate >= today.AddDays(-7)),
            NotificationsThisMonth = notifications.Count(n => n.CreatedDate >= today.AddDays(-30)),
            LastNotificationDate = notifications.Any() ? notifications.Max(n => n.CreatedDate) : DateTime.MinValue
        };

        // Group by type
        statistics.NotificationsByType = notifications
            .GroupBy(n => n.NotificationType)
            .ToDictionary(g => g.Key, g => g.Count());

        // Group by priority
        statistics.NotificationsByPriority = notifications
            .GroupBy(n => n.Priority)
            .ToDictionary(g => g.Key, g => g.Count());

        // Group by method
        statistics.NotificationsByMethod = notifications
            .GroupBy(n => n.Method)
            .ToDictionary(g => g.Key, g => g.Count());

        return statistics;
    }

    /// <summary>
    /// Get user notification preferences
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>User notification preferences</returns>
    public async Task<CveUserNotificationPreferences> GetUserPreferencesAsync(string userId)
    {
        // This would typically be stored in a separate table
        // For now, returning default preferences
        return new CveUserNotificationPreferences
        {
            UserId = userId,
            EmailNotifications = true,
            InAppNotifications = true,
            WebhookNotifications = false,
            EmailFrequency = "Daily",
            CriticalOnly = false,
            HighAndAbove = false,
            KnownExploitedOnly = false,
            NotificationTypes = new List<string> { "NewCve", "UpdatedCve", "CriticalCve", "KevAdded" },
            QuietHoursStart = "22:00",
            QuietHoursEnd = "08:00",
            Timezone = "UTC",
            WeekendNotifications = true,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Update user notification preferences
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="preferences">User notification preferences</param>
    /// <returns>True if successful</returns>
    public async Task<bool> UpdateUserPreferencesAsync(string userId, CveUserNotificationPreferences preferences)
    {
        // This would typically update a separate preferences table
        // For now, just return true
        return true;
    }

    /// <summary>
    /// Clean up old notifications
    /// </summary>
    /// <param name="retentionDays">Retention period in days</param>
    /// <returns>Number of notifications deleted</returns>
    public async Task<int> CleanupOldNotificationsAsync(int retentionDays = 90)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
        
        var oldNotifications = await Context.Set<CveNotification>()
            .Where(n => n.CreatedDate < cutoffDate && n.IsRead)
            .ToListAsync();

        var count = oldNotifications.Count;

        foreach (var notification in oldNotifications)
        {
            Context.Set<CveNotification>().Remove(notification);
        }

        await Context.SaveChangesNoAuditAsync();
        return count;
    }

    /// <summary>
    /// Get notification delivery history
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="days">Number of days to look back</param>
    /// <returns>Notification delivery history</returns>
    public async Task<List<CveNotificationDeliveryHistory>> GetDeliveryHistoryAsync(string userId, int days = 30)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);

        return await Context.Set<CveNotification>()
            .Where(n => n.UserId == userId && n.CreatedDate >= startDate)
            .Include(n => n.Cve)
            .Select(n => new CveNotificationDeliveryHistory
            {
                NotificationId = n.Id,
                CveId = n.Cve.CveId,
                Title = n.Title,
                NotificationType = n.NotificationType,
                Priority = n.Priority,
                Method = n.Method,
                Status = n.Status,
                CreatedDate = n.CreatedDate,
                SentDate = n.SentDate,
                ReadDate = n.ReadDate,
                ErrorMessage = n.ErrorMessage
            })
            .OrderByDescending(h => h.CreatedDate)
            .ToListAsync();
    }

    /// <summary>
    /// Delete multiple notifications by their IDs
    /// </summary>
    /// <param name="notificationIds">List of notification IDs to delete</param>
    /// <returns>Number of notifications deleted</returns>
    public async Task<int> DeleteMultipleAsync(List<Guid> notificationIds)
    {
        if (notificationIds == null || !notificationIds.Any())
        {
            return 0;
        }

        var notifications = await Context.Set<CveNotification>()
            .Where(n => notificationIds.Contains(n.Id))
            .ToListAsync();

        var count = notifications.Count;

        foreach (var notification in notifications)
        {
            Context.Set<CveNotification>().Remove(notification);
        }

        await Context.SaveChangesAsync();
        return count;
    }

    #region Private Methods

    private string GenerateNotificationTitle(Cve cve, string notificationType)
    {
        return notificationType switch
        {
            "NewCve" => $"New CVE: {cve.CveId}",
            "UpdatedCve" => $"Updated CVE: {cve.CveId}",
            "CriticalCve" => $"Critical CVE: {cve.CveId}",
            "KevAdded" => $"KEV Added: {cve.CveId}",
            _ => $"CVE Alert: {cve.CveId}"
        };
    }

    private string GenerateNotificationMessage(Cve cve, string notificationType, CveSubscription subscription)
    {
        var message = notificationType switch
        {
            "NewCve" => $"A new CVE has been published that matches your subscription '{subscription.Name}'.",
            "UpdatedCve" => $"A CVE has been updated that matches your subscription '{subscription.Name}'.",
            "CriticalCve" => $"A critical CVE has been published that matches your subscription '{subscription.Name}'.",
            "KevAdded" => $"A CVE has been added to the CISA KEV catalog that matches your subscription '{subscription.Name}'.",
            _ => $"A CVE alert has been generated for your subscription '{subscription.Name}'."
        };

        message += $"\n\nCVE ID: {cve.CveId}";
        message += $"\nTitle: {cve.Title}";
        
        if (cve.CvssV3BaseScore.HasValue)
        {
            message += $"\nCVSS Score: {cve.CvssV3BaseScore.Value} ({cve.CvssV3Severity})";
        }

        if (cve.IsKnownExploited)
        {
            message += $"\nKnown Exploited: Yes";
            if (cve.KevDueDate.HasValue)
            {
                message += $" (Due: {cve.KevDueDate.Value:yyyy-MM-dd})";
            }
        }

        message += $"\nPublished: {cve.PublishedDate:yyyy-MM-dd}";
        message += $"\n\nDescription: {cve.Description}";

        return message;
    }

    #endregion
}