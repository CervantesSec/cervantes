using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;

namespace Cervantes.Contracts;

/// <summary>
/// Interface for CVE Manager
/// </summary>
public interface ICveManager : IGenericManager<Cve>
{
    /// <summary>
    /// Get CVE by CVE identifier
    /// </summary>
    /// <param name="cveId">CVE identifier (e.g., CVE-2023-1234)</param>
    /// <returns>CVE entity</returns>
    Task<Cve> GetByCveIdAsync(string cveId);

    /// <summary>
    /// Get CVEs by multiple CVE identifiers
    /// </summary>
    /// <param name="cveIds">Collection of CVE identifiers</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetCvesByIdsAsync(IEnumerable<string> cveIds);

    /// <summary>
    /// Get CVEs by CVSS score range
    /// </summary>
    /// <param name="minScore">Minimum CVSS score</param>
    /// <param name="maxScore">Maximum CVSS score</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetByCvssScoreRangeAsync(double minScore, double maxScore);

    /// <summary>
    /// Get CVEs by severity
    /// </summary>
    /// <param name="severity">CVSS severity (LOW, MEDIUM, HIGH, CRITICAL)</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetBySeverityAsync(string severity);

    /// <summary>
    /// Get CVEs published within date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetByPublishedDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get CVEs modified within date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetByModifiedDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get CVEs by vendor
    /// </summary>
    /// <param name="vendor">Vendor name</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetByVendorAsync(string vendor);

    /// <summary>
    /// Get CVEs by product
    /// </summary>
    /// <param name="product">Product name</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetByProductAsync(string product);

    /// <summary>
    /// Get CVEs by CWE ID
    /// </summary>
    /// <param name="cweId">CWE identifier</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetByCweIdAsync(string cweId);

    /// <summary>
    /// Get known exploited CVEs (CISA KEV)
    /// </summary>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetKnownExploitedAsync();

    /// <summary>
    /// Get CVEs by EPSS score range
    /// </summary>
    /// <param name="minScore">Minimum EPSS score</param>
    /// <param name="maxScore">Maximum EPSS score</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetByEpssScoreRangeAsync(double minScore, double maxScore);

    /// <summary>
    /// Search CVEs by keyword
    /// </summary>
    /// <param name="keyword">Search keyword</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> SearchAsync(string keyword);

    /// <summary>
    /// Get CVEs with advanced search criteria
    /// </summary>
    /// <param name="searchModel">Search criteria</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetWithAdvancedSearchAsync(CveSearchViewModel searchModel);

    /// <summary>
    /// Get CVE statistics
    /// </summary>
    /// <returns>CVE statistics</returns>
    Task<CveStatistics> GetStatisticsAsync();

    /// <summary>
    /// Get CVE count by severity
    /// </summary>
    /// <returns>Dictionary of severity counts</returns>
    Task<Dictionary<string, int>> GetCountBySeverityAsync();

    /// <summary>
    /// Get CVE count by date range
    /// </summary>
    /// <param name="days">Number of days</param>
    /// <returns>Dictionary of date counts</returns>
    Task<Dictionary<DateTime, int>> GetCountByDateRangeAsync(int days);

    /// <summary>
    /// Get top affected vendors
    /// </summary>
    /// <param name="limit">Number of vendors to return</param>
    /// <returns>Dictionary of vendor counts</returns>
    Task<Dictionary<string, int>> GetTopVendorsAsync(int limit = 10);

    /// <summary>
    /// Get top affected products
    /// </summary>
    /// <param name="limit">Number of products to return</param>
    /// <returns>Dictionary of product counts</returns>
    Task<Dictionary<string, int>> GetTopProductsAsync(int limit = 10);

    /// <summary>
    /// Mark CVE as read
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    Task<bool> MarkAsReadAsync(Guid cveId, string userId);

    /// <summary>
    /// Mark CVE as favorite
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    Task<bool> MarkAsFavoriteAsync(Guid cveId, string userId);

    /// <summary>
    /// Archive CVE
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    Task<bool> ArchiveAsync(Guid cveId, string userId);

    /// <summary>
    /// Add or update CVE notes
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="notes">Notes text</param>
    /// <param name="userId">User identifier</param>
    /// <returns>True if successful</returns>
    Task<bool> UpdateNotesAsync(Guid cveId, string notes, string userId);

    /// <summary>
    /// Get CVEs for specific project
    /// </summary>
    /// <param name="projectId">Project identifier</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetByProjectAsync(Guid projectId);

    /// <summary>
    /// Get CVEs for user subscriptions
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>List of CVE entities</returns>
    Task<List<Cve>> GetByUserSubscriptionsAsync(string userId);

    /// <summary>
    /// Create or update CVE from external source
    /// </summary>
    /// <param name="cve">CVE entity</param>
    /// <param name="sourceId">Source identifier</param>
    /// <returns>Created or updated CVE</returns>
    Task<Cve> CreateOrUpdateFromSourceAsync(Cve cve, Guid sourceId);

    /// <summary>
    /// Bulk import CVEs
    /// </summary>
    /// <param name="cves">List of CVE entities</param>
    /// <param name="sourceId">Source identifier</param>
    /// <returns>Import result</returns>
    Task<CveImportResult> BulkImportAsync(List<Cve> cves, Guid sourceId);
    
    /// <summary>
    /// Delete multiple CVEs by their IDs
    /// </summary>
    /// <param name="cveIds">List of CVE IDs to delete</param>
    /// <returns>Number of deleted CVEs</returns>
    Task<int> DeleteMultipleAsync(List<Guid> cveIds);
}

/// <summary>
/// CVE statistics model
/// </summary>
public class CveStatistics
{
    public int TotalCves { get; set; }
    public int CriticalCves { get; set; }
    public int HighCves { get; set; }
    public int MediumCves { get; set; }
    public int LowCves { get; set; }
    public int KnownExploitedCves { get; set; }
    public int NewCvesToday { get; set; }
    public int NewCvesThisWeek { get; set; }
    public int NewCvesThisMonth { get; set; }
    public double AverageCvssScore { get; set; }
    public double AverageEpssScore { get; set; }
    public DateTime LastUpdateDate { get; set; }
}

/// <summary>
/// CVE import result model
/// </summary>
public class CveImportResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public int ProcessedCount { get; set; }
    public int NewCount { get; set; }
    public int UpdatedCount { get; set; }
    public int SkippedCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}