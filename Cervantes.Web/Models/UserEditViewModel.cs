using System;
using Cervantes.CORE;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Build.Framework;

namespace Cervantes.Web.Models;

public class UserEditViewModel
{
    public string Id { get; set; }
    /// <summary>
    /// User full name
    /// </summary>
    [Required]
    public string FullName { get; set; }

    /// <summary>
    /// User  name
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    [Required]
    public string Email { get; set; }

    /// <summary>
    /// Phone Number
    /// </summary>
    [Required]
    public string PhoneNumber { get; set; }
    
    public bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// User avatar
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// user Bio description
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// User Position
    /// </summary>
    [Required]
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
    public IFormFile upload { get; set; }

    /// <summary>
    /// Client Image
    /// </summary>
    public string ImagePath { get; set; }
    
    public bool LockoutEnabled { get; set; }
    
    /// <summary>
    /// lista de rol
    /// </summary>
    public RoleList Role { get; set; }

    /// <summary>
    /// opcion seleccionada
    /// </summary>
    public string Option { get; set; }

    public IList<SelectListItem> ItemList { get; set; }
}