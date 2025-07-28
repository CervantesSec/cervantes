using System.ComponentModel.DataAnnotations;

namespace Cervantes.CORE.ViewModel;

/// <summary>
/// CVE search view model for advanced search functionality
/// </summary>
public class CveSearchViewModel
{
    /// <summary>
    /// Free text search across CVE ID, title, and description
    /// </summary>
    [StringLength(500)]
    public string SearchTerm { get; set; }

    /// <summary>
    /// Specific CVE ID to search for
    /// </summary>
    [StringLength(20)]
    public string CveId { get; set; }

    /// <summary>
    /// Vendor name filter
    /// </summary>
    [StringLength(100)]
    public string Vendor { get; set; }

    /// <summary>
    /// Product name filter
    /// </summary>
    [StringLength(100)]
    public string Product { get; set; }

    /// <summary>
    /// Minimum CVSS v3 score
    /// </summary>
    [Range(0.0, 10.0)]
    public double? MinCvssScore { get; set; }

    /// <summary>
    /// Maximum CVSS v3 score
    /// </summary>
    [Range(0.0, 10.0)]
    public double? MaxCvssScore { get; set; }

    /// <summary>
    /// CVSS v3 severity filter (LOW, MEDIUM, HIGH, CRITICAL)
    /// </summary>
    public List<string> Severities { get; set; } = new();

    /// <summary>
    /// Minimum EPSS score
    /// </summary>
    [Range(0.0, 1.0)]
    public double? MinEpssScore { get; set; }

    /// <summary>
    /// Maximum EPSS score
    /// </summary>
    [Range(0.0, 1.0)]
    public double? MaxEpssScore { get; set; }

    /// <summary>
    /// CWE IDs filter
    /// </summary>
    public List<string> CweIds { get; set; } = new();

    /// <summary>
    /// Published date range start
    /// </summary>
    public DateTime? PublishedDateStart { get; set; }

    /// <summary>
    /// Published date range end
    /// </summary>
    public DateTime? PublishedDateEnd { get; set; }

    /// <summary>
    /// Last modified date range start
    /// </summary>
    public DateTime? ModifiedDateStart { get; set; }

    /// <summary>
    /// Last modified date range end
    /// </summary>
    public DateTime? ModifiedDateEnd { get; set; }

    /// <summary>
    /// Filter for known exploited CVEs only
    /// </summary>
    public bool? KnownExploited { get; set; }

    /// <summary>
    /// Filter for CVEs with CISA KEV due date
    /// </summary>
    public bool? HasKevDueDate { get; set; }

    /// <summary>
    /// CVE state filter (PUBLISHED, MODIFIED, WITHDRAWN, REJECTED)
    /// </summary>
    public List<string> States { get; set; } = new();

    /// <summary>
    /// Source identifier filter
    /// </summary>
    public List<string> Sources { get; set; } = new();

    /// <summary>
    /// Filter for favorite CVEs
    /// </summary>
    public bool? IsFavorite { get; set; }

    /// <summary>
    /// Filter for read/unread CVEs
    /// </summary>
    public bool? IsRead { get; set; }

    /// <summary>
    /// Filter for archived CVEs
    /// </summary>
    public bool? IsArchived { get; set; }

    /// <summary>
    /// Filter for CVEs with notes
    /// </summary>
    public bool? HasNotes { get; set; }

    /// <summary>
    /// Project ID filter
    /// </summary>
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// Tags filter
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Sort field
    /// </summary>
    public string SortBy { get; set; } = "PublishedDate";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "desc";

    /// <summary>
    /// Page number for pagination
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size for pagination
    /// </summary>
    [Range(1, 1000)]
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// Include related data
    /// </summary>
    public bool IncludeConfigurations { get; set; } = false;

    /// <summary>
    /// Include references
    /// </summary>
    public bool IncludeReferences { get; set; } = false;

    /// <summary>
    /// Include tags
    /// </summary>
    public bool IncludeTags { get; set; } = false;

