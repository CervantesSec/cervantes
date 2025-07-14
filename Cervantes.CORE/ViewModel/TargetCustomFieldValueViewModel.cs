using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class TargetCustomFieldValueViewModel
{
    public Guid Id { get; set; }
    
    public Guid TargetId { get; set; }
    
    public Guid TargetCustomFieldId { get; set; }
    
    public Guid CustomFieldId 
    { 
        get => TargetCustomFieldId; 
        set => TargetCustomFieldId = value; 
    }
    
    public string Name { get; set; }
    
    public string Label { get; set; }
    
    public TargetCustomFieldType Type { get; set; }
    
    public bool IsRequired { get; set; }
    
    public bool IsUnique { get; set; }
    
    public bool IsSearchable { get; set; }
    
    public bool IsVisible { get; set; }
    
    public int Order { get; set; }
    
    public string Options { get; set; }
    
    public string DefaultValue { get; set; }
    
    public string Description { get; set; }
    
    public string Value { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime ModifiedDate { get; set; }
    
    public string UserId { get; set; }
    
    /// <summary>
    /// Parsed options array for Select type fields
    /// </summary>
    public string[] OptionsArray
    {
        get
        {
            if (string.IsNullOrEmpty(Options))
                return new string[0];
                
            try
            {
                return JsonSerializer.Deserialize<string[]>(Options) ?? new string[0];
            }
            catch
            {
                return new string[0];
            }
        }
    }
    
    /// <summary>
    /// For Boolean fields, helper property
    /// </summary>
    public bool BoolValue
    {
        get => Value?.ToLower() == "true";
        set => Value = value.ToString().ToLower();
    }
}