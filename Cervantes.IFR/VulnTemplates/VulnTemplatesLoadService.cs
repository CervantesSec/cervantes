using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.IFR.VulnTemplates;

public class VulnTemplatesLoadService: IVulnTemplatesLoadService
{
    private IVulnManager vulnManager = null;
    private IVulnCweManager vulnCweManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private readonly ILogger<VulnTemplatesLoadService> _logger = null;

    public VulnTemplatesLoadService(IVulnManager vulnManager, IVulnCweManager vulnCweManager,
        IVulnCategoryManager vulnCategoryManager,ILogger<VulnTemplatesLoadService> logger)
    {
        this.vulnManager = vulnManager;
        this.vulnCweManager = vulnCweManager;
        this.vulnCategoryManager = vulnCategoryManager;
        _logger = logger;
    }

    public async Task CreateEnglish(string type)
    {
        switch (type)
        {
            case "All":
                await CreateEnglishGeneral();
                await CreateEnglishMastg();
                await CreateEnglishWstg();
                break;
            case "Wstg":
                await CreateEnglishWstg();
                break;
            case "Mastg":
                await CreateEnglishMastg();
                break;
            case "General":
                await CreateEnglishGeneral();
                break;
            
        }
        
    }
    
    public async Task CreateSpanish(string type)
    {
        switch (type)
        {
            case "All":
                await CreateSpanishGeneral();
                await CreateSpanishMastg();
                await CreateSpanishWstg();
                break;
            case "Wstg":
                await CreateSpanishWstg();
                break;
            case "Mastg":
                await CreateSpanishMastg();
                break;
            case "General":
                await CreateSpanishGeneral();
                break;
            
        }
    }
    
    public async Task CreatePortuguese(string type)
    {
        switch (type)
        {
            case "All":
                await CreatePortugueseWstg();
                await CreatePortugueseMastg();
                await CreatePortugueseGeneral();
                break;
            case "Wstg":
                await CreatePortugueseWstg();
                break;
            case "Mastg":
                await CreatePortugueseMastg();
                break;
            case "General":
                await CreatePortugueseGeneral();
                break;
            
        }
    }
    
    public async Task CreateEnglishGeneral()
    {
       

    }
    
    public async Task CreateEnglishMastg()
    {
        
    }
    
    public async Task CreateEnglishWstg()
    {
        Vuln insufficientAuthentication = VulnTemplatesWstgEN.InsufficientAuthentication;
        insufficientAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insufficientAuthentication.Id });
        insufficientAuthentication.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insufficientAuthentication.Id });
        vulnManager.Add(insufficientAuthentication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln weakPasswordRequirements = VulnTemplatesWstgEN.WeakPasswordRequirements;
        weakPasswordRequirements.VulnCwes.Add(new VulnCwe { CweId = 521, VulnId = weakPasswordRequirements.Id });
        vulnManager.Add(weakPasswordRequirements);
        await vulnManager.Context.SaveChangesAsync();

        Vuln insecureSessionManagement = VulnTemplatesWstgEN.InsecureSessionManagement;
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 384, VulnId = insecureSessionManagement.Id });
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionManagement.Id });
        vulnManager.Add(insecureSessionManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln idor = VulnTemplatesWstgEN.Idor;
        idor.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = idor.Id });
        vulnManager.Add(idor);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln crossSiteScripting = VulnTemplatesWstgEN.CrossSiteScripting;
        crossSiteScripting.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = crossSiteScripting.Id });
        vulnManager.Add(crossSiteScripting);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCryptographicStorage = VulnTemplatesWstgEN.InsecureCryptographicStorage;
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureCryptographicStorage.Id });
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe { CweId = 328, VulnId = insecureCryptographicStorage.Id });
        vulnManager.Add(insecureCryptographicStorage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDeserialization = VulnTemplatesWstgEN.InsecureDeserialization;
        insecureDeserialization.VulnCwes.Add(new VulnCwe { CweId = 502, VulnId = insecureDeserialization.Id });
        vulnManager.Add(insecureDeserialization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln sqlInjection = VulnTemplatesWstgEN.SqlInjection;
        sqlInjection.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = sqlInjection.Id });
        vulnManager.Add(sqlInjection);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln brokenAccessControl = VulnTemplatesWstgEN.BrokenAccessControl;
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = brokenAccessControl.Id });
        vulnManager.Add(brokenAccessControl);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln securityMisconfiguration = VulnTemplatesWstgEN.SecurityMisconfiguration;
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 16, VulnId = securityMisconfiguration.Id });
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 2, VulnId = securityMisconfiguration.Id });
        vulnManager.Add(securityMisconfiguration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln sensitiveDataExposure = VulnTemplatesWstgEN.SensitiveDataExposure;
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = sensitiveDataExposure.Id });
        vulnManager.Add(sensitiveDataExposure);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientLoggingMonitoring = VulnTemplatesWstgEN.InsufficientLoggingAndMonitoring;
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe { CweId = 778, VulnId = insufficientLoggingMonitoring.Id });
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe { CweId = 223, VulnId = insufficientLoggingMonitoring.Id });
        vulnManager.Add(insufficientLoggingMonitoring);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln usingComponentsWithKnownVulnerabilities = VulnTemplatesWstgEN.UsingComponentsWithKnownVulnerabilities;
        usingComponentsWithKnownVulnerabilities.VulnCwes.Add(new VulnCwe { CweId = 1035, VulnId = usingComponentsWithKnownVulnerabilities.Id });
        vulnManager.Add(usingComponentsWithKnownVulnerabilities);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln serverSideRequestForgery = VulnTemplatesWstgEN.ServerSideRequestForgery;
        serverSideRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 918, VulnId = serverSideRequestForgery.Id });
        vulnManager.Add(serverSideRequestForgery);
        await vulnManager.Context.SaveChangesAsync();
        
        
        
    }
    
    public async Task CreateSpanishGeneral()
    {
        
    }
    
    public async Task CreateSpanishMastg()
    {
        
    }
    
    public async Task CreateSpanishWstg()
    {
        
    }
    
    public async Task CreatePortugueseGeneral()
    {
        
    }
    
    public async Task CreatePortugueseMastg()
    {
        
    }
    
    public async Task CreatePortugueseWstg()
    {
        
    }
}