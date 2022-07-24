using System;
using Cervantes.CORE;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cervantes.Web.Areas.Workspace.Models;

public class VulnDetailsViewModel
{
    public Project Project { get; set; }
    public Vuln Vuln { get; set; }
    public IEnumerable<VulnNote> Notes { get; set; }
    public IEnumerable<VulnAttachment> Attachments { get; set; }
    public IEnumerable<VulnTargets> Targets { get; set; }

    /// <summary>
    /// File Uploaded
    /// </summary>
    public IFormFile upload { get; set; }

    public Visibility Visibility { get; set; }
    
    public IList<SelectListItem> TargetList { get; set; }
    public Guid TargetId { get; set; }

}