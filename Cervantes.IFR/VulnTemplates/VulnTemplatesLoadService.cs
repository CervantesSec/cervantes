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
        Vuln insecureDataStorage = VulnTemplatesMastgEN.InsecureDataStorage;
        insecureDataStorage.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insecureDataStorage.Id });
        vulnManager.Add(insecureDataStorage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCryptography = VulnTemplatesMastgEN.InsecureCryptography;
        insecureCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureCryptography.Id });
        vulnManager.Add(insecureCryptography);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAuthentication = VulnTemplatesMastgEN.InsecureAuthentication;
        insecureAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureAuthentication.Id });
        vulnManager.Add(insecureAuthentication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureNetworkCommunication = VulnTemplatesMastgEN.InsecureNetworkCommunication;
        insecureNetworkCommunication.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureNetworkCommunication.Id });
        vulnManager.Add(insecureNetworkCommunication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln privacyViolations = VulnTemplatesMastgEN.PrivacyViolation;
        privacyViolations.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = privacyViolations.Id });
        vulnManager.Add(privacyViolations);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataLeakage = VulnTemplatesMastgEN.InsecureDataLeakage;
        insecureDataLeakage.VulnCwes.Add(new VulnCwe { CweId = 532, VulnId = insecureDataLeakage.Id });
        vulnManager.Add(insecureDataLeakage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureKeyManagement = VulnTemplatesMastgEN.InsecureKeyManagement;
        insecureKeyManagement.VulnCwes.Add(new VulnCwe { CweId = 321, VulnId = insecureKeyManagement.Id });
        vulnManager.Add(insecureKeyManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureLocalAuthentication = VulnTemplatesMastgEN.InsecureLocalAuthentication;
        insecureLocalAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureLocalAuthentication.Id });
        vulnManager.Add(insecureLocalAuthentication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCertificatePinning = VulnTemplatesMastgEN.InsecureCertificatePinning;
        insecureCertificatePinning.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureCertificatePinning.Id });
        vulnManager.Add(insecureCertificatePinning);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureIPC = VulnTemplatesMastgEN.InsecureIPC;
        insecureIPC.VulnCwes.Add(new VulnCwe { CweId = 927, VulnId = insecureIPC.Id });
        vulnManager.Add(insecureIPC);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureWebview = VulnTemplatesMastgEN.InsecureWebView;
        insecureWebview.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureWebview.Id });
        vulnManager.Add(insecureWebview);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDeepLinking = VulnTemplatesMastgEN.InsecureDeepLinking;
        insecureDeepLinking.VulnCwes.Add(new VulnCwe { CweId = 939, VulnId = insecureDeepLinking.Id });
        vulnManager.Add(insecureDeepLinking);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureSessionHandling = VulnTemplatesMastgEN.InsecureSessionHandling;
        insecureSessionHandling.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionHandling.Id });
        vulnManager.Add(insecureSessionHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureTlsValidation = VulnTemplatesMastgEN.InsecureTlsValidation;
        insecureTlsValidation.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureTlsValidation.Id });
        vulnManager.Add(insecureTlsValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureClipboardUsage = VulnTemplatesMastgEN.InsecureClipboardUsage;
        insecureClipboardUsage.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureClipboardUsage.Id });
        vulnManager.Add(insecureClipboardUsage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataCaching = VulnTemplatesMastgEN.InsecureDataCaching;
        insecureDataCaching.VulnCwes.Add(new VulnCwe { CweId = 524, VulnId = insecureDataCaching.Id });
        vulnManager.Add(insecureDataCaching);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureBackupHandling = VulnTemplatesMastgEN.InsecureBackupHandling;
        insecureBackupHandling.VulnCwes.Add(new VulnCwe { CweId = 530, VulnId = insecureBackupHandling.Id });
        vulnManager.Add(insecureBackupHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientInputValidation = VulnTemplatesMastgEN.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureJailbreakRootDetection = VulnTemplatesMastgEN.InsecureJailbreakRootDetection;
        insecureJailbreakRootDetection.VulnCwes.Add(new VulnCwe { CweId = 919, VulnId = insecureJailbreakRootDetection.Id });
        vulnManager.Add(insecureJailbreakRootDetection);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCodeObfuscation = VulnTemplatesMastgEN.InsecureCodeObfuscation;
        insecureCodeObfuscation.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureCodeObfuscation.Id });
        vulnManager.Add(insecureCodeObfuscation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureRuntimeIntegrityChecks = VulnTemplatesMastgEN.InsecureRuntimeIntegrityChecks;
        insecureRuntimeIntegrityChecks.VulnCwes.Add(new VulnCwe { CweId = 693, VulnId = insecureRuntimeIntegrityChecks.Id });
        vulnManager.Add(insecureRuntimeIntegrityChecks);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAppPackaging = VulnTemplatesMastgEN.InsecureAppPackaging;
        insecureAppPackaging.VulnCwes.Add(new VulnCwe { CweId = 490, VulnId = insecureAppPackaging.Id });
        vulnManager.Add(insecureAppPackaging);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureMemoryManagement = VulnTemplatesMastgEN.InsecureMemoryManagement;
        insecureMemoryManagement.VulnCwes.Add(new VulnCwe { CweId = 316, VulnId = insecureMemoryManagement.Id });
        vulnManager.Add(insecureMemoryManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureComponentUpgrade = VulnTemplatesMastgEN.InsecureComponentUpgrade;
        insecureComponentUpgrade.VulnCwes.Add(new VulnCwe { CweId = 494, VulnId = insecureComponentUpgrade.Id });
        vulnManager.Add(insecureComponentUpgrade);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataResidency = VulnTemplatesMastgEN.InsecureDataResidency;
        insecureDataResidency.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insecureDataResidency.Id });
        vulnManager.Add(insecureDataResidency);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCloudSyncmechanism = VulnTemplatesMastgEN.InsecureCloudSyncMechanism;
        insecureCloudSyncmechanism.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insecureCloudSyncmechanism.Id });
        vulnManager.Add(insecureCloudSyncmechanism);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln vulnerableThirdPartyLibraries = VulnTemplatesMastgEN.VulnerableThirdPartyLibrary;
        vulnerableThirdPartyLibraries.VulnCwes.Add(new VulnCwe { CweId = 1035, VulnId = vulnerableThirdPartyLibraries.Id });
        vulnManager.Add(vulnerableThirdPartyLibraries);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataExfiltration = VulnTemplatesMastgEN.InsecureDataExfiltration;
        insecureDataExfiltration.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureDataExfiltration.Id });
        vulnManager.Add(insecureDataExfiltration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAPIVersioning = VulnTemplatesMastgEN.InsecureAPIVersioning;
        insecureAPIVersioning.VulnCwes.Add(new VulnCwe { CweId = 330, VulnId = insecureAPIVersioning.Id });
        vulnManager.Add(insecureAPIVersioning);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureQRCodeHandling = VulnTemplatesMastgEN.InsecureQRCodeHandling;
        insecureQRCodeHandling.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insecureQRCodeHandling.Id });
        vulnManager.Add(insecureQRCodeHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureNFCImplementation = VulnTemplatesMastgEN.InsecureNFCImplementation;
        insecureNFCImplementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureNFCImplementation.Id });
        vulnManager.Add(insecureNFCImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureARImpementation = VulnTemplatesMastgEN.InsecureARImplementation;
        insecureARImpementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureARImpementation.Id });
        vulnManager.Add(insecureARImpementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureIoTImplementation = VulnTemplatesMastgEN.InsecureIoTIntegration;
        insecureIoTImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureIoTImplementation.Id });
        vulnManager.Add(insecureIoTImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecurePushNotification = VulnTemplatesMastgEN.InsecurePushNotifications;
        insecurePushNotification.VulnCwes.Add(new VulnCwe { CweId = 223, VulnId = insecurePushNotification.Id });
        vulnManager.Add(insecurePushNotification);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAppCloning = VulnTemplatesMastgEN.InsecureAppCloning;
        insecureAppCloning.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureAppCloning.Id });
        vulnManager.Add(insecureAppCloning);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureScreenOverlay = VulnTemplatesMastgEN.InsecureScreenOverlay;
        insecureScreenOverlay.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = insecureScreenOverlay.Id });
        vulnManager.Add(insecureScreenOverlay);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAppWidget = VulnTemplatesMastgEN.InsecureAppWidget;
        insecureAppWidget.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureAppWidget.Id });
        vulnManager.Add(insecureAppWidget);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureEdgeComputingIntegration = VulnTemplatesMastgEN.InsecureEdgeComputingIntegration;
        insecureEdgeComputingIntegration.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureEdgeComputingIntegration.Id });
        vulnManager.Add(insecureEdgeComputingIntegration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAIMLImplementation = VulnTemplatesMastgEN.InsecureAIMLImplementation;
        insecureAIMLImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureAIMLImplementation.Id });
        vulnManager.Add(insecureAIMLImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureQuantumResistantCrypto = VulnTemplatesMastgEN.InsecureQuantumResistantCrypto;
        insecureQuantumResistantCrypto.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureQuantumResistantCrypto.Id });
        vulnManager.Add(insecureQuantumResistantCrypto);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureVoiceIntegration = VulnTemplatesMastgEN.InsecureVoiceUIIntegration;
        insecureVoiceIntegration.VulnCwes.Add(new VulnCwe { CweId = 350, VulnId = insecureVoiceIntegration.Id });
        vulnManager.Add(insecureVoiceIntegration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureMultiDeviceSynchronization= VulnTemplatesMastgEN.InsecureMultiDeviceSynchronization;
        insecureMultiDeviceSynchronization.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureMultiDeviceSynchronization.Id });
        vulnManager.Add(insecureMultiDeviceSynchronization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureBlockchainIntegration = VulnTemplatesMastgEN.InsecureBlockchainIntegration;
        insecureBlockchainIntegration.VulnCwes.Add(new VulnCwe { CweId = 320, VulnId = insecureBlockchainIntegration.Id });
        vulnManager.Add(insecureBlockchainIntegration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureKeychainKeystore = VulnTemplatesMastgEN.InsecureKeychainKeystore;
        insecureKeychainKeystore.VulnCwes.Add(new VulnCwe { CweId = 522, VulnId = insecureKeychainKeystore.Id });
        vulnManager.Add(insecureKeychainKeystore);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureRandomNumberGeneration = VulnTemplatesMastgEN.InsecureRandomNumberGeneration;
        insecureRandomNumberGeneration.VulnCwes.Add(new VulnCwe { CweId = 338, VulnId = insecureRandomNumberGeneration.Id });
        vulnManager.Add(insecureRandomNumberGeneration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureSSOImplementation = VulnTemplatesMastgEN.InsecureSSOImplementation;
        insecureSSOImplementation.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureSSOImplementation.Id });
        vulnManager.Add(insecureSSOImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureVPNUsage = VulnTemplatesMastgEN.InsecureVPNUsage;
        insecureVPNUsage.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureVPNUsage.Id });
        vulnManager.Add(insecureVPNUsage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCustomURLScheme = VulnTemplatesMastgEN.InsecureCustomURLScheme;
        insecureCustomURLScheme.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureCustomURLScheme.Id });
        vulnManager.Add(insecureCustomURLScheme);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln timeOfCheckToTimeOfUse = VulnTemplatesMastgEN.TimeOfCheckToTimeOfUse;
        timeOfCheckToTimeOfUse.VulnCwes.Add(new VulnCwe { CweId = 367, VulnId = timeOfCheckToTimeOfUse.Id });
        vulnManager.Add(timeOfCheckToTimeOfUse);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAntiDebugging = VulnTemplatesMastgEN.InsecureAntiDebugging;
        insecureAntiDebugging.VulnCwes.Add(new VulnCwe { CweId = 388, VulnId = insecureAntiDebugging.Id });
        vulnManager.Add(insecureAntiDebugging);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln overPrivilegedApp = VulnTemplatesMastgEN.OverPrivilegedApplication;
        overPrivilegedApp.VulnCwes.Add(new VulnCwe { CweId = 250, VulnId = overPrivilegedApp.Id });
        vulnManager.Add(overPrivilegedApp);
        await vulnManager.Context.SaveChangesAsync();
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
        Vuln insecureDataStorage = VulnTemplatesMastgES.InsecureDataStorage;
        insecureDataStorage.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insecureDataStorage.Id });
        vulnManager.Add(insecureDataStorage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCryptography = VulnTemplatesMastgES.InsecureCryptography;
        insecureCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureCryptography.Id });
        vulnManager.Add(insecureCryptography);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAuthentication = VulnTemplatesMastgES.InsecureAuthentication;
        insecureAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureAuthentication.Id });
        vulnManager.Add(insecureAuthentication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureNetworkCommunication = VulnTemplatesMastgES.InsecureNetworkCommunication;
        insecureNetworkCommunication.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureNetworkCommunication.Id });
        vulnManager.Add(insecureNetworkCommunication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln privacyViolations = VulnTemplatesMastgES.PrivacyViolation;
        privacyViolations.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = privacyViolations.Id });
        vulnManager.Add(privacyViolations);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataLeakage = VulnTemplatesMastgES.InsecureDataLeakage;
        insecureDataLeakage.VulnCwes.Add(new VulnCwe { CweId = 532, VulnId = insecureDataLeakage.Id });
        vulnManager.Add(insecureDataLeakage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureKeyManagement = VulnTemplatesMastgES.InsecureKeyManagement;
        insecureKeyManagement.VulnCwes.Add(new VulnCwe { CweId = 321, VulnId = insecureKeyManagement.Id });
        vulnManager.Add(insecureKeyManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureLocalAuthentication = VulnTemplatesMastgES.InsecureLocalAuthentication;
        insecureLocalAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureLocalAuthentication.Id });
        vulnManager.Add(insecureLocalAuthentication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCertificatePinning = VulnTemplatesMastgES.InsecureCertificatePinning;
        insecureCertificatePinning.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureCertificatePinning.Id });
        vulnManager.Add(insecureCertificatePinning);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureIPC = VulnTemplatesMastgES.InsecureIPC;
        insecureIPC.VulnCwes.Add(new VulnCwe { CweId = 927, VulnId = insecureIPC.Id });
        vulnManager.Add(insecureIPC);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureWebview = VulnTemplatesMastgES.InsecureWebView;
        insecureWebview.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureWebview.Id });
        vulnManager.Add(insecureWebview);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDeepLinking = VulnTemplatesMastgES.InsecureDeepLinking;
        insecureDeepLinking.VulnCwes.Add(new VulnCwe { CweId = 939, VulnId = insecureDeepLinking.Id });
        vulnManager.Add(insecureDeepLinking);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureSessionHandling = VulnTemplatesMastgES.InsecureSessionHandling;
        insecureSessionHandling.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionHandling.Id });
        vulnManager.Add(insecureSessionHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureTlsValidation = VulnTemplatesMastgES.InsecureTlsValidation;
        insecureTlsValidation.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureTlsValidation.Id });
        vulnManager.Add(insecureTlsValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureClipboardUsage = VulnTemplatesMastgES.InsecureClipboardUsage;
        insecureClipboardUsage.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureClipboardUsage.Id });
        vulnManager.Add(insecureClipboardUsage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataCaching = VulnTemplatesMastgES.InsecureDataCaching;
        insecureDataCaching.VulnCwes.Add(new VulnCwe { CweId = 524, VulnId = insecureDataCaching.Id });
        vulnManager.Add(insecureDataCaching);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureBackupHandling = VulnTemplatesMastgES.InsecureBackupHandling;
        insecureBackupHandling.VulnCwes.Add(new VulnCwe { CweId = 530, VulnId = insecureBackupHandling.Id });
        vulnManager.Add(insecureBackupHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientInputValidation = VulnTemplatesMastgES.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureJailbreakRootDetection = VulnTemplatesMastgES.InsecureJailbreakRootDetection;
        insecureJailbreakRootDetection.VulnCwes.Add(new VulnCwe { CweId = 919, VulnId = insecureJailbreakRootDetection.Id });
        vulnManager.Add(insecureJailbreakRootDetection);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCodeObfuscation = VulnTemplatesMastgES.InsecureCodeObfuscation;
        insecureCodeObfuscation.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureCodeObfuscation.Id });
        vulnManager.Add(insecureCodeObfuscation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureRuntimeIntegrityChecks = VulnTemplatesMastgES.InsecureRuntimeIntegrityChecks;
        insecureRuntimeIntegrityChecks.VulnCwes.Add(new VulnCwe { CweId = 693, VulnId = insecureRuntimeIntegrityChecks.Id });
        vulnManager.Add(insecureRuntimeIntegrityChecks);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAppPackaging = VulnTemplatesMastgES.InsecureAppPackaging;
        insecureAppPackaging.VulnCwes.Add(new VulnCwe { CweId = 490, VulnId = insecureAppPackaging.Id });
        vulnManager.Add(insecureAppPackaging);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureMemoryManagement = VulnTemplatesMastgES.InsecureMemoryManagement;
        insecureMemoryManagement.VulnCwes.Add(new VulnCwe { CweId = 316, VulnId = insecureMemoryManagement.Id });
        vulnManager.Add(insecureMemoryManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureComponentUpgrade = VulnTemplatesMastgES.InsecureComponentUpgrade;
        insecureComponentUpgrade.VulnCwes.Add(new VulnCwe { CweId = 494, VulnId = insecureComponentUpgrade.Id });
        vulnManager.Add(insecureComponentUpgrade);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataResidency = VulnTemplatesMastgES.InsecureDataResidency;
        insecureDataResidency.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insecureDataResidency.Id });
        vulnManager.Add(insecureDataResidency);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCloudSyncmechanism = VulnTemplatesMastgES.InsecureCloudSyncMechanism;
        insecureCloudSyncmechanism.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insecureCloudSyncmechanism.Id });
        vulnManager.Add(insecureCloudSyncmechanism);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln vulnerableThirdPartyLibraries = VulnTemplatesMastgES.VulnerableThirdPartyLibrary;
        vulnerableThirdPartyLibraries.VulnCwes.Add(new VulnCwe { CweId = 1035, VulnId = vulnerableThirdPartyLibraries.Id });
        vulnManager.Add(vulnerableThirdPartyLibraries);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataExfiltration = VulnTemplatesMastgES.InsecureDataExfiltration;
        insecureDataExfiltration.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureDataExfiltration.Id });
        vulnManager.Add(insecureDataExfiltration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAPIVersioning = VulnTemplatesMastgES.InsecureAPIVersioning;
        insecureAPIVersioning.VulnCwes.Add(new VulnCwe { CweId = 330, VulnId = insecureAPIVersioning.Id });
        vulnManager.Add(insecureAPIVersioning);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureQRCodeHandling = VulnTemplatesMastgES.InsecureQRCodeHandling;
        insecureQRCodeHandling.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insecureQRCodeHandling.Id });
        vulnManager.Add(insecureQRCodeHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureNFCImplementation = VulnTemplatesMastgES.InsecureNFCImplementation;
        insecureNFCImplementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureNFCImplementation.Id });
        vulnManager.Add(insecureNFCImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureARImpementation = VulnTemplatesMastgES.InsecureARImplementation;
        insecureARImpementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureARImpementation.Id });
        vulnManager.Add(insecureARImpementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureIoTImplementation = VulnTemplatesMastgES.InsecureIoTIntegration;
        insecureIoTImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureIoTImplementation.Id });
        vulnManager.Add(insecureIoTImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecurePushNotification = VulnTemplatesMastgES.InsecurePushNotifications;
        insecurePushNotification.VulnCwes.Add(new VulnCwe { CweId = 223, VulnId = insecurePushNotification.Id });
        vulnManager.Add(insecurePushNotification);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAppCloning = VulnTemplatesMastgES.InsecureAppCloning;
        insecureAppCloning.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureAppCloning.Id });
        vulnManager.Add(insecureAppCloning);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureScreenOverlay = VulnTemplatesMastgES.InsecureScreenOverlay;
        insecureScreenOverlay.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = insecureScreenOverlay.Id });
        vulnManager.Add(insecureScreenOverlay);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAppWidget = VulnTemplatesMastgES.InsecureAppWidget;
        insecureAppWidget.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureAppWidget.Id });
        vulnManager.Add(insecureAppWidget);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureEdgeComputingIntegration = VulnTemplatesMastgES.InsecureEdgeComputingIntegration;
        insecureEdgeComputingIntegration.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureEdgeComputingIntegration.Id });
        vulnManager.Add(insecureEdgeComputingIntegration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAIMLImplementation = VulnTemplatesMastgES.InsecureAIMLImplementation;
        insecureAIMLImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureAIMLImplementation.Id });
        vulnManager.Add(insecureAIMLImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureQuantumResistantCrypto = VulnTemplatesMastgES.InsecureQuantumResistantCrypto;
        insecureQuantumResistantCrypto.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureQuantumResistantCrypto.Id });
        vulnManager.Add(insecureQuantumResistantCrypto);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureVoiceIntegration = VulnTemplatesMastgES.InsecureVoiceUIIntegration;
        insecureVoiceIntegration.VulnCwes.Add(new VulnCwe { CweId = 350, VulnId = insecureVoiceIntegration.Id });
        vulnManager.Add(insecureVoiceIntegration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureMultiDeviceSynchronization= VulnTemplatesMastgES.InsecureMultiDeviceSynchronization;
        insecureMultiDeviceSynchronization.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureMultiDeviceSynchronization.Id });
        vulnManager.Add(insecureMultiDeviceSynchronization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureBlockchainIntegration = VulnTemplatesMastgES.InsecureBlockchainIntegration;
        insecureBlockchainIntegration.VulnCwes.Add(new VulnCwe { CweId = 320, VulnId = insecureBlockchainIntegration.Id });
        vulnManager.Add(insecureBlockchainIntegration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureKeychainKeystore = VulnTemplatesMastgES.InsecureKeychainKeystore;
        insecureKeychainKeystore.VulnCwes.Add(new VulnCwe { CweId = 522, VulnId = insecureKeychainKeystore.Id });
        vulnManager.Add(insecureKeychainKeystore);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureRandomNumberGeneration = VulnTemplatesMastgES.InsecureRandomNumberGeneration;
        insecureRandomNumberGeneration.VulnCwes.Add(new VulnCwe { CweId = 338, VulnId = insecureRandomNumberGeneration.Id });
        vulnManager.Add(insecureRandomNumberGeneration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureSSOImplementation = VulnTemplatesMastgES.InsecureSSOImplementation;
        insecureSSOImplementation.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureSSOImplementation.Id });
        vulnManager.Add(insecureSSOImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureVPNUsage = VulnTemplatesMastgES.InsecureVPNUsage;
        insecureVPNUsage.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureVPNUsage.Id });
        vulnManager.Add(insecureVPNUsage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCustomURLScheme = VulnTemplatesMastgES.InsecureCustomURLScheme;
        insecureCustomURLScheme.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureCustomURLScheme.Id });
        vulnManager.Add(insecureCustomURLScheme);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln timeOfCheckToTimeOfUse = VulnTemplatesMastgES.TimeOfCheckToTimeOfUse;
        timeOfCheckToTimeOfUse.VulnCwes.Add(new VulnCwe { CweId = 367, VulnId = timeOfCheckToTimeOfUse.Id });
        vulnManager.Add(timeOfCheckToTimeOfUse);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAntiDebugging = VulnTemplatesMastgES.InsecureAntiDebugging;
        insecureAntiDebugging.VulnCwes.Add(new VulnCwe { CweId = 388, VulnId = insecureAntiDebugging.Id });
        vulnManager.Add(insecureAntiDebugging);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln overPrivilegedApp = VulnTemplatesMastgES.OverPrivilegedApplication;
        overPrivilegedApp.VulnCwes.Add(new VulnCwe { CweId = 250, VulnId = overPrivilegedApp.Id });
        vulnManager.Add(overPrivilegedApp);
        await vulnManager.Context.SaveChangesAsync();
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
        Vuln insecureDataStorage = VulnTemplatesMastgPT.InsecureDataStorage;
        insecureDataStorage.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insecureDataStorage.Id });
        vulnManager.Add(insecureDataStorage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCryptography = VulnTemplatesMastgPT.InsecureCryptography;
        insecureCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureCryptography.Id });
        vulnManager.Add(insecureCryptography);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAuthentication = VulnTemplatesMastgPT.InsecureAuthentication;
        insecureAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureAuthentication.Id });
        vulnManager.Add(insecureAuthentication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureNetworkCommunication = VulnTemplatesMastgPT.InsecureNetworkCommunication;
        insecureNetworkCommunication.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureNetworkCommunication.Id });
        vulnManager.Add(insecureNetworkCommunication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln privacyViolations = VulnTemplatesMastgPT.PrivacyViolation;
        privacyViolations.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = privacyViolations.Id });
        vulnManager.Add(privacyViolations);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataLeakage = VulnTemplatesMastgPT.InsecureDataLeakage;
        insecureDataLeakage.VulnCwes.Add(new VulnCwe { CweId = 532, VulnId = insecureDataLeakage.Id });
        vulnManager.Add(insecureDataLeakage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureKeyManagement = VulnTemplatesMastgPT.InsecureKeyManagement;
        insecureKeyManagement.VulnCwes.Add(new VulnCwe { CweId = 321, VulnId = insecureKeyManagement.Id });
        vulnManager.Add(insecureKeyManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureLocalAuthentication = VulnTemplatesMastgPT.InsecureLocalAuthentication;
        insecureLocalAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureLocalAuthentication.Id });
        vulnManager.Add(insecureLocalAuthentication);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCertificatePinning = VulnTemplatesMastgPT.InsecureCertificatePinning;
        insecureCertificatePinning.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureCertificatePinning.Id });
        vulnManager.Add(insecureCertificatePinning);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureIPC = VulnTemplatesMastgPT.InsecureIPC;
        insecureIPC.VulnCwes.Add(new VulnCwe { CweId = 927, VulnId = insecureIPC.Id });
        vulnManager.Add(insecureIPC);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureWebview = VulnTemplatesMastgPT.InsecureWebView;
        insecureWebview.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureWebview.Id });
        vulnManager.Add(insecureWebview);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDeepLinking = VulnTemplatesMastgPT.InsecureDeepLinking;
        insecureDeepLinking.VulnCwes.Add(new VulnCwe { CweId = 939, VulnId = insecureDeepLinking.Id });
        vulnManager.Add(insecureDeepLinking);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureSessionHandling = VulnTemplatesMastgPT.InsecureSessionHandling;
        insecureSessionHandling.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionHandling.Id });
        vulnManager.Add(insecureSessionHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureTlsValidation = VulnTemplatesMastgPT.InsecureTlsValidation;
        insecureTlsValidation.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureTlsValidation.Id });
        vulnManager.Add(insecureTlsValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureClipboardUsage = VulnTemplatesMastgPT.InsecureClipboardUsage;
        insecureClipboardUsage.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureClipboardUsage.Id });
        vulnManager.Add(insecureClipboardUsage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataCaching = VulnTemplatesMastgPT.InsecureDataCaching;
        insecureDataCaching.VulnCwes.Add(new VulnCwe { CweId = 524, VulnId = insecureDataCaching.Id });
        vulnManager.Add(insecureDataCaching);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureBackupHandling = VulnTemplatesMastgPT.InsecureBackupHandling;
        insecureBackupHandling.VulnCwes.Add(new VulnCwe { CweId = 530, VulnId = insecureBackupHandling.Id });
        vulnManager.Add(insecureBackupHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insufficientInputValidation = VulnTemplatesMastgPT.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureJailbreakRootDetection = VulnTemplatesMastgPT.InsecureJailbreakRootDetection;
        insecureJailbreakRootDetection.VulnCwes.Add(new VulnCwe { CweId = 919, VulnId = insecureJailbreakRootDetection.Id });
        vulnManager.Add(insecureJailbreakRootDetection);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCodeObfuscation = VulnTemplatesMastgPT.InsecureCodeObfuscation;
        insecureCodeObfuscation.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureCodeObfuscation.Id });
        vulnManager.Add(insecureCodeObfuscation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureRuntimeIntegrityChecks = VulnTemplatesMastgPT.InsecureRuntimeIntegrityChecks;
        insecureRuntimeIntegrityChecks.VulnCwes.Add(new VulnCwe { CweId = 693, VulnId = insecureRuntimeIntegrityChecks.Id });
        vulnManager.Add(insecureRuntimeIntegrityChecks);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAppPackaging = VulnTemplatesMastgPT.InsecureAppPackaging;
        insecureAppPackaging.VulnCwes.Add(new VulnCwe { CweId = 490, VulnId = insecureAppPackaging.Id });
        vulnManager.Add(insecureAppPackaging);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureMemoryManagement = VulnTemplatesMastgPT.InsecureMemoryManagement;
        insecureMemoryManagement.VulnCwes.Add(new VulnCwe { CweId = 316, VulnId = insecureMemoryManagement.Id });
        vulnManager.Add(insecureMemoryManagement);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureComponentUpgrade = VulnTemplatesMastgPT.InsecureComponentUpgrade;
        insecureComponentUpgrade.VulnCwes.Add(new VulnCwe { CweId = 494, VulnId = insecureComponentUpgrade.Id });
        vulnManager.Add(insecureComponentUpgrade);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataResidency = VulnTemplatesMastgPT.InsecureDataResidency;
        insecureDataResidency.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insecureDataResidency.Id });
        vulnManager.Add(insecureDataResidency);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCloudSyncmechanism = VulnTemplatesMastgPT.InsecureCloudSyncMechanism;
        insecureCloudSyncmechanism.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insecureCloudSyncmechanism.Id });
        vulnManager.Add(insecureCloudSyncmechanism);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln vulnerableThirdPartyLibraries = VulnTemplatesMastgPT.VulnerableThirdPartyLibrary;
        vulnerableThirdPartyLibraries.VulnCwes.Add(new VulnCwe { CweId = 1035, VulnId = vulnerableThirdPartyLibraries.Id });
        vulnManager.Add(vulnerableThirdPartyLibraries);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureDataExfiltration = VulnTemplatesMastgPT.InsecureDataExfiltration;
        insecureDataExfiltration.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureDataExfiltration.Id });
        vulnManager.Add(insecureDataExfiltration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAPIVersioning = VulnTemplatesMastgPT.InsecureAPIVersioning;
        insecureAPIVersioning.VulnCwes.Add(new VulnCwe { CweId = 330, VulnId = insecureAPIVersioning.Id });
        vulnManager.Add(insecureAPIVersioning);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureQRCodeHandling = VulnTemplatesMastgPT.InsecureQRCodeHandling;
        insecureQRCodeHandling.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insecureQRCodeHandling.Id });
        vulnManager.Add(insecureQRCodeHandling);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureNFCImplementation = VulnTemplatesMastgPT.InsecureNFCImplementation;
        insecureNFCImplementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureNFCImplementation.Id });
        vulnManager.Add(insecureNFCImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureARImpementation = VulnTemplatesMastgPT.InsecureARImplementation;
        insecureARImpementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureARImpementation.Id });
        vulnManager.Add(insecureARImpementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureIoTImplementation = VulnTemplatesMastgPT.InsecureIoTIntegration;
        insecureIoTImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureIoTImplementation.Id });
        vulnManager.Add(insecureIoTImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecurePushNotification = VulnTemplatesMastgPT.InsecurePushNotifications;
        insecurePushNotification.VulnCwes.Add(new VulnCwe { CweId = 223, VulnId = insecurePushNotification.Id });
        vulnManager.Add(insecurePushNotification);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAppCloning = VulnTemplatesMastgPT.InsecureAppCloning;
        insecureAppCloning.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureAppCloning.Id });
        vulnManager.Add(insecureAppCloning);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureScreenOverlay = VulnTemplatesMastgPT.InsecureScreenOverlay;
        insecureScreenOverlay.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = insecureScreenOverlay.Id });
        vulnManager.Add(insecureScreenOverlay);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAppWidget = VulnTemplatesMastgPT.InsecureAppWidget;
        insecureAppWidget.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureAppWidget.Id });
        vulnManager.Add(insecureAppWidget);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureEdgeComputingIntegration = VulnTemplatesMastgPT.InsecureEdgeComputingIntegration;
        insecureEdgeComputingIntegration.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureEdgeComputingIntegration.Id });
        vulnManager.Add(insecureEdgeComputingIntegration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAIMLImplementation = VulnTemplatesMastgPT.InsecureAIMLImplementation;
        insecureAIMLImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureAIMLImplementation.Id });
        vulnManager.Add(insecureAIMLImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureQuantumResistantCrypto = VulnTemplatesMastgPT.InsecureQuantumResistantCrypto;
        insecureQuantumResistantCrypto.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureQuantumResistantCrypto.Id });
        vulnManager.Add(insecureQuantumResistantCrypto);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureVoiceIntegration = VulnTemplatesMastgPT.InsecureVoiceUIIntegration;
        insecureVoiceIntegration.VulnCwes.Add(new VulnCwe { CweId = 350, VulnId = insecureVoiceIntegration.Id });
        vulnManager.Add(insecureVoiceIntegration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureMultiDeviceSynchronization= VulnTemplatesMastgPT.InsecureMultiDeviceSynchronization;
        insecureMultiDeviceSynchronization.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureMultiDeviceSynchronization.Id });
        vulnManager.Add(insecureMultiDeviceSynchronization);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureBlockchainIntegration = VulnTemplatesMastgPT.InsecureBlockchainIntegration;
        insecureBlockchainIntegration.VulnCwes.Add(new VulnCwe { CweId = 320, VulnId = insecureBlockchainIntegration.Id });
        vulnManager.Add(insecureBlockchainIntegration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureKeychainKeystore = VulnTemplatesMastgPT.InsecureKeychainKeystore;
        insecureKeychainKeystore.VulnCwes.Add(new VulnCwe { CweId = 522, VulnId = insecureKeychainKeystore.Id });
        vulnManager.Add(insecureKeychainKeystore);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureRandomNumberGeneration = VulnTemplatesMastgPT.InsecureRandomNumberGeneration;
        insecureRandomNumberGeneration.VulnCwes.Add(new VulnCwe { CweId = 338, VulnId = insecureRandomNumberGeneration.Id });
        vulnManager.Add(insecureRandomNumberGeneration);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureSSOImplementation = VulnTemplatesMastgPT.InsecureSSOImplementation;
        insecureSSOImplementation.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureSSOImplementation.Id });
        vulnManager.Add(insecureSSOImplementation);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureVPNUsage = VulnTemplatesMastgPT.InsecureVPNUsage;
        insecureVPNUsage.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureVPNUsage.Id });
        vulnManager.Add(insecureVPNUsage);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureCustomURLScheme = VulnTemplatesMastgPT.InsecureCustomURLScheme;
        insecureCustomURLScheme.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureCustomURLScheme.Id });
        vulnManager.Add(insecureCustomURLScheme);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln timeOfCheckToTimeOfUse = VulnTemplatesMastgPT.TimeOfCheckToTimeOfUse;
        timeOfCheckToTimeOfUse.VulnCwes.Add(new VulnCwe { CweId = 367, VulnId = timeOfCheckToTimeOfUse.Id });
        vulnManager.Add(timeOfCheckToTimeOfUse);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln insecureAntiDebugging = VulnTemplatesMastgPT.InsecureAntiDebugging;
        insecureAntiDebugging.VulnCwes.Add(new VulnCwe { CweId = 388, VulnId = insecureAntiDebugging.Id });
        vulnManager.Add(insecureAntiDebugging);
        await vulnManager.Context.SaveChangesAsync();
        
        Vuln overPrivilegedApp = VulnTemplatesMastgPT.OverPrivilegedApplication;
        overPrivilegedApp.VulnCwes.Add(new VulnCwe { CweId = 250, VulnId = overPrivilegedApp.Id });
        vulnManager.Add(overPrivilegedApp);
        await vulnManager.Context.SaveChangesAsync();
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