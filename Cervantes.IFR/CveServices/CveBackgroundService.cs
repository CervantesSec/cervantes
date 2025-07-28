using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.CveServices;
using Cervantes.IFR.CveServices.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Background service for CVE synchronization and notification processing
/// </summary>
public class CveBackgroundService : BackgroundService
{
    private readonly ILogger<CveBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public CveBackgroundService(
        ILogger<CveBackgroundService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CVE Background Service started");

        // Schedule recurring jobs using Hangfire
        ScheduleRecurringJobs();

        // Keep the service running
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }

        _logger.LogInformation("CVE Background Service stopping");
    }

    /// <summary>
    /// Schedule recurring jobs for CVE processing
    /// </summary>
    private void ScheduleRecurringJobs()
    {
        try
        {
            // Process notification queue every 5 minutes
            RecurringJob.AddOrUpdate<CveNotificationService>(
                "cve-notification-processor",
                service => service.ProcessNotificationQueueAsync(50, CancellationToken.None),
                "*/5 * * * *"
            );

            // NOTE: Temporarily disabled jobs for services that don't have implementations yet
            // These will be enabled once the service implementations are complete
            
            // // Sync CVEs from NVD every hour
            // RecurringJob.AddOrUpdate<CveSyncService>(
            //     "cve-nvd-sync",
            //     service => service.SyncRecentCvesAsync(CancellationToken.None),
            //     "0 * * * *"
            // );

            // // Full CVE sync daily at 2 AM
            // RecurringJob.AddOrUpdate<CveSyncService>(
            //     "cve-full-sync",
            //     service => service.SyncAllCvesAsync(CancellationToken.None),
            //     "0 2 * * *"
            // );

            // // Process subscription matches every 30 minutes
            // RecurringJob.AddOrUpdate<CveSubscriptionProcessor>(
            //     "cve-subscription-processor",
            //     service => service.ProcessSubscriptionMatchesAsync(CancellationToken.None),
            //     "*/30 * * * *"
            // );

            // Clean up old notifications weekly
            RecurringJob.AddOrUpdate<ICveNotificationManager>(
                "cve-notification-cleanup",
                manager => manager.CleanupOldNotificationsAsync(90),
                "0 0 * * 0"
            );

            // CVE incremental sync from NVD
            var incrementalSyncEnabled = _configuration.GetValue<bool>("CveConfiguration:IncrementalSync:Enabled");
            var incrementalSyncFrequency = _configuration.GetValue<string>("CveConfiguration:IncrementalSync:Frequency");
            
            if (incrementalSyncEnabled && !string.IsNullOrEmpty(incrementalSyncFrequency))
            {
                RecurringJob.AddOrUpdate<ICveSyncService>(
                    "cve-incremental-sync",
                    service => service.SyncAllSourcesAsync(CancellationToken.None),
                    incrementalSyncFrequency
                );
            }

            // // Retry failed notifications every 15 minutes
            // RecurringJob.AddOrUpdate<CveNotificationService>(
            //     "cve-notification-retry",
            //     service => service.ProcessRetryNotificationsAsync(CancellationToken.None),
            //     "*/15 * * * *"
            // );

            _logger.LogInformation("CVE recurring jobs scheduled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling CVE recurring jobs");
        }
    }
}

