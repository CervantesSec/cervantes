using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// CVE (Common Vulnerabilities and Exposures) Entity
/// </summary>
public class Cve
{
    /// <summary>
    /// Unique identifier for CVE
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// CVE identifier (e.g., CVE-2023-1234)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string CveId { get; set; }

    /// <summary>
    /// CVE title/summary
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Title { get; set; }

    /// <summary>
    /// CVE description
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// CVE publication date
    /// </summary>
    public DateTime PublishedDate { get; set; }

    /// <summary>
    /// CVE last modified date
    /// </summary>
    public DateTime LastModifiedDate { get; set; }

    /// <summary>
    /// CVE creation date in our system
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// CVE modified date in our system
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// User who created the CVE entry
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id of the user who created the CVE entry
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// CVSS v3.1 base score
    /// </summary>
    public double? CvssV3BaseScore { get; set; }

    /// <summary>
    /// CVSS v3.1 vector string
    /// </summary>
    [StringLength(100)]
    public string? CvssV3Vector { get; set; }

    /// <summary>
    /// CVSS v3.1 severity (LOW, MEDIUM, HIGH, CRITICAL)
    /// </summary>
    [StringLength(20)]
    public string? CvssV3Severity { get; set; }

    /// <summary>
    /// CVSS v2 base score (for legacy support)
    /// </summary>
    public double? CvssV2BaseScore { get; set; }

    /// <summary>
    /// CVSS v2 vector string
    /// </summary>
    [StringLength(100)]
    public string? CvssV2Vector { get; set; }

    /// <summary>
    /// CVSS v2 severity
    /// </summary>
    [StringLength(20)]
    public string? CvssV2Severity { get; set; }

    /// <summary>
    /// EPSS (Exploit Prediction Scoring System) score
    /// </summary>
    public double? EpssScore { get; set; }

    /// <summary>
    /// EPSS percentile
    /// </summary>
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
    [StringLength(20)]
    public string? PrimaryCweId { get; set; }

    /// <summary>
    /// Primary CWE name
    /// </summary>
    [StringLength(200)]
    public string? PrimaryCweName { get; set; }

    /// <summary>
    /// CVE state (PUBLISHED, MODIFIED, WITHDRAWN, REJECTED)
    /// </summary>
    [StringLength(20)]
    public string? State { get; set; }

    /// <summary>
    /// Assigner organization
    /// </summary>
    [StringLength(100)]
    public string? AssignerOrgId { get; set; }

    /// <summary>
    /// Source information
    /// </summary>
    [StringLength(100)]
    public string? SourceIdentifier { get; set; }

    /// <summary>
    /// Whether this CVE is marked as favorite by user
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Whether this CVE has been read by user
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Whether this CVE is archived
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Custom notes for this CVE
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Associated CVE configurations (affected products)
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<CveConfiguration> Configurations { get; set; } = new List<CveConfiguration>();

    /// <summary>
    /// Associated CVE references
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<CveReference> References { get; set; } = new List<CveReference>();

    /// <summary>
    /// Associated CVE tags
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<CveTag> Tags { get; set; } = new List<CveTag>();

    /// <summary>
    /// Associated CVE-CWE mappings
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<CveCwe> CweMappings { get; set; } = new List<CveCwe>();

    /// <summary>
    /// Associated vulnerabilities in projects
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<VulnCve> VulnCves { get; set; } = new List<VulnCve>();

    /// <summary>
    /// Associated project mappings
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<CveProjectMapping> ProjectMappings { get; set; } = new List<CveProjectMapping>();
}