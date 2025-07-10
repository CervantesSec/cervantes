using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ProjectCustomFieldValueManager : GenericManager<ProjectCustomFieldValue>, IProjectCustomFieldValueManager
{
    public ProjectCustomFieldValueManager(IApplicationDbContext context) : base(context)
    {
    }
}