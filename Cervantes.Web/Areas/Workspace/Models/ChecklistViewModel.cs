using System.Collections.Generic;
using Cervantes.CORE;

namespace Cervantes.Web.Areas.Workspace.Models;

public class ChecklistViewModel
{
    public Project Project { get; set; }
    public IEnumerable<WSTG> WSTG { get; set; }
    public IEnumerable<CORE.MASTG> MASTG { get; set; }
}