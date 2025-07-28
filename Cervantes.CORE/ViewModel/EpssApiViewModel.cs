namespace Cervantes.CORE.ViewModel;

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

