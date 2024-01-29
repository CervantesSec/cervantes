using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class ReportDownloadModel
{
    public Guid Id { get; set; }
    public ReportFileType FileType { get; set; }
}