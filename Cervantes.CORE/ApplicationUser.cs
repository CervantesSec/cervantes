using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Cervantes.CORE;

public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// User full name
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// User avatar
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// user Bio description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// User Position
    /// </summary>
    public string Position { get; set; }
    
    public Guid ClientId { get; set; }
}