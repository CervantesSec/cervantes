using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// CVE Reference Entity - External references for CVEs
/// </summary>
public class CveReference
{
    /// <summary>
    /// Unique identifier for CVE Reference
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
    /// Reference URL
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Url { get; set; }

    /// <summary>
    /// Reference name/title
    /// </summary>
    [StringLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// Reference source
    /// </summary>
    [StringLength(100)]
    public string Source { get; set; }

    /// <summary>
    /// Reference tags (e.g., "Exploit", "Patch", "Vendor Advisory")
    /// </summary>
    [StringLength(500)]
    public string Tags { get; set; }

    /// <summary>
    /// Reference type (e.g., "Advisory", "Exploit", "Patch")
    /// </summary>
    [StringLength(50)]
    public string ReferenceType { get; set; }

    /// <summary>
    /// Reference creation date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Reference modified date
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// User who created the reference
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id of the user who created the reference
    /// </summary>
    public string UserId { get; set; }
}