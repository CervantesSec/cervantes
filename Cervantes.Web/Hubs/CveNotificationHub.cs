using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Hubs;

/// <summary>
/// SignalR hub for real-time CVE notifications
/// </summary>
[Authorize]
public class CveNotificationHub : Hub
{
    private readonly ILogger<CveNotificationHub> _logger;
    private readonly ICveNotificationManager _notificationManager;
    private readonly ICveSubscriptionManager _subscriptionManager;

    public CveNotificationHub(
        ILogger<CveNotificationHub> logger,
        ICveNotificationManager notificationManager,
        ICveSubscriptionManager subscriptionManager)
    {
        _logger = logger;
        _notificationManager = notificationManager;
        _subscriptionManager = subscriptionManager;
    }

    /// <summary>
    /// Called when client connects to the hub
    /// </summary>
    /// <returns>Task</returns>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var userName = Context.User?.Identity?.Name ?? "Unknown";
        
        _logger.LogInformation("User {UserName} ({UserId}) connected to CVE notification hub", userName, userId);

        // Join user to their personal notification group
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

        // Send unread notification count
        await SendUnreadNotificationCount(userId);

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when client disconnects from the hub
    /// </summary>
    /// <param name="exception">Exception if any</param>
    /// <returns>Task</returns>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.UserIdentifier;
        var userName = Context.User?.Identity?.Name ?? "Unknown";
        
