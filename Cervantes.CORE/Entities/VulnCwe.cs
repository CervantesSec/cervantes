using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

public class VulnCwe
{
    public Guid Id { get; set; }
    
    [ForeignKey("CweId")]
    [JsonIgnore]
    public virtual Cwe Cwe { get; set; }

    /// <summary>
    /// Id Cwe
    /// </summary>
    public int CweId { get; set; }
    
    [ForeignKey("VulnId")]
    [JsonIgnore]
    public virtual Vuln Vuln { get; set; }

    /// <summary>
    /// Id vuln
    /// </summary>
    public Guid VulnId { get; set; } 
}