using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application;

public class VulnAttachmentManager : GenericManager<VulnAttachment>, IVulnAttachmentManager
{
    /// <summary>
    /// VulnAttachment Manager Constructor
    /// </summary>
    /// <param name="context"></param>
    public VulnAttachmentManager(IApplicationDbContext context) : base(context)
    {
    }
}