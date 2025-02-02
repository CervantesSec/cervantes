using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;

namespace Cervantes.CORE.ViewModels;

public class UserViewModel
{

    public string Id { get; set; }
    /// <summary>
    /// User full name
    /// </summary>
    public string FullName { get; set; }
    
    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }
    
    public bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// User Position
    /// </summary>
    public string Position { get; set; }
    
    public DateTimeOffset? LockoutEnd { get; set; }

    public string Role { get; set; }
    
    public bool ExternalLogin { get; set; }
    public string Avatar { get; set; }
   
}