using Cervantes.CORE;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cervantes.Web.Models;

public class UserViewModel : IdentityUser
{
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
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// User Position
    /// </summary>

    public string ConfirmPassword { get; set; }

    /// <summary>
    /// File Uploaded
    /// </summary>
    [Required]
    public IFormFile upload { get; set; }

    /// <summary>
    /// Client Image
    /// </summary>
    public string ImagePath { get; set; }

    public RoleList Role { get; set; }
    public IList<SelectListItem> ItemList { get; set; }
    public string Option { get; set; }

    public IEnumerable<Project> Project { get; set; }
    
    /// <summary>
    /// Client asssociated to project
    /// </summary>
    public virtual Client Client { get; set; }

    /// <summary>
    /// Client ID
    /// </summary>
    public int ClientId { get; set; }
    public IList<SelectListItem> ItemListClient { get; set; }
}