using System.ComponentModel.DataAnnotations;
using Cervantes.CORE;
using Microsoft.AspNetCore.Http;

namespace Cervantes.Web.Models;

public class DocumentViewModel
{
    [Required] public string Name { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// File path location
    /// </summary>
    [Required]
    public string FilePath { get; set; }

    /// <summary>
    /// User who created project
    /// </summary>
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Visibility of the document
    /// </summary>
    public Visibility Visibility { get; set; }

    /// <summary>
    /// File Uploaded
    /// </summary>
    [Required]
    public IFormFile Upload { get; set; }
}