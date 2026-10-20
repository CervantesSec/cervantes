using System.Collections.Generic;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

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
    
    public IEnumerable<ReportTemplate> ReportTemplates { get; set; }
    public IEnumerable<Target> Targets { get; set; }
    public IEnumerable<TargetServices> TargetServices { get; set; }
    public IEnumerable<CORE.Entities.Task> Tasks { get; set; }
    public IEnumerable<TaskAttachment> TaskAttachments { get; set; }
    public IEnumerable<TaskNote> TaskNotes { get; set; }
    public IEnumerable<TaskTargets> TaskTargets { get; set; }

    public IEnumerable<Vault> Vaults { get; set; }
    public IEnumerable<VulnCategory> VulnCategories { get; set; }
    public IEnumerable<Vuln> Vulns { get; set; }
    public IEnumerable<VulnAttachment> VulnAttachments { get; set; }
    public IEnumerable<VulnNote> VulnNotes { get; set; }
    public IEnumerable<VulnTargets> VulnTargets { get; set; }
    public IEnumerable<MASTG> Mastgs { get; set; }
    public IEnumerable<WSTG> Wstgs { get; set; }
    public IEnumerable<Jira> Jira { get; set; }
    public IEnumerable<JiraComments> JiraComments { get; set; }
    public IEnumerable<ReportComponents> ReportComponents { get; set; }
    public IEnumerable<ReportParts> ReportParts { get; set; }
    public IEnumerable<KnowledgeBaseCategories> KnowledgeBaseCategories { get; set; }
    public IEnumerable<KnowledgeBase> KnowledgeBase { get; set; }

    // Added after the first backups: older files leave these null.
    public IEnumerable<VulnCustomField>? VulnCustomFields { get; set; }
    public IEnumerable<VulnCustomFieldValue>? VulnCustomFieldValues { get; set; }
    public IEnumerable<ProjectCustomField>? ProjectCustomFields { get; set; }
    public IEnumerable<ProjectCustomFieldValue>? ProjectCustomFieldValues { get; set; }
    public IEnumerable<TargetCustomField>? TargetCustomFields { get; set; }
    public IEnumerable<TargetCustomFieldValue>? TargetCustomFieldValues { get; set; }
    public IEnumerable<ClientCustomField>? ClientCustomFields { get; set; }
    public IEnumerable<ClientCustomFieldValue>? ClientCustomFieldValues { get; set; }
    public IEnumerable<VulnCwe>? VulnCwes { get; set; }
    public IEnumerable<RssCategory>? RssCategories { get; set; }
    public IEnumerable<RssSource>? RssSources { get; set; }
    public IEnumerable<KnowledgeBaseTags>? KnowledgeBaseTags { get; set; }
    public IEnumerable<KnowledgeBaseTagLink>? KnowledgeBaseTagLinks { get; set; }
    public IEnumerable<ChecklistTemplate>? ChecklistTemplates { get; set; }
    public IEnumerable<ChecklistCategory>? ChecklistCategories { get; set; }
    public IEnumerable<ChecklistItem>? ChecklistItems { get; set; }
    public IEnumerable<Checklist>? Checklists { get; set; }
    public IEnumerable<ChecklistExecution>? ChecklistExecutions { get; set; }


}