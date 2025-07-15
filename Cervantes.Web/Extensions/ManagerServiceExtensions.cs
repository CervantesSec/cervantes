using Cervantes.Application;
using Cervantes.Contracts;
using Cervantes.IFR.ChecklistMigration;

namespace Cervantes.Web.Extensions;

/// <summary>
/// Extension methods for registering managers in the service collection
/// </summary>
public static class ManagerServiceExtensions
{
    /// <summary>
    /// Registers all managers with the service collection
    /// </summary>
    /// <param name="services">The service collection to add the managers to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCervantesManagers(this IServiceCollection services)
    {
        // Core Entity Managers
        services.AddCoreEntityManagers();
        
        // Security and Access Managers
        services.AddSecurityManagers();
        
        // Project and Client Managers
        services.AddProjectManagers();
        
        // Reporting and Documentation Managers
        services.AddReportingManagers();
        
        // Knowledge Base and Communication Managers
        services.AddKnowledgeBaseManagers();
        
        // Integration and External Service Managers
        services.AddIntegrationManagers();
        
        return services;
    }

    /// <summary>
    /// Registers core entity managers (Users, Roles, Organizations)
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCoreEntityManagers(this IServiceCollection services)
    {
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IRoleManager, RoleManager>();
        services.AddScoped<IOrganizationManager, OrganizationManager>();
        
        return services;
    }

    /// <summary>
    /// Registers security and vulnerability managers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSecurityManagers(this IServiceCollection services)
    {
        services.AddScoped<IVulnManager, VulnManager>();
        services.AddScoped<IVulnCategoryManager, VulnCategoryManager>();
        services.AddScoped<IVulnNoteManager, VulnNoteManager>();
        services.AddScoped<IVulnAttachmentManager, VulnAttachmentManager>();
        services.AddScoped<IVulnTargetManager, VulnTargetManager>();
        services.AddScoped<ICweManager, CweManager>();
        services.AddScoped<IVulnCweManager, VulnCweManager>();
        services.AddScoped<IVaultManager, VaultManager>();
        services.AddScoped<IVulnCustomFieldManager, VulnCustomFieldManager>();
        services.AddScoped<IVulnCustomFieldValueManager, VulnCustomFieldValueManager>();
        
        return services;
    }

    /// <summary>
    /// Registers project and client management
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddProjectManagers(this IServiceCollection services)
    {
        services.AddScoped<IClientManager, ClientManager>();
        services.AddScoped<IProjectManager, ProjectManager>();
        services.AddScoped<IProjectUserManager, ProjectUserManager>();
        services.AddScoped<IProjectNoteManager, ProjectNoteManager>();
        services.AddScoped<IProjectAttachmentManager, ProjectAttachmentManager>();
        services.AddScoped<ITargetManager, TargetManager>();
        services.AddScoped<ITargetServicesManager, TargetServicesManager>();
        services.AddScoped<ITaskManager, TaskManager>();
        services.AddScoped<ITaskNoteManager, TaskNoteManager>();
        services.AddScoped<ITaskTargetManager, TaskTargetManager>();
        services.AddScoped<ITaskAttachmentManager, TaskAttachmentManager>();
        services.AddScoped<IProjectCustomFieldManager, ProjectCustomFieldManager>();
        services.AddScoped<IProjectCustomFieldValueManager, ProjectCustomFieldValueManager>();
        services.AddScoped<IClientCustomFieldManager, ClientCustomFieldManager>();
        services.AddScoped<IClientCustomFieldValueManager, ClientCustomFieldValueManager>();
        services.AddScoped<ITargetCustomFieldManager, TargetCustomFieldManager>();
        services.AddScoped<ITargetCustomFieldValueManager, TargetCustomFieldValueManager>();
        
        return services;
    }

    /// <summary>
    /// Registers reporting and documentation managers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddReportingManagers(this IServiceCollection services)
    {
        services.AddScoped<IReportManager, ReportManager>();
        services.AddScoped<IReportTemplateManager, ReportTemplateManager>();
        services.AddScoped<IReportComponentsManager, ReportComponentsManager>();
        services.AddScoped<IReportsPartsManager, ReportPartsManager>();
        services.AddScoped<IDocumentManager, DocumentManager>();
        services.AddScoped<INoteManager, NoteManager>();
        services.AddScoped<ILogManager, Cervantes.Application.LogManager>();
        services.AddScoped<IAuditManager, AuditManager>();
        
        return services;
    }

    /// <summary>
    /// Registers knowledge base and communication managers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddKnowledgeBaseManagers(this IServiceCollection services)
    {
        services.AddScoped<IKnowledgeBaseManager, KnowledgeBaseManager>();
        services.AddScoped<IKnowledgeBaseCategoryManager, KnowledgeBaseCategoryManager>();
        services.AddTransient<IKnowledgeBaseTagManager, KnowledgeBaseTagManager>();
        services.AddTransient<IChatManager, ChatManager>();
        services.AddScoped<IChatMessageManager, ChatMessageManager>();
        
        return services;
    }

    /// <summary>
    /// Registers integration and external service managers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddIntegrationManagers(this IServiceCollection services)
    {
        services.AddScoped<IJiraManager, JiraManager>();
        services.AddScoped<IJiraCommentManager, JiraCommentManager>();
        services.AddScoped<IWSTGManager, WSTGManager>();
        services.AddScoped<IMASTGManager, MASTGManager>();
        
        // Custom Checklist System Managers
        services.AddScoped<IChecklistTemplateManager, ChecklistTemplateManager>();
        services.AddScoped<IChecklistCategoryManager, ChecklistCategoryManager>();
        services.AddScoped<IChecklistItemManager, ChecklistItemManager>();
        services.AddScoped<IChecklistManager, ChecklistManager>();
        services.AddScoped<IChecklistExecutionManager, ChecklistExecutionManager>();
        services.AddScoped<ChecklistMigrationService>();
        services.AddScoped<IRssNewsManager, RssNewsManager>();
        services.AddScoped<IRssSourceManager, RssSourceManager>();
        services.AddScoped<IRssCategoryManager, RssCategoryManager>();
        
        return services;
    }
}