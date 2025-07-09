using System.ComponentModel;

namespace Cervantes.CORE.Entities;

/// <summary>
/// Estado general de un checklist
/// </summary>
public enum ChecklistStatus
{
    [Description("Not Started")]
    NotStarted = 0,
    
    [Description("In Progress")]
    InProgress = 1,
    
    [Description("Completed")]
    Completed = 2,
    
    [Description("On Hold")]
    OnHold = 3,
    
    [Description("Cancelled")]
    Cancelled = 4
}