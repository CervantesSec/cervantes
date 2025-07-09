using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// Item individual dentro de una categoría de checklist
/// </summary>
public class ChecklistItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Código del item (ej: "WSTG-INFO-01", "CUSTOM-AUTH-01")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Code { get; set; }

    /// <summary>
    /// Nombre/título del item
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Name { get; set; }

    /// <summary>
    /// Descripción detallada del item
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Objetivos del test/verificación
    /// </summary>
    public string? Objectives { get; set; }

    /// <summary>
    /// Procedimiento a seguir
    /// </summary>
    public string? TestProcedure { get; set; }

    /// <summary>
    /// Criterios de éxito/fallo
    /// </summary>
    public string? PassCriteria { get; set; }

    /// <summary>
    /// Categoría a la que pertenece este item
    /// </summary>
    [ForeignKey("ChecklistCategoryId")]
    [JsonIgnore]
    public virtual ChecklistCategory ChecklistCategory { get; set; }

    /// <summary>
    /// ID de la categoría
    /// </summary>
    public Guid ChecklistCategoryId { get; set; }

    /// <summary>
    /// Orden de visualización del item dentro de la categoría
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Indica si el item es obligatorio
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Nivel de severidad/importancia (1-5)
    /// </summary>
    public int Severity { get; set; } = 3;

    /// <summary>
    /// Referencias externas (URLs, documentos, etc.)
    /// </summary>
    public string? References { get; set; }

    /// <summary>
    /// Ejecuciones de este item en checklists específicos
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<ChecklistExecution> Executions { get; set; } = new List<ChecklistExecution>();
}