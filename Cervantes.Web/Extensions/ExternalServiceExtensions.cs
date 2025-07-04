using Cervantes.IFR.CervantesAI;
using Cervantes.IFR.Email;
using Cervantes.IFR.Export;
using Cervantes.IFR.File;
using Cervantes.IFR.Jira;

namespace Cervantes.Web.Extensions;

/// <summary>
/// Extension methods for registering external services and integrations
/// </summary>
public static class ExternalServiceExtensions
{
    /// <summary>
    /// Registers all external services and integrations
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Email Services
        services.AddEmailServices(configuration);
        
        // Jira Integration
        services.AddJiraServices(configuration);
        
        // AI Services
        services.AddAiServices(configuration);
        
        // File and Export Services
        services.AddFileServices();
        
        return services;
    }

    /// <summary>
    /// Registers email services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEmailConfiguration>(configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
        services.AddScoped<IEmailService, EmailService>();
        
        return services;
    }

    /// <summary>
    /// Registers Jira integration services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddJiraServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJiraConfiguration>(configuration.GetSection("JiraConfiguration").Get<JiraConfiguration>());
        services.AddScoped<IJIraService, JiraService>();
        
        return services;
    }

    /// <summary>
    /// Registers AI services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAiConfiguration>(configuration.GetSection("AIConfiguration").Get<AiConfiguration>());
        services.AddScoped<IAiService, AiService>();
        
        return services;
    }

    /// <summary>
    /// Registers file and export services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddFileServices(this IServiceCollection services)
    {
        services.AddScoped<IFileCheck, FileCheck>();
        services.AddSingleton<IExportToCsv, ExportToCsv>();
        
        return services;
    }
}