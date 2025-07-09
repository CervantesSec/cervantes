using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// Ejecución de un item específico de checklist
/// </summary>
public class ChecklistExecution
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Checklist al que pertenece esta ejecución
    /// </summary>
    [ForeignKey("ChecklistId")]
    [JsonIgnore]
    public virtual Checklist Checklist { get; set; }

    /// <summary>
    /// ID del checklist
    /// </summary>
    public Guid ChecklistId { get; set; }

    /// <summary>
    /// Item del checklist que se está ejecutando
    /// </summary>
    [ForeignKey("ChecklistItemId")]
    [JsonIgnore]
    public virtual ChecklistItem ChecklistItem { get; set; }

    /// <summary>
    /// ID del item
    /// </summary>
    public Guid ChecklistItemId { get; set; }

    /// <summary>
    /// Estado de la ejecución de este item
    /// </summary>
    public ChecklistItemStatus Status { get; set; } = ChecklistItemStatus.NotTested;

    /// <summary>
    /// Notas/observaciones de la ejecución
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Evidencia (archivos, screenshots, etc.)
    /// </summary>
    public string? Evidence { get; set; }

    /// <summary>
    /// Usuario que realizó la verificación
    /// </summary>
    [ForeignKey("TestedByUserId")]
    [JsonIgnore]
    public virtual ApplicationUser? TestedByUser { get; set; }

    /// <summary>
    /// ID del usuario que realizó la verificación
    /// </summary>
    public string? TestedByUserId { get; set; }

    /// <summary>
    /// Fecha de la verificación
    /// </summary>
    public DateTime? TestedDate { get; set; }

    /// <summary>
    /// Tiempo estimado para completar (en minutos)
    /// </summary>
    public int? EstimatedTimeMinutes { get; set; }

    /// <summary>
    /// Tiempo real utilizado (en minutos)
    /// </summary>
    public int? ActualTimeMinutes { get; set; }

    /// <summary>
    /// Puntuación/rating (1-5) de la facilidad de ejecución
    /// </summary>
    public int? DifficultyRating { get; set; }

    /// <summary>
    /// Vulnerabilidad asociada si se encontró alguna
    /// </summary>
    [ForeignKey("VulnId")]
    [JsonIgnore]
    public virtual Vuln? Vulnerability { get; set; }

    /// <summary>
    /// ID de la vulnerabilidad asociada
    /// </summary>
    public Guid? VulnId { get; set; }
}