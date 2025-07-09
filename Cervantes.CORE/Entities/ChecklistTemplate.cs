using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// Plantilla de checklist que define la estructura base que pueden usar los usuarios
/// </summary>
public class ChecklistTemplate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre de la plantilla (ej: "OWASP WSTG v4.2", "Custom Security Checklist")
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    /// <summary>
    /// Descripción de la plantilla
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Usuario que creó la plantilla
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// ID del usuario que creó la plantilla
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Organización a la que pertenece esta plantilla (null = disponible para todos)
    /// </summary>
    [ForeignKey("OrganizationId")]
    [JsonIgnore]
    public virtual Organization? Organization { get; set; }

    /// <summary>
    /// ID de la organización (null = plantilla global)
    /// </summary>
    public int? OrganizationId { get; set; }

    /// <summary>
    /// Indica si es una plantilla del sistema (WSTG, MASTG) que no se puede editar
    /// </summary>
    public bool IsSystemTemplate { get; set; } = false;

    /// <summary>
    /// Indica si la plantilla está activa
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// Versión de la plantilla
    /// </summary>
    [MaxLength(50)]
    public string? Version { get; set; }

    /// <summary>
    /// Categorías de la plantilla
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<ChecklistCategory> Categories { get; set; } = new List<ChecklistCategory>();

    /// <summary>
    /// Instancias de checklist creadas a partir de esta plantilla
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<Checklist> Checklists { get; set; } = new List<Checklist>();
}