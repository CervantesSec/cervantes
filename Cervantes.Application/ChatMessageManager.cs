using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ChatMessageManager: GenericManager<ChatMessage>, IChatMessageManager
{
    public ChatMessageManager(IApplicationDbContext context) : base(context)
    {
    }
}