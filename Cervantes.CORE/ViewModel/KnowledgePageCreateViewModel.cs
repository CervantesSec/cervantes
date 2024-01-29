namespace Cervantes.CORE.ViewModel;

public class KnowledgePageCreateViewModel
{
    public string Title { get; set; }
    public string Content { get; set; }
    public int Order { get; set; }
    public Guid CategoryId { get; set; }
}