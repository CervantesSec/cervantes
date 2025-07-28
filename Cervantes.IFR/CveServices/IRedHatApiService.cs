using Cervantes.CORE.Entities;
using Cervantes.IFR.CveServices.Models;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Interface for Red Hat Security Data API service
/// </summary>
public interface IRedHatApiService
{
    /// <summary>
    /// Get CVEs from Red Hat Security Data API
    /// </summary>
    /// <param name="options">Filter options for the request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of Red Hat CVE responses</returns>
    Task<List<RedHatCveResponse>> GetCvesAsync(RedHatApiOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get specific CVE by ID from Red Hat
    /// </summary>
    /// <param name="cveId">CVE identifier (e.g., CVE-2023-12345)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Red Hat CVE response or null if not found</returns>
    Task<RedHatCveResponse?> GetCveByIdAsync(string cveId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get CVEs modified since a specific date
    /// </summary>
    /// <param name="lastSyncDate">Date to check for modifications since</param>
    /// <param name="maxResults">Maximum number of results to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of modified CVEs</returns>
    Task<List<RedHatCveResponse>> GetModifiedCvesSinceAsync(DateTime lastSyncDate, int maxResults = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// Convert Red Hat CVE response to Cervantes CVE entity
    /// </summary>
    /// <param name="redHatCve">Red Hat CVE response</param>
    /// <param name="userId">User ID for audit (optional - will use admin user if not provided)</param>
    /// <returns>Cervantes CVE entity</returns>
    Cve ConvertToCveEntity(RedHatCveResponse redHatCve, string? userId = null);

    /// <summary>
    /// Test API connection to Red Hat Security Data API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if connection is successful</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available Red Hat products for filtering
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available products</returns>
    Task<List<string>> GetAvailableProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get CVE statistics from Red Hat API
    /// </summary>
    /// <param name="options">Filter options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary with statistics</returns>
    Task<Dictionary<string, int>> GetCveStatisticsAsync(RedHatApiOptions? options = null, CancellationToken cancellationToken = default);
}