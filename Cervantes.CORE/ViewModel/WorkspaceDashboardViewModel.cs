using System.Collections.Generic;
using Cervantes.CORE.Entities;
using Task = Cervantes.CORE.Entities.Task;

namespace Cervantes.CORE.ViewModel;

public class WorkspaceDashboardViewModel
{
    public Project Project { get; set; }
    public IEnumerable<ProjectUser> Members { get; set; }
    public IEnumerable<Vuln> Vulns { get; set; }
    public IEnumerable<Task> Tasks { get; set; }
    public IEnumerable<Target> Targets { get; set; }
    public IEnumerable<ProjectNote> Notes { get; set; }
    public int VulnNumber { get; set; }
    public int TasksNumber { get; set; }
    public int TargetsNumber { get; set; }
    public int MembersNumber { get; set; }
    public int NotesNumber { get; set; }
    public int AttachmentsNumber { get; set; }
    public int VulnInfo { get; set; }
    public int VulnLow { get; set; }
    public int VulnMedium { get; set; }
    public int VulnHigh { get; set; }
    public int VulnCritical { get; set; }
}