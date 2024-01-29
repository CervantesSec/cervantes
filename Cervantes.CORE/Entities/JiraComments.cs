using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

public class JiraComments
{
    /// <summary>
    /// Id jira
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    /// <summary>
    /// User who created project
    /// </summary>
    [ForeignKey("JiraId")]
    [JsonIgnore]
    public virtual Jira Jira { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public Guid JiraId { get; set; }
    
    public string? JiraIdComment { get; set; }
    public string? Author { get; set; }
    public string? Body { get; set; }

    public string? GroupLevel { get; set; }

    public string? RoleLevel { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdateAuthor { get; set; }
    public DateTime? UpdatedDate { get; set; }
}