using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Document = System.Reflection.Metadata.Document;
using Pgvector;

namespace Cervantes.DAL;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    /// <summary>
    /// Implemnt save async method
    /// </summary>
    /// <returns></returns>
    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
    
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
}