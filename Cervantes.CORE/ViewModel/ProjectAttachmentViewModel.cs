using System;
using Microsoft.AspNetCore.Components.Forms;

namespace Cervantes.CORE.ViewModels;

public class ProjectAttachmentViewModel
{
    public string Name { get; set; }
    public Guid ProjectId { get; set; }
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
}