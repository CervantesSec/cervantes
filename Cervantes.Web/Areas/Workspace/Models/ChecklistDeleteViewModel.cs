using System;
using Cervantes.CORE;

namespace Cervantes.Web.Areas.Workspace.Models;

public class ChecklistDeleteViewModel
{
    public ChecklistType Type { get; set; }
    
    public Project Project { get; set; }
    
    public WSTG Wstg { get; set; }
    public CORE.MASTG Mastg { get; set; }
    /// <summary>
    /// Target Associated
    /// </summary>
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id Target
    /// </summary>
    public Guid TargetId { get; set; }
}