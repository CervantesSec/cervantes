using System.ComponentModel.DataAnnotations;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class ChecklistCreateViewModelNew
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    public Guid ChecklistTemplateId { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    public Guid? TargetId { get; set; }

    public string? Notes { get; set; }
}

public class ChecklistExecutionUpdateViewModel
{
    public Guid Id { get; set; }
    
    public ChecklistItemStatus Status { get; set; }
    
    public string? Notes { get; set; }
    
    public string? Evidence { get; set; }
    
    public int? EstimatedTimeMinutes { get; set; }
    
    public int? ActualTimeMinutes { get; set; }
    
    public int? DifficultyRating { get; set; }
    
    public Guid? VulnId { get; set; }
}

public class ChecklistExecutionBulkUpdateViewModel
{
    public List<Guid> ExecutionIds { get; set; } = new();
    
    public ChecklistItemStatus Status { get; set; }
}

public class ChecklistUpdateViewModel
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    public string? Notes { get; set; }
    
    public ChecklistStatus Status { get; set; }
}