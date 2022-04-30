using Cervantes.CORE;
using System.Collections.Generic;

namespace Cervantes.Web.Areas.Workspace.Models
{
    public class TargetViewModel
    {
        public IEnumerable<Target> Target { get; set; }
        public Project Project { get; set; }
    }
}
