using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class ReportPartsManager: GenericManager<ReportParts>, IReportsPartsManager
{
    public ReportPartsManager(IApplicationDbContext context) : base(context)
    {
    }
}