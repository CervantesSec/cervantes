namespace Cervantes.CORE.ViewModel;

public class ChatMessageViewModel
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public string Content { get; set; }
    public bool IsUser { get; set; }
    public DateTime Timestamp { get; set; }
    
    public int MessageIndex { get; set; }
}