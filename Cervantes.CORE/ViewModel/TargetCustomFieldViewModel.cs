using System.ComponentModel.DataAnnotations;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class TargetCustomFieldViewModel
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Label is required")]
    [StringLength(200, ErrorMessage = "Label cannot exceed 200 characters")]
    public string Label { get; set; }
    
    [Required(ErrorMessage = "Type is required")]
    public TargetCustomFieldType Type { get; set; }
    
    public string TypeDisplay => Type.ToString();
    
    public bool IsRequired { get; set; }
    
    public bool IsUnique { get; set; }
    
    public bool IsSearchable { get; set; }
    
    public bool IsVisible { get; set; }
    
    public int Order { get; set; }
    
    public string Options { get; set; }
    
    public string DefaultValue { get; set; }
    
    public string Description { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime ModifiedDate { get; set; }
    
    public string UserId { get; set; }
    
    public bool IsActive { get; set; }
}