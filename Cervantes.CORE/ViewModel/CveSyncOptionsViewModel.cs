using System.ComponentModel.DataAnnotations;

namespace Cervantes.CORE.ViewModel;

/// <summary>
/// View model for CVE synchronization options
/// </summary>
public class CveSyncOptionsViewModel
{
    /// <summary>
    /// Start year for CVE synchronization (e.g., 2020)
    /// </summary>
    [Display(Name = "Start Year")]
    [Range(1999, 2030, ErrorMessage = "Year must be between 1999 and 2030")]
    public int? StartYear { get; set; }

    /// <summary>
    /// End year for CVE synchronization (e.g., 2024)
    /// </summary>
    [Display(Name = "End Year")]
    [Range(1999, 2030, ErrorMessage = "Year must be between 1999 and 2030")]
    public int? EndYear { get; set; }

    /// <summary>
    /// Start date for published CVEs
    /// </summary>
    [Display(Name = "Published Date From")]
    public DateTime? PublishedDateStart { get; set; }

    /// <summary>
    /// End date for published CVEs
    /// </summary>
    [Display(Name = "Published Date To")]
    public DateTime? PublishedDateEnd { get; set; }

    /// <summary>
    /// Start date for last modified CVEs
    /// </summary>
    [Display(Name = "Last Modified From")]
    public DateTime? LastModifiedStart { get; set; }

    /// <summary>
    /// End date for last modified CVEs
    /// </summary>
    [Display(Name = "Last Modified To")]
    public DateTime? LastModifiedEnd { get; set; }

    /// <summary>
    /// Minimum CVSS v3 base score filter
    /// </summary>
    [Display(Name = "Minimum CVSS Score")]
    [Range(0.0, 10.0, ErrorMessage = "CVSS score must be between 0.0 and 10.0")]
    public double? MinCvssScore { get; set; }

    /// <summary>
    /// Maximum CVSS v3 base score filter
    /// </summary>
    [Display(Name = "Maximum CVSS Score")]
    [Range(0.0, 10.0, ErrorMessage = "CVSS score must be between 0.0 and 10.0")]
    public double? MaxCvssScore { get; set; }

    /// <summary>
    /// CVSS v3 severities to include
    /// </summary>
    [Display(Name = "Severities")]
    public List<string> Severities { get; set; } = new();

    /// <summary>
    /// Only sync CVEs with known exploits (CISA KEV)
    /// </summary>
    [Display(Name = "Only Known Exploited CVEs")]
    public bool OnlyKnownExploited { get; set; }

    /// <summary>
    /// Keyword search filter
    /// </summary>
    [Display(Name = "Keyword Filter")]
    [StringLength(100, ErrorMessage = "Keyword filter must be less than 100 characters")]
    public string? KeywordFilter { get; set; }

    /// <summary>
    /// Maximum number of CVEs to sync per request
    /// </summary>
    [Display(Name = "Results Per Page")]
    [Range(1, 2000, ErrorMessage = "Results per page must be between 1 and 2000")]
    public int ResultsPerPage { get; set; } = 100;

    /// <summary>
    /// Maximum total CVEs to sync (0 = unlimited)
    /// </summary>
    [Display(Name = "Max Total CVEs")]
    [Range(0, 50000, ErrorMessage = "Max total CVEs must be between 0 and 50,000")]
    public int MaxTotalCves { get; set; } = 1000;

    /// <summary>
    /// Skip CVEs that already exist in the database
    /// </summary>
    [Display(Name = "Skip Existing CVEs")]
    public bool SkipExisting { get; set; } = true;

    /// <summary>
    /// Update existing CVEs if newer version available
    /// </summary>
    [Display(Name = "Update Existing CVEs")]
    public bool UpdateExisting { get; set; } = true;

    /// <summary>
    /// Sync source to use
    /// </summary>
    [Display(Name = "Sync Source")]
    public string SyncSource { get; set; } = "NVD";

    /// <summary>
    /// Available sync sources
    /// </summary>
    public static List<string> AvailableSyncSources => new() { "NVD", "RedHat", "MITRE" };

    /// <summary>
    /// Quick preset options
    /// </summary>
    [Display(Name = "Quick Preset")]
    public string? QuickPreset { get; set; }

    /// <summary>
    /// Available severity options
    /// </summary>
    public static List<string> AvailableSeverities => new() { "LOW", "MEDIUM", "HIGH", "CRITICAL" };

    /// <summary>
    /// Available quick preset options
    /// </summary>
    public static Dictionary<string, string> AvailablePresets => new()
    {
        { "recent_critical", "Recent Critical (Last 30 days, CRITICAL)" },
        { "recent_high", "Recent High+ (Last 30 days, HIGH & CRITICAL)" },
        { "last_120_days", "Last 120 days (All severities)" },
        { "last_60_days", "Last 60 days (All severities)" },
        { "known_exploited", "Known Exploited (CISA KEV only)" },
        { "current_month", "Current Month" },
        { "custom", "Custom (Configure manually)" }
    };

