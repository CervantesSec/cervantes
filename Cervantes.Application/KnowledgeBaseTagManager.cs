using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class KnowledgeBaseTagManager: GenericManager<KnowledgeBaseTags>, IKnowledgeBaseTagManager
{
    /// <summary>
    /// Client Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public KnowledgeBaseTagManager(IApplicationDbContext context) : base(context)
    {
    }
}
