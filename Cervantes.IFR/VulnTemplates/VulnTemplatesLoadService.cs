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
        
        Vuln xmlExternalEntityProcessing = VulnTemplatesWstgEN.XmlExternalEntityProcessing;
        xmlExternalEntityProcessing.VulnCwes.Add(new VulnCwe { CweId = 611, VulnId = xmlExternalEntityProcessing.Id });
        vulnManager.Add(xmlExternalEntityProcessing);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfrateLimiting = VulnTemplatesWstgEN.LackOfRateLimiting;
        lackOfrateLimiting.VulnCwes.Add(new VulnCwe { CweId = 770, VulnId = lackOfrateLimiting.Id });
        vulnManager.Add(lackOfrateLimiting);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln inadequateOAuth2Implementation = VulnTemplatesWstgEN.InadequateOAuth2Implementation;
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe { CweId = 346, VulnId = inadequateOAuth2Implementation.Id });
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe { CweId = 601, VulnId = inadequateOAuth2Implementation.Id });
        vulnManager.Add(inadequateOAuth2Implementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientAntiAutomation = VulnTemplatesWstgEN.InsufficientAntiAutomation;
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 799, VulnId = insufficientAntiAutomation.Id });
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 837, VulnId = insufficientAntiAutomation.Id });
        vulnManager.Add(insufficientAntiAutomation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln weakCryptographicAlgorithms = VulnTemplatesWstgEN.WeakCryptographicAlgorithms;
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = weakCryptographicAlgorithms.Id });
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = weakCryptographicAlgorithms.Id });
        vulnManager.Add(weakCryptographicAlgorithms);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperCertificateValidation = VulnTemplatesWstgEN.ImproperCertificateValidation;
        improperCertificateValidation.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = improperCertificateValidation.Id });
        vulnManager.Add(improperCertificateValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln crossSiteRequestForgery = VulnTemplatesWstgEN.CrossSiteRequestForgery;
        crossSiteRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 352, VulnId = crossSiteRequestForgery.Id });
        vulnManager.Add(crossSiteRequestForgery);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientPasswordRecovery = VulnTemplatesWstgEN.InsufficientPasswordRecoveryMechanism;
        insufficientPasswordRecovery.VulnCwes.Add(new VulnCwe { CweId = 640, VulnId = insufficientPasswordRecovery.Id });
        vulnManager.Add(insufficientPasswordRecovery);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfInputSanitization = VulnTemplatesWstgEN.LackOfInputSanitization;
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = lackOfInputSanitization.Id });
        vulnManager.Add(lackOfInputSanitization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientSesssionTimeout = VulnTemplatesWstgEN.InsufficientSessionTimeout;
        insufficientSesssionTimeout.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insufficientSesssionTimeout.Id });
        vulnManager.Add(insufficientSesssionTimeout);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperErrorHandling = VulnTemplatesWstgEN.ImproperErrorHandling;
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 209, VulnId = improperErrorHandling.Id });
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = improperErrorHandling.Id });
        vulnManager.Add(improperErrorHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln missingSecurityHeaders = VulnTemplatesWstgEN.MissingSecurityHeaders;
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 693, VulnId = missingSecurityHeaders.Id });
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = missingSecurityHeaders.Id });
        vulnManager.Add(missingSecurityHeaders);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureUseOfCryptography = VulnTemplatesWstgEN.InsecureUseOfCryptography;
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureUseOfCryptography.Id });
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 320, VulnId = insecureUseOfCryptography.Id });
        vulnManager.Add(insecureUseOfCryptography);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln brokenFunctionLevelAuthorization = VulnTemplatesWstgEN.BrokenFunctionLevelAuthorization;
        brokenFunctionLevelAuthorization.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = brokenFunctionLevelAuthorization.Id });
        vulnManager.Add(brokenFunctionLevelAuthorization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCommunication = VulnTemplatesWstgEN.InsecureCommunicationChannels;
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureCommunication.Id });
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = insecureCommunication.Id });
        vulnManager.Add(insecureCommunication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfSecureDefaults = VulnTemplatesWstgEN.LackOfSecureDefaults;
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 276, VulnId = lackOfSecureDefaults.Id });
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 1188, VulnId = lackOfSecureDefaults.Id });
        vulnManager.Add(lackOfSecureDefaults);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientProtectionFromAutomatedThreats = VulnTemplatesWstgEN.InsufficientProtectionFromAutomatedThreats;
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe { CweId = 799, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe { CweId = 307, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        vulnManager.Add(insufficientProtectionFromAutomatedThreats);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln unvalidatedRedirectsAndForwards = VulnTemplatesWstgEN.UnvalidatedRedirectsAndForwards;
        unvalidatedRedirectsAndForwards.VulnCwes.Add(new VulnCwe { CweId = 601, VulnId = unvalidatedRedirectsAndForwards.Id });
        vulnManager.Add(unvalidatedRedirectsAndForwards);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAuthenticationMechanism = VulnTemplatesWstgEN.InsecureAuthenticationMechanism;
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureAuthenticationMechanism.Id });
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe { CweId = 384, VulnId = insecureAuthenticationMechanism.Id });
        vulnManager.Add(insecureAuthenticationMechanism);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insuffificientAntiCachingHeaders = VulnTemplatesWstgEN.InsufficientAntiCachingHeaders;
        insuffificientAntiCachingHeaders.VulnCwes.Add(new VulnCwe { CweId = 525, VulnId = insuffificientAntiCachingHeaders.Id });
        vulnManager.Add(insuffificientAntiCachingHeaders);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfProperTLSConfiguration = VulnTemplatesWstgEN.LackOfProperTLSConfiguration;
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = lackOfProperTLSConfiguration.Id });
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = lackOfProperTLSConfiguration.Id });
        vulnManager.Add(lackOfProperTLSConfiguration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientDataProtection = VulnTemplatesWstgEN.InsufficientDataProtection;
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insufficientDataProtection.Id });
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insufficientDataProtection.Id });
        vulnManager.Add(insufficientDataProtection);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperAssetManagement = VulnTemplatesWstgEN.ImproperAssetManagement;
        improperAssetManagement.VulnCwes.Add(new VulnCwe { CweId = 1059, VulnId = improperAssetManagement.Id });
        vulnManager.Add(improperAssetManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfSoftwareUpdate = VulnTemplatesWstgEN.LackOfSoftwareUpdates;
        lackOfSoftwareUpdate.VulnCwes.Add(new VulnCwe { CweId = 1104, VulnId = lackOfSoftwareUpdate.Id });
        vulnManager.Add(lackOfSoftwareUpdate);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientPrivacyControls = VulnTemplatesWstgEN.InsufficientPrivacyControls;
        insufficientPrivacyControls.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insufficientPrivacyControls.Id });
        vulnManager.Add(insufficientPrivacyControls);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureApiEndpoints = VulnTemplatesWstgEN.InsecureAPIEndpoints;
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = insecureApiEndpoints.Id });
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = insecureApiEndpoints.Id });
        vulnManager.Add(insecureApiEndpoints);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientInputValidation = VulnTemplatesWstgEN.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperOutputEncoding = VulnTemplatesWstgEN.ImproperOutputEncoding;
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = improperOutputEncoding.Id });
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 116, VulnId = improperOutputEncoding.Id });
        vulnManager.Add(improperOutputEncoding);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureFileHandling = VulnTemplatesWstgEN.InsecureFileHandling;
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 73, VulnId = insecureFileHandling.Id });
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 434, VulnId = insecureFileHandling.Id });
        vulnManager.Add(insecureFileHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfSecurePasswordStorage = VulnTemplatesWstgEN.LackOfSecurePasswordStorage;
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 256, VulnId = lackOfSecurePasswordStorage.Id });
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 916, VulnId = lackOfSecurePasswordStorage.Id });
        vulnManager.Add(lackOfSecurePasswordStorage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientProtectionAgainstDos = VulnTemplatesWstgEN.InsufficientProtectionAgainstDoS;
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe { CweId = 400, VulnId = insufficientProtectionAgainstDos.Id });
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe { CweId = 770, VulnId = insufficientProtectionAgainstDos.Id });
        vulnManager.Add(insufficientProtectionAgainstDos);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientSubresourceIntegrity = VulnTemplatesWstgEN.InsufficientSubresourceIntegrity;
        insufficientSubresourceIntegrity.VulnCwes.Add(new VulnCwe { CweId = 353, VulnId = insufficientSubresourceIntegrity.Id });
        vulnManager.Add(insufficientSubresourceIntegrity);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln inadequateSecurityArchitecture = VulnTemplatesWstgEN.InadequateSecurityArchitecture;
        inadequateSecurityArchitecture.VulnCwes.Add(new VulnCwe { CweId = 1008, VulnId = inadequateSecurityArchitecture.Id });
        vulnManager.Add(inadequateSecurityArchitecture);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln flawedBusinessLogic = VulnTemplatesWstgEN.FlawedBusinessLogic;
        flawedBusinessLogic.VulnCwes.Add(new VulnCwe { CweId = 840, VulnId = flawedBusinessLogic.Id });
        vulnManager.Add(flawedBusinessLogic);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureThirdPartyComponents = VulnTemplatesWstgEN.InsecureThirdPartyComponents;
        insecureThirdPartyComponents.VulnCwes.Add(new VulnCwe { CweId = 1104, VulnId = insecureThirdPartyComponents.Id });
        vulnManager.Add(insecureThirdPartyComponents);
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
        Vuln insufficientAuthentication = VulnTemplatesWstgES.InsufficientAuthentication;
        insufficientAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insufficientAuthentication.Id });
        insufficientAuthentication.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insufficientAuthentication.Id });
        vulnManager.Add(insufficientAuthentication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln weakPasswordRequirements = VulnTemplatesWstgES.WeakPasswordRequirements;
        weakPasswordRequirements.VulnCwes.Add(new VulnCwe { CweId = 521, VulnId = weakPasswordRequirements.Id });
        vulnManager.Add(weakPasswordRequirements);
        await vulnManager.Context.SaveChangesAsync();

        Vuln insecureSessionManagement = VulnTemplatesWstgES.InsecureSessionManagement;
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 384, VulnId = insecureSessionManagement.Id });
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionManagement.Id });
        vulnManager.Add(insecureSessionManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln idor = VulnTemplatesWstgES.Idor;
        idor.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = idor.Id });
        vulnManager.Add(idor);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln crossSiteScripting = VulnTemplatesWstgES.CrossSiteScripting;
        crossSiteScripting.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = crossSiteScripting.Id });
        vulnManager.Add(crossSiteScripting);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCryptographicStorage = VulnTemplatesWstgES.InsecureCryptographicStorage;
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureCryptographicStorage.Id });
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe { CweId = 328, VulnId = insecureCryptographicStorage.Id });
        vulnManager.Add(insecureCryptographicStorage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDeserialization = VulnTemplatesWstgES.InsecureDeserialization;
        insecureDeserialization.VulnCwes.Add(new VulnCwe { CweId = 502, VulnId = insecureDeserialization.Id });
        vulnManager.Add(insecureDeserialization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln sqlInjection = VulnTemplatesWstgES.SqlInjection;
        sqlInjection.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = sqlInjection.Id });
        vulnManager.Add(sqlInjection);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln brokenAccessControl = VulnTemplatesWstgES.BrokenAccessControl;
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = brokenAccessControl.Id });
        vulnManager.Add(brokenAccessControl);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln securityMisconfiguration = VulnTemplatesWstgES.SecurityMisconfiguration;
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 16, VulnId = securityMisconfiguration.Id });
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 2, VulnId = securityMisconfiguration.Id });
        vulnManager.Add(securityMisconfiguration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln sensitiveDataExposure = VulnTemplatesWstgES.SensitiveDataExposure;
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = sensitiveDataExposure.Id });
        vulnManager.Add(sensitiveDataExposure);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientLoggingMonitoring = VulnTemplatesWstgES.InsufficientLoggingAndMonitoring;
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe { CweId = 778, VulnId = insufficientLoggingMonitoring.Id });
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe { CweId = 223, VulnId = insufficientLoggingMonitoring.Id });
        vulnManager.Add(insufficientLoggingMonitoring);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln usingComponentsWithKnownVulnerabilities = VulnTemplatesWstgES.UsingComponentsWithKnownVulnerabilities;
        usingComponentsWithKnownVulnerabilities.VulnCwes.Add(new VulnCwe { CweId = 1035, VulnId = usingComponentsWithKnownVulnerabilities.Id });
        vulnManager.Add(usingComponentsWithKnownVulnerabilities);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln serverSideRequestForgery = VulnTemplatesWstgES.ServerSideRequestForgery;
        serverSideRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 918, VulnId = serverSideRequestForgery.Id });
        vulnManager.Add(serverSideRequestForgery);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln xmlExternalEntityProcessing = VulnTemplatesWstgES.XmlExternalEntityProcessing;
        xmlExternalEntityProcessing.VulnCwes.Add(new VulnCwe { CweId = 611, VulnId = xmlExternalEntityProcessing.Id });
        vulnManager.Add(xmlExternalEntityProcessing);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfrateLimiting = VulnTemplatesWstgES.LackOfRateLimiting;
        lackOfrateLimiting.VulnCwes.Add(new VulnCwe { CweId = 770, VulnId = lackOfrateLimiting.Id });
        vulnManager.Add(lackOfrateLimiting);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln inadequateOAuth2Implementation = VulnTemplatesWstgES.InadequateOAuth2Implementation;
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe { CweId = 346, VulnId = inadequateOAuth2Implementation.Id });
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe { CweId = 601, VulnId = inadequateOAuth2Implementation.Id });
        vulnManager.Add(inadequateOAuth2Implementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientAntiAutomation = VulnTemplatesWstgES.InsufficientAntiAutomation;
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 799, VulnId = insufficientAntiAutomation.Id });
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 837, VulnId = insufficientAntiAutomation.Id });
        vulnManager.Add(insufficientAntiAutomation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln weakCryptographicAlgorithms = VulnTemplatesWstgES.WeakCryptographicAlgorithms;
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = weakCryptographicAlgorithms.Id });
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = weakCryptographicAlgorithms.Id });
        vulnManager.Add(weakCryptographicAlgorithms);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperCertificateValidation = VulnTemplatesWstgES.ImproperCertificateValidation;
        improperCertificateValidation.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = improperCertificateValidation.Id });
        vulnManager.Add(improperCertificateValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln crossSiteRequestForgery = VulnTemplatesWstgES.CrossSiteRequestForgery;
        crossSiteRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 352, VulnId = crossSiteRequestForgery.Id });
        vulnManager.Add(crossSiteRequestForgery);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientPasswordRecovery = VulnTemplatesWstgES.InsufficientPasswordRecoveryMechanism;
        insufficientPasswordRecovery.VulnCwes.Add(new VulnCwe { CweId = 640, VulnId = insufficientPasswordRecovery.Id });
        vulnManager.Add(insufficientPasswordRecovery);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfInputSanitization = VulnTemplatesWstgES.LackOfInputSanitization;
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = lackOfInputSanitization.Id });
        vulnManager.Add(lackOfInputSanitization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientSesssionTimeout = VulnTemplatesWstgES.InsufficientSessionTimeout;
        insufficientSesssionTimeout.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insufficientSesssionTimeout.Id });
        vulnManager.Add(insufficientSesssionTimeout);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperErrorHandling = VulnTemplatesWstgES.ImproperErrorHandling;
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 209, VulnId = improperErrorHandling.Id });
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = improperErrorHandling.Id });
        vulnManager.Add(improperErrorHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln missingSecurityHeaders = VulnTemplatesWstgES.MissingSecurityHeaders;
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 693, VulnId = missingSecurityHeaders.Id });
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = missingSecurityHeaders.Id });
        vulnManager.Add(missingSecurityHeaders);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureUseOfCryptography = VulnTemplatesWstgES.InsecureUseOfCryptography;
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureUseOfCryptography.Id });
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 320, VulnId = insecureUseOfCryptography.Id });
        vulnManager.Add(insecureUseOfCryptography);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln brokenFunctionLevelAuthorization = VulnTemplatesWstgES.BrokenFunctionLevelAuthorization;
        brokenFunctionLevelAuthorization.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = brokenFunctionLevelAuthorization.Id });
        vulnManager.Add(brokenFunctionLevelAuthorization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCommunication = VulnTemplatesWstgES.InsecureCommunicationChannels;
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureCommunication.Id });
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = insecureCommunication.Id });
        vulnManager.Add(insecureCommunication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfSecureDefaults = VulnTemplatesWstgES.LackOfSecureDefaults;
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 276, VulnId = lackOfSecureDefaults.Id });
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 1188, VulnId = lackOfSecureDefaults.Id });
        vulnManager.Add(lackOfSecureDefaults);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientProtectionFromAutomatedThreats = VulnTemplatesWstgES.InsufficientProtectionFromAutomatedThreats;
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe { CweId = 799, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe { CweId = 307, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        vulnManager.Add(insufficientProtectionFromAutomatedThreats);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln unvalidatedRedirectsAndForwards = VulnTemplatesWstgES.UnvalidatedRedirectsAndForwards;
        unvalidatedRedirectsAndForwards.VulnCwes.Add(new VulnCwe { CweId = 601, VulnId = unvalidatedRedirectsAndForwards.Id });
        vulnManager.Add(unvalidatedRedirectsAndForwards);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAuthenticationMechanism = VulnTemplatesWstgES.InsecureAuthenticationMechanism;
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureAuthenticationMechanism.Id });
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe { CweId = 384, VulnId = insecureAuthenticationMechanism.Id });
        vulnManager.Add(insecureAuthenticationMechanism);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insuffificientAntiCachingHeaders = VulnTemplatesWstgES.InsufficientAntiCachingHeaders;
        insuffificientAntiCachingHeaders.VulnCwes.Add(new VulnCwe { CweId = 525, VulnId = insuffificientAntiCachingHeaders.Id });
        vulnManager.Add(insuffificientAntiCachingHeaders);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfProperTLSConfiguration = VulnTemplatesWstgES.LackOfProperTLSConfiguration;
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = lackOfProperTLSConfiguration.Id });
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = lackOfProperTLSConfiguration.Id });
        vulnManager.Add(lackOfProperTLSConfiguration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientDataProtection = VulnTemplatesWstgES.InsufficientDataProtection;
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insufficientDataProtection.Id });
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insufficientDataProtection.Id });
        vulnManager.Add(insufficientDataProtection);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperAssetManagement = VulnTemplatesWstgES.ImproperAssetManagement;
        improperAssetManagement.VulnCwes.Add(new VulnCwe { CweId = 1059, VulnId = improperAssetManagement.Id });
        vulnManager.Add(improperAssetManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfSoftwareUpdate = VulnTemplatesWstgES.LackOfSoftwareUpdates;
        lackOfSoftwareUpdate.VulnCwes.Add(new VulnCwe { CweId = 1104, VulnId = lackOfSoftwareUpdate.Id });
        vulnManager.Add(lackOfSoftwareUpdate);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientPrivacyControls = VulnTemplatesWstgES.InsufficientPrivacyControls;
        insufficientPrivacyControls.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insufficientPrivacyControls.Id });
        vulnManager.Add(insufficientPrivacyControls);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureApiEndpoints = VulnTemplatesWstgES.InsecureAPIEndpoints;
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = insecureApiEndpoints.Id });
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = insecureApiEndpoints.Id });
        vulnManager.Add(insecureApiEndpoints);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientInputValidation = VulnTemplatesWstgES.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperOutputEncoding = VulnTemplatesWstgES.ImproperOutputEncoding;
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = improperOutputEncoding.Id });
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 116, VulnId = improperOutputEncoding.Id });
        vulnManager.Add(improperOutputEncoding);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureFileHandling = VulnTemplatesWstgES.InsecureFileHandling;
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 73, VulnId = insecureFileHandling.Id });
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 434, VulnId = insecureFileHandling.Id });
        vulnManager.Add(insecureFileHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfSecurePasswordStorage = VulnTemplatesWstgES.LackOfSecurePasswordStorage;
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 256, VulnId = lackOfSecurePasswordStorage.Id });
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 916, VulnId = lackOfSecurePasswordStorage.Id });
        vulnManager.Add(lackOfSecurePasswordStorage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientProtectionAgainstDos = VulnTemplatesWstgES.InsufficientProtectionAgainstDoS;
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe { CweId = 400, VulnId = insufficientProtectionAgainstDos.Id });
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe { CweId = 770, VulnId = insufficientProtectionAgainstDos.Id });
        vulnManager.Add(insufficientProtectionAgainstDos);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientSubresourceIntegrity = VulnTemplatesWstgES.InsufficientSubresourceIntegrity;
        insufficientSubresourceIntegrity.VulnCwes.Add(new VulnCwe { CweId = 353, VulnId = insufficientSubresourceIntegrity.Id });
        vulnManager.Add(insufficientSubresourceIntegrity);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln inadequateSecurityArchitecture = VulnTemplatesWstgES.InadequateSecurityArchitecture;
        inadequateSecurityArchitecture.VulnCwes.Add(new VulnCwe { CweId = 1008, VulnId = inadequateSecurityArchitecture.Id });
        vulnManager.Add(inadequateSecurityArchitecture);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln flawedBusinessLogic = VulnTemplatesWstgES.FlawedBusinessLogic;
        flawedBusinessLogic.VulnCwes.Add(new VulnCwe { CweId = 840, VulnId = flawedBusinessLogic.Id });
        vulnManager.Add(flawedBusinessLogic);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureThirdPartyComponents = VulnTemplatesWstgES.InsecureThirdPartyComponents;
        insecureThirdPartyComponents.VulnCwes.Add(new VulnCwe { CweId = 1104, VulnId = insecureThirdPartyComponents.Id });
        vulnManager.Add(insecureThirdPartyComponents);
        await vulnManager.Context.SaveChangesAsync();
    }
    
    public async Task CreatePortugueseGeneral()
    {
        
    }
    
    public async Task CreatePortugueseMastg()
    {
        
    }
    
    public async Task CreatePortugueseWstg()
    {
        Vuln insufficientAuthentication = VulnTemplatesWstgPT.InsufficientAuthentication;
        insufficientAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insufficientAuthentication.Id });
        insufficientAuthentication.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insufficientAuthentication.Id });
        vulnManager.Add(insufficientAuthentication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln weakPasswordRequirements = VulnTemplatesWstgPT.WeakPasswordRequirements;
        weakPasswordRequirements.VulnCwes.Add(new VulnCwe { CweId = 521, VulnId = weakPasswordRequirements.Id });
        vulnManager.Add(weakPasswordRequirements);
        await vulnManager.Context.SaveChangesAsync();

        Vuln insecureSessionManagement = VulnTemplatesWstgPT.InsecureSessionManagement;
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 384, VulnId = insecureSessionManagement.Id });
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionManagement.Id });
        vulnManager.Add(insecureSessionManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln idor = VulnTemplatesWstgPT.Idor;
        idor.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = idor.Id });
        vulnManager.Add(idor);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln crossSiteScripting = VulnTemplatesWstgPT.CrossSiteScripting;
        crossSiteScripting.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = crossSiteScripting.Id });
        vulnManager.Add(crossSiteScripting);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCryptographicStorage = VulnTemplatesWstgPT.InsecureCryptographicStorage;
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureCryptographicStorage.Id });
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe { CweId = 328, VulnId = insecureCryptographicStorage.Id });
        vulnManager.Add(insecureCryptographicStorage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDeserialization = VulnTemplatesWstgPT.InsecureDeserialization;
        insecureDeserialization.VulnCwes.Add(new VulnCwe { CweId = 502, VulnId = insecureDeserialization.Id });
        vulnManager.Add(insecureDeserialization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln sqlInjection = VulnTemplatesWstgPT.SqlInjection;
        sqlInjection.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = sqlInjection.Id });
        vulnManager.Add(sqlInjection);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln brokenAccessControl = VulnTemplatesWstgPT.BrokenAccessControl;
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = brokenAccessControl.Id });
        vulnManager.Add(brokenAccessControl);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln securityMisconfiguration = VulnTemplatesWstgPT.SecurityMisconfiguration;
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 16, VulnId = securityMisconfiguration.Id });
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 2, VulnId = securityMisconfiguration.Id });
        vulnManager.Add(securityMisconfiguration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln sensitiveDataExposure = VulnTemplatesWstgPT.SensitiveDataExposure;
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = sensitiveDataExposure.Id });
        vulnManager.Add(sensitiveDataExposure);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientLoggingMonitoring = VulnTemplatesWstgPT.InsufficientLoggingAndMonitoring;
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe { CweId = 778, VulnId = insufficientLoggingMonitoring.Id });
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe { CweId = 223, VulnId = insufficientLoggingMonitoring.Id });
        vulnManager.Add(insufficientLoggingMonitoring);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln usingComponentsWithKnownVulnerabilities = VulnTemplatesWstgPT.UsingComponentsWithKnownVulnerabilities;
        usingComponentsWithKnownVulnerabilities.VulnCwes.Add(new VulnCwe { CweId = 1035, VulnId = usingComponentsWithKnownVulnerabilities.Id });
        vulnManager.Add(usingComponentsWithKnownVulnerabilities);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln serverSideRequestForgery = VulnTemplatesWstgPT.ServerSideRequestForgery;
        serverSideRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 918, VulnId = serverSideRequestForgery.Id });
        vulnManager.Add(serverSideRequestForgery);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln xmlExternalEntityProcessing = VulnTemplatesWstgPT.XmlExternalEntityProcessing;
        xmlExternalEntityProcessing.VulnCwes.Add(new VulnCwe { CweId = 611, VulnId = xmlExternalEntityProcessing.Id });
        vulnManager.Add(xmlExternalEntityProcessing);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfrateLimiting = VulnTemplatesWstgPT.LackOfRateLimiting;
        lackOfrateLimiting.VulnCwes.Add(new VulnCwe { CweId = 770, VulnId = lackOfrateLimiting.Id });
        vulnManager.Add(lackOfrateLimiting);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln inadequateOAuth2Implementation = VulnTemplatesWstgPT.InadequateOAuth2Implementation;
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe { CweId = 346, VulnId = inadequateOAuth2Implementation.Id });
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe { CweId = 601, VulnId = inadequateOAuth2Implementation.Id });
        vulnManager.Add(inadequateOAuth2Implementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientAntiAutomation = VulnTemplatesWstgPT.InsufficientAntiAutomation;
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 799, VulnId = insufficientAntiAutomation.Id });
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 837, VulnId = insufficientAntiAutomation.Id });
        vulnManager.Add(insufficientAntiAutomation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln weakCryptographicAlgorithms = VulnTemplatesWstgPT.WeakCryptographicAlgorithms;
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = weakCryptographicAlgorithms.Id });
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = weakCryptographicAlgorithms.Id });
        vulnManager.Add(weakCryptographicAlgorithms);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperCertificateValidation = VulnTemplatesWstgPT.ImproperCertificateValidation;
        improperCertificateValidation.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = improperCertificateValidation.Id });
        vulnManager.Add(improperCertificateValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln crossSiteRequestForgery = VulnTemplatesWstgPT.CrossSiteRequestForgery;
        crossSiteRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 352, VulnId = crossSiteRequestForgery.Id });
        vulnManager.Add(crossSiteRequestForgery);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientPasswordRecovery = VulnTemplatesWstgPT.InsufficientPasswordRecoveryMechanism;
        insufficientPasswordRecovery.VulnCwes.Add(new VulnCwe { CweId = 640, VulnId = insufficientPasswordRecovery.Id });
        vulnManager.Add(insufficientPasswordRecovery);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfInputSanitization = VulnTemplatesWstgPT.LackOfInputSanitization;
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = lackOfInputSanitization.Id });
        vulnManager.Add(lackOfInputSanitization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientSesssionTimeout = VulnTemplatesWstgPT.InsufficientSessionTimeout;
        insufficientSesssionTimeout.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insufficientSesssionTimeout.Id });
        vulnManager.Add(insufficientSesssionTimeout);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperErrorHandling = VulnTemplatesWstgPT.ImproperErrorHandling;
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 209, VulnId = improperErrorHandling.Id });
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = improperErrorHandling.Id });
        vulnManager.Add(improperErrorHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln missingSecurityHeaders = VulnTemplatesWstgPT.MissingSecurityHeaders;
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 693, VulnId = missingSecurityHeaders.Id });
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = missingSecurityHeaders.Id });
        vulnManager.Add(missingSecurityHeaders);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureUseOfCryptography = VulnTemplatesWstgPT.InsecureUseOfCryptography;
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureUseOfCryptography.Id });
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 320, VulnId = insecureUseOfCryptography.Id });
        vulnManager.Add(insecureUseOfCryptography);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln brokenFunctionLevelAuthorization = VulnTemplatesWstgPT.BrokenFunctionLevelAuthorization;
        brokenFunctionLevelAuthorization.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = brokenFunctionLevelAuthorization.Id });
        vulnManager.Add(brokenFunctionLevelAuthorization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCommunication = VulnTemplatesWstgPT.InsecureCommunicationChannels;
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureCommunication.Id });
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = insecureCommunication.Id });
        vulnManager.Add(insecureCommunication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfSecureDefaults = VulnTemplatesWstgPT.LackOfSecureDefaults;
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 276, VulnId = lackOfSecureDefaults.Id });
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 1188, VulnId = lackOfSecureDefaults.Id });
        vulnManager.Add(lackOfSecureDefaults);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientProtectionFromAutomatedThreats = VulnTemplatesWstgPT.InsufficientProtectionFromAutomatedThreats;
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe { CweId = 799, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe { CweId = 307, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        vulnManager.Add(insufficientProtectionFromAutomatedThreats);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln unvalidatedRedirectsAndForwards = VulnTemplatesWstgPT.UnvalidatedRedirectsAndForwards;
        unvalidatedRedirectsAndForwards.VulnCwes.Add(new VulnCwe { CweId = 601, VulnId = unvalidatedRedirectsAndForwards.Id });
        vulnManager.Add(unvalidatedRedirectsAndForwards);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAuthenticationMechanism = VulnTemplatesWstgPT.InsecureAuthenticationMechanism;
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureAuthenticationMechanism.Id });
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe { CweId = 384, VulnId = insecureAuthenticationMechanism.Id });
        vulnManager.Add(insecureAuthenticationMechanism);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insuffificientAntiCachingHeaders = VulnTemplatesWstgPT.InsufficientAntiCachingHeaders;
        insuffificientAntiCachingHeaders.VulnCwes.Add(new VulnCwe { CweId = 525, VulnId = insuffificientAntiCachingHeaders.Id });
        vulnManager.Add(insuffificientAntiCachingHeaders);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfProperTLSConfiguration = VulnTemplatesWstgPT.LackOfProperTLSConfiguration;
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = lackOfProperTLSConfiguration.Id });
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = lackOfProperTLSConfiguration.Id });
        vulnManager.Add(lackOfProperTLSConfiguration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientDataProtection = VulnTemplatesWstgPT.InsufficientDataProtection;
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insufficientDataProtection.Id });
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insufficientDataProtection.Id });
        vulnManager.Add(insufficientDataProtection);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperAssetManagement = VulnTemplatesWstgPT.ImproperAssetManagement;
        improperAssetManagement.VulnCwes.Add(new VulnCwe { CweId = 1059, VulnId = improperAssetManagement.Id });
        vulnManager.Add(improperAssetManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfSoftwareUpdate = VulnTemplatesWstgPT.LackOfSoftwareUpdates;
        lackOfSoftwareUpdate.VulnCwes.Add(new VulnCwe { CweId = 1104, VulnId = lackOfSoftwareUpdate.Id });
        vulnManager.Add(lackOfSoftwareUpdate);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientPrivacyControls = VulnTemplatesWstgPT.InsufficientPrivacyControls;
        insufficientPrivacyControls.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insufficientPrivacyControls.Id });
        vulnManager.Add(insufficientPrivacyControls);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureApiEndpoints = VulnTemplatesWstgPT.InsecureAPIEndpoints;
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = insecureApiEndpoints.Id });
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = insecureApiEndpoints.Id });
        vulnManager.Add(insecureApiEndpoints);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientInputValidation = VulnTemplatesWstgPT.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln improperOutputEncoding = VulnTemplatesWstgPT.ImproperOutputEncoding;
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = improperOutputEncoding.Id });
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 116, VulnId = improperOutputEncoding.Id });
        vulnManager.Add(improperOutputEncoding);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureFileHandling = VulnTemplatesWstgPT.InsecureFileHandling;
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 73, VulnId = insecureFileHandling.Id });
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 434, VulnId = insecureFileHandling.Id });
        vulnManager.Add(insecureFileHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln lackOfSecurePasswordStorage = VulnTemplatesWstgPT.LackOfSecurePasswordStorage;
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 256, VulnId = lackOfSecurePasswordStorage.Id });
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 916, VulnId = lackOfSecurePasswordStorage.Id });
        vulnManager.Add(lackOfSecurePasswordStorage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientProtectionAgainstDos = VulnTemplatesWstgPT.InsufficientProtectionAgainstDoS;
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe { CweId = 400, VulnId = insufficientProtectionAgainstDos.Id });
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe { CweId = 770, VulnId = insufficientProtectionAgainstDos.Id });
        vulnManager.Add(insufficientProtectionAgainstDos);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientSubresourceIntegrity = VulnTemplatesWstgPT.InsufficientSubresourceIntegrity;
        insufficientSubresourceIntegrity.VulnCwes.Add(new VulnCwe { CweId = 353, VulnId = insufficientSubresourceIntegrity.Id });
        vulnManager.Add(insufficientSubresourceIntegrity);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln inadequateSecurityArchitecture = VulnTemplatesWstgPT.InadequateSecurityArchitecture;
        inadequateSecurityArchitecture.VulnCwes.Add(new VulnCwe { CweId = 1008, VulnId = inadequateSecurityArchitecture.Id });
        vulnManager.Add(inadequateSecurityArchitecture);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln flawedBusinessLogic = VulnTemplatesWstgPT.FlawedBusinessLogic;
        flawedBusinessLogic.VulnCwes.Add(new VulnCwe { CweId = 840, VulnId = flawedBusinessLogic.Id });
        vulnManager.Add(flawedBusinessLogic);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureThirdPartyComponents = VulnTemplatesWstgPT.InsecureThirdPartyComponents;
        insecureThirdPartyComponents.VulnCwes.Add(new VulnCwe { CweId = 1104, VulnId = insecureThirdPartyComponents.Id });
        vulnManager.Add(insecureThirdPartyComponents);
        await vulnManager.Context.SaveChangesAsync();
    }
}