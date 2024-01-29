namespace Cervantes.CORE.ViewModel;

public class KnowledgePageEditViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int Order { get; set; }
    public Guid CategoryId { get; set; }
}