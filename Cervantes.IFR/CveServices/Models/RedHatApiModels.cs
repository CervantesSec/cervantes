using System.Text.Json.Serialization;

namespace Cervantes.IFR.CveServices.Models;

/// <summary>
/// Red Hat Security Data API models
/// </summary>
public class RedHatCveResponse
{
    [JsonPropertyName("CVE")]
    public string CveId { get; set; } = string.Empty;
    
    [JsonPropertyName("severity")]
    public string? Severity { get; set; }
    
    [JsonPropertyName("public_date")]
    public DateTime? PublicDate { get; set; }
    
    [JsonPropertyName("bugzilla")]
    public string? Bugzilla { get; set; }
    
    [JsonPropertyName("bugzilla_description")]
    public string? BugzillaDescription { get; set; }
    
    [JsonPropertyName("cvss3_score")]
    public string? Cvss3Score { get; set; }
    
    [JsonPropertyName("cvss3_scoring_vector")]
    public string? Cvss3ScoringVector { get; set; }
    
    [JsonPropertyName("CWE")]
    public string? Cwe { get; set; }
    
    [JsonPropertyName("affected_packages")]
    public List<string>? AffectedPackages { get; set; }
    
    [JsonPropertyName("resource_url")]
    public string? ResourceUrl { get; set; }
    
    [JsonPropertyName("cvss_score")]
    public string? CvssScore { get; set; }
    
    [JsonPropertyName("cvss_scoring_vector")]
    public string? CvssScoringVector { get; set; }
    
    [JsonPropertyName("acknowledgement")]
    public string? Acknowledgement { get; set; }
    
    [JsonPropertyName("upstream_fix")]
    public string? UpstreamFix { get; set; }
    
    [JsonPropertyName("references")]
    public List<string>? References { get; set; }
    
    [JsonPropertyName("mitigation")]
    public string? Mitigation { get; set; }
    
    [JsonPropertyName("statement")]
    public string? Statement { get; set; }
    
    [JsonPropertyName("details")]
    public List<string>? Details { get; set; }
}

/// <summary>
/// Red Hat API request options
/// </summary>
public class RedHatApiOptions
{
    /// <summary>
    /// Filter by CVE ID
    /// </summary>
    public string? CveId { get; set; }
    
    /// <summary>
    /// Filter by severity (low, moderate, important, critical)
    /// </summary>
    public string? Severity { get; set; }
    
    /// <summary>
    /// Filter by package name
    /// </summary>
    public string? Package { get; set; }
    
    /// <summary>
    /// Filter by product
    /// </summary>
    public string? Product { get; set; }
    
    /// <summary>
    /// Start date for filtering
    /// </summary>
    public DateTime? After { get; set; }
    
    /// <summary>
    /// End date for filtering
    /// </summary>
    public DateTime? Before { get; set; }
    
    /// <summary>
    /// Number of results per page (default 20, max 1000)
    /// </summary>
    public int PerPage { get; set; } = 20;
    
    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Filter by CVSS score threshold
    /// </summary>
    public double? CvssScoreAbove { get; set; }
    
    /// <summary>
    /// Whether to include details
    /// </summary>
    public bool IncludeFields { get; set; } = true;
}

/// <summary>
/// Red Hat API severity levels mapping
/// </summary>
public static class RedHatSeverityLevels
{
    public const string Low = "low";
    public const string Moderate = "moderate"; 
    public const string Important = "important";
    public const string Critical = "critical";
    
    public static readonly Dictionary<string, string> ToStandardSeverity = new()
    {
        { Low, "LOW" },
        { Moderate, "MEDIUM" },
        { Important, "HIGH" },
        { Critical, "CRITICAL" }
    };
    
    public static readonly List<string> AllSeverities = new()
    {
        Low, Moderate, Important, Critical
    };
}