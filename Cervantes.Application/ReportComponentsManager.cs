using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ReportComponentsManager: GenericManager<ReportComponents>, IReportComponentsManager
{
    public ReportComponentsManager(IApplicationDbContext context) : base(context)
    {
    }
}