using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Cervantes.CORE.Entities;

public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// User full name
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// User avatar
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// user Bio description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// User Position
    /// </summary>
    public string? Position { get; set; }
    
    public Guid? ClientId { get; set; }
    
    public bool ExternalLogin { get; set; }
    
    public virtual ICollection<ChatMessage> ChatMessagesFromUsers { get; set; }
    public virtual ICollection<ChatMessage> ChatMessagesToUsers { get; set; }
    public virtual ICollection<ApiKey> ApiKeys { get; set; }
    public ApplicationUser()
    {
        ChatMessagesFromUsers = new HashSet<ChatMessage>();
        ChatMessagesToUsers = new HashSet<ChatMessage>();
        ApiKeys = new HashSet<ApiKey>();
    }
    
}
