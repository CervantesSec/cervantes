using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ProjectCustomFieldManager : GenericManager<ProjectCustomField>, IProjectCustomFieldManager
{
    public ProjectCustomFieldManager(IApplicationDbContext context) : base(context)
    {
    }
}