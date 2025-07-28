namespace Cervantes.CORE.ViewModel;

/// <summary>
/// Enrichment result for a single CVE
/// </summary>
public class VulnEnrichmentResult
{
    public string CveId { get; set; } = string.Empty;
    public double? EpssScore { get; set; }
    public double? EpssPercentile { get; set; }
    public bool IsKnownExploited { get; set; }
    public DateTime? KevDueDate { get; set; }
    public string? KevRequiredAction { get; set; }
    public string? KevShortDescription { get; set; }
    public bool KnownRansomwareUse { get; set; }
}

/// <summary>
/// Batch enrichment options
/// </summary>
public class EnrichmentOptions
{
    /// <summary>
    /// Maximum number of CVEs to process in a single batch
    /// </summary>
    public int BatchSize { get; set; } = 100;
    
    /// <summary>
    /// Whether to include EPSS enrichment
    /// </summary>
    public bool IncludeEpss { get; set; } = true;
    
    /// <summary>
    /// Whether to include CISA KEV enrichment
    /// </summary>
    public bool IncludeCisaKev { get; set; } = true;
    
    /// <summary>
    /// Date threshold - only enrich CVEs newer than this date
    /// </summary>
    public DateTime? DateThreshold { get; set; }
    
    /// <summary>
    /// Whether to update existing enrichment data
    /// </summary>
    public bool UpdateExisting { get; set; } = false;
}