using Cervantes.CORE.Entities;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Interface for vulnerability enrichment service that orchestrates EPSS and CISA KEV enrichment
/// </summary>
public interface IVulnEnrichmentService
{
    /// <summary>
    /// Enrich a single CVE with EPSS and CISA KEV data
    /// </summary>
    /// <param name="cve">CVE entity to enrich</param>
    /// <param name="includeEpss">Whether to include EPSS enrichment</param>
    /// <param name="includeCisaKev">Whether to include CISA KEV enrichment</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any enrichment was successful</returns>
    Task<bool> EnrichCveAsync(Cve cve, bool includeEpss = true, bool includeCisaKev = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enrich multiple CVEs with EPSS and CISA KEV data
    /// </summary>
    /// <param name="cves">CVE entities to enrich</param>
    /// <param name="includeEpss">Whether to include EPSS enrichment</param>
    /// <param name="includeCisaKev">Whether to include CISA KEV enrichment</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of CVEs successfully enriched</returns>
    Task<int> EnrichCvesAsync(IEnumerable<Cve> cves, bool includeEpss = true, bool includeCisaKev = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Test connectivity to all enrichment APIs
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if all enabled services are accessible</returns>
    Task<bool> TestConnectivityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get enrichment statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary with enrichment statistics</returns>
    Task<Dictionary<string, object>> GetEnrichmentStatisticsAsync(CancellationToken cancellationToken = default);
}