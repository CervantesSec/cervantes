namespace Cervantes.CORE.ViewModel;

public class CreateChatViewModel
{
    public string Title { get; set; }
    public string Type { get; set; }
    public Guid ProjectId { get; set; }
    public Guid DocumentId { get; set; }
}