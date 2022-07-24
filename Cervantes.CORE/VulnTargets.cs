using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class VulnTargets
{
    public Guid Id { get; set; }
    
    [ForeignKey("VulnId")]
    public virtual Vuln Vuln { get; set; }

    /// <summary>
    /// Id vuln
    /// </summary>
    public Guid VulnId { get; set; }
    
    [ForeignKey("TargetId")]
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id target
    /// </summary>
    public Guid TargetId { get; set; }
    
}