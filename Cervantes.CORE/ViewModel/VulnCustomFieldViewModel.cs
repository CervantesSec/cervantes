using System;
using System.ComponentModel.DataAnnotations;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

/// <summary>
/// ViewModel for displaying and editing custom field definitions
/// </summary>
public class VulnCustomFieldViewModel
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(200)]
    public string Label { get; set; }

    [Required]
    public VulnCustomFieldType Type { get; set; }

    public bool IsRequired { get; set; }

    public bool IsUnique { get; set; }

    public bool IsSearchable { get; set; }

    public bool IsVisible { get; set; }

    public int Order { get; set; }

    public string Options { get; set; }

    public string DefaultValue { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; } = true;

    // For UI display
    public string TypeDisplay => Type.ToString();
    
    public string OptionsDisplay => string.IsNullOrEmpty(Options) ? "-" : Options;
}

/// <summary>
/// ViewModel for creating a new custom field
/// </summary>
public class VulnCustomFieldCreateViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(200)]
    public string Label { get; set; }

    [Required]
    public VulnCustomFieldType Type { get; set; }

    public bool IsRequired { get; set; }

    public bool IsUnique { get; set; }

    public bool IsSearchable { get; set; }

    public bool IsVisible { get; set; }

    public int Order { get; set; }

    public string Options { get; set; }

    public string DefaultValue { get; set; }

    public string Description { get; set; }

    public string UserId { get; set; }
}

/// <summary>
/// ViewModel for editing an existing custom field
/// </summary>
public class VulnCustomFieldEditViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(200)]
    public string Label { get; set; }

    [Required]
    public VulnCustomFieldType Type { get; set; }

    public bool IsRequired { get; set; }

    public bool IsUnique { get; set; }

    public bool IsSearchable { get; set; }

    public bool IsVisible { get; set; }

    public int Order { get; set; }

    public string Options { get; set; }

    public string DefaultValue { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }
}

/// <summary>
/// ViewModel for setting custom field values in vulnerability forms
/// </summary>
public class VulnCustomFieldValueViewModel
{
    public Guid CustomFieldId { get; set; }
    
    public string Name { get; set; }
    
    public string Label { get; set; }
    
    public VulnCustomFieldType Type { get; set; }
    
    public bool IsRequired { get; set; }
    
    public bool IsUnique { get; set; }
    
    public string Options { get; set; }
    
    public string DefaultValue { get; set; }
    
    public string Description { get; set; }
    
    public int Order { get; set; }
    
    // The actual value
    public string Value { get; set; }
    
    // For Select fields, parsed options
    public string[] OptionsArray => 
        string.IsNullOrEmpty(Options) ? Array.Empty<string>() : 
        System.Text.Json.JsonSerializer.Deserialize<string[]>(Options);
}