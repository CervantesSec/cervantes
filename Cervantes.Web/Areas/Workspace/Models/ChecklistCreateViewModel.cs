using System;
using System.Collections.Generic;
using Cervantes.CORE;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cervantes.Web.Areas.Workspace.Models;

public class ChecklistCreateViewModel
{
    /// <summary>
    /// Target Associated
    /// </summary>
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id Target
    /// </summary>
    public Guid TargetId { get; set; }
    
    public List<Guid> SelectedTargets { get; set; }

    public string TargetName { get; set; }
    public IList<SelectListItem> TargetList { get; set; }
    
    public ChecklistType Type { get; set; }
    
    public Project Project { get; set; }
}