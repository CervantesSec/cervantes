using System;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace Cervantes.CORE.ViewModel;

public class TargetImportViewModel
{
    public TargetImportType Type { get; set; }
    public Guid Project { get; set; }
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
}