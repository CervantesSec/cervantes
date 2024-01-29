using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ReportTemplateManager : GenericManager<ReportTemplate>, IReportTemplateManager
{
    public ReportTemplateManager(IApplicationDbContext context) : base(context)
    {
    }
}