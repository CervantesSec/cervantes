using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using Cervantes.CORE.Entities;
using Cervantes.IFR.CveServices.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.IFR.CveServices;

/// <summary>
/// Service for interacting with NVD API 2.0
/// </summary>
public class NvdApiService : INvdApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NvdApiService> _logger;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string NVD_API_BASE_URL = "https://services.nvd.nist.gov/rest/json/cves/2.0";
    private const int DEFAULT_RESULTS_PER_PAGE = 50;
    private const int MAX_RESULTS_PER_PAGE = 2000;
    private const int API_RATE_LIMIT_DELAY = 6000; // 6 seconds without API key
    private const int API_RATE_LIMIT_DELAY_WITH_KEY = 50; // 50ms with API key

    public NvdApiService(
        HttpClient httpClient,
        ILogger<NvdApiService> logger,
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
        _apiKey = configuration["CveConfiguration:NvdApiKey"];
        _baseUrl = configuration["CveConfiguration:NvdApiUrl"] ?? NVD_API_BASE_URL;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        // Don't set BaseAddress, we'll use the full URL in the request
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.Clear();
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Cervantes-CVE-Manager", "1.0"));

        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("apiKey", _apiKey);
        }

        _httpClient.Timeout = TimeSpan.FromSeconds(60); // Increase timeout for NVD API
        
        _logger.LogInformation("HttpClient configured for NVD API - BaseURL: {BaseUrl}, HasApiKey: {HasApiKey}", 
            _baseUrl, !string.IsNullOrEmpty(_apiKey));
    }

    public async Task<NvdApiResponse> GetCvesAsync(
        int startIndex = 0,
        int resultsPerPage = DEFAULT_RESULTS_PER_PAGE,
        DateTime? lastModStartDate = null,
        DateTime? lastModEndDate = null,
        DateTime? pubStartDate = null,
        DateTime? pubEndDate = null,
        string cvssV3Severity = null,
        string keywordSearch = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate date range (NVD API has a 120-day limit)
            if (lastModStartDate.HasValue && lastModEndDate.HasValue)
            {
                var dateRange = lastModEndDate.Value - lastModStartDate.Value;
                if (dateRange.TotalDays > 120)
                {
                    throw new ArgumentException("Date range cannot exceed 120 days for lastModStartDate/lastModEndDate");
                }
            }
            
            if (pubStartDate.HasValue && pubEndDate.HasValue)
            {
                var dateRange = pubEndDate.Value - pubStartDate.Value;
                if (dateRange.TotalDays > 120)
                {
                    throw new ArgumentException("Date range cannot exceed 120 days for pubStartDate/pubEndDate");
                }
            }

            var queryParams = new List<string>();
            
            if (startIndex > 0)
                queryParams.Add($"startIndex={startIndex}");

            var validatedResultsPerPage = Math.Min(resultsPerPage, MAX_RESULTS_PER_PAGE);
            queryParams.Add($"resultsPerPage={validatedResultsPerPage}");

            if (lastModStartDate.HasValue)
            {
                var utcDate = lastModStartDate.Value.Kind == DateTimeKind.Utc ? lastModStartDate.Value : lastModStartDate.Value.ToUniversalTime();
                queryParams.Add($"lastModStartDate={Uri.EscapeDataString(utcDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
            }

            if (lastModEndDate.HasValue)
            {
                var utcDate = lastModEndDate.Value.Kind == DateTimeKind.Utc ? lastModEndDate.Value : lastModEndDate.Value.ToUniversalTime();
                queryParams.Add($"lastModEndDate={Uri.EscapeDataString(utcDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
            }

            if (pubStartDate.HasValue)
            {
                var utcDate = pubStartDate.Value.Kind == DateTimeKind.Utc ? pubStartDate.Value : pubStartDate.Value.ToUniversalTime();
                queryParams.Add($"pubStartDate={Uri.EscapeDataString(utcDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
            }

            if (pubEndDate.HasValue)
            {
                var utcDate = pubEndDate.Value.Kind == DateTimeKind.Utc ? pubEndDate.Value : pubEndDate.Value.ToUniversalTime();
                queryParams.Add($"pubEndDate={Uri.EscapeDataString(utcDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}");
            }

            if (!string.IsNullOrEmpty(cvssV3Severity))
                queryParams.Add($"cvssV3Severity={cvssV3Severity.ToUpper()}");

            if (!string.IsNullOrEmpty(keywordSearch))
                queryParams.Add($"keywordSearch={Uri.EscapeDataString(keywordSearch)}");

            var queryString = string.Join("&", queryParams);
            var requestUrl = string.IsNullOrEmpty(queryString) ? _baseUrl : $"{_baseUrl}?{queryString}";

            _logger.LogInformation("Requesting CVEs from NVD API: {RequestUrl}", requestUrl);

            await ApplyRateLimit();

            // First, test with a simple request if this is a complex query
            var hasComplexParams = lastModStartDate.HasValue || lastModEndDate.HasValue || 
                                  pubStartDate.HasValue || pubEndDate.HasValue || 
                                  !string.IsNullOrEmpty(cvssV3Severity) || !string.IsNullOrEmpty(keywordSearch);
                                  
            if (hasComplexParams && startIndex == 0)
            {
                _logger.LogDebug("Complex parameters detected, testing basic API connectivity first");
                try 
                {
                    var testUrl = $"{_baseUrl}?resultsPerPage=1";
                    var testResponse = await _httpClient.GetAsync(testUrl, cancellationToken);
                    if (!testResponse.IsSuccessStatusCode)
                    {
                        var testErrorContent = await testResponse.Content.ReadAsStringAsync(cancellationToken);
                        _logger.LogError("NVD API basic connectivity test failed. Status: {StatusCode}, Content: {Content}", testResponse.StatusCode, testErrorContent);
                    }
                    else
                    {
                        _logger.LogDebug("NVD API basic connectivity test passed");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "NVD API basic connectivity test failed with exception");
                }
            }

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("NVD API request failed. Status: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
                response.EnsureSuccessStatusCode();
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<NvdApiResponse>(content, _jsonOptions);

            _logger.LogInformation("Successfully retrieved {Count} CVEs from NVD API", result?.Vulnerabilities?.Count ?? 0);

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching CVEs from NVD API");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error occurred while processing NVD API response");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching CVEs from NVD API");
            throw;
        }
    }

    /// <summary>
    /// Test basic API connectivity
    /// </summary>
    /// <returns>True if API is accessible</returns>
    public async Task<bool> TestApiConnectivityAsync()
    {
        try
        {
            _logger.LogInformation("Testing NVD API connectivity at: {BaseUrl}", _baseUrl);
            
            // Try the simplest possible request
            var testUrl = $"{_baseUrl}?resultsPerPage=1";
            _logger.LogInformation("Testing with URL: {TestUrl}", testUrl);
            
            var response = await _httpClient.GetAsync(testUrl);
            var content = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Test response - Status: {StatusCode}, Content length: {ContentLength}", 
                response.StatusCode, content.Length);
                
            if (content.Length > 0)
            {
                _logger.LogDebug("Test response content: {Content}", content.Substring(0, Math.Min(500, content.Length)));
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API connectivity test failed");
            return false;
        }
    }

    public async Task<NvdCveItem> GetCveByIdAsync(string cveId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(cveId))
                throw new ArgumentException("CVE ID cannot be null or empty", nameof(cveId));

            if (!IsValidCveId(cveId))
                throw new ArgumentException($"Invalid CVE ID format: {cveId}", nameof(cveId));

            var requestUrl = $"{_baseUrl}?cveId={cveId}";

            _logger.LogInformation("Requesting CVE {CveId} from NVD API", cveId);

            await ApplyRateLimit();

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<NvdApiResponse>(content, _jsonOptions);

            var cveItem = result?.Vulnerabilities?.FirstOrDefault()?.Cve;

            if (cveItem == null)
            {
                _logger.LogWarning("CVE {CveId} not found in NVD API response", cveId);
                return null;
            }

            _logger.LogInformation("Successfully retrieved CVE {CveId} from NVD API", cveId);

            return cveItem;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching CVE {CveId} from NVD API", cveId);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error occurred while processing NVD API response for CVE {CveId}", cveId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching CVE {CveId} from NVD API", cveId);
            throw;
        }
    }

    public async Task<List<NvdCveItem>> GetModifiedCvesSinceAsync(DateTime lastSyncDate, CancellationToken cancellationToken = default)
    {
        var allCves = new List<NvdCveItem>();
        var startIndex = 0;
        var resultsPerPage = MAX_RESULTS_PER_PAGE;

        try
        {
            while (true)
            {
                var response = await GetCvesAsync(
                    startIndex: startIndex,
                    resultsPerPage: resultsPerPage,
                    lastModStartDate: lastSyncDate,
                    cancellationToken: cancellationToken);

                if (response?.Vulnerabilities == null || !response.Vulnerabilities.Any())
                    break;

                allCves.AddRange(response.Vulnerabilities.Select(v => v.Cve));

                if (response.Vulnerabilities.Count < resultsPerPage)
                    break;

                startIndex += resultsPerPage;
            }

            _logger.LogInformation("Retrieved {Count} modified CVEs since {LastSyncDate}", allCves.Count, lastSyncDate);

            return allCves;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching modified CVEs since {LastSyncDate}", lastSyncDate);
            throw;
        }
    }

    public Cve ConvertToCveEntity(NvdCveItem nvdCveItem, string userId = null)
    {
        if (nvdCveItem == null)
            throw new ArgumentNullException(nameof(nvdCveItem));

        // If no userId provided, get the admin user ID
        if (string.IsNullOrEmpty(userId))
        {
            var adminUser = _userManager.Users.FirstOrDefault(u => u.Email == "admin@cervantes.local");
            if (adminUser == null)
            {
                throw new InvalidOperationException("Admin user with email 'admin@cervantes.local' not found. CVE sync requires a valid system user.");
            }
            userId = adminUser.Id;
        }

        var cve = new Cve
        {
            Id = Guid.NewGuid(),
            CveId = nvdCveItem.Id,
            PublishedDate = DateTime.SpecifyKind(nvdCveItem.Published, DateTimeKind.Utc),
            LastModifiedDate = DateTime.SpecifyKind(nvdCveItem.LastModified, DateTimeKind.Utc),
            State = nvdCveItem.VulnStatus,
            SourceIdentifier = !string.IsNullOrEmpty(nvdCveItem.SourceIdentifier) && nvdCveItem.SourceIdentifier.Length > 100 
                ? nvdCveItem.SourceIdentifier.Substring(0, 100) 
                : nvdCveItem.SourceIdentifier,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            UserId = userId
        };

        // Extract description (prefer English)
        var description = nvdCveItem.Descriptions?.FirstOrDefault(d => d.Lang == "en") ?? nvdCveItem.Descriptions?.FirstOrDefault();
        if (description != null)
        {
            cve.Description = description.Value;
            cve.Title = description.Value.Length > 500 ? description.Value.Substring(0, 500) : description.Value;
        }

        // Extract CVSS v3.1 metrics
        var cvssV31 = nvdCveItem.Metrics?.CvssMetricV31?.FirstOrDefault(m => m.Type == "Primary") ?? nvdCveItem.Metrics?.CvssMetricV31?.FirstOrDefault();
        if (cvssV31 != null)
        {
            cve.CvssV3BaseScore = cvssV31.CvssData?.BaseScore;
            cve.CvssV3Vector = cvssV31.CvssData?.VectorString;
            cve.CvssV3Severity = cvssV31.CvssData?.BaseSeverity;
        }

        // Extract CVSS v2 metrics (fallback or legacy)
        var cvssV2 = nvdCveItem.Metrics?.CvssMetricV2?.FirstOrDefault(m => m.Type == "Primary") ?? nvdCveItem.Metrics?.CvssMetricV2?.FirstOrDefault();
        if (cvssV2 != null)
        {
            cve.CvssV2BaseScore = cvssV2.CvssData?.BaseScore;
            cve.CvssV2Vector = cvssV2.CvssData?.VectorString;
            cve.CvssV2Severity = cvssV2.BaseSeverity;
        }

        // Extract primary CWE
        var primaryCwe = nvdCveItem.Weaknesses?.FirstOrDefault(w => w.Type == "Primary") ?? nvdCveItem.Weaknesses?.FirstOrDefault();
        if (primaryCwe != null)
        {
            var cweDescription = primaryCwe.Description?.FirstOrDefault(d => d.Lang == "en") ?? primaryCwe.Description?.FirstOrDefault();
            if (cweDescription != null)
            {
                cve.PrimaryCweId = ExtractCweId(cweDescription.Value);
                cve.PrimaryCweName = cweDescription.Value;
            }
        }

        return cve;
    }

    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // First try the detailed connectivity test
            var basicConnectivityPassed = await TestApiConnectivityAsync();
            if (!basicConnectivityPassed)
            {
                _logger.LogWarning("Basic API connectivity test failed");
                return false;
            }

            // Then try the full API call
            var response = await GetCvesAsync(resultsPerPage: 1, cancellationToken: cancellationToken);
            return response != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to test connection to NVD API");
            return false;
        }
    }

    private async Task ApplyRateLimit()
    {
        var delay = string.IsNullOrEmpty(_apiKey) ? API_RATE_LIMIT_DELAY : API_RATE_LIMIT_DELAY_WITH_KEY;
        await Task.Delay(delay);
    }

    private static bool IsValidCveId(string cveId)
    {
        if (string.IsNullOrEmpty(cveId))
            return false;

        // CVE ID format: CVE-YYYY-NNNNN (where YYYY is year and NNNNN is sequence number)
        var cvePattern = @"^CVE-\d{4}-\d{4,}$";
        return Regex.IsMatch(cveId, cvePattern, RegexOptions.IgnoreCase);
    }

    private static string ExtractCweId(string cweDescription)
    {
        if (string.IsNullOrEmpty(cweDescription))
            return null;

        // Extract CWE ID from description like "CWE-79"
        var cwePattern = @"CWE-(\d+)";
        var match = Regex.Match(cweDescription, cwePattern, RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : null;
    }
}