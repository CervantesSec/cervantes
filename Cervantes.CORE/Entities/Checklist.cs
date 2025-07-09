using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// Instancia de checklist aplicada a un proyecto/target específico
/// </summary>
public class Checklist
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre de esta instancia de checklist
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    /// <summary>
    /// Plantilla base utilizada
    /// </summary>
    [ForeignKey("ChecklistTemplateId")]
    [JsonIgnore]
    public virtual ChecklistTemplate ChecklistTemplate { get; set; }

    /// <summary>
    /// ID de la plantilla base
    /// </summary>
    public Guid ChecklistTemplateId { get; set; }

    /// <summary>
    /// Proyecto al que pertenece este checklist
    /// </summary>
    [ForeignKey("ProjectId")]
    [JsonIgnore]
    public virtual Project Project { get; set; }

    /// <summary>
    /// ID del proyecto
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Target específico (opcional, puede aplicarse a todo el proyecto)
    /// </summary>
    [ForeignKey("TargetId")]
    [JsonIgnore]
    public virtual Target? Target { get; set; }

    /// <summary>
    /// ID del target (opcional)
    /// </summary>
    public Guid? TargetId { get; set; }

    /// <summary>
    /// Usuario que creó este checklist
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// ID del usuario creador
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Estado general del checklist
    /// </summary>
    public ChecklistStatus Status { get; set; } = ChecklistStatus.NotStarted;

    /// <summary>
    /// Fecha de creación
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// Fecha de inicio del checklist
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Fecha de finalización del checklist
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Notas generales del checklist
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Porcentaje de completitud (calculado)
    /// </summary>
    public decimal CompletionPercentage { get; set; } = 0;

    /// <summary>
    /// Ejecuciones de los items individuales
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<ChecklistExecution> Executions { get; set; } = new List<ChecklistExecution>();
}