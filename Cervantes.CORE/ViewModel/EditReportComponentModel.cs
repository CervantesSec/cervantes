using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class EditReportComponentModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public string CssContent { get; set; }
    public Language Language { get; set; }
    public ReportPartType ComponentType { get; set; }
}