using Cervantes.CORE;
using System.Collections.Generic;

namespace Cervantes.Web.Areas.Workspace.Models;

public class VulnViewModel
{
    public Project Project { get; set; }
    public IEnumerable<Vuln> Vulns { get; set; }
}