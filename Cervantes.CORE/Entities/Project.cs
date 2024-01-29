using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cervantes.CORE.Entities;

public class Project
{
    /// <summary>
    /// Project Id
    /// </summary>
    ///
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Project Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Project Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Project Start Date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Project End Date
    /// </summary>
    public DateTime EndDate { get; set; }

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

    /// <summary>
    /// Client asssociated to project
    /// </summary>
    [ForeignKey("ClientId")]
    [JsonIgnore]
    public virtual Client Client { get; set; }

    /// <summary>
    /// Client ID
    /// </summary>
    public Guid ClientId { get; set; }

    /// <summary>
    /// Is project a template
    /// </summary>
    public bool Template { get; set; }

    /// <summary>
    /// Project Status
    /// </summary>
    public ProjectStatus Status { get; set; }

    /// <summary>
    /// Project Type
    /// </summary>
    public ProjectType ProjectType { get; set; }
    
    /// <summary>
    /// Project Language
    /// </summary>
    public Language Language { get; set; }
    
    /// <summary>
    /// Project Scoreing type
    /// </summary>
    public Score Score { get; set; }
    
    public string FindingsId { get; set; }
    public string ExecutiveSummary { get; set; }
}