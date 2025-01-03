using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ChatManager:GenericManager<Chat>, IChatManager
{
    public ChatManager(IApplicationDbContext context) : base(context)
    {
    }
}