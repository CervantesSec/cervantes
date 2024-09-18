namespace Cervantes.CORE.Entities;

public class ReportComponents
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string Content { get; set; }
    public string ContentCss { get; set; }
    public Language Language { get; set; }
    public ReportPartType ComponentType { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}