        _logger.LogInformation("User {UserName} ({UserId}) disconnected from CVE notification hub", userName, userId);

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    /// <param name="notificationId">Notification ID</param>
    /// <returns>Task</returns>
    public async Task MarkNotificationAsRead(string notificationId)
    {
        var userId = Context.UserIdentifier;
        
        try
        {
            if (Guid.TryParse(notificationId, out var notificationGuid))
            {
                var success = await _notificationManager.MarkAsReadAsync(notificationGuid, userId);
                
                if (success)
                {
                    // Send updated unread count
                    await SendUnreadNotificationCount(userId);
                    
                    // Notify client that notification was marked as read
                    await Clients.Caller.SendAsync("NotificationMarkedAsRead", notificationId);
                }
                else
                {
                    await Clients.Caller.SendAsync("Error", "Failed to mark notification as read");
                }
            }
            else
            {
                await Clients.Caller.SendAsync("Error", "Invalid notification ID");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read for user {UserId}", userId);
            await Clients.Caller.SendAsync("Error", "An error occurred while marking notification as read");
        }
    }

    /// <summary>
    /// Mark all notifications as read for the user
    /// </summary>
    /// <returns>Task</returns>
    public async Task MarkAllNotificationsAsRead()
    {
        var userId = Context.UserIdentifier;
        
        try
        {
            var count = await _notificationManager.MarkAllAsReadAsync(userId);
            
            // Send updated unread count
            await SendUnreadNotificationCount(userId);
            
            // Notify client that all notifications were marked as read
            await Clients.Caller.SendAsync("AllNotificationsMarkedAsRead", count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
            await Clients.Caller.SendAsync("Error", "An error occurred while marking all notifications as read");
        }
    }

    /// <summary>
    /// Get recent notifications for the user
    /// </summary>
    /// <param name="count">Number of notifications to retrieve</param>
    /// <returns>Task</returns>
    public async Task GetRecentNotifications(int count = 10)
    {
        var userId = Context.UserIdentifier;
        
        try
        {
            var notifications = await _notificationManager.GetByUserIdAsync(userId);
            var recentNotifications = notifications.Take(count).ToList();
            
            var notificationData = recentNotifications.Select(n => new
            {
                id = n.Id,
                title = n.Title,
                message = n.Message,
                priority = n.Priority,
                notificationType = n.NotificationType,
                isRead = n.IsRead,
                createdDate = n.CreatedDate,
                cveId = n.Cve?.CveId,
                cveTitle = n.Cve?.Title,
                cvssScore = n.Cve?.CvssV3BaseScore,
                cvssSeverity = n.Cve?.CvssV3Severity,
                isKnownExploited = n.Cve?.IsKnownExploited ?? false
            }).ToList();

            await Clients.Caller.SendAsync("RecentNotifications", notificationData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent notifications for user {UserId}", userId);
            await Clients.Caller.SendAsync("Error", "An error occurred while retrieving notifications");
        }
    }

    /// <summary>
    /// Subscribe to CVE notifications for specific criteria
    /// </summary>
    /// <param name="criteria">Subscription criteria</param>
    /// <returns>Task</returns>
    public async Task SubscribeToNotifications(string criteria)
    {
        var userId = Context.UserIdentifier;
        
        try
        {
            // Add user to notification group based on criteria
            await Groups.AddToGroupAsync(Context.ConnectionId, $"criteria_{criteria}");
            
            _logger.LogInformation("User {UserId} subscribed to notifications for criteria: {Criteria}", userId, criteria);
            
            await Clients.Caller.SendAsync("SubscriptionConfirmed", criteria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing user {UserId} to notifications", userId);
            await Clients.Caller.SendAsync("Error", "An error occurred while subscribing to notifications");
        }
    }

    /// <summary>
    /// Send unread notification count to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Task</returns>
    private async Task SendUnreadNotificationCount(string userId)
    {
        try
        {
            var statistics = await _notificationManager.GetStatisticsAsync(userId);
            await Clients.Group($"user_{userId}").SendAsync("UnreadNotificationCount", statistics.UnreadNotifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending unread notification count to user {UserId}", userId);
        }
    }
}

/// <summary>
/// Extension methods for CVE notification hub
/// </summary>
public static class CveNotificationHubExtensions
{
    /// <summary>
    /// Send real-time notification to user
    /// </summary>
    /// <param name="hubContext">Hub context</param>
    /// <param name="notification">CVE notification</param>
    /// <returns>Task</returns>
    public static async Task SendNotificationToUser(this IHubContext<CveNotificationHub> hubContext, CveNotification notification)
    {
        if (notification.User == null) return;

        var notificationData = new
        {
            id = notification.Id,
            title = notification.Title,
            message = notification.Message,
            priority = notification.Priority,
            notificationType = notification.NotificationType,
            isRead = notification.IsRead,
            createdDate = notification.CreatedDate,
            cveId = notification.Cve?.CveId,
            cveTitle = notification.Cve?.Title,
            cvssScore = notification.Cve?.CvssV3BaseScore,
            cvssSeverity = notification.Cve?.CvssV3Severity,
            isKnownExploited = notification.Cve?.IsKnownExploited ?? false
        };

        await hubContext.Clients.Group($"user_{notification.UserId}").SendAsync("NewNotification", notificationData);
    }

    /// <summary>
    /// Send notification to all users subscribed to specific criteria
    /// </summary>
    /// <param name="hubContext">Hub context</param>
    /// <param name="criteria">Subscription criteria</param>
    /// <param name="notification">CVE notification</param>
    /// <returns>Task</returns>
    public static async Task SendNotificationToCriteria(this IHubContext<CveNotificationHub> hubContext, string criteria, CveNotification notification)
    {
        var notificationData = new
        {
            id = notification.Id,
            title = notification.Title,
            message = notification.Message,
            priority = notification.Priority,
            notificationType = notification.NotificationType,
            createdDate = notification.CreatedDate,
            cveId = notification.Cve?.CveId,
            cveTitle = notification.Cve?.Title,
            cvssScore = notification.Cve?.CvssV3BaseScore,
            cvssSeverity = notification.Cve?.CvssV3Severity,
            isKnownExploited = notification.Cve?.IsKnownExploited ?? false
        };

        await hubContext.Clients.Group($"criteria_{criteria}").SendAsync("NewNotification", notificationData);
    }
}