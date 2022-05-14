using Cervantes.CORE;
using System.Collections.Generic;

namespace Cervantes.Web.Areas.Workspace.Models;

public class TaskViewModel
{
    public Project Project { get; set; }
    public IEnumerable<Task> Tasks { get; set; }
}