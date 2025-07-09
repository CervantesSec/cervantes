using System.ComponentModel;

namespace Cervantes.CORE.Entities;

/// <summary>
/// Estado de un item individual de checklist
/// </summary>
public enum ChecklistItemStatus
{
    [Description("Not Tested")]
    NotTested = 0,
    
    [Description("Passed")]
    Passed = 1,
    
    [Description("Failed")]
    Failed = 2,
    
    [Description("Not Applicable")]
    NotApplicable = 3,
    
    [Description("In Progress")]
    InProgress = 4,
    
    [Description("Needs Review")]
    NeedsReview = 5,
    
    [Description("Skipped")]
    Skipped = 6
}