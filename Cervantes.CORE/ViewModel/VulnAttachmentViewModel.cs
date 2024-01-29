using System;
using Microsoft.AspNetCore.Components.Forms;

namespace Cervantes.CORE.ViewModel;

public class VulnAttachmentViewModel
{
    public string Name { get; set; }
    public Guid VulnId { get; set; }
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
}