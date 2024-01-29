using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

public class TargetServices
{
    /// <summary>
    /// Id Target Services
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// User who created targetservice
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Target associated
    /// </summary>
    [ForeignKey("TargetId")]
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id taregt
    /// </summary>
    public Guid TargetId { get; set; }

    /// <summary>
    /// Name of the service
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description of the service
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Port of the service
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Version of the service
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Notes of the service
    /// </summary>
    public string Note { get; set; }
}