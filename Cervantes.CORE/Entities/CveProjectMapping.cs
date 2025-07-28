using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// CVE-Project Mapping Entity - Links CVEs to projects for monitoring
/// </summary>
public class CveProjectMapping
{
    /// <summary>
    /// Unique identifier for CVE-Project mapping
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
    /// Associated Project
    /// </summary>
    [ForeignKey("ProjectId")]
    [JsonIgnore]
    public virtual Project Project { get; set; }

    /// <summary>
    /// Project identifier
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Mapping relevance score (0-1)
    /// </summary>
    public double RelevanceScore { get; set; }

    /// <summary>
    /// Whether this mapping was created automatically
    /// </summary>
    public bool IsAutomatic { get; set; }

    /// <summary>
    /// Whether this mapping has been validated by a user
    /// </summary>
    public bool IsValidated { get; set; }

    /// <summary>
    /// Mapping priority (High, Medium, Low)
    /// </summary>
    [StringLength(20)]
    public string Priority { get; set; }

    /// <summary>
    /// Mapping status (New, Reviewed, Dismissed)
    /// </summary>
    [StringLength(20)]
    public string Status { get; set; }

    /// <summary>
    /// Mapping notes
    /// </summary>
    public string Notes { get; set; }

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