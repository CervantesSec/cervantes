using Cervantes.CORE.Entities;

namespace Cervantes.Contracts;

/// <summary>
/// Interface for CVE Subscription Manager
/// </summary>
public interface ICveSubscriptionManager : IGenericManager<CveSubscription>
{
    /// <summary>
    /// Get subscriptions by user ID
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of CVE subscriptions</returns>
    Task<List<CveSubscription>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Get active subscriptions
    /// </summary>
    /// <returns>List of active CVE subscriptions</returns>
    Task<List<CveSubscription>> GetActiveSubscriptionsAsync();

    /// <summary>
    /// Get subscriptions by project ID
    /// </summary>
    /// <param name="projectId">Project identifier</param>
    /// <returns>List of CVE subscriptions</returns>
    Task<List<CveSubscription>> GetByProjectIdAsync(Guid projectId);

    /// <summary>
    /// Get subscriptions by vendor
    /// </summary>
    /// <param name="vendor">Vendor name</param>
    /// <returns>List of CVE subscriptions</returns>
    Task<List<CveSubscription>> GetByVendorAsync(string vendor);

    /// <summary>
    /// Get subscriptions by product
    /// </summary>
    /// <param name="product">Product name</param>
    /// <returns>List of CVE subscriptions</returns>
    Task<List<CveSubscription>> GetByProductAsync(string product);

    /// <summary>
    /// Get subscriptions that match CVE criteria
    /// </summary>
    /// <param name="cve">CVE entity</param>
    /// <returns>List of matching subscriptions</returns>
    Task<List<CveSubscription>> GetMatchingSubscriptionsAsync(Cve cve);

    /// <summary>
    /// Test subscription criteria against CVE
    /// </summary>
    /// <param name="subscription">CVE subscription</param>
    /// <param name="cve">CVE entity</param>
    /// <returns>True if CVE matches subscription criteria</returns>
    Task<bool> TestSubscriptionAsync(CveSubscription subscription, Cve cve);

    /// <summary>
    /// Activate subscription
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    Task<bool> ActivateAsync(Guid subscriptionId, string userId);

    /// <summary>
    /// Deactivate subscription
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    Task<bool> DeactivateAsync(Guid subscriptionId, string userId);

    /// <summary>
    /// Update subscription filters
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <param name="filters">Filter criteria</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    Task<bool> UpdateFiltersAsync(Guid subscriptionId, CveSubscriptionFilters filters, string userId);

    /// <summary>
    /// Get subscription statistics
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <returns>Subscription statistics</returns>
    Task<CveSubscriptionStatistics> GetStatisticsAsync(Guid subscriptionId);

    /// <summary>
    /// Get user subscription summary
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>User subscription summary</returns>
    Task<CveUserSubscriptionSummary> GetUserSummaryAsync(string userId);

    /// <summary>
    /// Create default subscriptions for user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of created subscriptions</returns>
    Task<List<CveSubscription>> CreateDefaultSubscriptionsAsync(string userId);

    /// <summary>
    /// Create subscription from project targets
    /// </summary>
    /// <param name="projectId">Project identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>Created subscription</returns>
    Task<CveSubscription> CreateFromProjectTargetsAsync(Guid projectId, string userId);

    /// <summary>
    /// Validate subscription configuration
    /// </summary>
    /// <param name="subscription">CVE subscription</param>
    /// <returns>Validation result</returns>
    Task<CveSubscriptionValidationResult> ValidateSubscriptionAsync(CveSubscription subscription);
}

/// <summary>
/// CVE subscription filters model
/// </summary>
public class CveSubscriptionFilters
{
    public string Vendor { get; set; }
    public string Product { get; set; }
    public List<string> Keywords { get; set; } = new();
    public double? MinCvssScore { get; set; }
    public double? MaxCvssScore { get; set; }
    public double? MinEpssScore { get; set; }
    public bool OnlyKnownExploited { get; set; }
    public List<string> CweFilter { get; set; } = new();
    public List<string> SeverityFilter { get; set; } = new();
}

/// <summary>
/// CVE subscription statistics model
/// </summary>
public class CveSubscriptionStatistics
{
    public Guid SubscriptionId { get; set; }
    public string Name { get; set; }
    public int TotalMatches { get; set; }
    public int MatchesToday { get; set; }
    public int MatchesThisWeek { get; set; }
    public int MatchesThisMonth { get; set; }
    public int NotificationsSent { get; set; }
    public int NotificationsPending { get; set; }
    public int NotificationsFailed { get; set; }
    public DateTime LastMatchDate { get; set; }
    public DateTime LastNotificationDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// CVE user subscription summary model
/// </summary>
public class CveUserSubscriptionSummary
{
    public string UserId { get; set; }
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int TotalNotifications { get; set; }
    public int UnreadNotifications { get; set; }
    public int TotalCveMatches { get; set; }
    public int CveMatchesToday { get; set; }
    public int CveMatchesThisWeek { get; set; }
    public DateTime LastNotificationDate { get; set; }
    public List<CveSubscriptionStatistics> SubscriptionStats { get; set; } = new();
}

/// <summary>
/// CVE subscription validation result model
/// </summary>
public class CveSubscriptionValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
}