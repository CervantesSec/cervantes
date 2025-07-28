using System.ComponentModel.DataAnnotations;

namespace Cervantes.CORE.ViewModel;

/// <summary>
/// CVE create view model for manual CVE creation
/// </summary>
public class CveCreateViewModel
{
    /// <summary>
    /// CVE identifier (e.g., CVE-2023-1234)
    /// </summary>
    [Required(ErrorMessage = "CVE ID is required")]
    [StringLength(20, ErrorMessage = "CVE ID cannot exceed 20 characters")]
    [RegularExpression(@"^CVE-\d{4}-\d{4,}$", ErrorMessage = "CVE ID format is invalid. Expected format: CVE-YYYY-NNNN")]
    public string CveId { get; set; }

    /// <summary>
    /// CVE title/summary
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(500, ErrorMessage = "Title cannot exceed 500 characters")]
    public string Title { get; set; }

    /// <summary>
    /// CVE description
    /// </summary>
    [Required(ErrorMessage = "Description is required")]
    [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
    public string Description { get; set; }

    /// <summary>
    /// CVE publication date
    /// </summary>
    [Required(ErrorMessage = "Published date is required")]
    public DateTime PublishedDate { get; set; }

    /// <summary>
    /// CVE last modified date
    /// </summary>
    public DateTime? LastModifiedDate { get; set; }

    /// <summary>
    /// CVSS v3.1 base score
    /// </summary>
    [Range(0.0, 10.0, ErrorMessage = "CVSS v3 base score must be between 0.0 and 10.0")]
    public double? CvssV3BaseScore { get; set; }

    /// <summary>
    /// CVSS v3.1 vector string
    /// </summary>
    [StringLength(100, ErrorMessage = "CVSS v3 vector cannot exceed 100 characters")]
    public string CvssV3Vector { get; set; }

    /// <summary>
    /// CVSS v3.1 severity (LOW, MEDIUM, HIGH, CRITICAL)
    /// </summary>
    [StringLength(20, ErrorMessage = "CVSS v3 severity cannot exceed 20 characters")]
    public string CvssV3Severity { get; set; }

    /// <summary>
    /// CVSS v2 base score (for legacy support)
    /// </summary>
    [Range(0.0, 10.0, ErrorMessage = "CVSS v2 base score must be between 0.0 and 10.0")]
    public double? CvssV2BaseScore { get; set; }

    /// <summary>
    /// CVSS v2 vector string
    /// </summary>
    [StringLength(100, ErrorMessage = "CVSS v2 vector cannot exceed 100 characters")]
    public string CvssV2Vector { get; set; }

    /// <summary>
    /// CVSS v2 severity
    /// </summary>
    [StringLength(20, ErrorMessage = "CVSS v2 severity cannot exceed 20 characters")]
    public string CvssV2Severity { get; set; }

    /// <summary>
    /// EPSS (Exploit Prediction Scoring System) score
    /// </summary>
    [Range(0.0, 1.0, ErrorMessage = "EPSS score must be between 0.0 and 1.0")]
    public double? EpssScore { get; set; }

    /// <summary>
    /// EPSS percentile
    /// </summary>
    [Range(0.0, 100.0, ErrorMessage = "EPSS percentile must be between 0.0 and 100.0")]
    public double? EpssPercentile { get; set; }

    /// <summary>
    /// Whether this CVE is in CISA KEV catalog
    /// </summary>
    public bool IsKnownExploited { get; set; }

    /// <summary>
    /// CISA KEV due date if applicable
    /// </summary>
    public DateTime? KevDueDate { get; set; }

    /// <summary>
    /// Primary CWE (Common Weakness Enumeration) ID
    /// </summary>
    [StringLength(20, ErrorMessage = "Primary CWE ID cannot exceed 20 characters")]
    public string PrimaryCweId { get; set; }

    /// <summary>
    /// Primary CWE name
    /// </summary>
    [StringLength(200, ErrorMessage = "Primary CWE name cannot exceed 200 characters")]
    public string PrimaryCweName { get; set; }

    /// <summary>
    /// CVE state (PUBLISHED, MODIFIED, WITHDRAWN, REJECTED)
    /// </summary>
    [StringLength(20, ErrorMessage = "State cannot exceed 20 characters")]
    public string State { get; set; } = "PUBLISHED";

    /// <summary>
    /// Assigner organization
    /// </summary>
    [StringLength(100, ErrorMessage = "Assigner organization cannot exceed 100 characters")]
    public string AssignerOrgId { get; set; }

    /// <summary>
    /// Source information
    /// </summary>
    [StringLength(100, ErrorMessage = "Source identifier cannot exceed 100 characters")]
    public string SourceIdentifier { get; set; }

    /// <summary>
    /// Custom notes for this CVE
    /// </summary>
    [StringLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters")]
    public string Notes { get; set; }

    /// <summary>
    /// Associated project ID (optional)
    /// </summary>
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// CVE configurations (affected products)
    /// </summary>
    public List<CveConfigurationCreateViewModel> Configurations { get; set; } = new();

    /// <summary>
    /// CVE references
    /// </summary>
    public List<CveReferenceCreateViewModel> References { get; set; } = new();

    /// <summary>
    /// CVE tags
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Additional CWE mappings
    /// </summary>
    public List<string> CweIds { get; set; } = new();

    /// <summary>
    /// Validate the create view model
    /// </summary>
    /// <returns>Validation errors</returns>
    public List<string> Validate()
    {
        var errors = new List<string>();

        // CVE ID validation
        if (!string.IsNullOrEmpty(CveId) && !System.Text.RegularExpressions.Regex.IsMatch(CveId, @"^CVE-\d{4}-\d{4,}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            errors.Add("CVE ID format is invalid. Expected format: CVE-YYYY-NNNN");
        }

        // Date validation
        if (PublishedDate == default(DateTime))
        {
            errors.Add("Published date is required");
        }

        if (LastModifiedDate.HasValue && LastModifiedDate < PublishedDate)
        {
            errors.Add("Last modified date cannot be earlier than published date");
        }

        // CVSS validation
        if (CvssV3BaseScore.HasValue && (CvssV3BaseScore < 0 || CvssV3BaseScore > 10))
        {
            errors.Add("CVSS v3 base score must be between 0.0 and 10.0");
        }

        if (CvssV2BaseScore.HasValue && (CvssV2BaseScore < 0 || CvssV2BaseScore > 10))
        {
            errors.Add("CVSS v2 base score must be between 0.0 and 10.0");
        }

        // EPSS validation
        if (EpssScore.HasValue && (EpssScore < 0 || EpssScore > 1))
        {
            errors.Add("EPSS score must be between 0.0 and 1.0");
        }

        if (EpssPercentile.HasValue && (EpssPercentile < 0 || EpssPercentile > 100))
        {
            errors.Add("EPSS percentile must be between 0.0 and 100.0");
        }

        // KEV validation
        if (IsKnownExploited && !KevDueDate.HasValue)
        {
            errors.Add("CISA KEV due date is required for known exploited CVEs");
        }

        if (!IsKnownExploited && KevDueDate.HasValue)
        {
            errors.Add("CISA KEV due date should only be set for known exploited CVEs");
        }

        // Severity validation
        if (!string.IsNullOrEmpty(CvssV3Severity))
        {
            var validSeverities = new[] { "LOW", "MEDIUM", "HIGH", "CRITICAL" };
            if (!validSeverities.Contains(CvssV3Severity.ToUpper()))
            {
                errors.Add("CVSS v3 severity must be one of: LOW, MEDIUM, HIGH, CRITICAL");
            }
        }

        // State validation
        if (!string.IsNullOrEmpty(State))
        {
            var validStates = new[] { "PUBLISHED", "MODIFIED", "WITHDRAWN", "REJECTED" };
            if (!validStates.Contains(State.ToUpper()))
            {
                errors.Add("CVE state must be one of: PUBLISHED, MODIFIED, WITHDRAWN, REJECTED");
            }
        }

        return errors;
    }

    /// <summary>
    /// Set default values
    /// </summary>
    public void SetDefaults()
    {
        if (PublishedDate == default(DateTime))
        {
            PublishedDate = DateTime.UtcNow;
        }

        if (string.IsNullOrEmpty(State))
        {
            State = "PUBLISHED";
        }

        if (string.IsNullOrEmpty(SourceIdentifier))
        {
            SourceIdentifier = "manual";
        }

        LastModifiedDate ??= PublishedDate;
    }
}

/// <summary>
/// CVE configuration create view model
/// </summary>
public class CveConfigurationCreateViewModel
{
    /// <summary>
    /// CPE (Common Platform Enumeration) match string
    /// </summary>
    [Required(ErrorMessage = "CPE URI is required")]
    [StringLength(500, ErrorMessage = "CPE URI cannot exceed 500 characters")]
    public string CpeUri { get; set; }

    /// <summary>
    /// Vendor name
    /// </summary>
    [Required(ErrorMessage = "Vendor is required")]
    [StringLength(100, ErrorMessage = "Vendor cannot exceed 100 characters")]
    public string Vendor { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    [Required(ErrorMessage = "Product is required")]
    [StringLength(100, ErrorMessage = "Product cannot exceed 100 characters")]
    public string Product { get; set; }

    /// <summary>
    /// Version information
    /// </summary>
    [StringLength(50, ErrorMessage = "Version cannot exceed 50 characters")]
    public string Version { get; set; }

    /// <summary>
    /// Version start including
    /// </summary>
    [StringLength(50, ErrorMessage = "Version start including cannot exceed 50 characters")]
    public string VersionStartIncluding { get; set; }

    /// <summary>
    /// Version start excluding
    /// </summary>
    [StringLength(50, ErrorMessage = "Version start excluding cannot exceed 50 characters")]
    public string VersionStartExcluding { get; set; }

    /// <summary>
    /// Version end including
    /// </summary>
    [StringLength(50, ErrorMessage = "Version end including cannot exceed 50 characters")]
    public string VersionEndIncluding { get; set; }

    /// <summary>
    /// Version end excluding
    /// </summary>
    [StringLength(50, ErrorMessage = "Version end excluding cannot exceed 50 characters")]
    public string VersionEndExcluding { get; set; }

    /// <summary>
    /// Whether this is a vulnerable configuration
    /// </summary>
    public bool IsVulnerable { get; set; } = true;

    /// <summary>
    /// Running on information
    /// </summary>
    [StringLength(100, ErrorMessage = "Running on cannot exceed 100 characters")]
    public string RunningOn { get; set; }
}

/// <summary>
/// CVE reference create view model
/// </summary>
public class CveReferenceCreateViewModel
{
    /// <summary>
    /// Reference URL
    /// </summary>
    [Required(ErrorMessage = "URL is required")]
    [StringLength(1000, ErrorMessage = "URL cannot exceed 1000 characters")]
    [Url(ErrorMessage = "Invalid URL format")]
    public string Url { get; set; }

    /// <summary>
    /// Reference name/title
    /// </summary>
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; }

    /// <summary>
    /// Reference source
    /// </summary>
    [StringLength(100, ErrorMessage = "Source cannot exceed 100 characters")]
    public string Source { get; set; }

    /// <summary>
    /// Reference tags (comma-separated)
    /// </summary>
    [StringLength(500, ErrorMessage = "Tags cannot exceed 500 characters")]
    public string Tags { get; set; }

    /// <summary>
    /// Reference type (e.g., "Advisory", "Exploit", "Patch")
    /// </summary>
    [StringLength(50, ErrorMessage = "Reference type cannot exceed 50 characters")]
    public string ReferenceType { get; set; }
}