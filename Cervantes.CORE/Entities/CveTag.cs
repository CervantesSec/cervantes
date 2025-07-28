using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// CVE Tag Entity - Custom tags for CVEs
/// </summary>
public class CveTag
{
    /// <summary>
    /// Unique identifier for CVE Tag
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
    /// Tag name
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// Tag color for UI display
    /// </summary>
    [StringLength(20)]
    public string Color { get; set; }

    /// <summary>
    /// Tag description
    /// </summary>
    [StringLength(200)]
    public string Description { get; set; }

    /// <summary>
    /// Whether this is a system tag (predefined)
    /// </summary>
    public bool IsSystemTag { get; set; }

    /// <summary>
    /// Tag creation date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Tag modified date
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// User who created the tag
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id of the user who created the tag
    /// </summary>
    public string UserId { get; set; }
}