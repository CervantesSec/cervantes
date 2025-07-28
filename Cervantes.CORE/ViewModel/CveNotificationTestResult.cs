using System;

namespace Cervantes.CORE.ViewModel;

/// <summary>
/// Result of CVE notification test
/// </summary>
public class CveNotificationTestResult
{
    /// <summary>
    /// Whether the test was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Success or error message
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// Error message if test failed
    /// </summary>
    public string ErrorMessage { get; set; } = "";

    /// <summary>
    /// ID of the created test notification
    /// </summary>
    public Guid? NotificationId { get; set; }

    /// <summary>
    /// Name of the subscription tested
    /// </summary>
    public string SubscriptionName { get; set; } = "";

    /// <summary>
    /// Notification method used
    /// </summary>
    public string NotificationMethod { get; set; } = "";

    /// <summary>
    /// Test execution timestamp
    /// </summary>
    public DateTime TestedAt { get; set; } = DateTime.UtcNow;
}