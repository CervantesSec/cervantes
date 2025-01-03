namespace Cervantes.CORE.ViewModel;

public class ChatMessageCreateViewModel
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public string Content { get; set; }
    public bool IsUser { get; set; }
    public DateTime Timestamp { get; set; }
}