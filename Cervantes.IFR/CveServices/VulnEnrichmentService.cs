using Cervantes.CORE.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Service for enriching CVE data with EPSS scores and CISA KEV information
/// </summary>
public class VulnEnrichmentService : IVulnEnrichmentService
{
    private readonly IEpssApiService _epssApiService;
    private readonly ICisaKevApiService _cisaKevApiService;
    private readonly ILogger<VulnEnrichmentService> _logger;
    private readonly IConfiguration _configuration;
    private readonly bool _epssEnabled;
    private readonly bool _cisaKevEnabled;

    public VulnEnrichmentService(
        IEpssApiService epssApiService,
        ICisaKevApiService cisaKevApiService,
        ILogger<VulnEnrichmentService> logger,
        IConfiguration configuration)
    {
        _epssApiService = epssApiService;
        _cisaKevApiService = cisaKevApiService;
        _logger = logger;
        _configuration = configuration;

        // Check if enrichment is enabled in configuration
        _epssEnabled = configuration.GetValue<bool>("CveConfiguration:EnableEpssEnrichment", true);
        _cisaKevEnabled = configuration.GetValue<bool>("CveConfiguration:EnableCisaKevEnrichment", true);

        _logger.LogInformation("VulnEnrichmentService initialized - EPSS: {EpssEnabled}, CISA KEV: {CisaKevEnabled}", 
            _epssEnabled, _cisaKevEnabled);
    }

    public async Task<bool> EnrichCveAsync(Cve cve, bool includeEpss = true, bool includeCisaKev = true, CancellationToken cancellationToken = default)
    {
        if (cve == null || string.IsNullOrEmpty(cve.CveId))
            return false;

        _logger.LogInformation("Starting enrichment for CVE {CveId} - EPSS: {IncludeEpss}, KEV: {IncludeKev}", cve.CveId, includeEpss, includeCisaKev);

        bool enrichmentPerformed = false;

        try
        {
            // Enrich with EPSS data
            if (_epssEnabled && includeEpss)
            {
                try
                {
                    var epssResult = await _epssApiService.EnrichCveWithEpssAsync(cve, cancellationToken);
                    if (epssResult)
                    {
                        enrichmentPerformed = true;
                        _logger.LogDebug("Successfully enriched CVE {CveId} with EPSS data", cve.CveId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to enrich CVE {CveId} with EPSS data", cve.CveId);
                }
            }

            // Enrich with CISA KEV data
            if (_cisaKevEnabled && includeCisaKev)
            {
                try
                {
                    var kevResult = await _cisaKevApiService.EnrichCveWithKevAsync(cve, cancellationToken);
                    if (kevResult)
                    {
                        enrichmentPerformed = true;
                        _logger.LogDebug("Successfully enriched CVE {CveId} with CISA KEV data", cve.CveId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to enrich CVE {CveId} with CISA KEV data", cve.CveId);
                }
            }

            if (enrichmentPerformed)
            {
                _logger.LogInformation("Successfully enriched CVE {CveId}", cve.CveId);
            }

            return enrichmentPerformed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while enriching CVE {CveId}", cve.CveId);
            return false;
        }
    }

    public async Task<int> EnrichCvesAsync(IEnumerable<Cve> cves, bool includeEpss = true, bool includeCisaKev = true, CancellationToken cancellationToken = default)
    {
        var cveList = cves?.ToList() ?? new List<Cve>();
        
        if (!cveList.Any())
        {
            _logger.LogWarning("No CVEs provided for enrichment");
            return 0;
        }

        _logger.LogInformation("Starting vulnerability enrichment for {Count} CVEs", cveList.Count);

        int enrichedCount = 0;

        try
        {
            // Process CVEs in batches to manage memory and API limits
            const int batchSize = 50;
            
            for (int i = 0; i < cveList.Count; i += batchSize)
            {
                var batch = cveList.Skip(i).Take(batchSize).ToList();
                _logger.LogDebug("Processing enrichment batch {BatchNumber} ({BatchSize} CVEs)", 
                    (i / batchSize) + 1, batch.Count);

                foreach (var cve in batch)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        var enriched = await EnrichCveAsync(cve, includeEpss, includeCisaKev, cancellationToken);
                        if (enriched)
                        {
                            enrichedCount++;
                        }

                        // Small delay between CVEs to be respectful to APIs
                        await System.Threading.Tasks.Task.Delay(100, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to enrich CVE {CveId}", cve.CveId);
                    }
                }

                // Longer delay between batches
                if (i + batchSize < cveList.Count)
                {
                    await System.Threading.Tasks.Task.Delay(1000, cancellationToken);
                }
            }

            _logger.LogInformation("Completed vulnerability enrichment for {EnrichedCount}/{TotalCount} CVEs", 
                enrichedCount, cveList.Count);

            return enrichedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during bulk vulnerability enrichment");
            return enrichedCount;
        }
    }

    public async Task<bool> TestConnectivityAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Testing VulnEnrichment service connectivity");

        bool epssConnected = true;
        bool cisaKevConnected = true;

        try
        {
            // Test EPSS connectivity
            if (_epssEnabled)
            {
                epssConnected = await _epssApiService.TestConnectionAsync(cancellationToken);
                _logger.LogInformation("EPSS API connectivity: {Status}", epssConnected ? "OK" : "Failed");
            }

            // Test CISA KEV connectivity
            if (_cisaKevEnabled)
            {
                cisaKevConnected = await _cisaKevApiService.TestConnectionAsync(cancellationToken);
                _logger.LogInformation("CISA KEV API connectivity: {Status}", cisaKevConnected ? "OK" : "Failed");
            }

            var overallStatus = epssConnected && cisaKevConnected;
            _logger.LogInformation("VulnEnrichment service overall connectivity: {Status}", overallStatus ? "OK" : "Failed");

            return overallStatus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while testing VulnEnrichment service connectivity");
            return false;
        }
    }

    public async Task<Dictionary<string, object>> GetEnrichmentStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var stats = new Dictionary<string, object>();

        try
        {
            stats["EpssEnabled"] = _epssEnabled;
            stats["CisaKevEnabled"] = _cisaKevEnabled;
            stats["LastTest"] = DateTime.UtcNow;

            // Test connectivity for current status
            if (_epssEnabled)
            {
                var epssConnected = await _epssApiService.TestConnectionAsync(cancellationToken);
                stats["EpssConnected"] = epssConnected;
            }

            if (_cisaKevEnabled)
            {
                var kevConnected = await _cisaKevApiService.TestConnectionAsync(cancellationToken);
                stats["CisaKevConnected"] = kevConnected;
                
                // Get KEV catalog update date
                var lastUpdate = await _cisaKevApiService.GetLastCatalogUpdateAsync(cancellationToken);
                if (lastUpdate.HasValue)
                {
                    stats["CisaKevLastUpdate"] = lastUpdate.Value;
                }
            }

            _logger.LogInformation("Generated VulnEnrichment statistics");
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating VulnEnrichment statistics");
            stats["Error"] = ex.Message;
            return stats;
        }
    }
}