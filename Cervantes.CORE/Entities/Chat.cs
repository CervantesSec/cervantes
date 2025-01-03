using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE.Entities;

public class Chat
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastMessageAt { get; set; }
}