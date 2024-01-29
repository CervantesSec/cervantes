using System;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace Cervantes.CORE.ViewModel;

public class VulnImportViewModel
{
    public VulnImportType Type { get; set; }
    public Guid? Project { get; set; }
   //public IBrowserFile File { get; set; }
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
}