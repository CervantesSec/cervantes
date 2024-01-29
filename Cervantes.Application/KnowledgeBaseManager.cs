using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class KnowledgeBaseManager : GenericManager<KnowledgeBase>, IKnowledgeBaseManager
{
    public KnowledgeBaseManager(IApplicationDbContext context) : base(context)
    {
    }
}