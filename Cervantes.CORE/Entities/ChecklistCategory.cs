using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// Categoría dentro de una plantilla de checklist (ej: "Information Gathering", "Authentication")
/// </summary>
public class ChecklistCategory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre de la categoría
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    /// <summary>
    /// Descripción de la categoría
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Plantilla a la que pertenece esta categoría
    /// </summary>
    [ForeignKey("ChecklistTemplateId")]
    [JsonIgnore]
    public virtual ChecklistTemplate ChecklistTemplate { get; set; }

    /// <summary>
    /// ID de la plantilla
    /// </summary>
    public Guid ChecklistTemplateId { get; set; }

    /// <summary>
    /// Orden de visualización de la categoría
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Items de la categoría
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<ChecklistItem> Items { get; set; } = new List<ChecklistItem>();
}