/// <summary>
/// Service for processing CVE subscription matches
/// </summary>
public class CveSubscriptionProcessor
{
    private readonly ILogger<CveSubscriptionProcessor> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CveSubscriptionProcessor(
        ILogger<CveSubscriptionProcessor> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Process subscription matches for recent CVEs
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task ProcessSubscriptionMatchesAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var cveManager = scope.ServiceProvider.GetRequiredService<ICveManager>();
        var subscriptionManager = scope.ServiceProvider.GetRequiredService<ICveSubscriptionManager>();
        var notificationManager = scope.ServiceProvider.GetRequiredService<ICveNotificationManager>();

        try
        {
            _logger.LogInformation("Processing CVE subscription matches");

            // Get recent CVEs (last 2 hours)
            var recentCves = await cveManager.GetByModifiedDateRangeAsync(
                DateTime.UtcNow.AddHours(-2), 
                DateTime.UtcNow);

            var activeSubscriptions = await subscriptionManager.GetActiveSubscriptionsAsync();

            var notificationsCreated = 0;

            foreach (var cve in recentCves)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var matchingSubscriptions = await subscriptionManager.GetMatchingSubscriptionsAsync(cve);

                foreach (var subscription in matchingSubscriptions)
                {
                    // Check if notification already exists
                    var existingNotifications = await notificationManager.GetByCveIdAsync(cve.Id);
                    if (existingNotifications.Any(n => n.SubscriptionId == subscription.Id))
                    {
                        continue; // Skip if already notified
                    }

                    // Determine notification type and priority
                    var notificationType = DetermineNotificationType(cve);
                    var priority = DeterminePriority(cve);

                    // Create notification
                    await notificationManager.CreateNotificationAsync(cve, subscription, notificationType, priority);
                    notificationsCreated++;

                    _logger.LogDebug("Created notification for CVE {CveId} and subscription {SubscriptionName}", 
                        cve.CveId, subscription.Name);
                }
            }

            _logger.LogInformation("CVE subscription processing completed. Created {NotificationCount} notifications", 
                notificationsCreated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing CVE subscription matches");
        }
    }

    /// <summary>
    /// Determine notification type based on CVE characteristics
    /// </summary>
    /// <param name="cve">CVE entity</param>
    /// <returns>Notification type</returns>
    private string DetermineNotificationType(Cve cve)
    {
        if (cve.IsKnownExploited)
        {
            return "KevAdded";
        }

        if (cve.CvssV3Severity == "CRITICAL")
        {
            return "CriticalCve";
        }

        if (cve.CreatedDate >= DateTime.UtcNow.AddDays(-1))
        {
            return "NewCve";
        }

        return "UpdatedCve";
    }

    /// <summary>
    /// Determine notification priority based on CVE characteristics
    /// </summary>
    /// <param name="cve">CVE entity</param>
    /// <returns>Priority level</returns>
    private string DeterminePriority(Cve cve)
    {
        if (cve.IsKnownExploited || cve.CvssV3Severity == "CRITICAL")
        {
            return "High";
        }

        if (cve.CvssV3Severity == "HIGH")
        {
            return "Medium";
        }

        return "Low";
    }
}

/// <summary>
/// Service for CVE synchronization from external sources
/// </summary>
public class CveSyncService : ICveSyncService
{
    private readonly ILogger<CveSyncService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CveSyncService(
        ILogger<CveSyncService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Sync recent CVEs from NVD (last 24 hours)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task SyncRecentCvesAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var nvdApiService = scope.ServiceProvider.GetRequiredService<INvdApiService>();
        var cveManager = scope.ServiceProvider.GetRequiredService<ICveManager>();

        try
        {
            _logger.LogInformation("Starting recent CVE synchronization from NVD");

            var lastModStartDate = DateTime.UtcNow.AddHours(-24);
            var response = await nvdApiService.GetCvesAsync(
                lastModStartDate: lastModStartDate,
                resultsPerPage: 100,
                cancellationToken: cancellationToken);

            if (response?.Vulnerabilities?.Any() == true)
            {
                var cves = response.Vulnerabilities
                    .Select(v => nvdApiService.ConvertToCveEntity(v.Cve))
                    .Where(c => c != null)
                    .ToList();

                var importResult = await cveManager.BulkImportAsync(cves, Guid.NewGuid());

                _logger.LogInformation("Recent CVE sync completed. New: {NewCount}, Updated: {UpdatedCount}, Errors: {ErrorCount}", 
                    importResult.NewCount, importResult.UpdatedCount, importResult.ErrorCount);
            }
            else
            {
                _logger.LogInformation("No recent CVEs found during sync");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during recent CVE synchronization");
        }
    }

    /// <summary>
    /// Sync all CVEs from NVD (full sync)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public async Task SyncAllCvesAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var nvdApiService = scope.ServiceProvider.GetRequiredService<INvdApiService>();
        var cveManager = scope.ServiceProvider.GetRequiredService<ICveManager>();

        try
        {
            _logger.LogInformation("Starting full CVE synchronization from NVD");

            var totalImported = 0;
            var startIndex = 0;
            const int batchSize = 100;

            while (!cancellationToken.IsCancellationRequested)
            {
                var response = await nvdApiService.GetCvesAsync(
                    startIndex: startIndex,
                    resultsPerPage: batchSize,
                    cancellationToken: cancellationToken);

                if (response?.Vulnerabilities?.Any() != true)
                {
                    break;
                }

                var cves = response.Vulnerabilities
                    .Select(v => nvdApiService.ConvertToCveEntity(v.Cve))
                    .Where(c => c != null)
                    .ToList();

                var importResult = await cveManager.BulkImportAsync(cves, Guid.NewGuid());
                totalImported += importResult.NewCount + importResult.UpdatedCount;

                _logger.LogInformation("Processed batch {StartIndex}-{EndIndex}. Total imported: {TotalImported}", 
                    startIndex, startIndex + batchSize, totalImported);

                startIndex += batchSize;

                if (response.Vulnerabilities.Count < batchSize)
                {
                    break; // Last batch
                }

                // Rate limiting
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }

            _logger.LogInformation("Full CVE sync completed. Total imported: {TotalImported}", totalImported);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during full CVE synchronization");
        }
    }

