using Cervantes.CORE.Entities;
using Cervantes.IFR.CveServices.Models;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Interface for NVD API service
/// </summary>
public interface INvdApiService
{
    /// <summary>
    /// Get CVEs from NVD API with pagination
    /// </summary>
    /// <param name="startIndex">Start index for pagination</param>
    /// <param name="resultsPerPage">Number of results per page</param>
    /// <param name="lastModStartDate">Filter by last modified start date</param>
    /// <param name="lastModEndDate">Filter by last modified end date</param>
    /// <param name="pubStartDate">Filter by published start date</param>
    /// <param name="pubEndDate">Filter by published end date</param>
    /// <param name="cvssV3Severity">Filter by CVSS v3 severity</param>
    /// <param name="keywordSearch">Keyword search</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>NVD API response</returns>
    Task<NvdApiResponse> GetCvesAsync(
        int startIndex = 0,
        int resultsPerPage = 50,
        DateTime? lastModStartDate = null,
        DateTime? lastModEndDate = null,
        DateTime? pubStartDate = null,
        DateTime? pubEndDate = null,
        string cvssV3Severity = null,
        string keywordSearch = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get specific CVE by ID
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>CVE data</returns>
    Task<NvdCveItem> GetCveByIdAsync(string cveId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get CVEs modified since last sync
    /// </summary>
    /// <param name="lastSyncDate">Last sync date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of modified CVEs</returns>
    Task<List<NvdCveItem>> GetModifiedCvesSinceAsync(DateTime lastSyncDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Convert NVD CVE item to Cervantes CVE entity
    /// </summary>
    /// <param name="nvdCveItem">NVD CVE item</param>
    /// <param name="userId">User ID for audit (optional - will use admin user if not provided)</param>
    /// <returns>Cervantes CVE entity</returns>
    Cve ConvertToCveEntity(NvdCveItem nvdCveItem, string userId = null);

    /// <summary>
    /// Test API connection
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if connection is successful</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Test basic API connectivity with detailed logging
    /// </summary>
    /// <returns>True if API is accessible</returns>
    Task<bool> TestApiConnectivityAsync();
}