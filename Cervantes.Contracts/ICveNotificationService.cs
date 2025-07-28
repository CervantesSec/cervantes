using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;

namespace Cervantes.Contracts;

/// <summary>
/// Interface for CVE notification service
/// </summary>
public interface ICveNotificationService
{
    /// <summary>
    /// Process notification queue and send pending notifications
    /// </summary>
    /// <param name="batchSize">Number of notifications to process</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result</returns>
    Task<CveNotificationProcessingResult> ProcessNotificationQueueAsync(int batchSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send individual notification
    /// </summary>
    /// <param name="notification">CVE notification to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if sent successfully</returns>
    Task<bool> SendNotificationAsync(CveNotification notification, CancellationToken cancellationToken = default);
}

