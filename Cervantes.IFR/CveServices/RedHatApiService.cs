using System.Globalization;
using System.Net;
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
/// Service for interacting with Red Hat Security Data API
/// </summary>
public class RedHatApiService : IRedHatApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RedHatApiService> _logger;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string REDHAT_API_BASE_URL = "https://access.redhat.com/hydra/rest/securitydata";
    private const int DEFAULT_RESULTS_PER_PAGE = 20;
    private const int MAX_RESULTS_PER_PAGE = 1000;
    private const int API_RATE_LIMIT_DELAY = 1000; // 1 second delay between requests

    public RedHatApiService(
        HttpClient httpClient,
        ILogger<RedHatApiService> logger,
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
        _baseUrl = configuration["CveConfiguration:Sources:RedHat:ApiUrl"] ?? REDHAT_API_BASE_URL;

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
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.Clear();
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Cervantes-RedHat-CVE-Manager", "1.0"));

        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        
        _logger.LogInformation("HttpClient configured for Red Hat Security Data API - BaseURL: {BaseUrl}", _baseUrl);
    }

    public async Task<List<RedHatCveResponse>> GetCvesAsync(RedHatApiOptions? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            options ??= new RedHatApiOptions();
            
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(options.CveId))
                queryParams.Add($"cve={Uri.EscapeDataString(options.CveId)}");

            if (!string.IsNullOrEmpty(options.Severity))
                queryParams.Add($"severity={Uri.EscapeDataString(options.Severity)}");

            if (!string.IsNullOrEmpty(options.Package))
                queryParams.Add($"package={Uri.EscapeDataString(options.Package)}");

            if (!string.IsNullOrEmpty(options.Product))
                queryParams.Add($"product={Uri.EscapeDataString(options.Product)}");

            if (options.After.HasValue)
            {
                var afterDate = options.After.Value.Kind == DateTimeKind.Utc ? options.After.Value : options.After.Value.ToUniversalTime();
                queryParams.Add($"after={Uri.EscapeDataString(afterDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");
            }

            if (options.Before.HasValue)
            {
                var beforeDate = options.Before.Value.Kind == DateTimeKind.Utc ? options.Before.Value : options.Before.Value.ToUniversalTime();
                queryParams.Add($"before={Uri.EscapeDataString(beforeDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))}");
            }

            var validatedPerPage = Math.Min(Math.Max(options.PerPage, 1), MAX_RESULTS_PER_PAGE);
            queryParams.Add($"per_page={validatedPerPage}");
            
            if (options.Page > 1)
                queryParams.Add($"page={options.Page}");

            if (options.CvssScoreAbove.HasValue)
                queryParams.Add($"cvss_score={options.CvssScoreAbove.Value.ToString("F1", CultureInfo.InvariantCulture)}");

            // Try both endpoints: cve.json and cve with Accept header
            var endpoints = new[] 
            {
                $"{_baseUrl.TrimEnd('/')}/cve.json",
                $"{_baseUrl.TrimEnd('/')}/cve"
            };
            var endpoint = endpoints[0]; // Start with .json endpoint
            var queryString = string.Join("&", queryParams);
            var requestUrl = string.IsNullOrEmpty(queryString) ? endpoint : $"{endpoint}?{queryString}";

            _logger.LogInformation("Red Hat API Request - Base URL: {BaseUrl}", _baseUrl);
            _logger.LogInformation("Red Hat API Request - Full URL: {RequestUrl}", requestUrl);
            _logger.LogInformation("Red Hat API Request - Query Parameters: [{QueryParams}]", string.Join(", ", queryParams));

            await ApplyRateLimit();

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            
            _logger.LogInformation("Red Hat API Response - Status: {StatusCode}", response.StatusCode);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Red Hat API request failed with primary endpoint. Status: {StatusCode}, Reason: {ReasonPhrase}, Content: {Content}", 
                    response.StatusCode, response.ReasonPhrase, errorContent);
                
                // Try the alternative endpoint (without .json extension)
                if (endpoint == endpoints[0] && response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Trying alternative endpoint without .json extension...");
                    var altEndpoint = endpoints[1];
                    var altRequestUrl = string.IsNullOrEmpty(queryString) ? altEndpoint : $"{altEndpoint}?{queryString}";
                    
                    _logger.LogInformation("Red Hat API Alternative Request - URL: {RequestUrl}", altRequestUrl);
                    
                    await ApplyRateLimit();
                    var altResponse = await _httpClient.GetAsync(altRequestUrl, cancellationToken);
                    
                    _logger.LogInformation("Red Hat API Alternative Response - Status: {StatusCode}", altResponse.StatusCode);
                    
                    if (altResponse.IsSuccessStatusCode)
                    {
                        response = altResponse; // Use the successful response
                    }
                    else
                    {
                        var altErrorContent = await altResponse.Content.ReadAsStringAsync(cancellationToken);
                        _logger.LogError("Alternative endpoint also failed. Status: {StatusCode}, Reason: {ReasonPhrase}, Content: {Content}", 
                            altResponse.StatusCode, altResponse.ReasonPhrase, altErrorContent);
                    }
                }
                
                // Try a simple request to test basic connectivity
                if (!response.IsSuccessStatusCode && queryParams.Any())
                {
                    _logger.LogWarning("Retrying with basic endpoint without parameters to test connectivity...");
                    try
                    {
                        var basicResponse = await _httpClient.GetAsync(endpoints[0], cancellationToken);
                        _logger.LogWarning("Basic connectivity test (.json) - Status: {StatusCode}, Reason: {ReasonPhrase}", 
                            basicResponse.StatusCode, basicResponse.ReasonPhrase);
                        
                        var basicResponse2 = await _httpClient.GetAsync(endpoints[1], cancellationToken);
                        _logger.LogWarning("Basic connectivity test (no ext) - Status: {StatusCode}, Reason: {ReasonPhrase}", 
                            basicResponse2.StatusCode, basicResponse2.ReasonPhrase);
                    }
                    catch (Exception basicEx)
                    {
                        _logger.LogError(basicEx, "Basic connectivity test also failed");
                    }
                }
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Red Hat API request failed with status {response.StatusCode}: {response.ReasonPhrase}. Content: {errorContent}");
                }
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            // Red Hat API returns an array of CVE objects
            var result = JsonSerializer.Deserialize<List<RedHatCveResponse>>(content, _jsonOptions) ?? new List<RedHatCveResponse>();

            _logger.LogInformation("Successfully retrieved {Count} CVEs from Red Hat API", result.Count);

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while fetching CVEs from Red Hat API");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error occurred while processing Red Hat API response");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching CVEs from Red Hat API");
            throw;
        }
    }

    public async Task<RedHatCveResponse?> GetCveByIdAsync(string cveId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(cveId))
                throw new ArgumentException("CVE ID cannot be null or empty", nameof(cveId));

            if (!IsValidCveId(cveId))
                throw new ArgumentException($"Invalid CVE ID format: {cveId}", nameof(cveId));

            var options = new RedHatApiOptions { CveId = cveId, PerPage = 1 };
            var results = await GetCvesAsync(options, cancellationToken);

            var cve = results.FirstOrDefault(c => c.CveId?.Equals(cveId, StringComparison.OrdinalIgnoreCase) == true);

            if (cve == null)
            {
                _logger.LogWarning("CVE {CveId} not found in Red Hat API response", cveId);
                return null;
            }

            _logger.LogInformation("Successfully retrieved CVE {CveId} from Red Hat API", cveId);
            return cve;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching CVE {CveId} from Red Hat API", cveId);
            throw;
        }
    }

    public async Task<List<RedHatCveResponse>> GetModifiedCvesSinceAsync(DateTime lastSyncDate, int maxResults = 1000, CancellationToken cancellationToken = default)
    {
        var allCves = new List<RedHatCveResponse>();
        var currentPage = 1;
        var perPage = Math.Min(maxResults, MAX_RESULTS_PER_PAGE);

        try
        {
            while (allCves.Count < maxResults)
            {
                var options = new RedHatApiOptions
                {
                    After = lastSyncDate,
                    PerPage = Math.Min(perPage, maxResults - allCves.Count),
                    Page = currentPage
                };

                var results = await GetCvesAsync(options, cancellationToken);

                if (!results.Any())
                    break;

                allCves.AddRange(results);

                if (results.Count < perPage)
                    break;

                currentPage++;
                
                // Prevent excessive pagination
                if (currentPage > 50)
                {
                    _logger.LogWarning("Stopping Red Hat CVE sync at page {Page} to prevent excessive API calls", currentPage);
                    break;
                }
            }

            _logger.LogInformation("Retrieved {Count} modified CVEs from Red Hat API since {LastSyncDate}", 
                allCves.Count, lastSyncDate);

            return allCves.Take(maxResults).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching modified CVEs since {LastSyncDate} from Red Hat API", 
                lastSyncDate);
            throw;
        }
    }

    public Cve ConvertToCveEntity(RedHatCveResponse redHatCve, string? userId = null)
    {
        if (redHatCve == null)
            throw new ArgumentNullException(nameof(redHatCve));

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
            CveId = redHatCve.CveId,
            PublishedDate = redHatCve.PublicDate.HasValue ? DateTime.SpecifyKind(redHatCve.PublicDate.Value, DateTimeKind.Utc) : DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow, // Red Hat doesn't provide last modified, use current time
            State = "Published", // Red Hat CVEs are typically published
            SourceIdentifier = "redhat.com",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            UserId = userId
        };

        // Set description from bugzilla description or create from available info
        if (!string.IsNullOrEmpty(redHatCve.BugzillaDescription))
        {
            cve.Description = redHatCve.BugzillaDescription;
            cve.Title = redHatCve.BugzillaDescription.Length > 500 ? 
                redHatCve.BugzillaDescription.Substring(0, 500) : 
                redHatCve.BugzillaDescription;
        }
        else if (!string.IsNullOrEmpty(redHatCve.Statement))
        {
            cve.Description = redHatCve.Statement;
            cve.Title = redHatCve.Statement.Length > 500 ? 
                redHatCve.Statement.Substring(0, 500) : 
                redHatCve.Statement;
        }
        else
        {
            cve.Description = $"Red Hat CVE {redHatCve.CveId}";
            cve.Title = $"Red Hat CVE {redHatCve.CveId}";
        }

        // Parse CVSS v3 metrics
        if (!string.IsNullOrEmpty(redHatCve.Cvss3Score) && double.TryParse(redHatCve.Cvss3Score, NumberStyles.Float, CultureInfo.InvariantCulture, out var cvss3Score))
        {
            cve.CvssV3BaseScore = cvss3Score;
            cve.CvssV3Vector = redHatCve.Cvss3ScoringVector ?? "";
            cve.CvssV3Severity = MapRedHatSeverityToStandard(redHatCve.Severity);
        }

        // Parse CVSS v2 metrics (fallback)
        if (!string.IsNullOrEmpty(redHatCve.CvssScore) && double.TryParse(redHatCve.CvssScore, NumberStyles.Float, CultureInfo.InvariantCulture, out var cvss2Score))
        {
            cve.CvssV2BaseScore = cvss2Score;
            cve.CvssV2Vector = redHatCve.CvssScoringVector ?? "";
            cve.CvssV2Severity = MapRedHatSeverityToStandard(redHatCve.Severity);
        }

        // Extract CWE information
        if (!string.IsNullOrEmpty(redHatCve.Cwe))
        {
            var cweId = ExtractCweId(redHatCve.Cwe);
            if (!string.IsNullOrEmpty(cweId))
            {
                cve.PrimaryCweId = cweId;
                cve.PrimaryCweName = redHatCve.Cwe;
            }
        }

        return cve;
    }

    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Testing Red Hat Security Data API connectivity at: {BaseUrl}", _baseUrl);
            
            // Try a simple request with minimal results
            var options = new RedHatApiOptions { PerPage = 1 };
            var result = await GetCvesAsync(options, cancellationToken);
            
            var isConnected = result != null;
            _logger.LogInformation("Red Hat API connectivity test result: {IsConnected}", isConnected);
            
            return isConnected;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to test connection to Red Hat Security Data API");
            return false;
        }
    }

    public async Task<List<string>> GetAvailableProductsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Red Hat doesn't have a dedicated products endpoint, so we'll return common Red Hat products
            await Task.Delay(1, cancellationToken); // Placeholder for potential API call
            
            var products = new List<string>
            {
                "Red Hat Enterprise Linux",
                "Red Hat OpenShift",
                "Red Hat Ansible",
                "Red Hat Satellite",
                "Red Hat CloudForms",
                "Red Hat Virtualization",
                "Red Hat Storage",
                "Red Hat JBoss",
                "Red Hat Developer Toolset"
            };

            _logger.LogInformation("Returning {Count} predefined Red Hat products", products.Count);
            return products;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting Red Hat products");
            return new List<string>();
        }
    }

    public async Task<Dictionary<string, int>> GetCveStatisticsAsync(RedHatApiOptions? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var stats = new Dictionary<string, int>();
            
            // Get a sample of CVEs to calculate statistics
            var sampleOptions = options ?? new RedHatApiOptions();
            sampleOptions.PerPage = 100; // Get a reasonable sample size
            
            var cves = await GetCvesAsync(sampleOptions, cancellationToken);
            
            stats["Total"] = cves.Count;
            stats["Critical"] = cves.Count(c => c.Severity?.Equals("critical", StringComparison.OrdinalIgnoreCase) == true);
            stats["Important"] = cves.Count(c => c.Severity?.Equals("important", StringComparison.OrdinalIgnoreCase) == true);
            stats["Moderate"] = cves.Count(c => c.Severity?.Equals("moderate", StringComparison.OrdinalIgnoreCase) == true);
            stats["Low"] = cves.Count(c => c.Severity?.Equals("low", StringComparison.OrdinalIgnoreCase) == true);
            stats["WithCvss3"] = cves.Count(c => !string.IsNullOrEmpty(c.Cvss3Score));
            stats["WithCwe"] = cves.Count(c => !string.IsNullOrEmpty(c.Cwe));

            _logger.LogInformation("Generated statistics for {Count} Red Hat CVEs", cves.Count);
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating Red Hat CVE statistics");
            return new Dictionary<string, int>();
        }
    }

    private async Task ApplyRateLimit()
    {
        await Task.Delay(API_RATE_LIMIT_DELAY);
    }

    private static bool IsValidCveId(string cveId)
    {
        if (string.IsNullOrEmpty(cveId))
            return false;

        // CVE ID format: CVE-YYYY-NNNNN (where YYYY is year and NNNNN is sequence number)
        var cvePattern = @"^CVE-\d{4}-\d{4,}$";
        return Regex.IsMatch(cveId, cvePattern, RegexOptions.IgnoreCase);
    }

    private static string? ExtractCweId(string cweDescription)
    {
        if (string.IsNullOrEmpty(cweDescription))
            return null;

        // Extract CWE ID from description like "CWE-79" or "CWE-79: Cross-site Scripting"
        var cwePattern = @"CWE-(\d+)";
        var match = Regex.Match(cweDescription, cwePattern, RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : null;
    }

    private static string MapRedHatSeverityToStandard(string? redHatSeverity)
    {
        if (string.IsNullOrEmpty(redHatSeverity))
            return "UNKNOWN";

        return redHatSeverity.ToLower() switch
        {
            "critical" => "CRITICAL",
            "important" => "HIGH",
            "moderate" => "MEDIUM",
            "low" => "LOW",
            _ => "UNKNOWN"
        };
    }
}