using Microsoft.AspNetCore.Components.Forms;

namespace Cervantes.CORE.ViewModel;

public class BackupFormViewModel
{
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
}