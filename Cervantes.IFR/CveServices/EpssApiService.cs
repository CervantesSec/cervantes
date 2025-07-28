using System.Globalization;
using System.Text.Json;
using Cervantes.CORE.Entities;
using Cervantes.IFR.CveServices.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Service for interacting with FIRST EPSS (Exploit Prediction Scoring System) API
/// </summary>
public class EpssApiService : IEpssApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EpssApiService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string DEFAULT_EPSS_API_URL = "https://api.first.org/data/v1/epss";
    private const int API_RATE_LIMIT_DELAY = 1000; // 1 second delay between requests
    private const int MAX_BATCH_SIZE = 100; // EPSS API limit

    public EpssApiService(
        HttpClient httpClient,
        ILogger<EpssApiService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _baseUrl = configuration["CveConfiguration:Sources:VulnEnrichment:EpssApiUrl"] ?? DEFAULT_EPSS_API_URL;

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
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Cervantes-EPSS-Client", "1.0"));
        
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        
        _logger.LogInformation("HttpClient configured for EPSS API - BaseURL: {BaseUrl}", _baseUrl);
    }

    public async Task<EpssApiResponse?> GetEpssScoresAsync(EpssApiOptions? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            options ??= new EpssApiOptions();
            
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(options.CveId))
                queryParams.Add($"cve={Uri.EscapeDataString(options.CveId)}");

            if (options.Date.HasValue)
            {
                var dateStr = options.Date.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                queryParams.Add($"date={dateStr}");
            }

            if (options.EpssScoreGreaterThan.HasValue)
                queryParams.Add($"epss-gt={options.EpssScoreGreaterThan.Value.ToString("F3", CultureInfo.InvariantCulture)}");

            if (options.PercentileGreaterThan.HasValue)
                queryParams.Add($"percentile-gt={options.PercentileGreaterThan.Value.ToString("F3", CultureInfo.InvariantCulture)}");

            // Validate and set limit (EPSS API max is 100)
            var validatedLimit = Math.Min(Math.Max(options.Limit, 1), MAX_BATCH_SIZE);
            queryParams.Add($"limit={validatedLimit}");

            if (options.Offset > 0)
                queryParams.Add($"offset={options.Offset}");

            var queryString = string.Join("&", queryParams);
            var requestUrl = string.IsNullOrEmpty(queryString) ? _baseUrl : $"{_baseUrl}?{queryString}";

            _logger.LogInformation("EPSS API Request - URL: {RequestUrl}", requestUrl);

            await ApplyRateLimit();

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            
            _logger.LogInformation("EPSS API Response - Status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("EPSS API request failed. Status: {StatusCode}, Reason: {ReasonPhrase}, Content: {Content}", 
                    response.StatusCode, response.ReasonPhrase, errorContent);
                throw new HttpRequestException($"EPSS API request failed with status {response.StatusCode}: {response.ReasonPhrase}");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            var result = JsonSerializer.Deserialize<EpssApiResponse>(content, _jsonOptions);

            if (result != null)
            {
                _logger.LogInformation("Successfully retrieved {Count} EPSS scores from API", result.Data?.Count ?? 0);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching EPSS scores");
            return null;
        }
    }

    public async Task<EpssData?> GetEpssScoreForCveAsync(string cveId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(cveId))
                return null;

            var options = new EpssApiOptions { CveId = cveId, Limit = 1 };
            var response = await GetEpssScoresAsync(options, cancellationToken);

            return response?.Data?.FirstOrDefault(c => c.Cve.Equals(cveId, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching EPSS score for CVE {CveId}", cveId);
            return null;
        }
    }

    public async Task<bool> EnrichCveWithEpssAsync(Cve cve, CancellationToken cancellationToken = default)
    {
        try
        {
            if (cve == null || string.IsNullOrEmpty(cve.CveId))
                return false;

            var epssData = await GetEpssScoreForCveAsync(cve.CveId, cancellationToken);
            if (epssData == null)
            {
                _logger.LogInformation("No EPSS data found for CVE {CveId}", cve.CveId);
                return false;
            }

            _logger.LogInformation("Found EPSS data for CVE {CveId}: Score={EpssScore}, Percentile={Percentile}", 
                cve.CveId, epssData.EpssScore, epssData.Percentile);

            // Parse EPSS score
            if (double.TryParse(epssData.EpssScore, NumberStyles.Float, CultureInfo.InvariantCulture, out var epssScore))
            {
                cve.EpssScore = epssScore;
            }

            // Parse EPSS percentile
            if (double.TryParse(epssData.Percentile, NumberStyles.Float, CultureInfo.InvariantCulture, out var percentile))
            {
                cve.EpssPercentile = percentile;
            }

            _logger.LogDebug("Enriched CVE {CveId} with EPSS score: {EpssScore}, percentile: {Percentile}", 
                cve.CveId, cve.EpssScore, cve.EpssPercentile);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while enriching CVE {CveId} with EPSS data", cve?.CveId);
            return false;
        }
    }

    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Testing EPSS API connectivity at: {BaseUrl}", _baseUrl);
            
            // Try a simple request with minimal results
            var options = new EpssApiOptions { Limit = 1 };
            var result = await GetEpssScoresAsync(options, cancellationToken);
            
            var isConnected = result != null && result.StatusCode == 200;
            _logger.LogInformation("EPSS API connectivity test result: {IsConnected}", isConnected);
            
            return isConnected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to test connection to EPSS API");
            return false;
        }
    }

    private async System.Threading.Tasks.Task ApplyRateLimit()
    {
        await System.Threading.Tasks.Task.Delay(API_RATE_LIMIT_DELAY);
    }
}