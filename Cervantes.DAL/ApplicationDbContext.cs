using System;
using Cervantes.Contracts;
using Cervantes.CORE;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Task = Cervantes.CORE.Task;
using Npgsql;

namespace Cervantes.DAL;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
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
    public DbSet<Client> Clients { get; set; }
  
    /// <summary>
    /// Table Documents
    /// </summary>
    public DbSet<Document> Documents { get; set; }

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
    public DbSet<CORE.Task> Tasks { get; set; }

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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder){
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Log>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();
    }
    
    public DbSet<Report> Reports { get; set; }
    public DbSet<Vault> Vaults { get; set; }
    public DbSet<ReportTemplate> ReportTemplates { get; set; }
}