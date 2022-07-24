using System.Collections.Generic;
using Cervantes.CORE;

namespace Cervantes.Web.Models;

public class BackupViewModel
{
    public IEnumerable<ApplicationUser> Users { get; set; }
    public IEnumerable<Client> Clients { get; set; }
    public IEnumerable<Project> Projects { get; set; }

    public IEnumerable<ProjectUser> ProjectUsers { get; set; }
    public IEnumerable<ProjectAttachment> ProjectAttachments { get; set; }
    public IEnumerable<ProjectNote> ProjectNotes { get; set; }
    public IEnumerable<Document> Documents { get; set; }
    public IEnumerable<Note> Notes { get; set; }
    public Organization Organization { get; set; }
    public IEnumerable<Report> Reports { get; set; }
    public IEnumerable<Target> Targets { get; set; }
    public IEnumerable<TargetServices> TargetServices { get; set; }
    public IEnumerable<Task> Tasks { get; set; }
    public IEnumerable<TaskAttachment> TaskAttachments { get; set; }
    public IEnumerable<TaskNote> TaskNotes { get; set; }
    public IEnumerable<TaskTargets> TaskTargets { get; set; }

    public IEnumerable<Vault> Vaults { get; set; }
    public IEnumerable<VulnCategory> VulnCategories { get; set; }
    public IEnumerable<Vuln> Vulns { get; set; }
    public IEnumerable<VulnAttachment> VulnAttachments { get; set; }
    public IEnumerable<VulnNote> VulnNotes { get; set; }
    public IEnumerable<VulnTargets> VulnTargets { get; set; }

}