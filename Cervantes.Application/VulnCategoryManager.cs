﻿using Cervantes.CORE;
using Cervantes.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.Application
{
    public class VulnCategoryManager: GenericManager<VulnCategory>, IVulnCategoryManager
    {
        /// <summary>
        /// VulnCategory Manager Constructor
        /// </summary>
        /// <param name="context"></param>
        public VulnCategoryManager(IApplicationDbContext context) : base(context)
        {
        }
    }
}
