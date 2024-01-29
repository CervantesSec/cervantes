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
    /// User  name
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Phone Number
    /// </summary>
    public string PhoneNumber { get; set; }
    
    public bool TwoFactorEnabled { get; set; }

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

    /// <summary>
    /// User Position
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// User Position
    /// </summary>
    public string ConfirmPassword { get; set; }
    
    /// <summary>
    /// File Uploaded
    /// </summary>
    public IBrowserFile File { get; set; }

    /// <summary>
    /// Client Image
    /// </summary>
    public string ImagePath { get; set; }
    
    public bool LockoutEnabled { get; set; }
    
    /// <summary>
    /// lista de rol
    /// </summary>
    public List<IdentityRole> Roles { get; set; }
    
    public string Role { get; set; }
    public List<Client> Clients { get; set; }
    public Guid ClientId { get; set; }
    /// <summary>
    /// opcion seleccionada
    /// </summary>
    public string Option { get; set; }
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
}