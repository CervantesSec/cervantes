using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Cervantes.Application;

/// <summary>
/// CVE Subscription Manager implementation
/// </summary>
public class CveSubscriptionManager : GenericManager<CveSubscription>, ICveSubscriptionManager
{
    public CveSubscriptionManager(IApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get subscriptions by user ID
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of CVE subscriptions</returns>
    public async Task<List<CveSubscription>> GetByUserIdAsync(string userId)
    {
        return await Context.Set<CveSubscription>()
            .Where(s => s.UserId == userId)
            .Include(s => s.Project)
            .Include(s => s.Notifications)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get active subscriptions
    /// </summary>
    /// <returns>List of active CVE subscriptions</returns>
    public async Task<List<CveSubscription>> GetActiveSubscriptionsAsync()
    {
        return await Context.Set<CveSubscription>()
            .Where(s => s.IsActive)
            .Include(s => s.Project)
            .Include(s => s.User)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get subscriptions by project ID
    /// </summary>
    /// <param name="projectId">Project identifier</param>
    /// <returns>List of CVE subscriptions</returns>
    public async Task<List<CveSubscription>> GetByProjectIdAsync(Guid projectId)
    {
        return await Context.Set<CveSubscription>()
            .Where(s => s.ProjectId == projectId)
            .Include(s => s.User)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get subscriptions by vendor
    /// </summary>
    /// <param name="vendor">Vendor name</param>
    /// <returns>List of CVE subscriptions</returns>
    public async Task<List<CveSubscription>> GetByVendorAsync(string vendor)
    {
        return await Context.Set<CveSubscription>()
            .Where(s => s.Vendor.ToLower().Contains(vendor.ToLower()) && s.IsActive)
            .Include(s => s.User)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get subscriptions by product
    /// </summary>
    /// <param name="product">Product name</param>
    /// <returns>List of CVE subscriptions</returns>
    public async Task<List<CveSubscription>> GetByProductAsync(string product)
    {
        return await Context.Set<CveSubscription>()
            .Where(s => s.Product.ToLower().Contains(product.ToLower()) && s.IsActive)
            .Include(s => s.User)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get subscriptions that match CVE criteria
    /// </summary>
    /// <param name="cve">CVE entity</param>
    /// <returns>List of matching subscriptions</returns>
    public async Task<List<CveSubscription>> GetMatchingSubscriptionsAsync(Cve cve)
    {
        var subscriptions = await GetActiveSubscriptionsAsync();
        var matchingSubscriptions = new List<CveSubscription>();

        foreach (var subscription in subscriptions)
        {
            if (await TestSubscriptionAsync(subscription, cve))
            {
                matchingSubscriptions.Add(subscription);
            }
        }

        return matchingSubscriptions;
    }

    /// <summary>
    /// Test subscription criteria against CVE
    /// </summary>
    /// <param name="subscription">CVE subscription</param>
    /// <param name="cve">CVE entity</param>
    /// <returns>True if CVE matches subscription criteria</returns>
    public async Task<bool> TestSubscriptionAsync(CveSubscription subscription, Cve cve)
    {
        // Check vendor filter
        if (!string.IsNullOrEmpty(subscription.Vendor))
        {
            var configurations = await Context.Set<CveConfiguration>()
                .Where(c => c.CveId == cve.Id)
                .ToListAsync();

            if (!configurations.Any(c => c.Vendor.ToLower().Contains(subscription.Vendor.ToLower())))
            {
                return false;
            }
        }

        // Check product filter
        if (!string.IsNullOrEmpty(subscription.Product))
        {
            var configurations = await Context.Set<CveConfiguration>()
                .Where(c => c.CveId == cve.Id)
                .ToListAsync();

            if (!configurations.Any(c => c.Product.ToLower().Contains(subscription.Product.ToLower())))
            {
                return false;
            }
        }

        // Check CVSS score range
        if (subscription.MinCvssScore.HasValue && cve.CvssV3BaseScore < subscription.MinCvssScore.Value)
        {
            return false;
        }

        if (subscription.MaxCvssScore.HasValue && cve.CvssV3BaseScore > subscription.MaxCvssScore.Value)
        {
            return false;
        }

        // Check EPSS score range
        if (subscription.MinEpssScore.HasValue && cve.EpssScore < subscription.MinEpssScore.Value)
        {
            return false;
        }

        // Check known exploited filter
        if (subscription.OnlyKnownExploited && !cve.IsKnownExploited)
        {
            return false;
        }

        // Check keywords
        if (!string.IsNullOrEmpty(subscription.Keywords))
        {
            var keywords = JsonSerializer.Deserialize<List<string>>(subscription.Keywords);
            if (keywords?.Any() == true)
            {
                var hasKeyword = keywords.Any(keyword => 
                    cve.Title.ToLower().Contains(keyword.ToLower()) ||
                    cve.Description.ToLower().Contains(keyword.ToLower()));

                if (!hasKeyword)
                {
                    return false;
                }
            }
        }

        // Check CWE filter
        if (!string.IsNullOrEmpty(subscription.CweFilter))
        {
            var cweIds = JsonSerializer.Deserialize<List<string>>(subscription.CweFilter);
            if (cweIds?.Any() == true)
            {
                var hasCwe = cweIds.Any(cweId => 
                    cve.PrimaryCweId == cweId || 
                    cve.CweMappings.Any(mapping => mapping.Cwe.Id.ToString() == cweId));

                if (!hasCwe)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Activate subscription
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    public async Task<bool> ActivateAsync(Guid subscriptionId, string userId)
    {
        var subscription = await Context.Set<CveSubscription>().FindAsync(subscriptionId);
        if (subscription == null || subscription.UserId != userId)
        {
            return false;
        }

        subscription.IsActive = true;
        subscription.ModifiedDate = DateTime.UtcNow;
        
        await Context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Deactivate subscription
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    public async Task<bool> DeactivateAsync(Guid subscriptionId, string userId)
    {
        var subscription = await Context.Set<CveSubscription>().FindAsync(subscriptionId);
        if (subscription == null || subscription.UserId != userId)
        {
            return false;
        }

        subscription.IsActive = false;
        subscription.ModifiedDate = DateTime.UtcNow;
        
        await Context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Update subscription filters
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <param name="filters">Filter criteria</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    public async Task<bool> UpdateFiltersAsync(Guid subscriptionId, CveSubscriptionFilters filters, string userId)
    {
        var subscription = await Context.Set<CveSubscription>().FindAsync(subscriptionId);
        if (subscription == null || subscription.UserId != userId)
        {
            return false;
        }

        subscription.Vendor = filters.Vendor;
        subscription.Product = filters.Product;
        subscription.Keywords = JsonSerializer.Serialize(filters.Keywords);
        subscription.MinCvssScore = filters.MinCvssScore;
        subscription.MaxCvssScore = filters.MaxCvssScore;
        subscription.MinEpssScore = filters.MinEpssScore;
        subscription.OnlyKnownExploited = filters.OnlyKnownExploited;
        subscription.CweFilter = JsonSerializer.Serialize(filters.CweFilter);
        subscription.ModifiedDate = DateTime.UtcNow;
        
        await Context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Get subscription statistics
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <returns>Subscription statistics</returns>
    public async Task<CveSubscriptionStatistics> GetStatisticsAsync(Guid subscriptionId)
    {
        var subscription = await Context.Set<CveSubscription>()
            .Include(s => s.Notifications)
            .FirstOrDefaultAsync(s => s.Id == subscriptionId);

        if (subscription == null)
        {
            return null;
        }

        var notifications = subscription.Notifications.ToList();
        var today = DateTime.UtcNow.Date;

        return new CveSubscriptionStatistics
        {
            SubscriptionId = subscriptionId,
            Name = subscription.Name,
            TotalMatches = notifications.Count,
            MatchesToday = notifications.Count(n => n.CreatedDate.Date == today),
            MatchesThisWeek = notifications.Count(n => n.CreatedDate >= today.AddDays(-7)),
            MatchesThisMonth = notifications.Count(n => n.CreatedDate >= today.AddDays(-30)),
            NotificationsSent = notifications.Count(n => n.IsSent),
            NotificationsPending = notifications.Count(n => !n.IsSent && n.Status == "Pending"),
            NotificationsFailed = notifications.Count(n => n.Status == "Failed"),
            LastMatchDate = notifications.Any() ? notifications.Max(n => n.CreatedDate) : DateTime.MinValue,
            LastNotificationDate = notifications.Where(n => n.IsSent).Any() ? notifications.Where(n => n.IsSent).Max(n => n.SentDate.Value) : DateTime.MinValue,
            IsActive = subscription.IsActive
        };
    }

    /// <summary>
    /// Get user subscription summary
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>User subscription summary</returns>
    public async Task<CveUserSubscriptionSummary> GetUserSummaryAsync(string userId)
    {
        var subscriptions = await GetByUserIdAsync(userId);
        var allNotifications = subscriptions.SelectMany(s => s.Notifications).ToList();
        var today = DateTime.UtcNow.Date;

        var summary = new CveUserSubscriptionSummary
        {
            UserId = userId,
            TotalSubscriptions = subscriptions.Count,
            ActiveSubscriptions = subscriptions.Count(s => s.IsActive),
            TotalNotifications = allNotifications.Count,
            UnreadNotifications = allNotifications.Count(n => !n.IsRead),
            TotalCveMatches = allNotifications.Count,
            CveMatchesToday = allNotifications.Count(n => n.CreatedDate.Date == today),
            CveMatchesThisWeek = allNotifications.Count(n => n.CreatedDate >= today.AddDays(-7)),
            LastNotificationDate = allNotifications.Any() ? allNotifications.Max(n => n.CreatedDate) : DateTime.MinValue
        };

        // Get statistics for each subscription
        foreach (var subscription in subscriptions)
        {
            var stats = await GetStatisticsAsync(subscription.Id);
            if (stats != null)
            {
                summary.SubscriptionStats.Add(stats);
            }
        }

        return summary;
    }

    /// <summary>
    /// Create default subscriptions for user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of created subscriptions</returns>
    public async Task<List<CveSubscription>> CreateDefaultSubscriptionsAsync(string userId)
    {
        var defaultSubscriptions = new List<CveSubscription>
        {
            new CveSubscription
            {
                Id = Guid.NewGuid(),
                Name = "Critical CVEs",
                Description = "All critical severity CVEs",
                IsActive = true,
                Vendor = "",
                Product = "",
                MinCvssScore = 9.0,
                Keywords = "[]",
                CweFilter = "[]",
                NotificationFrequency = "Immediate",
                NotificationMethod = "Email",
                WebhookUrl = "",
                UserId = userId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            },
            new CveSubscription
            {
                Id = Guid.NewGuid(),
                Name = "Known Exploited CVEs",
                Description = "CVEs in CISA KEV catalog",
                IsActive = true,
                Vendor = "",
                Product = "",
                OnlyKnownExploited = true,
                Keywords = "[]",
                CweFilter = "[]",
                NotificationFrequency = "Immediate",
                NotificationMethod = "Email",
                WebhookUrl = "",
                UserId = userId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            }
        };

        foreach (var subscription in defaultSubscriptions)
        {
            await AddAsync(subscription);
        }

        await Context.SaveChangesAsync();
        return defaultSubscriptions;
    }

    /// <summary>
    /// Create subscription from project targets
    /// </summary>
    /// <param name="projectId">Project identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>Created subscription</returns>
    public async Task<CveSubscription> CreateFromProjectTargetsAsync(Guid projectId, string userId)
    {
        var project = await Context.Set<Project>().FindAsync(projectId);
        if (project == null)
        {
            return null;
        }

        var subscription = new CveSubscription
        {
            Id = Guid.NewGuid(),
            Name = $"Project {project.Name} CVEs",
            Description = $"CVEs relevant to project {project.Name}",
            IsActive = true,
            ProjectId = projectId,
            NotificationFrequency = "Daily",
            NotificationMethod = "Email",
            UserId = userId,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        return await AddAsync(subscription);
    }

    /// <summary>
    /// Validate subscription configuration
    /// </summary>
    /// <param name="subscription">CVE subscription</param>
    /// <returns>Validation result</returns>
    public async Task<CveSubscriptionValidationResult> ValidateSubscriptionAsync(CveSubscription subscription)
    {
        var result = new CveSubscriptionValidationResult { IsValid = true };

        // Validate name
        if (string.IsNullOrEmpty(subscription.Name))
        {
            result.Errors.Add("Subscription name is required");
            result.IsValid = false;
        }

        // Validate CVSS score range
        if (subscription.MinCvssScore.HasValue && subscription.MaxCvssScore.HasValue)
        {
            if (subscription.MinCvssScore > subscription.MaxCvssScore)
            {
                result.Errors.Add("Minimum CVSS score cannot be greater than maximum CVSS score");
                result.IsValid = false;
            }
        }

        // Validate CVSS score values
        if (subscription.MinCvssScore.HasValue && (subscription.MinCvssScore < 0 || subscription.MinCvssScore > 10))
        {
            result.Errors.Add("Minimum CVSS score must be between 0.0 and 10.0");
            result.IsValid = false;
        }

        if (subscription.MaxCvssScore.HasValue && (subscription.MaxCvssScore < 0 || subscription.MaxCvssScore > 10))
        {
            result.Errors.Add("Maximum CVSS score must be between 0.0 and 10.0");
            result.IsValid = false;
        }

        // Validate EPSS score
        if (subscription.MinEpssScore.HasValue && (subscription.MinEpssScore < 0 || subscription.MinEpssScore > 1))
        {
            result.Errors.Add("Minimum EPSS score must be between 0.0 and 1.0");
            result.IsValid = false;
        }

        // Validate notification frequency
        var validFrequencies = new[] { "Immediate", "Daily", "Weekly" };
        if (!string.IsNullOrEmpty(subscription.NotificationFrequency) && !validFrequencies.Contains(subscription.NotificationFrequency))
        {
            result.Errors.Add("Invalid notification frequency. Valid values are: Immediate, Daily, Weekly");
            result.IsValid = false;
        }

        // Validate notification method
        var validMethods = new[] { "Email", "InApp", "Webhook" };
        if (!string.IsNullOrEmpty(subscription.NotificationMethod) && !validMethods.Contains(subscription.NotificationMethod))
        {
            result.Errors.Add("Invalid notification method. Valid values are: Email, InApp, Webhook");
            result.IsValid = false;
        }

        // Validate webhook URL if webhook method is selected
        if (subscription.NotificationMethod == "Webhook" && string.IsNullOrEmpty(subscription.WebhookUrl))
        {
            result.Errors.Add("Webhook URL is required when notification method is Webhook");
            result.IsValid = false;
        }

        // Check for duplicate subscription names for the same user
        var existingSubscription = await Context.Set<CveSubscription>()
            .FirstOrDefaultAsync(s => s.Name == subscription.Name && s.UserId == subscription.UserId && s.Id != subscription.Id);

        if (existingSubscription != null)
        {
            result.Errors.Add("A subscription with this name already exists");
            result.IsValid = false;
        }

        // Add suggestions
        if (!subscription.MinCvssScore.HasValue && !subscription.MaxCvssScore.HasValue)
        {
            result.Suggestions.Add("Consider setting CVSS score filters to reduce noise");
        }

        if (string.IsNullOrEmpty(subscription.Vendor) && string.IsNullOrEmpty(subscription.Product))
        {
            result.Suggestions.Add("Consider adding vendor or product filters to make the subscription more specific");
        }

        return result;
    }
}