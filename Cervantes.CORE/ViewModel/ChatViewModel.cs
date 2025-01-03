namespace Cervantes.CORE.ViewModel;

public class ChatViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    
    public DateTime LastMessageAt { get; set; }
}