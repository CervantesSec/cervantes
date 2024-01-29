using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class CweManager: GenericManager<Cwe>, ICweManager
{
    /// <summary>
    /// Cwe Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public CweManager(IApplicationDbContext context) : base(context)
    {
    }
}