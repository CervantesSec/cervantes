using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

public class VulnTargets
{
    public Guid Id { get; set; }
    
    [ForeignKey("VulnId")]
    [JsonIgnore]
    public virtual Vuln Vuln { get; set; }

    /// <summary>
    /// Id vuln
    /// </summary>
    public Guid VulnId { get; set; }
    
    [ForeignKey("TargetId")]
    [JsonIgnore]
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id target
    /// </summary>
    public Guid TargetId { get; set; }
    
}