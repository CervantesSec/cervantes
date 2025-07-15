using Cervantes.Server.Helpers;
using Cervantes.Web.Controllers;
using Cervantes.Web.Helpers;

namespace Cervantes.Web.Extensions;

/// <summary>
/// Extension methods for registering controllers in the service collection
/// </summary>
public static class ControllerServiceExtensions
{
    /// <summary>
    /// Registers all Cervantes controllers with the service collection
    /// </summary>
    /// <param name="services">The service collection to add the controllers to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCervantesControllers(this IServiceCollection services)
    {
        // Core Business Controllers
        services.AddCoreBusinessControllers();
        
        // Project Management Controllers
        services.AddProjectManagementControllers();
        
        // Security and Vulnerability Controllers
        services.AddSecurityControllers();
        
        // Reporting and Documentation Controllers
        services.AddReportingControllers();
        
        // Integration and Utility Controllers
        services.AddIntegrationControllers();
        
        // Helper Services
        services.AddHelperServices();
        
        return services;
    }

    /// <summary>
    /// Registers core business controllers (Clients, Users, Organizations)
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCoreBusinessControllers(this IServiceCollection services)
    {
        services.AddScoped<ClientsController>();
        services.AddScoped<ClientCustomFieldController>();
        services.AddScoped<UserController>();
        services.AddScoped<OrganizationController>();
        
        return services;
    }

    /// <summary>
    /// Registers project management controllers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddProjectManagementControllers(this IServiceCollection services)
    {
        services.AddScoped<ProjectController>();
        services.AddScoped<ProjectCustomFieldController>();
        services.AddScoped<TargetController>();
        services.AddScoped<TargetCustomFieldController>();
        services.AddScoped<TaskController>();
        services.AddScoped<WorkspacesController>();
        services.AddScoped<ChecklistController>();
        
        return services;
    }

    /// <summary>
    /// Registers security and vulnerability management controllers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSecurityControllers(this IServiceCollection services)
    {
        services.AddScoped<VulnController>();
        services.AddScoped<VulnCustomFieldController>();
        services.AddScoped<VaultController>();
        
        return services;
    }

    /// <summary>
    /// Registers reporting and documentation controllers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddReportingControllers(this IServiceCollection services)
    {
        services.AddScoped<ReportController>();
        services.AddScoped<DocumentController>();
        services.AddScoped<NoteController>();
        services.AddScoped<LogController>();
        services.AddScoped<AuditController>();
        services.AddScoped<BackupController>();
        
        return services;
    }

    /// <summary>
    /// Registers integration and utility controllers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddIntegrationControllers(this IServiceCollection services)
    {
        services.AddScoped<JiraController>();
        services.AddScoped<ChatController>();
        services.AddScoped<SearchController>();
        services.AddScoped<CalendarController>();
        services.AddScoped<KnowledgeBaseController>();
        
        return services;
    }

    /// <summary>
    /// Registers helper services and utilities
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddHelperServices(this IServiceCollection services)
    {
        services.AddScoped<Sanitizer>();
        
        return services;
    }
}