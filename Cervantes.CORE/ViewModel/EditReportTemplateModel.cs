using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class EditReportTemplateModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public Language Language { get; set; }
    public ReportType ReportType { get; set; }

    public List<ReportPartsModel> Components { get; set; }
    
}