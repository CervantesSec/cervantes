using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class KnowledgeBaseCategoryManager : GenericManager<KnowledgeBaseCategories>, IKnowledgeBaseCategoryManager
{
    public KnowledgeBaseCategoryManager(IApplicationDbContext context) : base(context)
    {
    }
}