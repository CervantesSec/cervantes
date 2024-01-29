using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ReportManager : GenericManager<Report>, IReportManager
{
    public ReportManager(IApplicationDbContext context) : base(context)
    {
    }
}