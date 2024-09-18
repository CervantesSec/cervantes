using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class ReportImportViewModel
{
    public string Name { get; set; }
    public Language Language { get; set; }
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
}