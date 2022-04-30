using Cervantes.CORE;
using System.Collections.Generic;

namespace Cervantes.Web.Areas.Workspace.Models
{
    public class TargetDetailsViewModel
    {
        public Project Project { get; set; }
        public Target Target { get; set; }
        public IEnumerable<TargetServices> TargetServices { get; set; }

    }
}
