using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// CVE-CWE Mapping Entity - Links CVEs to CWEs
/// </summary>
public class CveCwe
{
    /// <summary>
    /// Unique identifier for CVE-CWE mapping
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
    /// Associated CWE
    /// </summary>
    [ForeignKey("CweId")]
    [JsonIgnore]
    public virtual Cwe Cwe { get; set; }

    /// <summary>
    /// CWE identifier
    /// </summary>
    [Required]
    public int CweId { get; set; }

    /// <summary>
    /// Whether this is the primary CWE for this CVE
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// CWE source (e.g., "cna", "nvd")
    /// </summary>
    [StringLength(20)]
    public string Source { get; set; }

    /// <summary>
    /// Mapping creation date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Mapping modified date
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// User who created the mapping
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id of the user who created the mapping
    /// </summary>
    public string UserId { get; set; }
}