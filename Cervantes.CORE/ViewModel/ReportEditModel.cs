using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class ReportEditModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Language Language { get; set; }
    public string Version { get; set; }
    public string HtmlCode { get; set; }
}