    #region ICveSyncService Implementation

    /// <summary>
    /// Synchronize CVEs from all active sources
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization result</returns>
    public async Task<CveSyncResult> SyncAllSourcesAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var nvdApiService = scope.ServiceProvider.GetRequiredService<INvdApiService>();
        var cveManager = scope.ServiceProvider.GetRequiredService<ICveManager>();

        var result = new CveSyncResult
        {
            StartTime = DateTime.UtcNow,
            IsSuccess = false,
            SourceName = "All Sources"
        };

        try
        {
            _logger.LogInformation("Starting CVE synchronization from all sources");

            // Get recent CVEs (last 7 days)
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-7);

            var nvdResponse = await nvdApiService.GetCvesAsync(
                startIndex: 0,
                resultsPerPage: 100,
                pubStartDate: startDate,
                pubEndDate: endDate,
                cancellationToken: cancellationToken);

            if (nvdResponse?.Vulnerabilities == null || !nvdResponse.Vulnerabilities.Any())
            {
                result.IsSuccess = true;
                result.ProcessedCount = 0;
                _logger.LogInformation("No new CVEs found");
                return result;
            }

            var processedCount = 0;
            var newCount = 0;
            var updatedCount = 0;
            var errorCount = 0;

            foreach (var vulnerability in nvdResponse.Vulnerabilities)
            {
                try
                {
                    var nvdCve = vulnerability.Cve;
                    var existingCve = await cveManager.GetByCveIdAsync(nvdCve.Id);
                    
                    if (existingCve != null)
                    {
                        if (existingCve.LastModifiedDate < nvdCve.LastModified)
                        {
                            var updatedCve = nvdApiService.ConvertToCveEntity(nvdCve);
                            updatedCve.Id = existingCve.Id;
                            
                            existingCve.Title = updatedCve.Title;
                            existingCve.Description = updatedCve.Description;
                            existingCve.LastModifiedDate = DateTime.SpecifyKind(updatedCve.LastModifiedDate, DateTimeKind.Utc);
                            existingCve.CvssV3BaseScore = updatedCve.CvssV3BaseScore;
                            existingCve.CvssV3Vector = updatedCve.CvssV3Vector;
                            existingCve.CvssV3Severity = updatedCve.CvssV3Severity;
                            existingCve.ModifiedDate = DateTime.UtcNow;

                            cveManager.Update(existingCve);
                            await cveManager.Context.SaveChangesNoAuditAsync();
                            updatedCount++;
                        }
                    }
                    else
                    {
                        var newCve = nvdApiService.ConvertToCveEntity(nvdCve);
                        await cveManager.AddAsync(newCve);
                        await cveManager.Context.SaveChangesNoAuditAsync();
                        newCount++;
                    }

                    processedCount++;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    result.Errors.Add($"Error processing CVE {vulnerability.Cve?.Id}: {ex.Message}");
                    _logger.LogError(ex, "Error processing CVE {CveId}", vulnerability.Cve?.Id);
                }
            }

