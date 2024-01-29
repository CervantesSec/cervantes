using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

public class Report
{
    /// <summary>
    /// Porject Note Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// User who created project
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    [ForeignKey("ProjectId")]
    [JsonIgnore]
    public virtual Project Project { get; set; }
    
    
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Created Date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Note Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    public string Description { get; set; }

    public string Version { get; set; }
    
    public Language Language { get; set; }
    public string HtmlCode { get; set; }
    public ReportType ReportType { get; set; }

}