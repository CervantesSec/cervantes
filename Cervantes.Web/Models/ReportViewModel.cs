using Cervantes.CORE;
using System.Collections.Generic;

namespace Cervantes.Web.Models
{
    public class ReportViewModel
    {
        public Organization Organization { get; set; }
        public IEnumerable<Vuln> Vulns { get; set; }
        public IEnumerable<Target> Targets { get; set; }

        public IEnumerable<TargetServices> TargetServices { get; set; }
        public Project Project { get; set; }
        public IEnumerable<ProjectUser> Users { get; set; }

        public IEnumerable<Report> Reports { get; set; }
    }
}