    /// <summary>
    /// Include CWE mappings
    /// </summary>
    public bool IncludeCweMappings { get; set; } = false;

    /// <summary>
    /// Include project mappings
    /// </summary>
    public bool IncludeProjectMappings { get; set; } = false;

    /// <summary>
    /// Date range preset (today, week, month, year)
    /// </summary>
    public string DateRangePreset { get; set; }

    /// <summary>
    /// Custom search filters as JSON
    /// </summary>
    public string CustomFilters { get; set; }

    /// <summary>
    /// Search name for saved searches
    /// </summary>
    [StringLength(100)]
    public string SearchName { get; set; }

    /// <summary>
    /// User ID for user-specific searches
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Validate search parameters
    /// </summary>
    /// <returns>Validation result</returns>
    public List<string> Validate()
    {
        var errors = new List<string>();

        if (MinCvssScore.HasValue && MaxCvssScore.HasValue && MinCvssScore > MaxCvssScore)
        {
            errors.Add("Minimum CVSS score cannot be greater than maximum CVSS score");
        }

        if (MinEpssScore.HasValue && MaxEpssScore.HasValue && MinEpssScore > MaxEpssScore)
        {
            errors.Add("Minimum EPSS score cannot be greater than maximum EPSS score");
        }

        if (PublishedDateStart.HasValue && PublishedDateEnd.HasValue && PublishedDateStart > PublishedDateEnd)
        {
            errors.Add("Published date start cannot be greater than published date end");
        }

        if (ModifiedDateStart.HasValue && ModifiedDateEnd.HasValue && ModifiedDateStart > ModifiedDateEnd)
        {
            errors.Add("Modified date start cannot be greater than modified date end");
        }

        if (Page < 1)
        {
            errors.Add("Page number must be greater than 0");
        }

        if (PageSize < 1 || PageSize > 1000)
        {
            errors.Add("Page size must be between 1 and 1000");
        }

        if (!string.IsNullOrEmpty(CveId) && !System.Text.RegularExpressions.Regex.IsMatch(CveId, @"^CVE-\d{4}-\d{4,}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            errors.Add("CVE ID format is invalid. Expected format: CVE-YYYY-NNNN");
        }

        return errors;
    }

    /// <summary>
    /// Apply date range preset
    /// </summary>
    public void ApplyDateRangePreset()
    {
        var now = DateTime.UtcNow;
        
        switch (DateRangePreset?.ToLower())
        {
            case "today":
                PublishedDateStart = now.Date;
                PublishedDateEnd = now.Date.AddDays(1);
                break;
            case "week":
                PublishedDateStart = now.Date.AddDays(-7);
                PublishedDateEnd = now.Date.AddDays(1);
                break;
            case "month":
                PublishedDateStart = now.Date.AddDays(-30);
                PublishedDateEnd = now.Date.AddDays(1);
                break;
            case "year":
                PublishedDateStart = now.Date.AddDays(-365);
                PublishedDateEnd = now.Date.AddDays(1);
                break;
        }
    }

    /// <summary>
    /// Get search hash for caching
    /// </summary>
    /// <returns>Search hash</returns>
    public string GetSearchHash()
    {
        var searchString = $"{SearchTerm}|{CveId}|{Vendor}|{Product}|{MinCvssScore}|{MaxCvssScore}|{string.Join(",", Severities)}|{MinEpssScore}|{MaxEpssScore}|{string.Join(",", CweIds)}|{PublishedDateStart}|{PublishedDateEnd}|{ModifiedDateStart}|{ModifiedDateEnd}|{KnownExploited}|{HasKevDueDate}|{string.Join(",", States)}|{string.Join(",", Sources)}|{IsFavorite}|{IsRead}|{IsArchived}|{HasNotes}|{ProjectId}|{string.Join(",", Tags)}|{SortBy}|{SortDirection}|{Page}|{PageSize}|{CustomFilters}|{UserId}";
        
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(searchString));
        return Convert.ToBase64String(hash);
    }
}