using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class CreateReportComponentModel
{
    public string Name { get; set; }
    public string Content { get; set; }
    public Language Language { get; set; }
    public ReportPartType ComponentType { get; set; }
}