using Cervantes.CORE;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Cervantes.Web.Areas.Workspace.Models;

public class TaskDetailsViewModel
{
    public Project Project { get; set; }
    public Task Task { get; set; }
    public IEnumerable<TaskNote> Notes { get; set; }
    public IEnumerable<TaskAttachment> Attachments { get; set; }

    /// <summary>
    /// File Uploaded
    /// </summary>
    public IFormFile upload { get; set; }
}