    /// <summary>
    /// Apply quick preset settings
    /// </summary>
    /// <param name="preset">Preset name</param>
    public void ApplyQuickPreset(string preset)
    {
        // Clear previous settings
        ResetToDefaults();
        
        var now = DateTime.UtcNow;
        
        switch (preset.ToLower())
        {
            case "recent_critical":
                PublishedDateStart = now.AddDays(-30);
                PublishedDateEnd = now;
                Severities = new List<string> { "CRITICAL" };
                MaxTotalCves = 500;
                break;
                
            case "recent_high":
                PublishedDateStart = now.AddDays(-30);
                PublishedDateEnd = now;
                Severities = new List<string> { "HIGH", "CRITICAL" };
                MaxTotalCves = 1000;
                break;
                
            case "last_120_days":
                PublishedDateStart = now.AddDays(-120);
                PublishedDateEnd = now;
                MaxTotalCves = 5000;
                break;
                
            case "last_60_days":
                PublishedDateStart = now.AddDays(-60);
                PublishedDateEnd = now;
                MaxTotalCves = 2500;
                break;
                
            case "known_exploited":
                OnlyKnownExploited = true;
                MaxTotalCves = 2000;
                break;
                
            case "current_month":
                var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                PublishedDateStart = startOfMonth;
                PublishedDateEnd = now;
                MaxTotalCves = 2000;
                break;
        }
        
        QuickPreset = preset;
    }

    /// <summary>
    /// Reset all options to default values
    /// </summary>
    public void ResetToDefaults()
    {
        StartYear = null;
        EndYear = null;
        PublishedDateStart = null;
        PublishedDateEnd = null;
        LastModifiedStart = null;
        LastModifiedEnd = null;
        MinCvssScore = null;
        MaxCvssScore = null;
        Severities.Clear();
        OnlyKnownExploited = false;
        KeywordFilter = null;
        ResultsPerPage = 100;
        MaxTotalCves = 1000;
        SkipExisting = true;
        UpdateExisting = true;
        QuickPreset = null;
    }

    /// <summary>
    /// Validate the sync options
    /// </summary>
    /// <returns>List of validation errors</returns>
    public List<string> Validate()
    {
        var errors = new List<string>();

        if (StartYear.HasValue && EndYear.HasValue && StartYear > EndYear)
        {
            errors.Add("Start year cannot be greater than end year");
        }

        if (PublishedDateStart.HasValue && PublishedDateEnd.HasValue && PublishedDateStart > PublishedDateEnd)
        {
            errors.Add("Published date start cannot be after published date end");
        }

        if (LastModifiedStart.HasValue && LastModifiedEnd.HasValue && LastModifiedStart > LastModifiedEnd)
        {
            errors.Add("Last modified start cannot be after last modified end");
        }

        // NVD API date range limits (120 days maximum)
        if (PublishedDateStart.HasValue && PublishedDateEnd.HasValue)
        {
            var publishedDateRange = PublishedDateEnd.Value - PublishedDateStart.Value;
            if (publishedDateRange.TotalDays > 120)
            {
                errors.Add("Published date range cannot exceed 120 days (NVD API limitation)");
            }
        }

        if (LastModifiedStart.HasValue && LastModifiedEnd.HasValue)
        {
            var lastModDateRange = LastModifiedEnd.Value - LastModifiedStart.Value;
            if (lastModDateRange.TotalDays > 120)
            {
                errors.Add("Last modified date range cannot exceed 120 days (NVD API limitation)");
            }
        }

        if (MinCvssScore.HasValue && MaxCvssScore.HasValue && MinCvssScore > MaxCvssScore)
        {
            errors.Add("Minimum CVSS score cannot be greater than maximum CVSS score");
        }

        if (MaxTotalCves > 50000)
        {
            errors.Add("Maximum total CVEs cannot exceed 50,000 for performance reasons");
        }

        // Check for reasonable date ranges
        if (StartYear.HasValue && StartYear < 1999)
        {
            errors.Add("Start year cannot be before 1999 (first CVE year)");
        }

        if (EndYear.HasValue && EndYear > DateTime.Now.Year + 1)
        {
            errors.Add("End year cannot be more than one year in the future");
        }

        return errors;
    }

    /// <summary>
    /// Get estimated number of CVEs for the current filters
    /// </summary>
    /// <returns>Estimated count or null if cannot estimate</returns>
    public int? GetEstimatedCount()
    {
        // Simple estimation logic based on known CVE patterns
        if (OnlyKnownExploited)
        {
            return 1500; // Approximate CISA KEV count
        }

        if (StartYear.HasValue && EndYear.HasValue)
        {
            var yearSpan = EndYear.Value - StartYear.Value + 1;
            var avgCvesPerYear = StartYear >= 2010 ? 15000 : 5000; // More CVEs in recent years
            var estimated = yearSpan * avgCvesPerYear;

            // Adjust for severity filters
            if (Severities.Any())
            {
                var severityMultiplier = Severities.Contains("CRITICAL") ? 0.1 : 
                                       Severities.Contains("HIGH") ? 0.3 : 
                                       Severities.Contains("MEDIUM") ? 0.4 : 0.2;
                estimated = (int)(estimated * severityMultiplier);
            }

            return Math.Min(estimated, MaxTotalCves);
        }

        return null; // Cannot estimate without year range
    }
}