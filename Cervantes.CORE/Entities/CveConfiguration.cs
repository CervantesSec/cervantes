using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// CVE Configuration Entity - Represents affected products/versions
/// </summary>
public class CveConfiguration
{
    /// <summary>
    /// Unique identifier for CVE Configuration
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Associated CVE
    /// </summary>
    [ForeignKey("CveId")]
    [JsonIgnore]
    public virtual Cve Cve { get; set; }

    /// <summary>
    /// CVE identifier
    /// </summary>
    [Required]
    public Guid CveId { get; set; }

    /// <summary>
    /// CPE (Common Platform Enumeration) match string
    /// </summary>
    [Required]
    [StringLength(500)]
    public string CpeUri { get; set; }

    /// <summary>
    /// Vendor name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Vendor { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Product { get; set; }

    /// <summary>
    /// Version information
    /// </summary>
    [StringLength(50)]
    public string Version { get; set; }

    /// <summary>
    /// Version start including
    /// </summary>
    [StringLength(50)]
    public string VersionStartIncluding { get; set; }

    /// <summary>
    /// Version start excluding
    /// </summary>
    [StringLength(50)]
    public string VersionStartExcluding { get; set; }

    /// <summary>
    /// Version end including
    /// </summary>
    [StringLength(50)]
    public string VersionEndIncluding { get; set; }

    /// <summary>
    /// Version end excluding
    /// </summary>
    [StringLength(50)]
    public string VersionEndExcluding { get; set; }

    /// <summary>
    /// Whether this is a vulnerable configuration
    /// </summary>
    public bool IsVulnerable { get; set; }

    /// <summary>
    /// Running on information
    /// </summary>
    [StringLength(100)]
    public string RunningOn { get; set; }

    /// <summary>
    /// Configuration created date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Configuration modified date
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// User who created the configuration
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id of the user who created the configuration
    /// </summary>
    public string UserId { get; set; }
}