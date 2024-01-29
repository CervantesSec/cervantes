using System;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace Cervantes.CORE.ViewModel;

public class ReportTemplateViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public Language Language { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public virtual ApplicationUser User { get; set; }
    
    public string UserId { get; set; }
    
    public string FilePath { get; set; }

    public IBrowserFile File { get; set; }
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
}