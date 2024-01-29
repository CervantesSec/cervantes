using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cervantes.CORE.Entities;

namespace Cervantes.Application;

public class DocumentManager : GenericManager<Document>, IDocumentManager
{
    /// <summary>
    /// Document Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public DocumentManager(IApplicationDbContext context) : base(context)
    {
    }
}