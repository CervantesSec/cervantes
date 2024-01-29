using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

public class ReportParts
{
    public Guid Id { get; set; }
    
    [ForeignKey("ComponentId")]
    [JsonIgnore]
    public virtual ReportComponents Component { get; set; }
    public Guid ComponentId { get; set; }
    
    [ForeignKey("TemplateId")]
    [JsonIgnore]
    public virtual ReportTemplate Template { get; set; } 
    public Guid TemplateId { get; set; }
    
    public int Order { get; set; }
    public ReportPartType PartType { get; set; }
}