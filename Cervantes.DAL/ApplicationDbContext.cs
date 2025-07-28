using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Document = System.Reflection.Metadata.Document;
using Pgvector;

namespace Cervantes.DAL;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IHttpContextAccessor _httpContextAccessor)
        : base(options)
    {
        this._httpContextAccessor = _httpContextAccessor;
    }
    /// <summary>
    /// Implemnt save async method
    /// </summary>
    /// <returns></returns>
    public Task<int> SaveChangesAsync()
    {
        BeforeSaveChanges();
        return base.SaveChangesAsync();
    }
    public override int SaveChanges()
    {
        BeforeSaveChanges();
        return base.SaveChanges();
    }
    
    public Task<int> SaveChangesNoAuditAsync()
    {
        return base.SaveChangesAsync();
    }
    
    private void BeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;

                if (_httpContextAccessor.HttpContext != null)
                {
                    auditEntry.IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    auditEntry.Browser = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
                }
                
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Add;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            if (auditEntry.UserId != null)
                            {
                                auditEntry.UserId = entry.Property("CreatedBy").CurrentValue != null ? entry.Property("CreatedBy").CurrentValue.ToString() : "Null";

                            }
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            //auditEntry.UserId = entry.Property("LastModified").CurrentValue != null ? entry.Property("LastModified").CurrentValue.ToString() : "Null";
                            auditEntry.UserId = _httpContextAccessor.HttpContext?.User
                                ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                                //auditEntry.UserId = entry.Property("LastModifiedBy").CurrentValue != null ? entry.Property("LastModifiedBy").CurrentValue.ToString() : "Null";
                                auditEntry.UserId = _httpContextAccessor.HttpContext?.User
                                    ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                            }
                            break;
                    }
                }
            }
            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }
        }
    
    public DbSet<Audit> AuditLogs { get; set; }

    //public DbSet<ApplicationUser> Users { get; set; }

    /// <summary>
    /// Table Clients
    /// </summary>
    public DbSet<CORE.Entities.Client> Clients { get; set; }
  
    /// <summary>
    /// Table Documents
    /// </summary>
    public DbSet<CORE.Entities.Document> Documents { get; set; }

    /// <summary>
    /// Table Notes
    /// </summary>
    public DbSet<Note> Notes { get; set; }

    /// <summary>
    /// Table Organization
    /// </summary>
    public DbSet<Organization> Organization { get; set; }

    /// <summary>
    /// Table Project Attachments
    /// </summary>
    public DbSet<ProjectAttachment> ProjectAttachments { get; set; }

    /// <summary>
    /// Table Projects
    /// </summary>
    public DbSet<Project> Projects { get; set; }

    /// <summary>
    /// Table Project Notes
    /// </summary>
    public DbSet<ProjectNote> ProjectNotes { get; set; }

    /// <summary>
    /// Table PRoject Users
    /// </summary>
    public DbSet<ProjectUser> ProjectUsers { get; set; }

    /// <summary>
    /// Table Targets
    /// </summary>
    public DbSet<Target> Targets { get; set; }

    /// <summary>
    /// Tarble Target Services
    /// </summary>
    public DbSet<TargetServices> TargetServices { get; set; }

    /// <summary>
    /// Table Task Attachments
    /// </summary>
    public DbSet<TaskAttachment> TaskAttachments { get; set; }

    /// <summary>
    /// Tables Tasks
    /// </summary>
    public DbSet<CORE.Entities.Task> Tasks { get; set; }

    /// <summary>
    /// Table Task Notes
    /// </summary>
    public DbSet<TaskNote> TaskNotes { get; set; }

    /// <summary>
    /// Table Task Targets
    /// </summary>
    public DbSet<TaskTargets> TaskTargets { get; set; }
    
    /// <summary>
    /// Table Vulns
    /// </summary>
    public DbSet<Vuln> Vulns { get; set; }

    /// <summary>
    /// Table Vulns Attachments
    /// </summary>
    public DbSet<VulnAttachment> VulnAttachments { get; set; }

    /// <summary>
    /// Table Vuln Categories
    /// </summary>
    public DbSet<VulnCategory> VulnCategories { get; set; }

    /// <summary>
    /// Table Vuln Notes
    /// </summary>
    public DbSet<VulnNote> VulnNotes { get; set; }
    public DbSet<VulnTargets> VulnTargets { get; set; }

    public DbSet<Log> Logs { get; set; }
    
    public DbSet<WSTG> WSTG { get; set; }
    public DbSet<MASTG> MASTG { get; set; }
    
    /// <summary>
    /// Custom Checklist System Tables
    /// </summary>
    public DbSet<ChecklistTemplate> ChecklistTemplates { get; set; }
    public DbSet<ChecklistCategory> ChecklistCategories { get; set; }
    public DbSet<ChecklistItem> ChecklistItems { get; set; }
    public DbSet<Checklist> Checklists { get; set; }
    public DbSet<ChecklistExecution> ChecklistExecutions { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.Entity<Log>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
            
        // Custom Field Configuration
        modelBuilder.Entity<VulnCustomField>()
            .HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_VulnCustomField_Name");
            
        modelBuilder.Entity<VulnCustomField>()
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        modelBuilder.Entity<VulnCustomField>()
            .Property(e => e.Label)
            .IsRequired()
            .HasMaxLength(200);
            
        // Custom Field Value Configuration
        modelBuilder.Entity<VulnCustomFieldValue>()
            .HasIndex(e => new { e.VulnId, e.VulnCustomFieldId })
            .IsUnique()
            .HasDatabaseName("IX_VulnCustomFieldValue_VulnId_CustomFieldId");
            
        modelBuilder.Entity<VulnCustomFieldValue>()
            .HasOne(v => v.Vuln)
            .WithMany(u => u.CustomFieldValues)
            .HasForeignKey(v => v.VulnId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<VulnCustomFieldValue>()
            .HasOne(v => v.VulnCustomField)
            .WithMany(cf => cf.Values)
            .HasForeignKey(v => v.VulnCustomFieldId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Project Custom Field Configuration
        modelBuilder.Entity<ProjectCustomField>()
            .HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_ProjectCustomField_Name");
            
        modelBuilder.Entity<ProjectCustomField>()
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        modelBuilder.Entity<ProjectCustomField>()
            .Property(e => e.Label)
            .IsRequired()
            .HasMaxLength(200);
            
        // Project Custom Field Value Configuration
        modelBuilder.Entity<ProjectCustomFieldValue>()
            .HasIndex(e => new { e.ProjectId, e.ProjectCustomFieldId })
            .IsUnique()
            .HasDatabaseName("IX_ProjectCustomFieldValue_ProjectId_CustomFieldId");
            
        modelBuilder.Entity<ProjectCustomFieldValue>()
            .HasOne(v => v.Project)
            .WithMany(p => p.CustomFieldValues)
            .HasForeignKey(v => v.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<ProjectCustomFieldValue>()
            .HasOne(v => v.ProjectCustomField)
            .WithMany(cf => cf.Values)
            .HasForeignKey(v => v.ProjectCustomFieldId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Client Custom Field Configuration
        modelBuilder.Entity<ClientCustomField>()
            .HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_ClientCustomField_Name");
            
        modelBuilder.Entity<ClientCustomField>()
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        modelBuilder.Entity<ClientCustomField>()
            .Property(e => e.Label)
            .IsRequired()
            .HasMaxLength(200);
            
        // Client Custom Field Value Configuration
        modelBuilder.Entity<ClientCustomFieldValue>()
            .HasIndex(e => new { e.ClientId, e.ClientCustomFieldId })
            .IsUnique()
            .HasDatabaseName("IX_ClientCustomFieldValue_ClientId_CustomFieldId");
            
        modelBuilder.Entity<ClientCustomFieldValue>()
            .HasOne(v => v.Client)
            .WithMany(c => c.CustomFieldValues)
            .HasForeignKey(v => v.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<ClientCustomFieldValue>()
            .HasOne(v => v.ClientCustomField)
            .WithMany(cf => cf.Values)
            .HasForeignKey(v => v.ClientCustomFieldId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Target Custom Field Configuration
        modelBuilder.Entity<TargetCustomField>()
            .HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("IX_TargetCustomField_Name");
            
        modelBuilder.Entity<TargetCustomField>()
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        modelBuilder.Entity<TargetCustomField>()
            .Property(e => e.Label)
            .IsRequired()
            .HasMaxLength(200);
            
        // Target Custom Field Value Configuration
        modelBuilder.Entity<TargetCustomFieldValue>()
            .HasIndex(e => new { e.TargetId, e.TargetCustomFieldId })
            .IsUnique()
            .HasDatabaseName("IX_TargetCustomFieldValue_TargetId_CustomFieldId");
            
        modelBuilder.Entity<TargetCustomFieldValue>()
            .HasOne(v => v.Target)
            .WithMany(t => t.CustomFieldValues)
            .HasForeignKey(v => v.TargetId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<TargetCustomFieldValue>()
            .HasOne(v => v.TargetCustomField)
            .WithMany(cf => cf.Values)
            .HasForeignKey(v => v.TargetCustomFieldId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Checklist System Configuration
        modelBuilder.Entity<ChecklistTemplate>()
            .HasOne(ct => ct.User)
            .WithMany()
            .HasForeignKey(ct => ct.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<ChecklistTemplate>()
            .HasOne(ct => ct.Organization)
            .WithMany()
            .HasForeignKey(ct => ct.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);
            
        modelBuilder.Entity<ChecklistCategory>()
            .HasOne(cc => cc.ChecklistTemplate)
            .WithMany(ct => ct.Categories)
            .HasForeignKey(cc => cc.ChecklistTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<ChecklistItem>()
            .HasOne(ci => ci.ChecklistCategory)
            .WithMany(cc => cc.Items)
            .HasForeignKey(ci => ci.ChecklistCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Checklist>()
            .HasOne(c => c.ChecklistTemplate)
            .WithMany(ct => ct.Checklists)
            .HasForeignKey(c => c.ChecklistTemplateId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Checklist>()
            .HasOne(c => c.Project)
            .WithMany()
            .HasForeignKey(c => c.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Checklist>()
            .HasOne(c => c.Target)
            .WithMany()
            .HasForeignKey(c => c.TargetId)
            .OnDelete(DeleteBehavior.SetNull);
            
        modelBuilder.Entity<Checklist>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<ChecklistExecution>()
            .HasOne(ce => ce.Checklist)
            .WithMany(c => c.Executions)
            .HasForeignKey(ce => ce.ChecklistId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<ChecklistExecution>()
            .HasOne(ce => ce.ChecklistItem)
            .WithMany(ci => ci.Executions)
            .HasForeignKey(ce => ce.ChecklistItemId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<ChecklistExecution>()
            .HasOne(ce => ce.TestedByUser)
            .WithMany()
            .HasForeignKey(ce => ce.TestedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
            
        modelBuilder.Entity<ChecklistExecution>()
            .HasOne(ce => ce.Vulnerability)
            .WithMany()
            .HasForeignKey(ce => ce.VulnId)
            .OnDelete(DeleteBehavior.SetNull);
            
        // CVE System Configuration
        modelBuilder.Entity<Cve>()
            .HasKey(c => c.Id);
            
        modelBuilder.Entity<Cve>()
            .HasIndex(c => c.CveId)
            .IsUnique();
            
        modelBuilder.Entity<Cve>()
            .HasMany(c => c.Configurations)
            .WithOne(cc => cc.Cve)
            .HasForeignKey(cc => cc.CveId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Cve>()
            .HasMany(c => c.References)
            .WithOne(cr => cr.Cve)
            .HasForeignKey(cr => cr.CveId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Cve>()
            .HasMany(c => c.Tags)
            .WithOne(ct => ct.Cve)
            .HasForeignKey(ct => ct.CveId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Cve>()
            .HasMany(c => c.CweMappings)
            .WithOne(ccm => ccm.Cve)
            .HasForeignKey(ccm => ccm.CveId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Cve>()
            .HasMany(c => c.ProjectMappings)
            .WithOne(cpm => cpm.Cve)
            .HasForeignKey(cpm => cpm.CveId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<CveConfiguration>()
            .HasKey(cc => cc.Id);
            
        modelBuilder.Entity<CveReference>()
            .HasKey(cr => cr.Id);
            
        modelBuilder.Entity<CveTag>()
            .HasKey(ct => ct.Id);
            
        modelBuilder.Entity<CveCweMapping>()
            .HasKey(ccm => ccm.Id);
            
        modelBuilder.Entity<CveCweMapping>()
            .HasOne(ccm => ccm.Cwe)
            .WithMany()
            .HasForeignKey(ccm => ccm.CweId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<CveProjectMapping>()
            .HasKey(cpm => cpm.Id);
            
        modelBuilder.Entity<CveProjectMapping>()
            .HasOne(cpm => cpm.Project)
            .WithMany()
            .HasForeignKey(cpm => cpm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<CveSubscription>()
            .HasKey(cs => cs.Id);
            
        modelBuilder.Entity<CveSubscription>()
            .HasOne(cs => cs.User)
            .WithMany()
            .HasForeignKey(cs => cs.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<CveSubscription>()
            .HasOne(cs => cs.Project)
            .WithMany()
            .HasForeignKey(cs => cs.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);
            
        modelBuilder.Entity<CveSubscription>()
            .HasMany(cs => cs.Notifications)
            .WithOne(cn => cn.Subscription)
            .HasForeignKey(cn => cn.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<CveNotification>()
            .HasKey(cn => cn.Id);
            
        modelBuilder.Entity<CveNotification>()
            .HasOne(cn => cn.Cve)
            .WithMany()
            .HasForeignKey(cn => cn.CveId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<CveNotification>()
            .HasOne(cn => cn.User)
            .WithMany()
            .HasForeignKey(cn => cn.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<CveSyncSource>()
            .HasKey(css => css.Id);
            
        modelBuilder.Entity<CveSyncSource>()
            .HasIndex(css => css.Name)
            .IsUnique();
    }
    
    public DbSet<Report> Reports { get; set; }
    public DbSet<Vault> Vaults { get; set; }
    public DbSet<ReportTemplate> ReportTemplates { get; set; }
    public DbSet<Jira> Jira { get; set; }
    public DbSet<JiraComments> JiraComments { get; set; }
    public DbSet<Cwe> Cwe { get; set; }
    public DbSet<VulnCwe> VulnCwe { get; set; }
    
    public DbSet<KnowledgeBase> KnowledgeBase { get; set; }
    public DbSet<KnowledgeBaseCategories> KnowledgeBaseCategories { get; set; }
    public DbSet<KnowledgeBaseTags> KnowledgeBaseTags { get; set; }
    
    public DbSet<ReportComponents> ReportComponents { get; set; }
    public DbSet<ReportParts> ReportParts { get; set; }
    
    public DbSet<RssSource> RssSource { get; set; }
    public DbSet<RssNews> RssNews { get; set; }
    
    /// <summary>
    /// Table for custom field definitions
    /// </summary>
    public DbSet<VulnCustomField> VulnCustomFields { get; set; }
    
    /// <summary>
    /// Table for custom field values
    /// </summary>
    public DbSet<VulnCustomFieldValue> VulnCustomFieldValues { get; set; }
    
    /// <summary>
    /// Table for project custom field definitions
    /// </summary>
    public DbSet<ProjectCustomField> ProjectCustomFields { get; set; }
    
    /// <summary>
    /// Table for project custom field values
    /// </summary>
    public DbSet<ProjectCustomFieldValue> ProjectCustomFieldValues { get; set; }
    
    /// <summary>
    /// Table for client custom field definitions
    /// </summary>
    public DbSet<ClientCustomField> ClientCustomFields { get; set; }
    
    /// <summary>
    /// Table for client custom field values
    /// </summary>
    public DbSet<ClientCustomFieldValue> ClientCustomFieldValues { get; set; }
    
    /// <summary>
    /// Table for target custom field definitions
    /// </summary>
    public DbSet<TargetCustomField> TargetCustomFields { get; set; }
    
    /// <summary>
    /// Table for target custom field values
    /// </summary>
    public DbSet<TargetCustomFieldValue> TargetCustomFieldValues { get; set; }
    
    /// <summary>
    /// Table for CVE entities
    /// </summary>
    public DbSet<Cve> Cves { get; set; }
    
    /// <summary>
    /// Table for CVE configurations (affected products)
    /// </summary>
    public DbSet<CveConfiguration> CveConfigurations { get; set; }
    
    /// <summary>
    /// Table for CVE references
    /// </summary>
    public DbSet<CveReference> CveReferences { get; set; }
    
    /// <summary>
    /// Table for CVE tags
    /// </summary>
    public DbSet<CveTag> CveTags { get; set; }
    
    /// <summary>
    /// Table for CVE to CWE mappings
    /// </summary>
    public DbSet<CveCweMapping> CveCweMappings { get; set; }
    
    /// <summary>
    /// Table for CVE to project mappings
    /// </summary>
    public DbSet<CveProjectMapping> CveProjectMappings { get; set; }
    
    /// <summary>
    /// Table for CVE subscriptions
    /// </summary>
    public DbSet<CveSubscription> CveSubscriptions { get; set; }
    
    /// <summary>
    /// Table for CVE notifications
    /// </summary>
    public DbSet<CveNotification> CveNotifications { get; set; }
    
    /// <summary>
    /// Table for CVE synchronization sources
    /// </summary>
    public DbSet<CveSyncSource> CveSyncSources { get; set; }
}