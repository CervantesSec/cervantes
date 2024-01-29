using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class ReportChecklistCreateModel
{
    public string Name { get; set; }
    public Guid ChecklistId { get; set; }
    public string Description { get; set; }
    public Language Language { get; set; }
    public string Version { get; set; }
    public Guid ReportTemplateId { get; set; }
    public Guid ProjectId { get; set; }
}