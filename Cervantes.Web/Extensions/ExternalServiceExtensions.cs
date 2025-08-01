using Cervantes.IFR.CervantesAI;
using Cervantes.IFR.Email;
using Cervantes.IFR.Export;
using Cervantes.IFR.File;
using Cervantes.IFR.Jira;
using Cervantes.IFR.Ldap;
using Cervantes.IFR.CveServices;
using Cervantes.Contracts;

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
        
        // LDAP Services
        services.AddLdapServices(configuration);
        
        // File and Export Services
        services.AddFileServices();
        
        // CVE Services
        services.AddCveServices(configuration);
        
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
    /// Registers LDAP authentication services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddLdapServices(this IServiceCollection services, IConfiguration configuration)
    {
        var ldapConfig = configuration.GetSection("LdapConfiguration").Get<LdapConfiguration>();
        
        if (ldapConfig == null)
        {
            ldapConfig = new LdapConfiguration();
            configuration.GetSection("LdapConfiguration").Bind(ldapConfig);
        }
        
        services.AddSingleton<ILdapConfiguration>(ldapConfig);
        services.AddScoped<ILdapService, LdapService>();
        
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

    /// <summary>
    /// Registers CVE services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCveServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register HttpClient for CVE services
        services.AddHttpClient<INvdApiService, NvdApiService>();
        services.AddHttpClient<IRedHatApiService, RedHatApiService>();
        services.AddHttpClient<ICveNotificationService, CveNotificationService>();
        
        // Register CVE services
        services.AddScoped<ICveSyncService, CveSyncService>();
        
        // Register VulnEnrichment services
        services.AddHttpClient<IEpssApiService, EpssApiService>();
        services.AddHttpClient<ICisaKevApiService, CisaKevApiService>(); 
        services.AddScoped<IVulnEnrichmentService, VulnEnrichmentService>();
        
        return services;
    }
}