using System.Text;
using System.Text.Json;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.IFR.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Service for sending CVE notifications via different channels
/// </summary>
public class CveNotificationService : ICveNotificationService
{
    private readonly ILogger<CveNotificationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ICveNotificationManager _notificationManager;
    private readonly IEmailService _emailService;

    public CveNotificationService(
        ILogger<CveNotificationService> logger,
        IConfiguration configuration,
        HttpClient httpClient,
        ICveNotificationManager notificationManager,
        IEmailService emailService)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
        _notificationManager = notificationManager;
        _emailService = emailService;
    }

    /// <summary>
    /// Process notification queue and send pending notifications
    /// </summary>
    /// <param name="batchSize">Number of notifications to process</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result</returns>
    public async Task<CveNotificationProcessingResult> ProcessNotificationQueueAsync(int batchSize = 50, CancellationToken cancellationToken = default)
    {
        var result = new CveNotificationProcessingResult { IsSuccess = true };

        try
        {
            _logger.LogInformation("Starting CVE notification processing (batch size: {BatchSize})", batchSize);

            // Get pending notifications
            var pendingNotifications = await _notificationManager.GetPendingNotificationsAsync();
            var notificationsToProcess = pendingNotifications.Take(batchSize).ToList();

            result.ProcessedCount = notificationsToProcess.Count;

            foreach (var notification in notificationsToProcess)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("CVE notification processing cancelled");
                    break;
                }

                try
                {
                    var sent = await SendNotificationAsync(notification, cancellationToken);
                    if (sent)
                    {
                        result.SentCount++;
                        _logger.LogDebug("CVE notification {NotificationId} sent successfully", notification.Id);
                    }
                    else
                    {
                        result.FailedCount++;
                        _logger.LogWarning("Failed to send CVE notification {NotificationId}", notification.Id);
                    }
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"Failed to send notification {notification.Id}: {ex.Message}");
                    _logger.LogError(ex, "Error sending CVE notification {NotificationId}", notification.Id);
                    
                    await _notificationManager.MarkAsFailedAsync(notification.Id, ex.Message);
                    await ScheduleRetryAsync(notification);
                }
            }

            _logger.LogInformation("CVE notification processing completed. Processed: {ProcessedCount}, Sent: {SentCount}, Failed: {FailedCount}", 
                result.ProcessedCount, result.SentCount, result.FailedCount);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error processing CVE notification queue");
        }

        return result;
    }

    /// <summary>
    /// Send individual notification
    /// </summary>
    /// <param name="notification">CVE notification to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if sent successfully</returns>
    public async Task<bool> SendNotificationAsync(CveNotification notification, CancellationToken cancellationToken = default)
    {
        try
        {
            switch (notification.Method?.ToLower())
            {
                case "email":
                    return await SendEmailNotificationAsync(notification, cancellationToken);
                
                case "webhook":
                    return await SendWebhookNotificationAsync(notification, cancellationToken);
                
                case "inapp":
                    return await SendInAppNotificationAsync(notification, cancellationToken);
                
                default:
                    _logger.LogWarning("Unknown notification method: {Method}", notification.Method);
                    await _notificationManager.MarkAsFailedAsync(notification.Id, "Unknown notification method");
                    return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification {NotificationId}", notification.Id);
            await _notificationManager.MarkAsFailedAsync(notification.Id, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Send email notification using the centralized IEmailService
    /// </summary>
    /// <param name="notification">CVE notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if sent successfully</returns>
    private async Task<bool> SendEmailNotificationAsync(CveNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            if (!_emailService.IsEnabled())
            {
                _logger.LogWarning("Email service is disabled. Skipping email notification.");
                await _notificationManager.MarkAsFailedAsync(notification.Id, "Email service is disabled");
                return false;
            }

            var sent = await _emailService.SendCveNotificationAsync(notification);
            
            if (sent)
            {
                await _notificationManager.MarkAsSentAsync(notification.Id);
                _logger.LogInformation("Email notification sent successfully to {Email}", notification.User?.Email);
                return true;
            }
            else
            {
                await _notificationManager.MarkAsFailedAsync(notification.Id, "Failed to send email via EmailService");
                _logger.LogWarning("Failed to send email notification to {Email}", notification.User?.Email);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email notification");
            await _notificationManager.MarkAsFailedAsync(notification.Id, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Send webhook notification
    /// </summary>
    /// <param name="notification">CVE notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if sent successfully</returns>
    private async Task<bool> SendWebhookNotificationAsync(CveNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            var webhookUrl = notification.Subscription?.WebhookUrl;
            if (string.IsNullOrEmpty(webhookUrl))
            {
                _logger.LogWarning("Webhook URL not found for notification {NotificationId}", notification.Id);
                await _notificationManager.MarkAsFailedAsync(notification.Id, "Webhook URL not found");
                return false;
            }

            var payload = new
            {
                notification_id = notification.Id,
                cve_id = notification.Cve?.CveId,
                title = notification.Title,
                message = notification.Message,
                priority = notification.Priority,
                notification_type = notification.NotificationType,
                created_date = notification.CreatedDate,
                cve_details = notification.Cve != null ? new
                {
                    cve_id = notification.Cve.CveId,
                    title = notification.Cve.Title,
                    description = notification.Cve.Description,
                    cvss_score = notification.Cve.CvssV3BaseScore,
                    cvss_severity = notification.Cve.CvssV3Severity,
                    epss_score = notification.Cve.EpssScore,
                    known_exploited = notification.Cve.IsKnownExploited,
                    published_date = notification.Cve.PublishedDate,
                    last_modified = notification.Cve.LastModifiedDate
                } : null
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.Add("User-Agent", "Cervantes-CVE-Management/1.0");

            var response = await _httpClient.PostAsync(webhookUrl, content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                await _notificationManager.MarkAsSentAsync(notification.Id);
                _logger.LogInformation("Webhook notification sent successfully to {WebhookUrl}", webhookUrl);
                return true;
            }
            else
            {
                var errorMessage = $"Webhook returned status code: {response.StatusCode}";
                _logger.LogWarning("Webhook notification failed: {ErrorMessage}", errorMessage);
                await _notificationManager.MarkAsFailedAsync(notification.Id, errorMessage);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending webhook notification");
            await _notificationManager.MarkAsFailedAsync(notification.Id, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Send in-app notification
    /// </summary>
    /// <param name="notification">CVE notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if sent successfully</returns>
    private async Task<bool> SendInAppNotificationAsync(CveNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            // In-app notifications are already created in the database
            // Just mark as sent
            await _notificationManager.MarkAsSentAsync(notification.Id);
            _logger.LogInformation("In-app notification marked as sent for user {UserId}", notification.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing in-app notification");
            await _notificationManager.MarkAsFailedAsync(notification.Id, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Schedule notification for retry
    /// </summary>
    /// <param name="notification">CVE notification</param>
    /// <returns>Task</returns>
    private async Task ScheduleRetryAsync(CveNotification notification)
    {
        if (notification.RetryCount < 3)
        {
            var retryDelay = TimeSpan.FromMinutes(Math.Pow(2, notification.RetryCount) * 5); // Exponential backoff
            var nextRetryDate = DateTime.UtcNow.Add(retryDelay);
            
            await _notificationManager.ScheduleRetryAsync(notification.Id, nextRetryDate);
            _logger.LogInformation("Scheduled retry for notification {NotificationId} at {RetryDate}", 
                notification.Id, nextRetryDate);
        }
        else
        {
            _logger.LogWarning("Maximum retry attempts reached for notification {NotificationId}", notification.Id);
        }
    }

}