using Cervantes.IFR.Parsers.Acunetix;
using Cervantes.IFR.Parsers.Bandit;
using Cervantes.IFR.Parsers.Burp;
using Cervantes.IFR.Parsers.CSV;
using Cervantes.IFR.Parsers.DependencyCheck;
using Cervantes.IFR.Parsers.Masscan;
using Cervantes.IFR.Parsers.Nessus;
using Cervantes.IFR.Parsers.Nikto;
using Cervantes.IFR.Parsers.Nmap;
using Cervantes.IFR.Parsers.Nuclei;
using Cervantes.IFR.Parsers.OpenVAS;
using Cervantes.IFR.Parsers.Prowler;
using Cervantes.IFR.Parsers.Pwndoc;
using Cervantes.IFR.Parsers.Qualys;
using Cervantes.IFR.Parsers.Trivy;
using Cervantes.IFR.Parsers.Zap;

namespace Cervantes.Web.Extensions;

/// <summary>
/// Extension methods for registering vulnerability parsers in the service collection
/// </summary>
public static class ParserServiceExtensions
{
    /// <summary>
    /// Registers all vulnerability parsers with the service collection
    /// </summary>
    /// <param name="services">The service collection to add the parsers to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddVulnerabilityParsers(this IServiceCollection services)
    {
        // Web Security Parsers
        services.AddScoped<IBurpParser, BurpParser>();
        services.AddScoped<IZapParser, ZapParser>();
        services.AddScoped<IAcunetixParser, AcunetixParser>();
        services.AddScoped<INiktoParser, NiktoParser>();
        
        // Infrastructure Security Parsers
        services.AddScoped<INessusParser, NessusParser>();
        services.AddScoped<IOpenVASParser, OpenVASParser>();
        services.AddScoped<IQualysParser, QualysParser>();
        services.AddScoped<INucleiParser, NucleiParser>();
        services.AddScoped<IMasscanParser, MasscanParser>();
        services.AddScoped<INmapParser, NmapParser>();
        
        // Cloud Security Parsers
        services.AddScoped<IProwlerParser, ProwlerParser>();
        
        // Container Security Parsers
        services.AddScoped<ITrivyParser, TrivyParser>();
        
        // Code Analysis Parsers
        services.AddScoped<IBanditParser, BanditParser>();
        
        // Dependency Analysis Parsers
        services.AddScoped<IDependencyCheckParser, DependencyCheckParser>();
        
        // Data Import Parsers
        services.AddScoped<ICsvParser, CsvParser>();
        services.AddScoped<IPwndocParser, PwndocParser>();
        
        return services;
    }

    /// <summary>
    /// Registers web security parsers only
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddWebSecurityParsers(this IServiceCollection services)
    {
        services.AddScoped<IBurpParser, BurpParser>();
        services.AddScoped<IZapParser, ZapParser>();
        services.AddScoped<IAcunetixParser, AcunetixParser>();
        services.AddScoped<INiktoParser, NiktoParser>();
        
        return services;
    }

    /// <summary>
    /// Registers infrastructure security parsers only
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructureParsers(this IServiceCollection services)
    {
        services.AddScoped<INessusParser, NessusParser>();
        services.AddScoped<IOpenVASParser, OpenVASParser>();
        services.AddScoped<IQualysParser, QualysParser>();
        services.AddScoped<INucleiParser, NucleiParser>();
        services.AddScoped<IMasscanParser, MasscanParser>();
        services.AddScoped<INmapParser, NmapParser>();
        
        return services;
    }

    /// <summary>
    /// Registers cloud security parsers only
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCloudSecurityParsers(this IServiceCollection services)
    {
        services.AddScoped<IProwlerParser, ProwlerParser>();
        
        return services;
    }

    /// <summary>
    /// Registers container security parsers only
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddContainerSecurityParsers(this IServiceCollection services)
    {
        services.AddScoped<ITrivyParser, TrivyParser>();
        
        return services;
    }

    /// <summary>
    /// Registers code analysis parsers only
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCodeAnalysisParsers(this IServiceCollection services)
    {
        services.AddScoped<IBanditParser, BanditParser>();
        
        return services;
    }

    /// <summary>
    /// Registers dependency analysis parsers only
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDependencyAnalysisParsers(this IServiceCollection services)
    {
        services.AddScoped<IDependencyCheckParser, DependencyCheckParser>();
        
        return services;
    }

    /// <summary>
    /// Registers data import parsers only
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDataImportParsers(this IServiceCollection services)
    {
        services.AddScoped<ICsvParser, CsvParser>();
        services.AddScoped<IPwndocParser, PwndocParser>();
        
        return services;
    }
}