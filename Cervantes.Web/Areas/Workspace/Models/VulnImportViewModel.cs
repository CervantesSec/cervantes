using System;
using System.ComponentModel.DataAnnotations;
using Cervantes.CORE;
using Microsoft.AspNetCore.Http;

namespace Cervantes.Web.Areas.Workspace.Models;

public class VulnImportViewModel
{
    public Guid Project { get; set; }
    /// <summary>
    /// File Uploaded
    /// </summary>
    [Required]
    public IFormFile upload { get; set; }
    
    public VulnImportType Type { get; set; }
}