using System;
using System.ComponentModel.DataAnnotations;
using Cervantes.CORE;

namespace Cervantes.Web.Models;

public class NoteViewModel
{
    public Guid Id { get; set; }

    /// <summary>
    /// Note Name
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// User who created project
    /// </summary>
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    public DateTime CreatedDate { get; set; }
}