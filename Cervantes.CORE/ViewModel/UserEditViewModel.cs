using System;

namespace Cervantes.CORE.ViewModel;

public class UserEditViewModel
{
    public string Id { get; set; }
    public string FullName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Phone Number
    /// </summary>
    public string PhoneNumber { get; set; }
    

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
    public string ConfirmPassword { get; set; }
    public string Role { get; set; }
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
    
    public Guid? ClientId { get; set; }
    public string ImagePath { get; set; }
    public bool ExternalLogin { get; set; }

    public bool Lockout { get; set; }
    public bool TwoFactorEnabled { get; set; }
}