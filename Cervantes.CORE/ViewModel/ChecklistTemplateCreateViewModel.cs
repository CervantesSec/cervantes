using System;
using System.ComponentModel.DataAnnotations;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class ChecklistTemplateCreateViewModel
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Version { get; set; }

    public int? OrganizationId { get; set; }

    public List<ChecklistCategoryCreateViewModel> Categories { get; set; } = new();
}

public class ChecklistCategoryCreateViewModel
{
    // Set when the entry already exists, so an update keeps its id (and the executions that point to it).
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    public string? Description { get; set; }

    public int Order { get; set; }

    public List<ChecklistItemCreateViewModel> Items { get; set; } = new();
}

public class ChecklistItemCreateViewModel
{
    // Set when the entry already exists, so an update keeps its id (and the executions that point to it).
    public Guid? Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Code { get; set; }

    [Required]
    [MaxLength(500)]
    public string Name { get; set; }

    public string? Description { get; set; }

    public string? Objectives { get; set; }

    public string? TestProcedure { get; set; }

    public string? PassCriteria { get; set; }

    public int Order { get; set; }

    public bool IsRequired { get; set; } = false;

    public int Severity { get; set; } = 3;

    public string? References { get; set; }
}