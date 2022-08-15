using System;
using System.ComponentModel.DataAnnotations;
using Cervantes.CORE;
using Microsoft.AspNetCore.Http;


namespace Cervantes.Web.Models;

public class ReportTemplateViewModel
{
    // <summary>
    /// ReportTemplate Id
    /// </summary>
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public Language Language { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public virtual ApplicationUser User { get; set; }
    
    public string UserId { get; set; }
    
    public string FilePath { get; set; }

    [Required]
    public IFormFile upload { get; set; }
    
    
}