using Cervantes.CORE.Entities;
using Cervantes.IFR.CveServices.Models;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Interface for CISA KEV (Known Exploited Vulnerabilities) API service
/// </summary>
public interface ICisaKevApiService
{
    /// <summary>
    /// Get all known exploited vulnerabilities from CISA KEV catalog
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>CISA KEV API response</returns>
    Task<CisaKevApiResponse?> GetKnownExploitedVulnerabilitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a CVE is in the KEV catalog
    /// </summary>
    /// <param name="cveId">CVE identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>KEV vulnerability data if found</returns>
    Task<CisaKevVulnerability?> GetKevDataForCveAsync(string cveId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enrich CVE entity with CISA KEV data
    /// </summary>
    /// <param name="cve">CVE entity to enrich</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if enrichment was successful</returns>
    Task<bool> EnrichCveWithKevAsync(Cve cve, CancellationToken cancellationToken = default);

    /// <summary>
    /// Test connectivity to CISA KEV API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if connection is successful</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the last update date of the KEV catalog
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Last catalog update date if available</returns>
    Task<DateTime?> GetLastCatalogUpdateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear the internal cache of KEV data
    /// </summary>
    void ClearCache();
}