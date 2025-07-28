using System.Globalization;
using System.Text.Json;
using Cervantes.CORE.Entities;
using Cervantes.IFR.CveServices.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Service for interacting with CISA KEV (Known Exploited Vulnerabilities) API
/// </summary>
public class CisaKevApiService : ICisaKevApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CisaKevApiService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string DEFAULT_CISA_KEV_URL = "https://www.cisa.gov/sites/default/files/feeds/known_exploited_vulnerabilities.json";
    private const int API_RATE_LIMIT_DELAY = 2000; // 2 second delay between requests

    // Cache for KEV data to avoid repeated API calls
    private CisaKevApiResponse? _cachedKevData;
    private DateTime? _lastCacheUpdate;
    private readonly TimeSpan _cacheValidityDuration = TimeSpan.FromHours(1);

    public CisaKevApiService(
        HttpClient httpClient,
        ILogger<CisaKevApiService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _baseUrl = configuration["CveConfiguration:Sources:VulnEnrichment:CisaKevApiUrl"] ?? DEFAULT_CISA_KEV_URL;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.Clear();
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Cervantes-CISA-KEV-Client", "1.0"));
        
        _httpClient.Timeout = TimeSpan.FromSeconds(60); // KEV file can be large
        
        _logger.LogInformation("HttpClient configured for CISA KEV API - BaseURL: {BaseUrl}", _baseUrl);
    }

    public async Task<CisaKevApiResponse?> GetKnownExploitedVulnerabilitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if cached data is still valid
            if (_cachedKevData != null && _lastCacheUpdate.HasValue && 
                DateTime.UtcNow - _lastCacheUpdate.Value < _cacheValidityDuration)
            {
                _logger.LogInformation("Returning cached CISA KEV data (age: {Age})", 
                    DateTime.UtcNow - _lastCacheUpdate.Value);
                return _cachedKevData;
            }

            _logger.LogInformation("CISA KEV API Request - URL: {RequestUrl}", _baseUrl);

            await ApplyRateLimit();

            var response = await _httpClient.GetAsync(_baseUrl, cancellationToken);
            
            _logger.LogInformation("CISA KEV API Response - Status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("CISA KEV API request failed. Status: {StatusCode}, Reason: {ReasonPhrase}, Content: {Content}", 
                    response.StatusCode, response.ReasonPhrase, errorContent);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            var result = JsonSerializer.Deserialize<CisaKevApiResponse>(content, _jsonOptions);

            if (result != null)
            {
                _logger.LogInformation("Successfully retrieved {Count} known exploited vulnerabilities from CISA KEV", 
                    result.Vulnerabilities?.Count ?? 0);
                
                // Update cache
                _cachedKevData = result;
                _lastCacheUpdate = DateTime.UtcNow;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching CISA KEV data");
            return null;
        }
    }

    public async Task<CisaKevVulnerability?> GetKevDataForCveAsync(string cveId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(cveId))
                return null;

            var kevResponse = await GetKnownExploitedVulnerabilitiesAsync(cancellationToken);
            
            if (kevResponse?.Vulnerabilities == null)
                return null;

            var kevVuln = kevResponse.Vulnerabilities.FirstOrDefault(
                v => v.CveId.Equals(cveId, StringComparison.OrdinalIgnoreCase));

            if (kevVuln != null)
            {
                _logger.LogDebug("Found CVE {CveId} in CISA KEV catalog", cveId);
            }

            return kevVuln;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching CISA KEV data for CVE {CveId}", cveId);
            return null;
        }
    }

    public async Task<bool> EnrichCveWithKevAsync(Cve cve, CancellationToken cancellationToken = default)
    {
        try
        {
            if (cve == null || string.IsNullOrEmpty(cve.CveId))
                return false;

            var kevVuln = await GetKevDataForCveAsync(cve.CveId, cancellationToken);
            if (kevVuln == null)
            {
                // Not in KEV catalog, but this is still a successful enrichment
                _logger.LogInformation("CVE {CveId} not found in CISA KEV catalog - marking as not exploited", cve.CveId);
                cve.IsKnownExploited = false;
                return true;
            }

            _logger.LogInformation("Found CVE {CveId} in CISA KEV catalog - DueDate: {DueDate}", cve.CveId, kevVuln.DueDate);

            // CVE is in KEV catalog
            cve.IsKnownExploited = true;
            
            // Parse due date
            if (!string.IsNullOrEmpty(kevVuln.DueDate))
            {
                if (DateTime.TryParseExact(kevVuln.DueDate, "yyyy-MM-dd", 
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dueDate))
                {
                    cve.KevDueDate = DateTime.SpecifyKind(dueDate, DateTimeKind.Utc);
                }
            }

            _logger.LogDebug("Enriched CVE {CveId} with CISA KEV data - Known Exploited: {IsKnownExploited}, Due Date: {DueDate}", 
                cve.CveId, cve.IsKnownExploited, cve.KevDueDate);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while enriching CVE {CveId} with CISA KEV data", cve?.CveId);
            return false;
        }
    }

    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Testing CISA KEV API connectivity at: {BaseUrl}", _baseUrl);
            
            var result = await GetKnownExploitedVulnerabilitiesAsync(cancellationToken);
            
            var isConnected = result != null && result.Vulnerabilities?.Any() == true;
            _logger.LogInformation("CISA KEV API connectivity test result: {IsConnected}", isConnected);
            
            return isConnected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to test connection to CISA KEV API");
            return false;
        }
    }

    public async Task<DateTime?> GetLastCatalogUpdateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var kevResponse = await GetKnownExploitedVulnerabilitiesAsync(cancellationToken);
            
            if (kevResponse != null && !string.IsNullOrEmpty(kevResponse.DateReleased))
            {
                if (DateTime.TryParseExact(kevResponse.DateReleased, "yyyy-MM-dd", 
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate))
                {
                    return DateTime.SpecifyKind(releaseDate, DateTimeKind.Utc);
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting CISA KEV catalog update date");
            return null;
        }
    }

    public void ClearCache()
    {
        _cachedKevData = null;
        _lastCacheUpdate = null;
        _logger.LogInformation("CISA KEV cache cleared");
    }

    private async System.Threading.Tasks.Task ApplyRateLimit()
    {
        await System.Threading.Tasks.Task.Delay(API_RATE_LIMIT_DELAY);
    }
}