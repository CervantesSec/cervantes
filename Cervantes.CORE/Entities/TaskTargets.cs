using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

public class TaskTargets
{
    /// <summary>
    /// Task Target Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Task
    /// </summary>
    [ForeignKey("TaskId")]
    [JsonIgnore]
    public virtual Task Task { get; set; }
    
    /// <summary>
    /// TaskId
    /// </summary>
    public Guid TaskId { get; set; }
    
    /// <summary>
    /// Target
    /// </summary>
    [ForeignKey("TargetId")]
    [JsonIgnore]
    public virtual Target Target { get; set; }
    /// <summary>
    /// Target Id
    /// </summary>
    public Guid TargetId { get; set; }
    
}