using Cervantes.CORE.Entities;
using Cervantes.IFR.CveServices.Models;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Interface for EPSS (Exploit Prediction Scoring System) API service
/// </summary>
public interface IEpssApiService
{
    /// <summary>
    /// Get EPSS scores from the API
    /// </summary>
    /// <param name="options">API request options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>EPSS API response</returns>
    Task<EpssApiResponse?> GetEpssScoresAsync(EpssApiOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get EPSS score for a single CVE
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>EPSS data for the CVE</returns>
    Task<EpssData?> GetEpssScoreForCveAsync(string cveId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enrich CVE entity with EPSS data
    /// </summary>
    /// <param name="cve">CVE entity to enrich</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if enrichment was successful</returns>
    Task<bool> EnrichCveWithEpssAsync(Cve cve, CancellationToken cancellationToken = default);

    /// <summary>
    /// Test connectivity to EPSS API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if connection is successful</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);
}