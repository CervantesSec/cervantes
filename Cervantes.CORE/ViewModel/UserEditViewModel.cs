using System;
using System.Text.Json.Serialization;

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
    public string? PhoneNumber { get; set; }
    

    /// <summary>
    /// user Bio description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// User Position
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// New password; the current one is kept when empty.
    /// </summary>
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    /// <summary>New role; the current one is kept when empty.</summary>
    public string? Role { get; set; }
    /// <summary>Optional new avatar.</summary>
    public string? FileName { get; set; }
    public byte[]? FileContent { get; set; }
    
    public Guid? ClientId { get; set; }
    public string? ImagePath { get; set; }
    // These flags have no "keep" value: an API request must send them, otherwise an omitted flag would silently
    // become false (unlocking the user, disabling 2FA or turning the account local).
    [JsonRequired]
    public bool ExternalLogin { get; set; }

    [JsonRequired]
    public bool Lockout { get; set; }
    [JsonRequired]
    public bool TwoFactorEnabled { get; set; }
}