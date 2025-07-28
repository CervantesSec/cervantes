using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Interface for CVE synchronization service
/// </summary>
public interface ICveSyncService
{
    /// <summary>
    /// Synchronize CVEs from all active sources
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization result</returns>
    Task<CveSyncResult> SyncAllSourcesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronize CVEs from a specific source
    /// </summary>
    /// <param name="sourceId">Source identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization result</returns>
    Task<CveSyncResult> SyncSourceAsync(Guid sourceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronize CVEs modified since last sync
    /// </summary>
    /// <param name="sourceId">Source identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization result</returns>
    Task<CveSyncResult> SyncModifiedCvesAsync(Guid sourceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronize a specific CVE by ID
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="sourceId">Source identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization result</returns>
    Task<CveSyncResult> SyncCveByIdAsync(string cveId, Guid sourceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Process CVE for project correlation
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result</returns>
    Task<CveProcessingResult> ProcessCveForProjectsAsync(Guid cveId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Process CVE for subscription notifications
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="isNewCve">Whether this is a new CVE</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result</returns>
    Task<CveProcessingResult> ProcessCveForNotificationsAsync(Guid cveId, bool isNewCve, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enrich CVE with additional data (EPSS, CISA KEV, etc.)
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Enrichment result</returns>
    Task<CveEnrichmentResult> EnrichCveAsync(Guid cveId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get synchronization status for all sources
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization status</returns>
    Task<List<CveSyncStatus>> GetSyncStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronize CVEs with custom filtering options
    /// </summary>
    /// <param name="options">Sync options and filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization result</returns>
    Task<CveSyncResult> SyncWithOptionsAsync(CveSyncOptionsViewModel options, CancellationToken cancellationToken = default);
}

/// <summary>
/// CVE synchronization result
/// </summary>
public class CveSyncResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public int ProcessedCount { get; set; }
    public int NewCount { get; set; }
    public int UpdatedCount { get; set; }
    public int SkippedCount { get; set; }
    public int ErrorCount { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
    public string SourceName { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// CVE processing result
/// </summary>
public class CveProcessingResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public int ProjectMappingsCreated { get; set; }
    public int NotificationsCreated { get; set; }
    public int VulnerabilityMappingsCreated { get; set; }
    public List<string> ProcessedItems { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// CVE enrichment result
/// </summary>
public class CveEnrichmentResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public bool EpssDataAdded { get; set; }
    public bool KevStatusUpdated { get; set; }
    public bool AdditionalReferencesAdded { get; set; }
    public List<string> EnrichmentSources { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// CVE synchronization status
/// </summary>
public class CveSyncStatus
{
    public Guid SourceId { get; set; }
    public string SourceName { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastSyncDate { get; set; }
    public string LastSyncStatus { get; set; }
    public string LastSyncError { get; set; }
    public int LastSyncCount { get; set; }
    public DateTime? NextSyncDate { get; set; }
    public bool IsCurrentlyRunning { get; set; }
}