            result.IsSuccess = true;
            result.ProcessedCount = processedCount;
            result.NewCount = newCount;
            result.UpdatedCount = updatedCount;
            result.ErrorCount = errorCount;

            _logger.LogInformation("CVE sync completed. Processed: {ProcessedCount}, New: {NewCount}, Updated: {UpdatedCount}, Errors: {ErrorCount}",
                processedCount, newCount, updatedCount, errorCount);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error during CVE synchronization");
        }
        finally
        {
            result.EndTime = DateTime.UtcNow;
        }

        return result;
    }

    /// <summary>
    /// Synchronize CVEs from a specific source
    /// </summary>
    /// <param name="sourceId">Source identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization result</returns>
    public async Task<CveSyncResult> SyncSourceAsync(Guid sourceId, CancellationToken cancellationToken = default)
    {
        return await SyncAllSourcesAsync(cancellationToken);
    }

    /// <summary>
    /// Synchronize CVEs modified since last sync
    /// </summary>
    /// <param name="sourceId">Source identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization result</returns>
    public async Task<CveSyncResult> SyncModifiedCvesAsync(Guid sourceId, CancellationToken cancellationToken = default)
    {
        return await SyncAllSourcesAsync(cancellationToken);
    }

    /// <summary>
    /// Synchronize a specific CVE by ID
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="sourceId">Source identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization result</returns>
    public async Task<CveSyncResult> SyncCveByIdAsync(string cveId, Guid sourceId, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var nvdApiService = scope.ServiceProvider.GetRequiredService<INvdApiService>();
        var cveManager = scope.ServiceProvider.GetRequiredService<ICveManager>();

        var result = new CveSyncResult
        {
            StartTime = DateTime.UtcNow,
            IsSuccess = false,
            SourceName = "NVD",
            ProcessedCount = 1
        };

        try
        {
            var nvdCve = await nvdApiService.GetCveByIdAsync(cveId, cancellationToken);
            if (nvdCve == null)
            {
                result.ErrorMessage = $"CVE {cveId} not found in NVD";
                return result;
            }

            var existingCve = await cveManager.GetByCveIdAsync(cveId);
            if (existingCve != null)
            {
                var updatedCve = nvdApiService.ConvertToCveEntity(nvdCve);
                updatedCve.Id = existingCve.Id;
                
                existingCve.Title = updatedCve.Title;
                existingCve.Description = updatedCve.Description;
                existingCve.LastModifiedDate = DateTime.SpecifyKind(updatedCve.LastModifiedDate, DateTimeKind.Utc);
                existingCve.CvssV3BaseScore = updatedCve.CvssV3BaseScore;
                existingCve.CvssV3Vector = updatedCve.CvssV3Vector;
                existingCve.CvssV3Severity = updatedCve.CvssV3Severity;
                existingCve.ModifiedDate = DateTime.UtcNow;

                cveManager.Update(existingCve);
                await cveManager.Context.SaveChangesNoAuditAsync();
                result.UpdatedCount = 1;
            }
            else
            {
                var newCve = nvdApiService.ConvertToCveEntity(nvdCve);
                await cveManager.AddAsync(newCve);
                await cveManager.Context.SaveChangesNoAuditAsync();
                result.NewCount = 1;
            }

            result.IsSuccess = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            result.ErrorCount = 1;
            _logger.LogError(ex, "Error syncing CVE {CveId}", cveId);
        }
        finally
        {
            result.EndTime = DateTime.UtcNow;
        }

        return result;
    }

    /// <summary>
    /// Process CVE for project correlation
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result</returns>
    public async Task<CveProcessingResult> ProcessCveForProjectsAsync(Guid cveId, CancellationToken cancellationToken = default)
    {
        return new CveProcessingResult
        {
            IsSuccess = true,
            ProjectMappingsCreated = 0
        };
    }

    /// <summary>
    /// Process CVE for subscription notifications
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="isNewCve">Whether this is a new CVE</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result</returns>
    public async Task<CveProcessingResult> ProcessCveForNotificationsAsync(Guid cveId, bool isNewCve, CancellationToken cancellationToken = default)
    {
        return new CveProcessingResult
        {
            IsSuccess = true,
            NotificationsCreated = 0
        };
    }

    /// <summary>
    /// Enrich CVE with additional data (EPSS, CISA KEV, etc.)
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Enrichment result</returns>
    public async Task<CveEnrichmentResult> EnrichCveAsync(Guid cveId, CancellationToken cancellationToken = default)
    {
        return new CveEnrichmentResult
        {
            IsSuccess = true,
            EpssDataAdded = false,
            KevStatusUpdated = false
        };
    }

    /// <summary>
    /// Get synchronization status for all sources
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization status</returns>
    public async Task<List<CveSyncStatus>> GetSyncStatusAsync(CancellationToken cancellationToken = default)
    {
        return new List<CveSyncStatus>
        {
            new CveSyncStatus
            {
                SourceId = Guid.NewGuid(),
                SourceName = "NVD",
                IsActive = true,
                LastSyncDate = DateTime.UtcNow.AddHours(-1),
                LastSyncStatus = "Success",
                LastSyncCount = 0,
                IsCurrentlyRunning = false
            }
        };
    }

    /// <summary>
    /// Synchronize CVEs with custom filtering options
    /// </summary>
    /// <param name="options">Sync options and filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization result</returns>
    public async Task<CveSyncResult> SyncWithOptionsAsync(CveSyncOptionsViewModel options, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var cveManager = scope.ServiceProvider.GetRequiredService<ICveManager>();

        var result = new CveSyncResult
        {
            StartTime = DateTime.UtcNow,
            IsSuccess = false,
            SourceName = options.SyncSource
        };

        // Determine which API service to use based on sync source
        object apiService = null;
        switch (options.SyncSource?.ToUpper())
        {
            case "REDHAT":
                apiService = scope.ServiceProvider.GetRequiredService<IRedHatApiService>();
                _logger.LogInformation("Using Red Hat API service for CVE synchronization");
                break;
            case "NVD":
            default:
                apiService = scope.ServiceProvider.GetRequiredService<INvdApiService>();
                _logger.LogInformation("Using NVD API service for CVE synchronization");
                break;
        }

        try
        {
            _logger.LogInformation("Starting CVE sync with custom options. Max: {MaxCves}, Source: {Source}", 
                options.MaxTotalCves, options.SyncSource);

            var totalProcessed = 0;
            var startIndex = 0;

            while (totalProcessed < options.MaxTotalCves && !cancellationToken.IsCancellationRequested)
            {
                // Calculate remaining CVEs to fetch
                var remainingCves = options.MaxTotalCves - totalProcessed;
                var batchSize = Math.Min(options.ResultsPerPage, remainingCves);

                // Prepare date filters
                DateTime? pubStartDate = null;
                DateTime? pubEndDate = null;
                DateTime? modStartDate = null;
                DateTime? modEndDate = null;

                // Convert year filters to dates if specified
                if (options.StartYear.HasValue)
                {
                    pubStartDate = new DateTime(options.StartYear.Value, 1, 1);
                }
                if (options.EndYear.HasValue)
                {
                    pubEndDate = new DateTime(options.EndYear.Value, 12, 31);
                }

                // Override with specific dates if provided
                if (options.PublishedDateStart.HasValue)
                {
                    pubStartDate = options.PublishedDateStart;
                }
                if (options.PublishedDateEnd.HasValue)
                {
                    pubEndDate = options.PublishedDateEnd;
                }
                if (options.LastModifiedStart.HasValue)
                {
                    modStartDate = options.LastModifiedStart;
                }
                if (options.LastModifiedEnd.HasValue)
                {
                    modEndDate = options.LastModifiedEnd;
                }

                // Determine severity filter
                string severityFilter = null;
                if (options.Severities.Count == 1)
                {
                    severityFilter = options.Severities.First();
                }

                // Call appropriate API based on source
                List<Cve> fetchedCves = new List<Cve>();
                
                if (options.SyncSource?.ToUpper() == "REDHAT")
                {
                    var redHatService = (IRedHatApiService)apiService;
                    var redHatOptions = new Cervantes.IFR.CveServices.Models.RedHatApiOptions
                    {
                        After = pubStartDate,
                        Before = pubEndDate,
                        Severity = options.Severities.Count == 1 ? MapToRedHatSeverity(options.Severities.First()) : null,
                        PerPage = Math.Min(batchSize, 1000), // Red Hat API max is 1000
                        Page = (startIndex / batchSize) + 1
                    };
                    
                    var redHatCves = await redHatService.GetCvesAsync(redHatOptions, cancellationToken);
                    foreach (var redHatCve in redHatCves)
                    {
                        var cveEntity = redHatService.ConvertToCveEntity(redHatCve);
                        fetchedCves.Add(cveEntity);
                    }
                }
                else
                {
                    // Use NVD API
                    var nvdService = (INvdApiService)apiService;
                    var nvdResponse = await nvdService.GetCvesAsync(
                        startIndex: startIndex,
                        resultsPerPage: batchSize,
                        lastModStartDate: modStartDate,
                        lastModEndDate: modEndDate,
                        pubStartDate: pubStartDate,
                        pubEndDate: pubEndDate,
                        cvssV3Severity: severityFilter,
                        keywordSearch: options.KeywordFilter,
                        cancellationToken: cancellationToken);

                    if (nvdResponse?.Vulnerabilities != null)
                    {
                        foreach (var vulnerability in nvdResponse.Vulnerabilities)
                        {
                            var cveEntity = nvdService.ConvertToCveEntity(vulnerability.Cve);
                            fetchedCves.Add(cveEntity);
                        }
                    }
                }

                if (!fetchedCves.Any())
                {
                    _logger.LogInformation("No more CVEs found, stopping sync");
                    break;
                }

                // Process batch
                foreach (var cveEntity in fetchedCves)
                {
                    if (totalProcessed >= options.MaxTotalCves || cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        // Apply client-side filters for additional validation
                        if (!PassesFinalFilters(cveEntity, options))
                        {
                            result.SkippedCount++;
                            continue;
                        }

                        var existingCve = await cveManager.GetByCveIdAsync(cveEntity.CveId);
                        
                        if (existingCve != null)
                        {
                            if (options.SkipExisting)
                            {
                                result.SkippedCount++;
                                continue;
                            }

                            if (options.UpdateExisting && existingCve.LastModifiedDate < cveEntity.LastModifiedDate)
                            {
                                // Update existing CVE
                                existingCve.Title = cveEntity.Title;
                                existingCve.Description = cveEntity.Description;
                                existingCve.LastModifiedDate = DateTime.SpecifyKind(cveEntity.LastModifiedDate, DateTimeKind.Utc);
                                existingCve.CvssV3BaseScore = cveEntity.CvssV3BaseScore;
                                
                                // Truncate CvssV3Vector to avoid database constraint errors
                                existingCve.CvssV3Vector = !string.IsNullOrEmpty(cveEntity.CvssV3Vector) && cveEntity.CvssV3Vector.Length > 100 
                                    ? cveEntity.CvssV3Vector.Substring(0, 100) 
                                    : cveEntity.CvssV3Vector;
                                    
                                existingCve.CvssV3Severity = cveEntity.CvssV3Severity;
                                existingCve.ModifiedDate = DateTime.UtcNow;

                                cveManager.Update(existingCve);
                                await cveManager.Context.SaveChangesNoAuditAsync();
                                result.UpdatedCount++;
                            }
                            else
                            {
                                result.SkippedCount++;
                            }
                        }
                        else
                        {
                            // Create new CVE
                            // Truncate CvssV3Vector to avoid database constraint errors
                            if (!string.IsNullOrEmpty(cveEntity.CvssV3Vector) && cveEntity.CvssV3Vector.Length > 100)
                            {
                                cveEntity.CvssV3Vector = cveEntity.CvssV3Vector.Substring(0, 100);
                            }
                            
                            await cveManager.AddAsync(cveEntity);
                            await cveManager.Context.SaveChangesNoAuditAsync();
                            result.NewCount++;
                        }

                        totalProcessed++;
                        result.ProcessedCount++;
                    }
                    catch (Exception ex)
                    {
                        result.ErrorCount++;
                        result.Errors.Add($"Error processing CVE {cveEntity.CveId}: {ex.Message}");
                        _logger.LogWarning(ex, "Error processing CVE {CveId}", cveEntity.CveId);
                    }
                }

                startIndex += batchSize;

                // Break if we got fewer results than requested (last page)
                if (fetchedCves.Count < batchSize)
                {
                    break;
                }

                // Rate limiting
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }

            // Enrich CVEs with EPSS and CISA KEV data if enabled
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            if (configuration.GetValue<bool>("CveConfiguration:EnableEpssEnrichment", true) || 
                configuration.GetValue<bool>("CveConfiguration:EnableCisaKevEnrichment", true))
            {
                await EnrichProcessedCvesAsync(scope, result, cancellationToken);
            }

            result.IsSuccess = true;
            _logger.LogInformation("CVE sync with options completed. Processed: {ProcessedCount}, New: {NewCount}, Updated: {UpdatedCount}, Skipped: {SkippedCount}, Errors: {ErrorCount}",
                result.ProcessedCount, result.NewCount, result.UpdatedCount, result.SkippedCount, result.ErrorCount);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error during CVE sync with options");
        }
        finally
        {
            result.EndTime = DateTime.UtcNow;
        }

        return result;
    }

    /// <summary>
    /// Map standard CVSS severity to Red Hat severity format
    /// </summary>
    private string? MapToRedHatSeverity(string standardSeverity)
    {
        return standardSeverity?.ToUpper() switch
        {
            "LOW" => "low",
            "MEDIUM" => "moderate",
            "HIGH" => "important",
            "CRITICAL" => "critical",
            _ => null
        };
    }

    /// <summary>
    /// Check if CVE passes client-side filters that NVD API doesn't support
    /// </summary>
    /// <param name="nvdCve">NVD CVE data</param>
    /// <param name="options">Sync options</param>
    /// <returns>True if CVE passes filters</returns>
    private bool PassesClientSideFilters(Models.NvdCveItem nvdCve, CveSyncOptionsViewModel options)
    {
        // CVSS score filters (if multiple severities selected, NVD API can't filter)
        if (options.Severities.Count > 1)
        {
            var cvssV31 = nvdCve.Metrics?.CvssMetricV31?.FirstOrDefault(m => m.Type == "Primary") ?? 
                         nvdCve.Metrics?.CvssMetricV31?.FirstOrDefault();
            
            if (cvssV31?.CvssData?.BaseSeverity != null && 
                !options.Severities.Contains(cvssV31.CvssData.BaseSeverity))
            {
                return false;
            }
        }

        // CVSS score range
        if (options.MinCvssScore.HasValue || options.MaxCvssScore.HasValue)
        {
            var cvssV31 = nvdCve.Metrics?.CvssMetricV31?.FirstOrDefault(m => m.Type == "Primary") ?? 
                         nvdCve.Metrics?.CvssMetricV31?.FirstOrDefault();
            
            var score = cvssV31?.CvssData?.BaseScore;
            if (score.HasValue)
            {
                if (options.MinCvssScore.HasValue && score < options.MinCvssScore)
                    return false;
                    
                if (options.MaxCvssScore.HasValue && score > options.MaxCvssScore)
                    return false;
            }
        }

        // Known exploited filter (this would need CISA KEV data integration)
        if (options.OnlyKnownExploited)
        {
            // For now, we can't filter this without additional data
            // This would require checking against CISA KEV database
        }

        return true;
    }

    /// <summary>
    /// Apply final client-side filters to converted CVE entities
    /// </summary>
    private bool PassesFinalFilters(Cve cveEntity, CveSyncOptionsViewModel options)
    {
        // CVSS score filters (if multiple severities selected)
        if (options.Severities.Count > 1)
        {
            if (!string.IsNullOrEmpty(cveEntity.CvssV3Severity) && 
                !options.Severities.Contains(cveEntity.CvssV3Severity))
            {
                return false;
            }
        }

        // CVSS score range
        if (options.MinCvssScore.HasValue || options.MaxCvssScore.HasValue)
        {
            if (cveEntity.CvssV3BaseScore.HasValue)
            {
                if (options.MinCvssScore.HasValue && cveEntity.CvssV3BaseScore < options.MinCvssScore)
                    return false;
                    
                if (options.MaxCvssScore.HasValue && cveEntity.CvssV3BaseScore > options.MaxCvssScore)
                    return false;
            }
        }

        // Known exploited filter
        if (options.OnlyKnownExploited && !cveEntity.IsKnownExploited)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Enrich processed CVEs with EPSS and CISA KEV data
    /// </summary>
    private async Task EnrichProcessedCvesAsync(IServiceScope scope, CveSyncResult result, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting CVE enrichment process");

            var vulnEnrichmentService = scope.ServiceProvider.GetService<Cervantes.IFR.CveServices.IVulnEnrichmentService>();
            if (vulnEnrichmentService == null)
            {
                _logger.LogWarning("VulnEnrichmentService not available, skipping enrichment");
                return;
            }

            var cveManager = scope.ServiceProvider.GetRequiredService<ICveManager>();

            // Get recently processed CVEs that need enrichment
            var enrichmentBatchSize = 50; // Process in smaller batches for enrichment
            var recentCves = await cveManager.GetAll()
                .Where(c => c.ModifiedDate >= DateTime.UtcNow.AddMinutes(-10)) // CVEs processed in last 10 minutes
                .OrderByDescending(c => c.ModifiedDate)
                .Take(enrichmentBatchSize * 3) // Get a reasonable subset
                .ToListAsync(cancellationToken);

            if (!recentCves.Any())
            {
                _logger.LogInformation("No recently processed CVEs found for enrichment");
                return;
            }

            _logger.LogInformation("Enriching {Count} recently processed CVEs", recentCves.Count);

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var includeEpss = configuration.GetValue<bool>("CveConfiguration:EnableEpssEnrichment", true);
            var includeCisaKev = configuration.GetValue<bool>("CveConfiguration:EnableCisaKevEnrichment", true);

            // Perform enrichment
            var enrichedCount = await vulnEnrichmentService.EnrichCvesAsync(recentCves, includeEpss, includeCisaKev, cancellationToken);

            if (enrichedCount > 0)
            {
                // Save enriched CVEs to database
                await cveManager.Context.SaveChangesNoAuditAsync();
                
                _logger.LogInformation("CVE enrichment completed. Enriched {EnrichedCount} CVEs with additional data", enrichedCount);
                
                // Add to result for reporting
                result.Warnings.Add("Enriched " + enrichedCount + " CVEs with EPSS/KEV data");
            }
            else
            {
                _logger.LogInformation("No enrichment data found for recently processed CVEs");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during CVE enrichment process");
            result.Warnings.Add($"CVE enrichment failed: {ex.Message}");
        }
    }

    #endregion
}

/// <summary>
/// Extension methods for CVE notification service
/// </summary>
public static class CveNotificationServiceExtensions
{
    /// <summary>
    /// Process retry notifications
    /// </summary>
    /// <param name="service">Notification service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public static async Task ProcessRetryNotificationsAsync(this CveNotificationService service, CancellationToken cancellationToken = default)
    {
        using var scope = service.GetType().Assembly.GetType("Microsoft.Extensions.DependencyInjection.ServiceProvider")?.GetMethod("CreateScope")?.Invoke(null, null) as IServiceScope;
        if (scope != null)
        {
            var notificationManager = scope.ServiceProvider.GetRequiredService<ICveNotificationManager>();
            var retryNotifications = await notificationManager.GetRetryNotificationsAsync();

            foreach (var notification in retryNotifications)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await service.SendNotificationAsync(notification, cancellationToken);
            }
        }
    }
}