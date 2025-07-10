using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// Defines a custom field schema that can be used for projects
/// </summary>
public class ProjectCustomField
{
    /// <summary>
    /// Unique identifier for the custom field
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Internal name for the field (alphanumeric + spaces only, used in templates)
    /// Must be unique across all custom fields
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    /// <summary>
    /// Display label for the field in the UI
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Label { get; set; }

    /// <summary>
    /// Type of the custom field (Input, Textarea, Select, etc.)
    /// </summary>
    [Required]
    public ProjectCustomFieldType Type { get; set; }

    /// <summary>
    /// Whether this field is required when creating/editing projects
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Whether the value of this field must be unique across all projects
    /// </summary>
    public bool IsUnique { get; set; }

    /// <summary>
    /// Whether this field can be searched
    /// </summary>
    public bool IsSearchable { get; set; }

    /// <summary>
    /// Whether this field should be visible as a column in the project table
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Display order of the field in forms and tables
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// JSON string containing options for Select type fields
    /// Format: ["Option1", "Option2", "Option3"]
    /// </summary>
    public string Options { get; set; }

    /// <summary>
    /// Default value for the field
    /// </summary>
    public string DefaultValue { get; set; }

    /// <summary>
    /// Description/help text for the field
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// When the custom field was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// When the custom field was last modified
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// User who created the custom field
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// ID of the user who created the custom field
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Whether the field is active (can be used)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Collection of values for this custom field across all projects
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<ProjectCustomFieldValue> Values { get; set; } = new List<ProjectCustomFieldValue>();
}