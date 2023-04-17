using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class VulnCwe
{
    public Guid Id { get; set; }
    
    [ForeignKey("CweId")]
    public virtual Cwe Cwe { get; set; }

    /// <summary>
    /// Id Cwe
    /// </summary>
    public int CweId { get; set; }
    
    [ForeignKey("VulnId")]
    public virtual Vuln Vuln { get; set; }

    /// <summary>
    /// Id vuln
    /// </summary>
    public Guid VulnId { get; set; } 
}