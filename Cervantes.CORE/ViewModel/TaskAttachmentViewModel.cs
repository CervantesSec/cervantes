using System;
using Microsoft.AspNetCore.Components.Forms;

namespace Cervantes.CORE.ViewModel;

public class TaskAttachmentViewModel
{
    /// <summary>
    /// Task Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid TaskId { get; set; }
    
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
}