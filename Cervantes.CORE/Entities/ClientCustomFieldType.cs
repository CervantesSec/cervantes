using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.CORE.Entities;

/// <summary>
/// Defines the different types of custom fields that can be created for clients
/// </summary>
public enum ClientCustomFieldType
{
    /// <summary>
    /// Single line text input (e.g., title, reference ID)
    /// </summary>
    Input = 0,
    
    /// <summary>
    /// Multi-line text area (e.g., description, notes)
    /// </summary>
    Textarea = 1,
    
    /// <summary>
    /// Dropdown selection from predefined options (e.g., severity, status)
    /// </summary>
    Select = 2,
    
    /// <summary>
    /// Numeric value input (e.g., CVSS score, impact level)
    /// </summary>
    Number = 3,
    
    /// <summary>
    /// Date picker (e.g., discovery date, due date)
    /// </summary>
    Date = 4,
    
    /// <summary>
    /// Boolean checkbox (e.g., exploitable, public)
    /// </summary>
    Boolean = 5
}