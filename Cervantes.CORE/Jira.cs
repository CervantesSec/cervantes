using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class Jira
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
    [ForeignKey("VulnId")]
    public virtual Vuln Vuln { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public Guid VulnId { get; set; }
    
    /// <summary>
    /// User who created project
    /// </summary>
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }
    
    /// <summary>
    /// Vuln Created Date
    /// </summary>
    public DateTime CreatedDate { get; set; }
    public string JiraIdentifier { get; set; }
    public string JiraKey { get; set; }
    public string Name { get; set; }
    public string Reporter { get; set; }
    public string Assignee { get; set; }
    public string JiraType { get; set; }
    public string Label { get; set; }
    public long? Votes { get; set; }
    public string Interested { get; set; }
    public DateTime? JiraCreatedDate { get; set; }
    public DateTime? JiraUpdatedDate { get; set; }
    public string JiraStatus { get; set; }
    public string JiraComponent { get; set; }
    public string Priority { get; set; }
    public string JiraProject { get; set; }
    public string Resolution { get; set; }
    public DateTime? ResolutionDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string SecurityLevel { get; set; }
    
    
    
}