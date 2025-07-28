using System.Text.Json.Serialization;

namespace Cervantes.IFR.CveServices.Models;

/// <summary>
/// EPSS (Exploit Prediction Scoring System) API models
/// </summary>
public class EpssApiResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("status-code")]
    public int StatusCode { get; set; }
    
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
    
    [JsonPropertyName("access")]
    public string Access { get; set; } = string.Empty;
    
    [JsonPropertyName("total")]
    public int Total { get; set; }
    
    [JsonPropertyName("offset")]
    public int Offset { get; set; }
    
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
    
    [JsonPropertyName("data")]
    public List<EpssData> Data { get; set; } = new List<EpssData>();
}

/// <summary>
/// Individual EPSS data entry
/// </summary>
public class EpssData
{
    [JsonPropertyName("cve")]
    public string Cve { get; set; } = string.Empty;
    
    [JsonPropertyName("epss")]
    public string EpssScore { get; set; } = string.Empty;
    
    [JsonPropertyName("percentile")]
    public string Percentile { get; set; } = string.Empty;
    
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;
}

/// <summary>
/// CISA KEV (Known Exploited Vulnerabilities) API models
/// </summary>
public class CisaKevApiResponse
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("catalogVersion")]
    public string CatalogVersion { get; set; } = string.Empty;
    
    [JsonPropertyName("dateReleased")]
    public string DateReleased { get; set; } = string.Empty;
    
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("vulnerabilities")]
    public List<CisaKevVulnerability> Vulnerabilities { get; set; } = new List<CisaKevVulnerability>();
}

/// <summary>
/// Individual CISA KEV vulnerability entry
/// </summary>
public class CisaKevVulnerability
{
    [JsonPropertyName("cveID")]
    public string CveId { get; set; } = string.Empty;
    
    [JsonPropertyName("vendorProject")]
    public string VendorProject { get; set; } = string.Empty;
    
    [JsonPropertyName("product")]
    public string Product { get; set; } = string.Empty;
    
    [JsonPropertyName("vulnerabilityName")]
    public string VulnerabilityName { get; set; } = string.Empty;
    
    [JsonPropertyName("dateAdded")]
    public string DateAdded { get; set; } = string.Empty;
    
    [JsonPropertyName("shortDescription")]
    public string ShortDescription { get; set; } = string.Empty;
    
    [JsonPropertyName("requiredAction")]
    public string RequiredAction { get; set; } = string.Empty;
    
    [JsonPropertyName("dueDate")]
    public string DueDate { get; set; } = string.Empty;
    
    [JsonPropertyName("knownRansomwareCampaignUse")]
    public string KnownRansomwareCampaignUse { get; set; } = string.Empty;
    
    [JsonPropertyName("notes")]
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Options for EPSS API requests
/// </summary>
public class EpssApiOptions
{
    /// <summary>
    /// Filter by specific CVE ID
    /// </summary>
    public string? CveId { get; set; }
    
    /// <summary>
    /// Filter by date (YYYY-MM-DD format)
    /// </summary>
    public DateTime? Date { get; set; }
    
    /// <summary>
    /// Number of results per page (max 100)
    /// </summary>
    public int Limit { get; set; } = 100;
    
    /// <summary>
    /// Offset for pagination
    /// </summary>
    public int Offset { get; set; } = 0;
    
    /// <summary>
    /// Filter by EPSS score greater than
    /// </summary>
    public double? EpssScoreGreaterThan { get; set; }
    
    /// <summary>
    /// Filter by percentile greater than
    /// </summary>
    public double? PercentileGreaterThan { get; set; }
}

