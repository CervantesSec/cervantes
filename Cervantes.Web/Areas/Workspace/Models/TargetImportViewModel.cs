using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Cervantes.Web.Areas.Workspace.Models;

public class TargetImportViewModel
{
    public Guid Project { get; set; }
    /// <summary>
    /// File Uploaded
    /// </summary>
    [Required]
    public IFormFile upload { get; set; }
}