using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.IFR.VulnTemplates;

public class VulnTemplatesLoadService : IVulnTemplatesLoadService
{
    private IVulnManager vulnManager = null;
    private IVulnCweManager vulnCweManager = null;
    private IVulnCategoryManager vulnCategoryManager = null;
    private readonly ILogger<VulnTemplatesLoadService> _logger = null;

    public VulnTemplatesLoadService(IVulnManager vulnManager, IVulnCweManager vulnCweManager,
        IVulnCategoryManager vulnCategoryManager, ILogger<VulnTemplatesLoadService> logger)
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
        // Add Kerberoasting Vulnerability
        Vuln kerberoastingVuln = VulnTemplatesGeneralEN.KerberoastingVulnerability;
        vulnManager.Add(kerberoastingVuln);

// Add LLMNR Poisoning
        Vuln llmnrVuln = VulnTemplatesGeneralEN.LLMNRPoisoning;
        vulnManager.Add(llmnrVuln);

// Add Local Admin Password Reuse
        Vuln localAdminVuln = VulnTemplatesGeneralEN.LocalAdminPasswordReuse;
        vulnManager.Add(localAdminVuln);

// Add Unquoted Service Path
        Vuln unquotedServiceVuln = VulnTemplatesGeneralEN.UnquotedServicePath;
        vulnManager.Add(unquotedServiceVuln);

// Add SMB Signing Issues
        Vuln smbSigningVuln = VulnTemplatesGeneralEN.SMBSigningDisabled;
        vulnManager.Add(smbSigningVuln);

// Add Excessive Domain Admin Rights
        Vuln domainAdminVuln = VulnTemplatesGeneralEN.ExcessiveDomainAdminRights;
        vulnManager.Add(domainAdminVuln);

// Add GPP Password Exposure
        Vuln gppPasswordVuln = VulnTemplatesGeneralEN.GPPPasswordExposure;
        vulnManager.Add(gppPasswordVuln);

// Add Weak Service Account Permissions
        Vuln weakServiceAccountVuln = VulnTemplatesGeneralEN.WeakServiceAccountPermissions;
        vulnManager.Add(weakServiceAccountVuln);

// Add Clear Text Protocols
        Vuln clearTextVuln = VulnTemplatesGeneralEN.ClearTextProtocols;
        vulnManager.Add(clearTextVuln);

// Add Insecure LDAP Binding
        Vuln ldapBindingVuln = VulnTemplatesGeneralEN.InsecureLDAPBinding;
        vulnManager.Add(ldapBindingVuln);

// Add Weak TLS Configuration
        Vuln weakTlsVuln = VulnTemplatesGeneralEN.WeakTLSConfiguration;
        vulnManager.Add(weakTlsVuln);

// Add Default Credentials
        Vuln defaultCredsVuln = VulnTemplatesGeneralEN.DefaultCredentials;
        vulnManager.Add(defaultCredsVuln);

// Add Missing Security Updates
        Vuln missingUpdatesVuln = VulnTemplatesGeneralEN.MissingSecurityUpdates;
        vulnManager.Add(missingUpdatesVuln);

// Add Unsecured Shares
        Vuln unsecuredSharesVuln = VulnTemplatesGeneralEN.UnsecuredShares;
        vulnManager.Add(unsecuredSharesVuln);

// Add WinRM Misconfiguration
        Vuln winRmVuln = VulnTemplatesGeneralEN.WinRMMisconfiguration;
        vulnManager.Add(winRmVuln);

// Add Print Spooler Vulnerabilities
        Vuln printSpoolerVuln = VulnTemplatesGeneralEN.PrintSpoolerVulnerable;
        vulnManager.Add(printSpoolerVuln);

// Add Cached Domain Credentials
        Vuln cachedCredsVuln = VulnTemplatesGeneralEN.CachedDomainCredentials;
        vulnManager.Add(cachedCredsVuln);

// Add BitLocker Misconfiguration
        Vuln bitLockerVuln = VulnTemplatesGeneralEN.BitLockerMisconfiguration;
        vulnManager.Add(bitLockerVuln);

// Add WSUS Misconfiguration
        Vuln wsusVuln = VulnTemplatesGeneralEN.WSUSMisconfiguration;
        vulnManager.Add(wsusVuln);

// Add IPv6 Security Issues
        Vuln ipv6Vuln = VulnTemplatesGeneralEN.IPv6SecurityIssues;
        vulnManager.Add(ipv6Vuln);

// Add PowerShell Logging Gaps
        Vuln powerShellLoggingVuln = VulnTemplatesGeneralEN.PowerShellLoggingGaps;
        vulnManager.Add(powerShellLoggingVuln);

// Add DnsAdmins Abuse
        Vuln dnsAdminsVuln = VulnTemplatesGeneralEN.DnsAdminsAbuse;
        vulnManager.Add(dnsAdminsVuln);

// Add Exchange Misconfiguration
        Vuln exchangeVuln = VulnTemplatesGeneralEN.ExchangeMisconfiguration;
        vulnManager.Add(exchangeVuln);

// Add Backup System Access
        Vuln backupSystemVuln = VulnTemplatesGeneralEN.BackupSystemAccess;
        vulnManager.Add(backupSystemVuln);

// Add Certificate Template Vulnerabilities
        Vuln certTemplateVuln = VulnTemplatesGeneralEN.CertificateTemplateVulns;
        vulnManager.Add(certTemplateVuln);

// Add SQL Server Misconfigurations
        Vuln sqlServerVuln = VulnTemplatesGeneralEN.SQLServerMisconfigurations;
        vulnManager.Add(sqlServerVuln);

// Add RDP Security Issues
        Vuln rdpSecurityVuln = VulnTemplatesGeneralEN.RDPSecurityIssues;
        vulnManager.Add(rdpSecurityVuln);

// Add Excessive User Rights
        Vuln excessiveUserRightsVuln = VulnTemplatesGeneralEN.ExcessiveUserRights;
        vulnManager.Add(excessiveUserRightsVuln);

// Add Shadow Copy Abuse
        Vuln shadowCopyVuln = VulnTemplatesGeneralEN.ShadowCopyAbuse;
        vulnManager.Add(shadowCopyVuln);

// Add Service Principal Misconfiguration
        Vuln servicePrincipalVuln = VulnTemplatesGeneralEN.ServicePrincipalMisconfig;
        vulnManager.Add(servicePrincipalVuln);

// Add NTP Server Issues
        Vuln ntpServerVuln = VulnTemplatesGeneralEN.NTPServerIssues;
        vulnManager.Add(ntpServerVuln);

// Add Linked Server Vulnerabilities
        Vuln linkedServerVuln = VulnTemplatesGeneralEN.LinkedServerVulns;
        vulnManager.Add(linkedServerVuln);

// Add Defender Exclusions
        Vuln defenderExclusionsVuln = VulnTemplatesGeneralEN.DefenderExclusions;
        vulnManager.Add(defenderExclusionsVuln);

// Add Domain Trust Issues
        Vuln domainTrustVuln = VulnTemplatesGeneralEN.DomainTrustIssues;
        vulnManager.Add(domainTrustVuln);

// Add Hyper-V Security
        Vuln hyperVVuln = VulnTemplatesGeneralEN.HyperVSecurity;
        vulnManager.Add(hyperVVuln);

// Add WSUS Targeting
        Vuln wsusTargetingVuln = VulnTemplatesGeneralEN.WSUSTargeting;
        vulnManager.Add(wsusTargetingVuln);

// Add ADFS Security Issues
        Vuln adfsVuln = VulnTemplatesGeneralEN.ADFSSecurityIssues;
        vulnManager.Add(adfsVuln);

// Add Constrained Delegation
        Vuln constrainedDelegationVuln = VulnTemplatesGeneralEN.ConstrainedDelegation;
        vulnManager.Add(constrainedDelegationVuln);

// Add DNS Zone Transfer
        Vuln dnsZoneTransferVuln = VulnTemplatesGeneralEN.DNSZoneTransfer;
        vulnManager.Add(dnsZoneTransferVuln);

// Add DFS Share Permissions
        Vuln dfsShareVuln = VulnTemplatesGeneralEN.DFSSharePermissions;
        vulnManager.Add(dfsShareVuln);

// Add Network Device Misconfiguration
        Vuln networkDeviceVuln = VulnTemplatesGeneralEN.NetworkDeviceMisconfig;
        vulnManager.Add(networkDeviceVuln);

// Add Azure AD Connect Issues
        Vuln azureADConnectVuln = VulnTemplatesGeneralEN.AzureADConnectIssues;
        vulnManager.Add(azureADConnectVuln);

// Add SCCM Security Issues
        Vuln sccmVuln = VulnTemplatesGeneralEN.SCCMSecurityIssues;
        vulnManager.Add(sccmVuln);

// Add Remote Access Policy Issues
        Vuln remoteAccessVuln = VulnTemplatesGeneralEN.RemoteAccessPolicy;
        vulnManager.Add(remoteAccessVuln);

// Add ADCS Misconfiguration
        Vuln adcsVuln = VulnTemplatesGeneralEN.ADCSMisconfiguration;
        vulnManager.Add(adcsVuln);

// Add SQL Server Agent Jobs
        Vuln sqlAgentJobsVuln = VulnTemplatesGeneralEN.SQLServerAgentJobs;
        vulnManager.Add(sqlAgentJobsVuln);

// Add DHCP Misconfiguration
        Vuln dhcpVuln = VulnTemplatesGeneralEN.DHCPMisconfiguration;
        vulnManager.Add(dhcpVuln);

// Add Windows Firewall Policy
        Vuln firewallPolicyVuln = VulnTemplatesGeneralEN.WindowsFirewallPolicy;
        vulnManager.Add(firewallPolicyVuln);

// Add Password Filter DLL
        Vuln passwordFilterVuln = VulnTemplatesGeneralEN.PasswordFilterDLL;
        vulnManager.Add(passwordFilterVuln);

// Add Unauthorized Sudo Access
        Vuln sudoAccessVuln = VulnTemplatesGeneralEN.UnauthorizedSudoAccess;
        vulnManager.Add(sudoAccessVuln);

// Add Unencrypted Data Storage
        Vuln unencryptedDataVuln = VulnTemplatesGeneralEN.UnencryptedDataStorage;
        vulnManager.Add(unencryptedDataVuln);

// Add Insecure File Permissions
        Vuln insecureFileVuln = VulnTemplatesGeneralEN.InsecureFilePermissions;
        vulnManager.Add(insecureFileVuln);

// Add Insecure Service Configuration
        Vuln insecureServiceVuln = VulnTemplatesGeneralEN.InsecureServiceConfiguration;
        vulnManager.Add(insecureServiceVuln);

// Add Weak Password Storage
        Vuln weakPasswordStorageVuln = VulnTemplatesGeneralEN.WeakPasswordStorage;
        vulnManager.Add(weakPasswordStorageVuln);

// Add Unsecure Kernel Parameters
        Vuln unsecureKernelVuln = VulnTemplatesGeneralEN.UnsecureKernelParameters;
        vulnManager.Add(unsecureKernelVuln);

// Add Unsecured Cron Jobs
        Vuln unsecuredCronVuln = VulnTemplatesGeneralEN.UnsecuredCronJobs;
        vulnManager.Add(unsecuredCronVuln);

// Add Insecure Log Configuration
        Vuln insecureLogVuln = VulnTemplatesGeneralEN.InsecureLogConfiguration;
        vulnManager.Add(insecureLogVuln);

// Add Insecure Time Synchronization
        Vuln insecureTimeVuln = VulnTemplatesGeneralEN.InsecureTimeSynchronization;
        vulnManager.Add(insecureTimeVuln);

// Add Exposed Development Tools
        Vuln exposedDevToolsVuln = VulnTemplatesGeneralEN.ExposedDevelopmentTools;
        vulnManager.Add(exposedDevToolsVuln);

// Add Insecure Automount Configuration
        Vuln insecureAutomountVuln = VulnTemplatesGeneralEN.InsecureAutomountConfiguration;
        vulnManager.Add(insecureAutomountVuln);

// Add Unrestricted Core Dumps
        Vuln unrestrictedCoreDumpsVuln = VulnTemplatesGeneralEN.UnrestrictedCoreDumps;
        vulnManager.Add(unrestrictedCoreDumpsVuln);

// Add Insecure LDAP Configuration
        Vuln insecureLdapVuln = VulnTemplatesGeneralEN.InsecureLDAPConfiguration;
        vulnManager.Add(insecureLdapVuln);

// Add Missing Full Disk Encryption
        Vuln missingDiskEncryptionVuln = VulnTemplatesGeneralEN.MissingDiskEncryption;
        vulnManager.Add(missingDiskEncryptionVuln);

// Add Insecure SSH Configuration
        Vuln insecureSshVuln = VulnTemplatesGeneralEN.InsecureSSHConfiguration;
        vulnManager.Add(insecureSshVuln);

// Add Unrestricted USB Access
        Vuln unrestrictedUsbVuln = VulnTemplatesGeneralEN.UnrestrictedUSBAccess;
        vulnManager.Add(unrestrictedUsbVuln);

// Add Insecure Backup Configuration
        Vuln insecureBackupVuln = VulnTemplatesGeneralEN.InsecureBackupConfiguration;
        vulnManager.Add(insecureBackupVuln);

// Add Insecure Firewall Rules
        Vuln insecureFirewallVuln = VulnTemplatesGeneralEN.InsecureFirewallRules;
        vulnManager.Add(insecureFirewallVuln);

// Add Insecure Home Directories
        Vuln insecureHomeVuln = VulnTemplatesGeneralEN.InsecureHomeDirectories;
        vulnManager.Add(insecureHomeVuln);

// Add Insecure Keychain Configuration
        Vuln insecureKeychainVuln = VulnTemplatesGeneralEN.InsecureKeychainConfiguration;
        vulnManager.Add(insecureKeychainVuln);

// Add Disabled System Integrity Protection
        Vuln disabledSipVuln = VulnTemplatesGeneralEN.DisabledSystemIntegrityProtection;
        vulnManager.Add(disabledSipVuln);

// Add Unsecured Container Configuration
        Vuln unsecuredContainerVuln = VulnTemplatesGeneralEN.UnsecuredContainerConfiguration;
        vulnManager.Add(unsecuredContainerVuln);

// Add Insecure PAM Configuration
        Vuln insecurePamVuln = VulnTemplatesGeneralEN.InsecurePAMConfiguration;
        vulnManager.Add(insecurePamVuln);

// Add Insecure Audit Configuration
        Vuln insecureAuditVuln = VulnTemplatesGeneralEN.InsecureAuditConfiguration;
        vulnManager.Add(insecureAuditVuln);

// Add Insecure X Server Configuration
        Vuln insecureXServerVuln = VulnTemplatesGeneralEN.InsecureXServerConfiguration;
        vulnManager.Add(insecureXServerVuln);

// Add Insecure Resource Limits
        Vuln insecureResourceVuln = VulnTemplatesGeneralEN.InsecureResourceLimits;
        vulnManager.Add(insecureResourceVuln);

// Add Insecure Kernel Module Loading
        Vuln insecureKernelModuleVuln = VulnTemplatesGeneralEN.InsecureKernelModuleLoading;
        vulnManager.Add(insecureKernelModuleVuln);

// Add Insecure Syslog Configuration
        Vuln insecureSyslogVuln = VulnTemplatesGeneralEN.InsecureSyslogConfiguration;
        vulnManager.Add(insecureSyslogVuln);

// Add Insecure Gatekeeper Configuration
        Vuln insecureGatekeeperVuln = VulnTemplatesGeneralEN.InsecureGatekeeperConfiguration;
        vulnManager.Add(insecureGatekeeperVuln);

// Add Insecure Application Sandbox Configuration
        Vuln insecureSandboxVuln = VulnTemplatesGeneralEN.InsecureSandboxConfiguration;
        vulnManager.Add(insecureSandboxVuln);

// Add Insecure Systemd Service Configuration
        Vuln insecureSystemdVuln = VulnTemplatesGeneralEN.InsecureSystemdServiceConfiguration;
        vulnManager.Add(insecureSystemdVuln);

// Add Insecure XProtect Configuration
        Vuln insecureXProtectVuln = VulnTemplatesGeneralEN.InsecureXProtectConfiguration;
        vulnManager.Add(insecureXProtectVuln);

// Add Insecure Memory Protection
        Vuln insecureMemoryVuln = VulnTemplatesGeneralEN.InsecureMemoryProtection;
        vulnManager.Add(insecureMemoryVuln);

// Add Insecure IPC Configuration
        Vuln insecureIpcVuln = VulnTemplatesGeneralEN.InsecureIPCConfiguration;
        vulnManager.Add(insecureIpcVuln);

// Add Insecure Process Accounting
        Vuln insecureProcessVuln = VulnTemplatesGeneralEN.InsecureProcessAccounting;
        vulnManager.Add(insecureProcessVuln);

// Add AS-REP Roastable Accounts
        Vuln asRepRoastVuln = VulnTemplatesGeneralEN.ASREPRoastableAccounts;
        vulnManager.Add(asRepRoastVuln);

// Add Insecure Delegation Configuration
        Vuln insecureDelegationVuln = VulnTemplatesGeneralEN.InsecureDelegationConfiguration;
        vulnManager.Add(insecureDelegationVuln);

// Add Weak GPO Permissions
        Vuln weakGpoVuln = VulnTemplatesGeneralEN.WeakGPOPermissions;
        vulnManager.Add(weakGpoVuln);

// Add DCOM Exploitation
        Vuln dcomVuln = VulnTemplatesGeneralEN.DCOMExploitation;
        vulnManager.Add(dcomVuln);

// Add Weak Schema Permissions
        Vuln weakSchemaVuln = VulnTemplatesGeneralEN.WeakSchemaPermissions;
        vulnManager.Add(weakSchemaVuln);

// Add Insecure LDAPS Configuration
        Vuln insecureLdapsVuln = VulnTemplatesGeneralEN.InsecureLDAPSConfiguration;
        vulnManager.Add(insecureLdapsVuln);

// Add Privileged Group Membership
        Vuln privilegedGroupVuln = VulnTemplatesGeneralEN.PrivilegedGroupMembership;
        vulnManager.Add(privilegedGroupVuln);

// Save all changes
        await vulnManager.Context.SaveChangesAsync();
        
    }

    public async Task CreateEnglishMastg()
    {
        Vuln insecureDataStorage = VulnTemplatesMastgEN.InsecureDataStorage;
        insecureDataStorage.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insecureDataStorage.Id });
        vulnManager.Add(insecureDataStorage);

        Vuln insecureCryptography = VulnTemplatesMastgEN.InsecureCryptography;
        insecureCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureCryptography.Id });
        vulnManager.Add(insecureCryptography);

        Vuln insecureAuthentication = VulnTemplatesMastgEN.InsecureAuthentication;
        insecureAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureAuthentication.Id });
        vulnManager.Add(insecureAuthentication);

        Vuln insecureNetworkCommunication = VulnTemplatesMastgEN.InsecureNetworkCommunication;
        insecureNetworkCommunication.VulnCwes.Add(new VulnCwe
            { CweId = 319, VulnId = insecureNetworkCommunication.Id });
        vulnManager.Add(insecureNetworkCommunication);

        Vuln privacyViolations = VulnTemplatesMastgEN.PrivacyViolation;
        privacyViolations.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = privacyViolations.Id });
        vulnManager.Add(privacyViolations);

        Vuln insecureDataLeakage = VulnTemplatesMastgEN.InsecureDataLeakage;
        insecureDataLeakage.VulnCwes.Add(new VulnCwe { CweId = 532, VulnId = insecureDataLeakage.Id });
        vulnManager.Add(insecureDataLeakage);

        Vuln insecureKeyManagement = VulnTemplatesMastgEN.InsecureKeyManagement;
        insecureKeyManagement.VulnCwes.Add(new VulnCwe { CweId = 321, VulnId = insecureKeyManagement.Id });
        vulnManager.Add(insecureKeyManagement);

        Vuln insecureLocalAuthentication = VulnTemplatesMastgEN.InsecureLocalAuthentication;
        insecureLocalAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureLocalAuthentication.Id });
        vulnManager.Add(insecureLocalAuthentication);

        Vuln insecureCertificatePinning = VulnTemplatesMastgEN.InsecureCertificatePinning;
        insecureCertificatePinning.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureCertificatePinning.Id });
        vulnManager.Add(insecureCertificatePinning);

        Vuln insecureIPC = VulnTemplatesMastgEN.InsecureIPC;
        insecureIPC.VulnCwes.Add(new VulnCwe { CweId = 927, VulnId = insecureIPC.Id });
        vulnManager.Add(insecureIPC);
        

        Vuln insecureWebview = VulnTemplatesMastgEN.InsecureWebView;
        insecureWebview.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureWebview.Id });
        vulnManager.Add(insecureWebview);
        

        Vuln insecureDeepLinking = VulnTemplatesMastgEN.InsecureDeepLinking;
        insecureDeepLinking.VulnCwes.Add(new VulnCwe { CweId = 939, VulnId = insecureDeepLinking.Id });
        vulnManager.Add(insecureDeepLinking);
        

        Vuln insecureSessionHandling = VulnTemplatesMastgEN.InsecureSessionHandling;
        insecureSessionHandling.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionHandling.Id });
        vulnManager.Add(insecureSessionHandling);
        

        Vuln insecureTlsValidation = VulnTemplatesMastgEN.InsecureTlsValidation;
        insecureTlsValidation.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureTlsValidation.Id });
        vulnManager.Add(insecureTlsValidation);
        

        Vuln insecureClipboardUsage = VulnTemplatesMastgEN.InsecureClipboardUsage;
        insecureClipboardUsage.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureClipboardUsage.Id });
        vulnManager.Add(insecureClipboardUsage);
        

        Vuln insecureDataCaching = VulnTemplatesMastgEN.InsecureDataCaching;
        insecureDataCaching.VulnCwes.Add(new VulnCwe { CweId = 524, VulnId = insecureDataCaching.Id });
        vulnManager.Add(insecureDataCaching);
        

        Vuln insecureBackupHandling = VulnTemplatesMastgEN.InsecureBackupHandling;
        insecureBackupHandling.VulnCwes.Add(new VulnCwe { CweId = 530, VulnId = insecureBackupHandling.Id });
        vulnManager.Add(insecureBackupHandling);
        

        Vuln insufficientInputValidation = VulnTemplatesMastgEN.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        

        Vuln insecureJailbreakRootDetection = VulnTemplatesMastgEN.InsecureJailbreakRootDetection;
        insecureJailbreakRootDetection.VulnCwes.Add(new VulnCwe
            { CweId = 919, VulnId = insecureJailbreakRootDetection.Id });
        vulnManager.Add(insecureJailbreakRootDetection);
        

        Vuln insecureCodeObfuscation = VulnTemplatesMastgEN.InsecureCodeObfuscation;
        insecureCodeObfuscation.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureCodeObfuscation.Id });
        vulnManager.Add(insecureCodeObfuscation);
        

        Vuln insecureRuntimeIntegrityChecks = VulnTemplatesMastgEN.InsecureRuntimeIntegrityChecks;
        insecureRuntimeIntegrityChecks.VulnCwes.Add(new VulnCwe
            { CweId = 693, VulnId = insecureRuntimeIntegrityChecks.Id });
        vulnManager.Add(insecureRuntimeIntegrityChecks);
        

        Vuln insecureAppPackaging = VulnTemplatesMastgEN.InsecureAppPackaging;
        insecureAppPackaging.VulnCwes.Add(new VulnCwe { CweId = 490, VulnId = insecureAppPackaging.Id });
        vulnManager.Add(insecureAppPackaging);
        

        Vuln insecureMemoryManagement = VulnTemplatesMastgEN.InsecureMemoryManagement;
        insecureMemoryManagement.VulnCwes.Add(new VulnCwe { CweId = 316, VulnId = insecureMemoryManagement.Id });
        vulnManager.Add(insecureMemoryManagement);
        

        Vuln insecureComponentUpgrade = VulnTemplatesMastgEN.InsecureComponentUpgrade;
        insecureComponentUpgrade.VulnCwes.Add(new VulnCwe { CweId = 494, VulnId = insecureComponentUpgrade.Id });
        vulnManager.Add(insecureComponentUpgrade);
        

        Vuln insecureDataResidency = VulnTemplatesMastgEN.InsecureDataResidency;
        insecureDataResidency.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insecureDataResidency.Id });
        vulnManager.Add(insecureDataResidency);
        

        Vuln insecureCloudSyncmechanism = VulnTemplatesMastgEN.InsecureCloudSyncMechanism;
        insecureCloudSyncmechanism.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insecureCloudSyncmechanism.Id });
        vulnManager.Add(insecureCloudSyncmechanism);
        

        Vuln vulnerableThirdPartyLibraries = VulnTemplatesMastgEN.VulnerableThirdPartyLibrary;
        vulnerableThirdPartyLibraries.VulnCwes.Add(new VulnCwe
            { CweId = 1035, VulnId = vulnerableThirdPartyLibraries.Id });
        vulnManager.Add(vulnerableThirdPartyLibraries);
        

        Vuln insecureDataExfiltration = VulnTemplatesMastgEN.InsecureDataExfiltration;
        insecureDataExfiltration.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureDataExfiltration.Id });
        vulnManager.Add(insecureDataExfiltration);
        

        Vuln insecureAPIVersioning = VulnTemplatesMastgEN.InsecureAPIVersioning;
        insecureAPIVersioning.VulnCwes.Add(new VulnCwe { CweId = 330, VulnId = insecureAPIVersioning.Id });
        vulnManager.Add(insecureAPIVersioning);
        

        Vuln insecureQRCodeHandling = VulnTemplatesMastgEN.InsecureQRCodeHandling;
        insecureQRCodeHandling.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insecureQRCodeHandling.Id });
        vulnManager.Add(insecureQRCodeHandling);
        

        Vuln insecureNFCImplementation = VulnTemplatesMastgEN.InsecureNFCImplementation;
        insecureNFCImplementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureNFCImplementation.Id });
        vulnManager.Add(insecureNFCImplementation);
        

        Vuln insecureARImpementation = VulnTemplatesMastgEN.InsecureARImplementation;
        insecureARImpementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureARImpementation.Id });
        vulnManager.Add(insecureARImpementation);
        

        Vuln insecureIoTImplementation = VulnTemplatesMastgEN.InsecureIoTIntegration;
        insecureIoTImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureIoTImplementation.Id });
        vulnManager.Add(insecureIoTImplementation);
        

        Vuln insecurePushNotification = VulnTemplatesMastgEN.InsecurePushNotifications;
        insecurePushNotification.VulnCwes.Add(new VulnCwe { CweId = 223, VulnId = insecurePushNotification.Id });
        vulnManager.Add(insecurePushNotification);
        

        Vuln insecureAppCloning = VulnTemplatesMastgEN.InsecureAppCloning;
        insecureAppCloning.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureAppCloning.Id });
        vulnManager.Add(insecureAppCloning);
        

        Vuln insecureScreenOverlay = VulnTemplatesMastgEN.InsecureScreenOverlay;
        insecureScreenOverlay.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = insecureScreenOverlay.Id });
        vulnManager.Add(insecureScreenOverlay);
        

        Vuln insecureAppWidget = VulnTemplatesMastgEN.InsecureAppWidget;
        insecureAppWidget.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureAppWidget.Id });
        vulnManager.Add(insecureAppWidget);
        

        Vuln insecureEdgeComputingIntegration = VulnTemplatesMastgEN.InsecureEdgeComputingIntegration;
        insecureEdgeComputingIntegration.VulnCwes.Add(new VulnCwe
            { CweId = 319, VulnId = insecureEdgeComputingIntegration.Id });
        vulnManager.Add(insecureEdgeComputingIntegration);
        

        Vuln insecureAIMLImplementation = VulnTemplatesMastgEN.InsecureAIMLImplementation;
        insecureAIMLImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureAIMLImplementation.Id });
        vulnManager.Add(insecureAIMLImplementation);
        

        Vuln insecureQuantumResistantCrypto = VulnTemplatesMastgEN.InsecureQuantumResistantCrypto;
        insecureQuantumResistantCrypto.VulnCwes.Add(new VulnCwe
            { CweId = 327, VulnId = insecureQuantumResistantCrypto.Id });
        vulnManager.Add(insecureQuantumResistantCrypto);
        

        Vuln insecureVoiceIntegration = VulnTemplatesMastgEN.InsecureVoiceUIIntegration;
        insecureVoiceIntegration.VulnCwes.Add(new VulnCwe { CweId = 350, VulnId = insecureVoiceIntegration.Id });
        vulnManager.Add(insecureVoiceIntegration);
        

        Vuln insecureMultiDeviceSynchronization = VulnTemplatesMastgEN.InsecureMultiDeviceSynchronization;
        insecureMultiDeviceSynchronization.VulnCwes.Add(new VulnCwe
            { CweId = 319, VulnId = insecureMultiDeviceSynchronization.Id });
        vulnManager.Add(insecureMultiDeviceSynchronization);
        

        Vuln insecureBlockchainIntegration = VulnTemplatesMastgEN.InsecureBlockchainIntegration;
        insecureBlockchainIntegration.VulnCwes.Add(new VulnCwe
            { CweId = 320, VulnId = insecureBlockchainIntegration.Id });
        vulnManager.Add(insecureBlockchainIntegration);
        

        Vuln insecureKeychainKeystore = VulnTemplatesMastgEN.InsecureKeychainKeystore;
        insecureKeychainKeystore.VulnCwes.Add(new VulnCwe { CweId = 522, VulnId = insecureKeychainKeystore.Id });
        vulnManager.Add(insecureKeychainKeystore);
        

        Vuln insecureRandomNumberGeneration = VulnTemplatesMastgEN.InsecureRandomNumberGeneration;
        insecureRandomNumberGeneration.VulnCwes.Add(new VulnCwe
            { CweId = 338, VulnId = insecureRandomNumberGeneration.Id });
        vulnManager.Add(insecureRandomNumberGeneration);
        

        Vuln insecureSSOImplementation = VulnTemplatesMastgEN.InsecureSSOImplementation;
        insecureSSOImplementation.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureSSOImplementation.Id });
        vulnManager.Add(insecureSSOImplementation);
        

        Vuln insecureVPNUsage = VulnTemplatesMastgEN.InsecureVPNUsage;
        insecureVPNUsage.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureVPNUsage.Id });
        vulnManager.Add(insecureVPNUsage);
        

        Vuln insecureCustomURLScheme = VulnTemplatesMastgEN.InsecureCustomURLScheme;
        insecureCustomURLScheme.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureCustomURLScheme.Id });
        vulnManager.Add(insecureCustomURLScheme);
        

        Vuln timeOfCheckToTimeOfUse = VulnTemplatesMastgEN.TimeOfCheckToTimeOfUse;
        timeOfCheckToTimeOfUse.VulnCwes.Add(new VulnCwe { CweId = 367, VulnId = timeOfCheckToTimeOfUse.Id });
        vulnManager.Add(timeOfCheckToTimeOfUse);
        

        Vuln insecureAntiDebugging = VulnTemplatesMastgEN.InsecureAntiDebugging;
        insecureAntiDebugging.VulnCwes.Add(new VulnCwe { CweId = 388, VulnId = insecureAntiDebugging.Id });
        vulnManager.Add(insecureAntiDebugging);
        

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
        

        Vuln weakPasswordRequirements = VulnTemplatesWstgEN.WeakPasswordRequirements;
        weakPasswordRequirements.VulnCwes.Add(new VulnCwe { CweId = 521, VulnId = weakPasswordRequirements.Id });
        vulnManager.Add(weakPasswordRequirements);
        

        Vuln insecureSessionManagement = VulnTemplatesWstgEN.InsecureSessionManagement;
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 384, VulnId = insecureSessionManagement.Id });
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionManagement.Id });
        vulnManager.Add(insecureSessionManagement);
        

        Vuln idor = VulnTemplatesWstgEN.Idor;
        idor.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = idor.Id });
        vulnManager.Add(idor);
        

        Vuln crossSiteScripting = VulnTemplatesWstgEN.CrossSiteScripting;
        crossSiteScripting.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = crossSiteScripting.Id });
        vulnManager.Add(crossSiteScripting);
        

        Vuln insecureCryptographicStorage = VulnTemplatesWstgEN.InsecureCryptographicStorage;
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe
            { CweId = 327, VulnId = insecureCryptographicStorage.Id });
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe
            { CweId = 328, VulnId = insecureCryptographicStorage.Id });
        vulnManager.Add(insecureCryptographicStorage);
        

        Vuln insecureDeserialization = VulnTemplatesWstgEN.InsecureDeserialization;
        insecureDeserialization.VulnCwes.Add(new VulnCwe { CweId = 502, VulnId = insecureDeserialization.Id });
        vulnManager.Add(insecureDeserialization);
        

        Vuln sqlInjection = VulnTemplatesWstgEN.SqlInjection;
        sqlInjection.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = sqlInjection.Id });
        vulnManager.Add(sqlInjection);
        

        Vuln brokenAccessControl = VulnTemplatesWstgEN.BrokenAccessControl;
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = brokenAccessControl.Id });
        vulnManager.Add(brokenAccessControl);
        

        Vuln securityMisconfiguration = VulnTemplatesWstgEN.SecurityMisconfiguration;
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 16, VulnId = securityMisconfiguration.Id });
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 2, VulnId = securityMisconfiguration.Id });
        vulnManager.Add(securityMisconfiguration);
        

        Vuln sensitiveDataExposure = VulnTemplatesWstgEN.SensitiveDataExposure;
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = sensitiveDataExposure.Id });
        vulnManager.Add(sensitiveDataExposure);
        

        Vuln insufficientLoggingMonitoring = VulnTemplatesWstgEN.InsufficientLoggingAndMonitoring;
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe
            { CweId = 778, VulnId = insufficientLoggingMonitoring.Id });
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe
            { CweId = 223, VulnId = insufficientLoggingMonitoring.Id });
        vulnManager.Add(insufficientLoggingMonitoring);
        

        Vuln usingComponentsWithKnownVulnerabilities = VulnTemplatesWstgEN.UsingComponentsWithKnownVulnerabilities;
        usingComponentsWithKnownVulnerabilities.VulnCwes.Add(new VulnCwe
            { CweId = 1035, VulnId = usingComponentsWithKnownVulnerabilities.Id });
        vulnManager.Add(usingComponentsWithKnownVulnerabilities);
        

        Vuln serverSideRequestForgery = VulnTemplatesWstgEN.ServerSideRequestForgery;
        serverSideRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 918, VulnId = serverSideRequestForgery.Id });
        vulnManager.Add(serverSideRequestForgery);
        

        Vuln xmlExternalEntityProcessing = VulnTemplatesWstgEN.XmlExternalEntityProcessing;
        xmlExternalEntityProcessing.VulnCwes.Add(new VulnCwe { CweId = 611, VulnId = xmlExternalEntityProcessing.Id });
        vulnManager.Add(xmlExternalEntityProcessing);
        

        Vuln lackOfrateLimiting = VulnTemplatesWstgEN.LackOfRateLimiting;
        lackOfrateLimiting.VulnCwes.Add(new VulnCwe { CweId = 770, VulnId = lackOfrateLimiting.Id });
        vulnManager.Add(lackOfrateLimiting);
        

        Vuln inadequateOAuth2Implementation = VulnTemplatesWstgEN.InadequateOAuth2Implementation;
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe
            { CweId = 346, VulnId = inadequateOAuth2Implementation.Id });
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe
            { CweId = 601, VulnId = inadequateOAuth2Implementation.Id });
        vulnManager.Add(inadequateOAuth2Implementation);
        

        Vuln insufficientAntiAutomation = VulnTemplatesWstgEN.InsufficientAntiAutomation;
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 799, VulnId = insufficientAntiAutomation.Id });
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 837, VulnId = insufficientAntiAutomation.Id });
        vulnManager.Add(insufficientAntiAutomation);
        

        Vuln weakCryptographicAlgorithms = VulnTemplatesWstgEN.WeakCryptographicAlgorithms;
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = weakCryptographicAlgorithms.Id });
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = weakCryptographicAlgorithms.Id });
        vulnManager.Add(weakCryptographicAlgorithms);
        

        Vuln improperCertificateValidation = VulnTemplatesWstgEN.ImproperCertificateValidation;
        improperCertificateValidation.VulnCwes.Add(new VulnCwe
            { CweId = 295, VulnId = improperCertificateValidation.Id });
        vulnManager.Add(improperCertificateValidation);
        

        Vuln crossSiteRequestForgery = VulnTemplatesWstgEN.CrossSiteRequestForgery;
        crossSiteRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 352, VulnId = crossSiteRequestForgery.Id });
        vulnManager.Add(crossSiteRequestForgery);
        

        Vuln insufficientPasswordRecovery = VulnTemplatesWstgEN.InsufficientPasswordRecoveryMechanism;
        insufficientPasswordRecovery.VulnCwes.Add(new VulnCwe
            { CweId = 640, VulnId = insufficientPasswordRecovery.Id });
        vulnManager.Add(insufficientPasswordRecovery);
        

        Vuln lackOfInputSanitization = VulnTemplatesWstgEN.LackOfInputSanitization;
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = lackOfInputSanitization.Id });
        vulnManager.Add(lackOfInputSanitization);
        

        Vuln insufficientSesssionTimeout = VulnTemplatesWstgEN.InsufficientSessionTimeout;
        insufficientSesssionTimeout.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insufficientSesssionTimeout.Id });
        vulnManager.Add(insufficientSesssionTimeout);
        

        Vuln improperErrorHandling = VulnTemplatesWstgEN.ImproperErrorHandling;
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 209, VulnId = improperErrorHandling.Id });
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = improperErrorHandling.Id });
        vulnManager.Add(improperErrorHandling);
        

        Vuln missingSecurityHeaders = VulnTemplatesWstgEN.MissingSecurityHeaders;
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 693, VulnId = missingSecurityHeaders.Id });
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = missingSecurityHeaders.Id });
        vulnManager.Add(missingSecurityHeaders);
        

        Vuln insecureUseOfCryptography = VulnTemplatesWstgEN.InsecureUseOfCryptography;
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureUseOfCryptography.Id });
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 320, VulnId = insecureUseOfCryptography.Id });
        vulnManager.Add(insecureUseOfCryptography);
        

        Vuln brokenFunctionLevelAuthorization = VulnTemplatesWstgEN.BrokenFunctionLevelAuthorization;
        brokenFunctionLevelAuthorization.VulnCwes.Add(new VulnCwe
            { CweId = 285, VulnId = brokenFunctionLevelAuthorization.Id });
        vulnManager.Add(brokenFunctionLevelAuthorization);
        

        Vuln insecureCommunication = VulnTemplatesWstgEN.InsecureCommunicationChannels;
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureCommunication.Id });
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = insecureCommunication.Id });
        vulnManager.Add(insecureCommunication);
        

        Vuln lackOfSecureDefaults = VulnTemplatesWstgEN.LackOfSecureDefaults;
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 276, VulnId = lackOfSecureDefaults.Id });
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 1188, VulnId = lackOfSecureDefaults.Id });
        vulnManager.Add(lackOfSecureDefaults);
        

        Vuln insufficientProtectionFromAutomatedThreats =
            VulnTemplatesWstgEN.InsufficientProtectionFromAutomatedThreats;
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe
            { CweId = 799, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe
            { CweId = 307, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        vulnManager.Add(insufficientProtectionFromAutomatedThreats);
        

        Vuln unvalidatedRedirectsAndForwards = VulnTemplatesWstgEN.UnvalidatedRedirectsAndForwards;
        unvalidatedRedirectsAndForwards.VulnCwes.Add(new VulnCwe
            { CweId = 601, VulnId = unvalidatedRedirectsAndForwards.Id });
        vulnManager.Add(unvalidatedRedirectsAndForwards);
        

        Vuln insecureAuthenticationMechanism = VulnTemplatesWstgEN.InsecureAuthenticationMechanism;
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe
            { CweId = 287, VulnId = insecureAuthenticationMechanism.Id });
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe
            { CweId = 384, VulnId = insecureAuthenticationMechanism.Id });
        vulnManager.Add(insecureAuthenticationMechanism);
        

        Vuln insuffificientAntiCachingHeaders = VulnTemplatesWstgEN.InsufficientAntiCachingHeaders;
        insuffificientAntiCachingHeaders.VulnCwes.Add(new VulnCwe
            { CweId = 525, VulnId = insuffificientAntiCachingHeaders.Id });
        vulnManager.Add(insuffificientAntiCachingHeaders);
        

        Vuln lackOfProperTLSConfiguration = VulnTemplatesWstgEN.LackOfProperTLSConfiguration;
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe
            { CweId = 326, VulnId = lackOfProperTLSConfiguration.Id });
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe
            { CweId = 327, VulnId = lackOfProperTLSConfiguration.Id });
        vulnManager.Add(lackOfProperTLSConfiguration);
        

        Vuln insufficientDataProtection = VulnTemplatesWstgEN.InsufficientDataProtection;
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insufficientDataProtection.Id });
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insufficientDataProtection.Id });
        vulnManager.Add(insufficientDataProtection);
        

        Vuln improperAssetManagement = VulnTemplatesWstgEN.ImproperAssetManagement;
        improperAssetManagement.VulnCwes.Add(new VulnCwe { CweId = 1059, VulnId = improperAssetManagement.Id });
        vulnManager.Add(improperAssetManagement);
        

        Vuln lackOfSoftwareUpdate = VulnTemplatesWstgEN.LackOfSoftwareUpdates;
        lackOfSoftwareUpdate.VulnCwes.Add(new VulnCwe { CweId = 1104, VulnId = lackOfSoftwareUpdate.Id });
        vulnManager.Add(lackOfSoftwareUpdate);
        

        Vuln insufficientPrivacyControls = VulnTemplatesWstgEN.InsufficientPrivacyControls;
        insufficientPrivacyControls.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insufficientPrivacyControls.Id });
        vulnManager.Add(insufficientPrivacyControls);
        

        Vuln insecureApiEndpoints = VulnTemplatesWstgEN.InsecureAPIEndpoints;
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = insecureApiEndpoints.Id });
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = insecureApiEndpoints.Id });
        vulnManager.Add(insecureApiEndpoints);
        

        Vuln insufficientInputValidation = VulnTemplatesWstgEN.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        

        Vuln improperOutputEncoding = VulnTemplatesWstgEN.ImproperOutputEncoding;
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = improperOutputEncoding.Id });
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 116, VulnId = improperOutputEncoding.Id });
        vulnManager.Add(improperOutputEncoding);
        

        Vuln insecureFileHandling = VulnTemplatesWstgEN.InsecureFileHandling;
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 73, VulnId = insecureFileHandling.Id });
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 434, VulnId = insecureFileHandling.Id });
        vulnManager.Add(insecureFileHandling);
        

        Vuln lackOfSecurePasswordStorage = VulnTemplatesWstgEN.LackOfSecurePasswordStorage;
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 256, VulnId = lackOfSecurePasswordStorage.Id });
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 916, VulnId = lackOfSecurePasswordStorage.Id });
        vulnManager.Add(lackOfSecurePasswordStorage);
        

        Vuln insufficientProtectionAgainstDos = VulnTemplatesWstgEN.InsufficientProtectionAgainstDoS;
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe
            { CweId = 400, VulnId = insufficientProtectionAgainstDos.Id });
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe
            { CweId = 770, VulnId = insufficientProtectionAgainstDos.Id });
        vulnManager.Add(insufficientProtectionAgainstDos);
        

        Vuln insufficientSubresourceIntegrity = VulnTemplatesWstgEN.InsufficientSubresourceIntegrity;
        insufficientSubresourceIntegrity.VulnCwes.Add(new VulnCwe
            { CweId = 353, VulnId = insufficientSubresourceIntegrity.Id });
        vulnManager.Add(insufficientSubresourceIntegrity);
        

        Vuln inadequateSecurityArchitecture = VulnTemplatesWstgEN.InadequateSecurityArchitecture;
        inadequateSecurityArchitecture.VulnCwes.Add(new VulnCwe
            { CweId = 1008, VulnId = inadequateSecurityArchitecture.Id });
        vulnManager.Add(inadequateSecurityArchitecture);
        

        Vuln flawedBusinessLogic = VulnTemplatesWstgEN.FlawedBusinessLogic;
        flawedBusinessLogic.VulnCwes.Add(new VulnCwe { CweId = 840, VulnId = flawedBusinessLogic.Id });
        vulnManager.Add(flawedBusinessLogic);
        

        Vuln insecureThirdPartyComponents = VulnTemplatesWstgEN.InsecureThirdPartyComponents;
        insecureThirdPartyComponents.VulnCwes.Add(
            new VulnCwe { CweId = 1104, VulnId = insecureThirdPartyComponents.Id });
        vulnManager.Add(insecureThirdPartyComponents);
        await vulnManager.Context.SaveChangesAsync();
    }

    public async Task CreateSpanishGeneral()
    {
        // Add Kerberoasting Vulnerability
        Vuln kerberoastingVuln = VulnTemplatesGeneralES.KerberoastingVulnerability;
        vulnManager.Add(kerberoastingVuln);

// Add LLMNR Poisoning
        Vuln llmnrVuln = VulnTemplatesGeneralES.LLMNRPoisoning;
        vulnManager.Add(llmnrVuln);

// Add Local Admin Password Reuse
        Vuln localAdminVuln = VulnTemplatesGeneralES.LocalAdminPasswordReuse;
        vulnManager.Add(localAdminVuln);

// Add Unquoted Service Path
        Vuln unquotedServiceVuln = VulnTemplatesGeneralES.UnquotedServicePath;
        vulnManager.Add(unquotedServiceVuln);

// Add SMB Signing Issues
        Vuln smbSigningVuln = VulnTemplatesGeneralES.SMBSigningDisabled;
        vulnManager.Add(smbSigningVuln);

// Add Excessive Domain Admin Rights
        Vuln domainAdminVuln = VulnTemplatesGeneralES.ExcessiveDomainAdminRights;
        vulnManager.Add(domainAdminVuln);

// Add GPP Password Exposure
        Vuln gppPasswordVuln = VulnTemplatesGeneralES.GPPPasswordExposure;
        vulnManager.Add(gppPasswordVuln);

// Add Weak Service Account Permissions
        Vuln weakServiceAccountVuln = VulnTemplatesGeneralES.WeakServiceAccountPermissions;
        vulnManager.Add(weakServiceAccountVuln);

// Add Clear Text Protocols
        Vuln clearTextVuln = VulnTemplatesGeneralES.ClearTextProtocols;
        vulnManager.Add(clearTextVuln);

// Add Insecure LDAP Binding
        Vuln ldapBindingVuln = VulnTemplatesGeneralES.InsecureLDAPBinding;
        vulnManager.Add(ldapBindingVuln);

// Add Weak TLS Configuration
        Vuln weakTlsVuln = VulnTemplatesGeneralES.WeakTLSConfiguration;
        vulnManager.Add(weakTlsVuln);

// Add Default Credentials
        Vuln defaultCredsVuln = VulnTemplatesGeneralES.DefaultCredentials;
        vulnManager.Add(defaultCredsVuln);

// Add Missing Security Updates
        Vuln missingUpdatesVuln = VulnTemplatesGeneralES.MissingSecurityUpdates;
        vulnManager.Add(missingUpdatesVuln);

// Add Unsecured Shares
        Vuln unsecuredSharesVuln = VulnTemplatesGeneralES.UnsecuredShares;
        vulnManager.Add(unsecuredSharesVuln);

// Add WinRM Misconfiguration
        Vuln winRmVuln = VulnTemplatesGeneralES.WinRMMisconfiguration;
        vulnManager.Add(winRmVuln);

// Add Print Spooler Vulnerabilities
        Vuln printSpoolerVuln = VulnTemplatesGeneralES.PrintSpoolerVulnerable;
        vulnManager.Add(printSpoolerVuln);

// Add Cached Domain Credentials
        Vuln cachedCredsVuln = VulnTemplatesGeneralES.CachedDomainCredentials;
        vulnManager.Add(cachedCredsVuln);

// Add BitLocker Misconfiguration
        Vuln bitLockerVuln = VulnTemplatesGeneralES.BitLockerMisconfiguration;
        vulnManager.Add(bitLockerVuln);

// Add WSUS Misconfiguration
        Vuln wsusVuln = VulnTemplatesGeneralES.WSUSMisconfiguration;
        vulnManager.Add(wsusVuln);

// Add IPv6 Security Issues
        Vuln ipv6Vuln = VulnTemplatesGeneralES.IPv6SecurityIssues;
        vulnManager.Add(ipv6Vuln);

// Add PowerShell Logging Gaps
        Vuln powerShellLoggingVuln = VulnTemplatesGeneralES.PowerShellLoggingGaps;
        vulnManager.Add(powerShellLoggingVuln);

// Add DnsAdmins Abuse
        Vuln dnsAdminsVuln = VulnTemplatesGeneralES.DnsAdminsAbuse;
        vulnManager.Add(dnsAdminsVuln);

// Add Exchange Misconfiguration
        Vuln exchangeVuln = VulnTemplatesGeneralES.ExchangeMisconfiguration;
        vulnManager.Add(exchangeVuln);

// Add Backup System Access
        Vuln backupSystemVuln = VulnTemplatesGeneralES.BackupSystemAccess;
        vulnManager.Add(backupSystemVuln);

// Add Certificate Template Vulnerabilities
        Vuln certTemplateVuln = VulnTemplatesGeneralES.CertificateTemplateVulns;
        vulnManager.Add(certTemplateVuln);

// Add SQL Server Misconfigurations
        Vuln sqlServerVuln = VulnTemplatesGeneralES.SQLServerMisconfigurations;
        vulnManager.Add(sqlServerVuln);

// Add RDP Security Issues
        Vuln rdpSecurityVuln = VulnTemplatesGeneralES.RDPSecurityIssues;
        vulnManager.Add(rdpSecurityVuln);

// Add Excessive User Rights
        Vuln excessiveUserRightsVuln = VulnTemplatesGeneralES.ExcessiveUserRights;
        vulnManager.Add(excessiveUserRightsVuln);

// Add Shadow Copy Abuse
        Vuln shadowCopyVuln = VulnTemplatesGeneralES.ShadowCopyAbuse;
        vulnManager.Add(shadowCopyVuln);

// Add Service Principal Misconfiguration
        Vuln servicePrincipalVuln = VulnTemplatesGeneralES.ServicePrincipalMisconfig;
        vulnManager.Add(servicePrincipalVuln);

// Add NTP Server Issues
        Vuln ntpServerVuln = VulnTemplatesGeneralES.NTPServerIssues;
        vulnManager.Add(ntpServerVuln);

// Add Linked Server Vulnerabilities
        Vuln linkedServerVuln = VulnTemplatesGeneralES.LinkedServerVulns;
        vulnManager.Add(linkedServerVuln);

// Add Defender Exclusions
        Vuln defenderExclusionsVuln = VulnTemplatesGeneralES.DefenderExclusions;
        vulnManager.Add(defenderExclusionsVuln);

// Add Domain Trust Issues
        Vuln domainTrustVuln = VulnTemplatesGeneralES.DomainTrustIssues;
        vulnManager.Add(domainTrustVuln);

// Add Hyper-V Security
        Vuln hyperVVuln = VulnTemplatesGeneralES.HyperVSecurity;
        vulnManager.Add(hyperVVuln);

// Add WSUS Targeting
        Vuln wsusTargetingVuln = VulnTemplatesGeneralES.WSUSTargeting;
        vulnManager.Add(wsusTargetingVuln);

// Add ADFS Security Issues
        Vuln adfsVuln = VulnTemplatesGeneralES.ADFSSecurityIssues;
        vulnManager.Add(adfsVuln);

// Add Constrained Delegation
        Vuln constrainedDelegationVuln = VulnTemplatesGeneralES.ConstrainedDelegation;
        vulnManager.Add(constrainedDelegationVuln);

// Add DNS Zone Transfer
        Vuln dnsZoneTransferVuln = VulnTemplatesGeneralES.DNSZoneTransfer;
        vulnManager.Add(dnsZoneTransferVuln);

// Add DFS Share Permissions
        Vuln dfsShareVuln = VulnTemplatesGeneralES.DFSSharePermissions;
        vulnManager.Add(dfsShareVuln);

// Add Network Device Misconfiguration
        Vuln networkDeviceVuln = VulnTemplatesGeneralES.NetworkDeviceMisconfig;
        vulnManager.Add(networkDeviceVuln);

// Add Azure AD Connect Issues
        Vuln azureADConnectVuln = VulnTemplatesGeneralES.AzureADConnectIssues;
        vulnManager.Add(azureADConnectVuln);

// Add SCCM Security Issues
        Vuln sccmVuln = VulnTemplatesGeneralES.SCCMSecurityIssues;
        vulnManager.Add(sccmVuln);

// Add Remote Access Policy Issues
        Vuln remoteAccessVuln = VulnTemplatesGeneralES.RemoteAccessPolicy;
        vulnManager.Add(remoteAccessVuln);

// Add ADCS Misconfiguration
        Vuln adcsVuln = VulnTemplatesGeneralES.ADCSMisconfiguration;
        vulnManager.Add(adcsVuln);

// Add SQL Server Agent Jobs
        Vuln sqlAgentJobsVuln = VulnTemplatesGeneralES.SQLServerAgentJobs;
        vulnManager.Add(sqlAgentJobsVuln);

// Add DHCP Misconfiguration
        Vuln dhcpVuln = VulnTemplatesGeneralES.DHCPMisconfiguration;
        vulnManager.Add(dhcpVuln);

// Add Windows Firewall Policy
        Vuln firewallPolicyVuln = VulnTemplatesGeneralES.WindowsFirewallPolicy;
        vulnManager.Add(firewallPolicyVuln);

// Add Password Filter DLL
        Vuln passwordFilterVuln = VulnTemplatesGeneralES.PasswordFilterDLL;
        vulnManager.Add(passwordFilterVuln);

// Add Unauthorized Sudo Access
        Vuln sudoAccessVuln = VulnTemplatesGeneralES.UnauthorizedSudoAccess;
        vulnManager.Add(sudoAccessVuln);

// Add Unencrypted Data Storage
        Vuln unencryptedDataVuln = VulnTemplatesGeneralES.UnencryptedDataStorage;
        vulnManager.Add(unencryptedDataVuln);

// Add Insecure File Permissions
        Vuln insecureFileVuln = VulnTemplatesGeneralES.InsecureFilePermissions;
        vulnManager.Add(insecureFileVuln);

// Add Insecure Service Configuration
        Vuln insecureServiceVuln = VulnTemplatesGeneralES.InsecureServiceConfiguration;
        vulnManager.Add(insecureServiceVuln);

// Add Weak Password Storage
        Vuln weakPasswordStorageVuln = VulnTemplatesGeneralES.WeakPasswordStorage;
        vulnManager.Add(weakPasswordStorageVuln);

// Add Unsecure Kernel Parameters
        Vuln unsecureKernelVuln = VulnTemplatesGeneralES.UnsecureKernelParameters;
        vulnManager.Add(unsecureKernelVuln);

// Add Unsecured Cron Jobs
        Vuln unsecuredCronVuln = VulnTemplatesGeneralES.UnsecuredCronJobs;
        vulnManager.Add(unsecuredCronVuln);

// Add Insecure Log Configuration
        Vuln insecureLogVuln = VulnTemplatesGeneralES.InsecureLogConfiguration;
        vulnManager.Add(insecureLogVuln);

// Add Insecure Time Synchronization
        Vuln insecureTimeVuln = VulnTemplatesGeneralES.InsecureTimeSynchronization;
        vulnManager.Add(insecureTimeVuln);

// Add Exposed Development Tools
        Vuln exposedDevToolsVuln = VulnTemplatesGeneralES.ExposedDevelopmentTools;
        vulnManager.Add(exposedDevToolsVuln);

// Add Insecure Automount Configuration
        Vuln insecureAutomountVuln = VulnTemplatesGeneralES.InsecureAutomountConfiguration;
        vulnManager.Add(insecureAutomountVuln);

// Add Unrestricted Core Dumps
        Vuln unrestrictedCoreDumpsVuln = VulnTemplatesGeneralES.UnrestrictedCoreDumps;
        vulnManager.Add(unrestrictedCoreDumpsVuln);

// Add Insecure LDAP Configuration
        Vuln insecureLdapVuln = VulnTemplatesGeneralES.InsecureLDAPConfiguration;
        vulnManager.Add(insecureLdapVuln);

// Add Missing Full Disk Encryption
        Vuln missingDiskEncryptionVuln = VulnTemplatesGeneralES.MissingDiskEncryption;
        vulnManager.Add(missingDiskEncryptionVuln);

// Add Insecure SSH Configuration
        Vuln insecureSshVuln = VulnTemplatesGeneralES.InsecureSSHConfiguration;
        vulnManager.Add(insecureSshVuln);

// Add Unrestricted USB Access
        Vuln unrestrictedUsbVuln = VulnTemplatesGeneralES.UnrestrictedUSBAccess;
        vulnManager.Add(unrestrictedUsbVuln);

// Add Insecure Backup Configuration
        Vuln insecureBackupVuln = VulnTemplatesGeneralES.InsecureBackupConfiguration;
        vulnManager.Add(insecureBackupVuln);

// Add Insecure Firewall Rules
        Vuln insecureFirewallVuln = VulnTemplatesGeneralES.InsecureFirewallRules;
        vulnManager.Add(insecureFirewallVuln);

// Add Insecure Home Directories
        Vuln insecureHomeVuln = VulnTemplatesGeneralES.InsecureHomeDirectories;
        vulnManager.Add(insecureHomeVuln);

// Add Insecure Keychain Configuration
        Vuln insecureKeychainVuln = VulnTemplatesGeneralES.InsecureKeychainConfiguration;
        vulnManager.Add(insecureKeychainVuln);

// Add Disabled System Integrity Protection
        Vuln disabledSipVuln = VulnTemplatesGeneralES.DisabledSystemIntegrityProtection;
        vulnManager.Add(disabledSipVuln);

// Add Unsecured Container Configuration
        Vuln unsecuredContainerVuln = VulnTemplatesGeneralES.UnsecuredContainerConfiguration;
        vulnManager.Add(unsecuredContainerVuln);

// Add Insecure PAM Configuration
        Vuln insecurePamVuln = VulnTemplatesGeneralES.InsecurePAMConfiguration;
        vulnManager.Add(insecurePamVuln);

// Add Insecure Audit Configuration
        Vuln insecureAuditVuln = VulnTemplatesGeneralES.InsecureAuditConfiguration;
        vulnManager.Add(insecureAuditVuln);

// Add Insecure X Server Configuration
        Vuln insecureXServerVuln = VulnTemplatesGeneralES.InsecureXServerConfiguration;
        vulnManager.Add(insecureXServerVuln);

// Add Insecure Resource Limits
        Vuln insecureResourceVuln = VulnTemplatesGeneralES.InsecureResourceLimits;
        vulnManager.Add(insecureResourceVuln);

// Add Insecure Kernel Module Loading
        Vuln insecureKernelModuleVuln = VulnTemplatesGeneralES.InsecureKernelModuleLoading;
        vulnManager.Add(insecureKernelModuleVuln);

// Add Insecure Syslog Configuration
        Vuln insecureSyslogVuln = VulnTemplatesGeneralES.InsecureSyslogConfiguration;
        vulnManager.Add(insecureSyslogVuln);

// Add Insecure Gatekeeper Configuration
        Vuln insecureGatekeeperVuln = VulnTemplatesGeneralES.InsecureGatekeeperConfiguration;
        vulnManager.Add(insecureGatekeeperVuln);

// Add Insecure Application Sandbox Configuration
        Vuln insecureSandboxVuln = VulnTemplatesGeneralES.InsecureSandboxConfiguration;
        vulnManager.Add(insecureSandboxVuln);

// Add Insecure Systemd Service Configuration
        Vuln insecureSystemdVuln = VulnTemplatesGeneralES.InsecureSystemdServiceConfiguration;
        vulnManager.Add(insecureSystemdVuln);

// Add Insecure XProtect Configuration
        Vuln insecureXProtectVuln = VulnTemplatesGeneralES.InsecureXProtectConfiguration;
        vulnManager.Add(insecureXProtectVuln);

// Add Insecure Memory Protection
        Vuln insecureMemoryVuln = VulnTemplatesGeneralES.InsecureMemoryProtection;
        vulnManager.Add(insecureMemoryVuln);

// Add Insecure IPC Configuration
        Vuln insecureIpcVuln = VulnTemplatesGeneralES.InsecureIPCConfiguration;
        vulnManager.Add(insecureIpcVuln);

// Add Insecure Process Accounting
        Vuln insecureProcessVuln = VulnTemplatesGeneralES.InsecureProcessAccounting;
        vulnManager.Add(insecureProcessVuln);

// Add AS-REP Roastable Accounts
        Vuln asRepRoastVuln = VulnTemplatesGeneralES.ASREPRoastableAccounts;
        vulnManager.Add(asRepRoastVuln);

// Add Insecure Delegation Configuration
        Vuln insecureDelegationVuln = VulnTemplatesGeneralES.InsecureDelegationConfiguration;
        vulnManager.Add(insecureDelegationVuln);

// Add Weak GPO Permissions
        Vuln weakGpoVuln = VulnTemplatesGeneralES.WeakGPOPermissions;
        vulnManager.Add(weakGpoVuln);

// Add DCOM Exploitation
        Vuln dcomVuln = VulnTemplatesGeneralES.DCOMExploitation;
        vulnManager.Add(dcomVuln);

// Add Weak Schema Permissions
        Vuln weakSchemaVuln = VulnTemplatesGeneralES.WeakSchemaPermissions;
        vulnManager.Add(weakSchemaVuln);

// Add Insecure LDAPS Configuration
        Vuln insecureLdapsVuln = VulnTemplatesGeneralES.InsecureLDAPSConfiguration;
        vulnManager.Add(insecureLdapsVuln);

// Add Privileged Group Membership
        Vuln privilegedGroupVuln = VulnTemplatesGeneralES.PrivilegedGroupMembership;
        vulnManager.Add(privilegedGroupVuln);

// Save all changes
        await vulnManager.Context.SaveChangesAsync();
    }

    public async Task CreateSpanishMastg()
    {
        Vuln insecureDataStorage = VulnTemplatesMastgES.InsecureDataStorage;
        insecureDataStorage.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insecureDataStorage.Id });
        vulnManager.Add(insecureDataStorage);
        

        Vuln insecureCryptography = VulnTemplatesMastgES.InsecureCryptography;
        insecureCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureCryptography.Id });
        vulnManager.Add(insecureCryptography);
        

        Vuln insecureAuthentication = VulnTemplatesMastgES.InsecureAuthentication;
        insecureAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureAuthentication.Id });
        vulnManager.Add(insecureAuthentication);
        

        Vuln insecureNetworkCommunication = VulnTemplatesMastgES.InsecureNetworkCommunication;
        insecureNetworkCommunication.VulnCwes.Add(new VulnCwe
            { CweId = 319, VulnId = insecureNetworkCommunication.Id });
        vulnManager.Add(insecureNetworkCommunication);
        

        Vuln privacyViolations = VulnTemplatesMastgES.PrivacyViolation;
        privacyViolations.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = privacyViolations.Id });
        vulnManager.Add(privacyViolations);
        

        Vuln insecureDataLeakage = VulnTemplatesMastgES.InsecureDataLeakage;
        insecureDataLeakage.VulnCwes.Add(new VulnCwe { CweId = 532, VulnId = insecureDataLeakage.Id });
        vulnManager.Add(insecureDataLeakage);
        

        Vuln insecureKeyManagement = VulnTemplatesMastgES.InsecureKeyManagement;
        insecureKeyManagement.VulnCwes.Add(new VulnCwe { CweId = 321, VulnId = insecureKeyManagement.Id });
        vulnManager.Add(insecureKeyManagement);
        

        Vuln insecureLocalAuthentication = VulnTemplatesMastgES.InsecureLocalAuthentication;
        insecureLocalAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureLocalAuthentication.Id });
        vulnManager.Add(insecureLocalAuthentication);
        

        Vuln insecureCertificatePinning = VulnTemplatesMastgES.InsecureCertificatePinning;
        insecureCertificatePinning.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureCertificatePinning.Id });
        vulnManager.Add(insecureCertificatePinning);
        

        Vuln insecureIPC = VulnTemplatesMastgES.InsecureIPC;
        insecureIPC.VulnCwes.Add(new VulnCwe { CweId = 927, VulnId = insecureIPC.Id });
        vulnManager.Add(insecureIPC);
        

        Vuln insecureWebview = VulnTemplatesMastgES.InsecureWebView;
        insecureWebview.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureWebview.Id });
        vulnManager.Add(insecureWebview);
        

        Vuln insecureDeepLinking = VulnTemplatesMastgES.InsecureDeepLinking;
        insecureDeepLinking.VulnCwes.Add(new VulnCwe { CweId = 939, VulnId = insecureDeepLinking.Id });
        vulnManager.Add(insecureDeepLinking);
        

        Vuln insecureSessionHandling = VulnTemplatesMastgES.InsecureSessionHandling;
        insecureSessionHandling.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionHandling.Id });
        vulnManager.Add(insecureSessionHandling);
        

        Vuln insecureTlsValidation = VulnTemplatesMastgES.InsecureTlsValidation;
        insecureTlsValidation.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureTlsValidation.Id });
        vulnManager.Add(insecureTlsValidation);
        

        Vuln insecureClipboardUsage = VulnTemplatesMastgES.InsecureClipboardUsage;
        insecureClipboardUsage.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureClipboardUsage.Id });
        vulnManager.Add(insecureClipboardUsage);
        

        Vuln insecureDataCaching = VulnTemplatesMastgES.InsecureDataCaching;
        insecureDataCaching.VulnCwes.Add(new VulnCwe { CweId = 524, VulnId = insecureDataCaching.Id });
        vulnManager.Add(insecureDataCaching);
        

        Vuln insecureBackupHandling = VulnTemplatesMastgES.InsecureBackupHandling;
        insecureBackupHandling.VulnCwes.Add(new VulnCwe { CweId = 530, VulnId = insecureBackupHandling.Id });
        vulnManager.Add(insecureBackupHandling);
        

        Vuln insufficientInputValidation = VulnTemplatesMastgES.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        

        Vuln insecureJailbreakRootDetection = VulnTemplatesMastgES.InsecureJailbreakRootDetection;
        insecureJailbreakRootDetection.VulnCwes.Add(new VulnCwe
            { CweId = 919, VulnId = insecureJailbreakRootDetection.Id });
        vulnManager.Add(insecureJailbreakRootDetection);
        

        Vuln insecureCodeObfuscation = VulnTemplatesMastgES.InsecureCodeObfuscation;
        insecureCodeObfuscation.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureCodeObfuscation.Id });
        vulnManager.Add(insecureCodeObfuscation);
        

        Vuln insecureRuntimeIntegrityChecks = VulnTemplatesMastgES.InsecureRuntimeIntegrityChecks;
        insecureRuntimeIntegrityChecks.VulnCwes.Add(new VulnCwe
            { CweId = 693, VulnId = insecureRuntimeIntegrityChecks.Id });
        vulnManager.Add(insecureRuntimeIntegrityChecks);
        

        Vuln insecureAppPackaging = VulnTemplatesMastgES.InsecureAppPackaging;
        insecureAppPackaging.VulnCwes.Add(new VulnCwe { CweId = 490, VulnId = insecureAppPackaging.Id });
        vulnManager.Add(insecureAppPackaging);
        

        Vuln insecureMemoryManagement = VulnTemplatesMastgES.InsecureMemoryManagement;
        insecureMemoryManagement.VulnCwes.Add(new VulnCwe { CweId = 316, VulnId = insecureMemoryManagement.Id });
        vulnManager.Add(insecureMemoryManagement);
        

        Vuln insecureComponentUpgrade = VulnTemplatesMastgES.InsecureComponentUpgrade;
        insecureComponentUpgrade.VulnCwes.Add(new VulnCwe { CweId = 494, VulnId = insecureComponentUpgrade.Id });
        vulnManager.Add(insecureComponentUpgrade);
        

        Vuln insecureDataResidency = VulnTemplatesMastgES.InsecureDataResidency;
        insecureDataResidency.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insecureDataResidency.Id });
        vulnManager.Add(insecureDataResidency);
        

        Vuln insecureCloudSyncmechanism = VulnTemplatesMastgES.InsecureCloudSyncMechanism;
        insecureCloudSyncmechanism.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insecureCloudSyncmechanism.Id });
        vulnManager.Add(insecureCloudSyncmechanism);
        

        Vuln vulnerableThirdPartyLibraries = VulnTemplatesMastgES.VulnerableThirdPartyLibrary;
        vulnerableThirdPartyLibraries.VulnCwes.Add(new VulnCwe
            { CweId = 1035, VulnId = vulnerableThirdPartyLibraries.Id });
        vulnManager.Add(vulnerableThirdPartyLibraries);
        

        Vuln insecureDataExfiltration = VulnTemplatesMastgES.InsecureDataExfiltration;
        insecureDataExfiltration.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureDataExfiltration.Id });
        vulnManager.Add(insecureDataExfiltration);
        

        Vuln insecureAPIVersioning = VulnTemplatesMastgES.InsecureAPIVersioning;
        insecureAPIVersioning.VulnCwes.Add(new VulnCwe { CweId = 330, VulnId = insecureAPIVersioning.Id });
        vulnManager.Add(insecureAPIVersioning);
        

        Vuln insecureQRCodeHandling = VulnTemplatesMastgES.InsecureQRCodeHandling;
        insecureQRCodeHandling.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insecureQRCodeHandling.Id });
        vulnManager.Add(insecureQRCodeHandling);
        

        Vuln insecureNFCImplementation = VulnTemplatesMastgES.InsecureNFCImplementation;
        insecureNFCImplementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureNFCImplementation.Id });
        vulnManager.Add(insecureNFCImplementation);
        

        Vuln insecureARImpementation = VulnTemplatesMastgES.InsecureARImplementation;
        insecureARImpementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureARImpementation.Id });
        vulnManager.Add(insecureARImpementation);
        

        Vuln insecureIoTImplementation = VulnTemplatesMastgES.InsecureIoTIntegration;
        insecureIoTImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureIoTImplementation.Id });
        vulnManager.Add(insecureIoTImplementation);
        

        Vuln insecurePushNotification = VulnTemplatesMastgES.InsecurePushNotifications;
        insecurePushNotification.VulnCwes.Add(new VulnCwe { CweId = 223, VulnId = insecurePushNotification.Id });
        vulnManager.Add(insecurePushNotification);
        

        Vuln insecureAppCloning = VulnTemplatesMastgES.InsecureAppCloning;
        insecureAppCloning.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureAppCloning.Id });
        vulnManager.Add(insecureAppCloning);
        

        Vuln insecureScreenOverlay = VulnTemplatesMastgES.InsecureScreenOverlay;
        insecureScreenOverlay.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = insecureScreenOverlay.Id });
        vulnManager.Add(insecureScreenOverlay);
        

        Vuln insecureAppWidget = VulnTemplatesMastgES.InsecureAppWidget;
        insecureAppWidget.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureAppWidget.Id });
        vulnManager.Add(insecureAppWidget);
        

        Vuln insecureEdgeComputingIntegration = VulnTemplatesMastgES.InsecureEdgeComputingIntegration;
        insecureEdgeComputingIntegration.VulnCwes.Add(new VulnCwe
            { CweId = 319, VulnId = insecureEdgeComputingIntegration.Id });
        vulnManager.Add(insecureEdgeComputingIntegration);
        

        Vuln insecureAIMLImplementation = VulnTemplatesMastgES.InsecureAIMLImplementation;
        insecureAIMLImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureAIMLImplementation.Id });
        vulnManager.Add(insecureAIMLImplementation);
        

        Vuln insecureQuantumResistantCrypto = VulnTemplatesMastgES.InsecureQuantumResistantCrypto;
        insecureQuantumResistantCrypto.VulnCwes.Add(new VulnCwe
            { CweId = 327, VulnId = insecureQuantumResistantCrypto.Id });
        vulnManager.Add(insecureQuantumResistantCrypto);
        

        Vuln insecureVoiceIntegration = VulnTemplatesMastgES.InsecureVoiceUIIntegration;
        insecureVoiceIntegration.VulnCwes.Add(new VulnCwe { CweId = 350, VulnId = insecureVoiceIntegration.Id });
        vulnManager.Add(insecureVoiceIntegration);
        

        Vuln insecureMultiDeviceSynchronization = VulnTemplatesMastgES.InsecureMultiDeviceSynchronization;
        insecureMultiDeviceSynchronization.VulnCwes.Add(new VulnCwe
            { CweId = 319, VulnId = insecureMultiDeviceSynchronization.Id });
        vulnManager.Add(insecureMultiDeviceSynchronization);
        

        Vuln insecureBlockchainIntegration = VulnTemplatesMastgES.InsecureBlockchainIntegration;
        insecureBlockchainIntegration.VulnCwes.Add(new VulnCwe
            { CweId = 320, VulnId = insecureBlockchainIntegration.Id });
        vulnManager.Add(insecureBlockchainIntegration);
        

        Vuln insecureKeychainKeystore = VulnTemplatesMastgES.InsecureKeychainKeystore;
        insecureKeychainKeystore.VulnCwes.Add(new VulnCwe { CweId = 522, VulnId = insecureKeychainKeystore.Id });
        vulnManager.Add(insecureKeychainKeystore);
        

        Vuln insecureRandomNumberGeneration = VulnTemplatesMastgES.InsecureRandomNumberGeneration;
        insecureRandomNumberGeneration.VulnCwes.Add(new VulnCwe
            { CweId = 338, VulnId = insecureRandomNumberGeneration.Id });
        vulnManager.Add(insecureRandomNumberGeneration);
        

        Vuln insecureSSOImplementation = VulnTemplatesMastgES.InsecureSSOImplementation;
        insecureSSOImplementation.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureSSOImplementation.Id });
        vulnManager.Add(insecureSSOImplementation);
        

        Vuln insecureVPNUsage = VulnTemplatesMastgES.InsecureVPNUsage;
        insecureVPNUsage.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureVPNUsage.Id });
        vulnManager.Add(insecureVPNUsage);
        

        Vuln insecureCustomURLScheme = VulnTemplatesMastgES.InsecureCustomURLScheme;
        insecureCustomURLScheme.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureCustomURLScheme.Id });
        vulnManager.Add(insecureCustomURLScheme);
        

        Vuln timeOfCheckToTimeOfUse = VulnTemplatesMastgES.TimeOfCheckToTimeOfUse;
        timeOfCheckToTimeOfUse.VulnCwes.Add(new VulnCwe { CweId = 367, VulnId = timeOfCheckToTimeOfUse.Id });
        vulnManager.Add(timeOfCheckToTimeOfUse);
        

        Vuln insecureAntiDebugging = VulnTemplatesMastgES.InsecureAntiDebugging;
        insecureAntiDebugging.VulnCwes.Add(new VulnCwe { CweId = 388, VulnId = insecureAntiDebugging.Id });
        vulnManager.Add(insecureAntiDebugging);
        

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
        

        Vuln weakPasswordRequirements = VulnTemplatesWstgES.WeakPasswordRequirements;
        weakPasswordRequirements.VulnCwes.Add(new VulnCwe { CweId = 521, VulnId = weakPasswordRequirements.Id });
        vulnManager.Add(weakPasswordRequirements);
        

        Vuln insecureSessionManagement = VulnTemplatesWstgES.InsecureSessionManagement;
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 384, VulnId = insecureSessionManagement.Id });
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionManagement.Id });
        vulnManager.Add(insecureSessionManagement);
        

        Vuln idor = VulnTemplatesWstgES.Idor;
        idor.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = idor.Id });
        vulnManager.Add(idor);
        

        Vuln crossSiteScripting = VulnTemplatesWstgES.CrossSiteScripting;
        crossSiteScripting.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = crossSiteScripting.Id });
        vulnManager.Add(crossSiteScripting);
        

        Vuln insecureCryptographicStorage = VulnTemplatesWstgES.InsecureCryptographicStorage;
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe
            { CweId = 327, VulnId = insecureCryptographicStorage.Id });
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe
            { CweId = 328, VulnId = insecureCryptographicStorage.Id });
        vulnManager.Add(insecureCryptographicStorage);
        

        Vuln insecureDeserialization = VulnTemplatesWstgES.InsecureDeserialization;
        insecureDeserialization.VulnCwes.Add(new VulnCwe { CweId = 502, VulnId = insecureDeserialization.Id });
        vulnManager.Add(insecureDeserialization);
        

        Vuln sqlInjection = VulnTemplatesWstgES.SqlInjection;
        sqlInjection.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = sqlInjection.Id });
        vulnManager.Add(sqlInjection);
        

        Vuln brokenAccessControl = VulnTemplatesWstgES.BrokenAccessControl;
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = brokenAccessControl.Id });
        vulnManager.Add(brokenAccessControl);
        

        Vuln securityMisconfiguration = VulnTemplatesWstgES.SecurityMisconfiguration;
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 16, VulnId = securityMisconfiguration.Id });
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 2, VulnId = securityMisconfiguration.Id });
        vulnManager.Add(securityMisconfiguration);
        

        Vuln sensitiveDataExposure = VulnTemplatesWstgES.SensitiveDataExposure;
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = sensitiveDataExposure.Id });
        vulnManager.Add(sensitiveDataExposure);
        

        Vuln insufficientLoggingMonitoring = VulnTemplatesWstgES.InsufficientLoggingAndMonitoring;
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe
            { CweId = 778, VulnId = insufficientLoggingMonitoring.Id });
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe
            { CweId = 223, VulnId = insufficientLoggingMonitoring.Id });
        vulnManager.Add(insufficientLoggingMonitoring);
        

        Vuln usingComponentsWithKnownVulnerabilities = VulnTemplatesWstgES.UsingComponentsWithKnownVulnerabilities;
        usingComponentsWithKnownVulnerabilities.VulnCwes.Add(new VulnCwe
            { CweId = 1035, VulnId = usingComponentsWithKnownVulnerabilities.Id });
        vulnManager.Add(usingComponentsWithKnownVulnerabilities);
        

        Vuln serverSideRequestForgery = VulnTemplatesWstgES.ServerSideRequestForgery;
        serverSideRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 918, VulnId = serverSideRequestForgery.Id });
        vulnManager.Add(serverSideRequestForgery);
        

        Vuln xmlExternalEntityProcessing = VulnTemplatesWstgES.XmlExternalEntityProcessing;
        xmlExternalEntityProcessing.VulnCwes.Add(new VulnCwe { CweId = 611, VulnId = xmlExternalEntityProcessing.Id });
        vulnManager.Add(xmlExternalEntityProcessing);
        

        Vuln lackOfrateLimiting = VulnTemplatesWstgES.LackOfRateLimiting;
        lackOfrateLimiting.VulnCwes.Add(new VulnCwe { CweId = 770, VulnId = lackOfrateLimiting.Id });
        vulnManager.Add(lackOfrateLimiting);
        

        Vuln inadequateOAuth2Implementation = VulnTemplatesWstgES.InadequateOAuth2Implementation;
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe
            { CweId = 346, VulnId = inadequateOAuth2Implementation.Id });
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe
            { CweId = 601, VulnId = inadequateOAuth2Implementation.Id });
        vulnManager.Add(inadequateOAuth2Implementation);
        

        Vuln insufficientAntiAutomation = VulnTemplatesWstgES.InsufficientAntiAutomation;
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 799, VulnId = insufficientAntiAutomation.Id });
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 837, VulnId = insufficientAntiAutomation.Id });
        vulnManager.Add(insufficientAntiAutomation);
        

        Vuln weakCryptographicAlgorithms = VulnTemplatesWstgES.WeakCryptographicAlgorithms;
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = weakCryptographicAlgorithms.Id });
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = weakCryptographicAlgorithms.Id });
        vulnManager.Add(weakCryptographicAlgorithms);
        

        Vuln improperCertificateValidation = VulnTemplatesWstgES.ImproperCertificateValidation;
        improperCertificateValidation.VulnCwes.Add(new VulnCwe
            { CweId = 295, VulnId = improperCertificateValidation.Id });
        vulnManager.Add(improperCertificateValidation);
        

        Vuln crossSiteRequestForgery = VulnTemplatesWstgES.CrossSiteRequestForgery;
        crossSiteRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 352, VulnId = crossSiteRequestForgery.Id });
        vulnManager.Add(crossSiteRequestForgery);
        

        Vuln insufficientPasswordRecovery = VulnTemplatesWstgES.InsufficientPasswordRecoveryMechanism;
        insufficientPasswordRecovery.VulnCwes.Add(new VulnCwe
            { CweId = 640, VulnId = insufficientPasswordRecovery.Id });
        vulnManager.Add(insufficientPasswordRecovery);
        

        Vuln lackOfInputSanitization = VulnTemplatesWstgES.LackOfInputSanitization;
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = lackOfInputSanitization.Id });
        vulnManager.Add(lackOfInputSanitization);
        

        Vuln insufficientSesssionTimeout = VulnTemplatesWstgES.InsufficientSessionTimeout;
        insufficientSesssionTimeout.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insufficientSesssionTimeout.Id });
        vulnManager.Add(insufficientSesssionTimeout);
        

        Vuln improperErrorHandling = VulnTemplatesWstgES.ImproperErrorHandling;
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 209, VulnId = improperErrorHandling.Id });
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = improperErrorHandling.Id });
        vulnManager.Add(improperErrorHandling);
        

        Vuln missingSecurityHeaders = VulnTemplatesWstgES.MissingSecurityHeaders;
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 693, VulnId = missingSecurityHeaders.Id });
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = missingSecurityHeaders.Id });
        vulnManager.Add(missingSecurityHeaders);
        

        Vuln insecureUseOfCryptography = VulnTemplatesWstgES.InsecureUseOfCryptography;
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureUseOfCryptography.Id });
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 320, VulnId = insecureUseOfCryptography.Id });
        vulnManager.Add(insecureUseOfCryptography);
        

        Vuln brokenFunctionLevelAuthorization = VulnTemplatesWstgES.BrokenFunctionLevelAuthorization;
        brokenFunctionLevelAuthorization.VulnCwes.Add(new VulnCwe
            { CweId = 285, VulnId = brokenFunctionLevelAuthorization.Id });
        vulnManager.Add(brokenFunctionLevelAuthorization);
        

        Vuln insecureCommunication = VulnTemplatesWstgES.InsecureCommunicationChannels;
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureCommunication.Id });
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = insecureCommunication.Id });
        vulnManager.Add(insecureCommunication);
        

        Vuln lackOfSecureDefaults = VulnTemplatesWstgES.LackOfSecureDefaults;
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 276, VulnId = lackOfSecureDefaults.Id });
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 1188, VulnId = lackOfSecureDefaults.Id });
        vulnManager.Add(lackOfSecureDefaults);
        

        Vuln insufficientProtectionFromAutomatedThreats =
            VulnTemplatesWstgES.InsufficientProtectionFromAutomatedThreats;
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe
            { CweId = 799, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe
            { CweId = 307, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        vulnManager.Add(insufficientProtectionFromAutomatedThreats);
        

        Vuln unvalidatedRedirectsAndForwards = VulnTemplatesWstgES.UnvalidatedRedirectsAndForwards;
        unvalidatedRedirectsAndForwards.VulnCwes.Add(new VulnCwe
            { CweId = 601, VulnId = unvalidatedRedirectsAndForwards.Id });
        vulnManager.Add(unvalidatedRedirectsAndForwards);
        

        Vuln insecureAuthenticationMechanism = VulnTemplatesWstgES.InsecureAuthenticationMechanism;
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe
            { CweId = 287, VulnId = insecureAuthenticationMechanism.Id });
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe
            { CweId = 384, VulnId = insecureAuthenticationMechanism.Id });
        vulnManager.Add(insecureAuthenticationMechanism);
        

        Vuln insuffificientAntiCachingHeaders = VulnTemplatesWstgES.InsufficientAntiCachingHeaders;
        insuffificientAntiCachingHeaders.VulnCwes.Add(new VulnCwe
            { CweId = 525, VulnId = insuffificientAntiCachingHeaders.Id });
        vulnManager.Add(insuffificientAntiCachingHeaders);
        

        Vuln lackOfProperTLSConfiguration = VulnTemplatesWstgES.LackOfProperTLSConfiguration;
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe
            { CweId = 326, VulnId = lackOfProperTLSConfiguration.Id });
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe
            { CweId = 327, VulnId = lackOfProperTLSConfiguration.Id });
        vulnManager.Add(lackOfProperTLSConfiguration);
        

        Vuln insufficientDataProtection = VulnTemplatesWstgES.InsufficientDataProtection;
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insufficientDataProtection.Id });
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insufficientDataProtection.Id });
        vulnManager.Add(insufficientDataProtection);
        

        Vuln improperAssetManagement = VulnTemplatesWstgES.ImproperAssetManagement;
        improperAssetManagement.VulnCwes.Add(new VulnCwe { CweId = 1059, VulnId = improperAssetManagement.Id });
        vulnManager.Add(improperAssetManagement);
        

        Vuln lackOfSoftwareUpdate = VulnTemplatesWstgES.LackOfSoftwareUpdates;
        lackOfSoftwareUpdate.VulnCwes.Add(new VulnCwe { CweId = 1104, VulnId = lackOfSoftwareUpdate.Id });
        vulnManager.Add(lackOfSoftwareUpdate);
        

        Vuln insufficientPrivacyControls = VulnTemplatesWstgES.InsufficientPrivacyControls;
        insufficientPrivacyControls.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insufficientPrivacyControls.Id });
        vulnManager.Add(insufficientPrivacyControls);
        

        Vuln insecureApiEndpoints = VulnTemplatesWstgES.InsecureAPIEndpoints;
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = insecureApiEndpoints.Id });
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = insecureApiEndpoints.Id });
        vulnManager.Add(insecureApiEndpoints);
        

        Vuln insufficientInputValidation = VulnTemplatesWstgES.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        

        Vuln improperOutputEncoding = VulnTemplatesWstgES.ImproperOutputEncoding;
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = improperOutputEncoding.Id });
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 116, VulnId = improperOutputEncoding.Id });
        vulnManager.Add(improperOutputEncoding);
        

        Vuln insecureFileHandling = VulnTemplatesWstgES.InsecureFileHandling;
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 73, VulnId = insecureFileHandling.Id });
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 434, VulnId = insecureFileHandling.Id });
        vulnManager.Add(insecureFileHandling);
        

        Vuln lackOfSecurePasswordStorage = VulnTemplatesWstgES.LackOfSecurePasswordStorage;
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 256, VulnId = lackOfSecurePasswordStorage.Id });
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 916, VulnId = lackOfSecurePasswordStorage.Id });
        vulnManager.Add(lackOfSecurePasswordStorage);
        

        Vuln insufficientProtectionAgainstDos = VulnTemplatesWstgES.InsufficientProtectionAgainstDoS;
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe
            { CweId = 400, VulnId = insufficientProtectionAgainstDos.Id });
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe
            { CweId = 770, VulnId = insufficientProtectionAgainstDos.Id });
        vulnManager.Add(insufficientProtectionAgainstDos);
        

        Vuln insufficientSubresourceIntegrity = VulnTemplatesWstgES.InsufficientSubresourceIntegrity;
        insufficientSubresourceIntegrity.VulnCwes.Add(new VulnCwe
            { CweId = 353, VulnId = insufficientSubresourceIntegrity.Id });
        vulnManager.Add(insufficientSubresourceIntegrity);
        

        Vuln inadequateSecurityArchitecture = VulnTemplatesWstgES.InadequateSecurityArchitecture;
        inadequateSecurityArchitecture.VulnCwes.Add(new VulnCwe
            { CweId = 1008, VulnId = inadequateSecurityArchitecture.Id });
        vulnManager.Add(inadequateSecurityArchitecture);
        

        Vuln flawedBusinessLogic = VulnTemplatesWstgES.FlawedBusinessLogic;
        flawedBusinessLogic.VulnCwes.Add(new VulnCwe { CweId = 840, VulnId = flawedBusinessLogic.Id });
        vulnManager.Add(flawedBusinessLogic);
        

        Vuln insecureThirdPartyComponents = VulnTemplatesWstgES.InsecureThirdPartyComponents;
        insecureThirdPartyComponents.VulnCwes.Add(
            new VulnCwe { CweId = 1104, VulnId = insecureThirdPartyComponents.Id });
        vulnManager.Add(insecureThirdPartyComponents);
        await vulnManager.Context.SaveChangesAsync();
    }

    public async Task CreatePortugueseGeneral()
    {
                 // Add Kerberoasting Vulnerability
        Vuln kerberoastingVuln = VulnTemplatesGeneralEN.KerberoastingVulnerability;
        vulnManager.Add(kerberoastingVuln);

// Add LLMNR Poisoning
        Vuln llmnrVuln = VulnTemplatesGeneralEN.LLMNRPoisoning;
        vulnManager.Add(llmnrVuln);

// Add Local Admin Password Reuse
        Vuln localAdminVuln = VulnTemplatesGeneralEN.LocalAdminPasswordReuse;
        vulnManager.Add(localAdminVuln);

// Add Unquoted Service Path
        Vuln unquotedServiceVuln = VulnTemplatesGeneralEN.UnquotedServicePath;
        vulnManager.Add(unquotedServiceVuln);

// Add SMB Signing Issues
        Vuln smbSigningVuln = VulnTemplatesGeneralEN.SMBSigningDisabled;
        vulnManager.Add(smbSigningVuln);

// Add Excessive Domain Admin Rights
        Vuln domainAdminVuln = VulnTemplatesGeneralEN.ExcessiveDomainAdminRights;
        vulnManager.Add(domainAdminVuln);

// Add GPP Password Exposure
        Vuln gppPasswordVuln = VulnTemplatesGeneralEN.GPPPasswordExposure;
        vulnManager.Add(gppPasswordVuln);

// Add Weak Service Account Permissions
        Vuln weakServiceAccountVuln = VulnTemplatesGeneralEN.WeakServiceAccountPermissions;
        vulnManager.Add(weakServiceAccountVuln);

// Add Clear Text Protocols
        Vuln clearTextVuln = VulnTemplatesGeneralEN.ClearTextProtocols;
        vulnManager.Add(clearTextVuln);

// Add Insecure LDAP Binding
        Vuln ldapBindingVuln = VulnTemplatesGeneralEN.InsecureLDAPBinding;
        vulnManager.Add(ldapBindingVuln);

// Add Weak TLS Configuration
        Vuln weakTlsVuln = VulnTemplatesGeneralEN.WeakTLSConfiguration;
        vulnManager.Add(weakTlsVuln);

// Add Default Credentials
        Vuln defaultCredsVuln = VulnTemplatesGeneralEN.DefaultCredentials;
        vulnManager.Add(defaultCredsVuln);

// Add Missing Security Updates
        Vuln missingUpdatesVuln = VulnTemplatesGeneralEN.MissingSecurityUpdates;
        vulnManager.Add(missingUpdatesVuln);

// Add Unsecured Shares
        Vuln unsecuredSharesVuln = VulnTemplatesGeneralEN.UnsecuredShares;
        vulnManager.Add(unsecuredSharesVuln);

// Add WinRM Misconfiguration
        Vuln winRmVuln = VulnTemplatesGeneralEN.WinRMMisconfiguration;
        vulnManager.Add(winRmVuln);

// Add Print Spooler Vulnerabilities
        Vuln printSpoolerVuln = VulnTemplatesGeneralEN.PrintSpoolerVulnerable;
        vulnManager.Add(printSpoolerVuln);

// Add Cached Domain Credentials
        Vuln cachedCredsVuln = VulnTemplatesGeneralEN.CachedDomainCredentials;
        vulnManager.Add(cachedCredsVuln);

// Add BitLocker Misconfiguration
        Vuln bitLockerVuln = VulnTemplatesGeneralEN.BitLockerMisconfiguration;
        vulnManager.Add(bitLockerVuln);

// Add WSUS Misconfiguration
        Vuln wsusVuln = VulnTemplatesGeneralEN.WSUSMisconfiguration;
        vulnManager.Add(wsusVuln);

// Add IPv6 Security Issues
        Vuln ipv6Vuln = VulnTemplatesGeneralEN.IPv6SecurityIssues;
        vulnManager.Add(ipv6Vuln);

// Add PowerShell Logging Gaps
        Vuln powerShellLoggingVuln = VulnTemplatesGeneralEN.PowerShellLoggingGaps;
        vulnManager.Add(powerShellLoggingVuln);

// Add DnsAdmins Abuse
        Vuln dnsAdminsVuln = VulnTemplatesGeneralEN.DnsAdminsAbuse;
        vulnManager.Add(dnsAdminsVuln);

// Add Exchange Misconfiguration
        Vuln exchangeVuln = VulnTemplatesGeneralEN.ExchangeMisconfiguration;
        vulnManager.Add(exchangeVuln);

// Add Backup System Access
        Vuln backupSystemVuln = VulnTemplatesGeneralEN.BackupSystemAccess;
        vulnManager.Add(backupSystemVuln);

// Add Certificate Template Vulnerabilities
        Vuln certTemplateVuln = VulnTemplatesGeneralEN.CertificateTemplateVulns;
        vulnManager.Add(certTemplateVuln);

// Add SQL Server Misconfigurations
        Vuln sqlServerVuln = VulnTemplatesGeneralEN.SQLServerMisconfigurations;
        vulnManager.Add(sqlServerVuln);

// Add RDP Security Issues
        Vuln rdpSecurityVuln = VulnTemplatesGeneralEN.RDPSecurityIssues;
        vulnManager.Add(rdpSecurityVuln);

// Add Excessive User Rights
        Vuln excessiveUserRightsVuln = VulnTemplatesGeneralEN.ExcessiveUserRights;
        vulnManager.Add(excessiveUserRightsVuln);

// Add Shadow Copy Abuse
        Vuln shadowCopyVuln = VulnTemplatesGeneralEN.ShadowCopyAbuse;
        vulnManager.Add(shadowCopyVuln);

// Add Service Principal Misconfiguration
        Vuln servicePrincipalVuln = VulnTemplatesGeneralEN.ServicePrincipalMisconfig;
        vulnManager.Add(servicePrincipalVuln);

// Add NTP Server Issues
        Vuln ntpServerVuln = VulnTemplatesGeneralEN.NTPServerIssues;
        vulnManager.Add(ntpServerVuln);

// Add Linked Server Vulnerabilities
        Vuln linkedServerVuln = VulnTemplatesGeneralEN.LinkedServerVulns;
        vulnManager.Add(linkedServerVuln);

// Add Defender Exclusions
        Vuln defenderExclusionsVuln = VulnTemplatesGeneralEN.DefenderExclusions;
        vulnManager.Add(defenderExclusionsVuln);

// Add Domain Trust Issues
        Vuln domainTrustVuln = VulnTemplatesGeneralEN.DomainTrustIssues;
        vulnManager.Add(domainTrustVuln);

// Add Hyper-V Security
        Vuln hyperVVuln = VulnTemplatesGeneralEN.HyperVSecurity;
        vulnManager.Add(hyperVVuln);

// Add WSUS Targeting
        Vuln wsusTargetingVuln = VulnTemplatesGeneralEN.WSUSTargeting;
        vulnManager.Add(wsusTargetingVuln);

// Add ADFS Security Issues
        Vuln adfsVuln = VulnTemplatesGeneralEN.ADFSSecurityIssues;
        vulnManager.Add(adfsVuln);

// Add Constrained Delegation
        Vuln constrainedDelegationVuln = VulnTemplatesGeneralEN.ConstrainedDelegation;
        vulnManager.Add(constrainedDelegationVuln);

// Add DNS Zone Transfer
        Vuln dnsZoneTransferVuln = VulnTemplatesGeneralEN.DNSZoneTransfer;
        vulnManager.Add(dnsZoneTransferVuln);

// Add DFS Share Permissions
        Vuln dfsShareVuln = VulnTemplatesGeneralEN.DFSSharePermissions;
        vulnManager.Add(dfsShareVuln);

// Add Network Device Misconfiguration
        Vuln networkDeviceVuln = VulnTemplatesGeneralEN.NetworkDeviceMisconfig;
        vulnManager.Add(networkDeviceVuln);

// Add Azure AD Connect Issues
        Vuln azureADConnectVuln = VulnTemplatesGeneralEN.AzureADConnectIssues;
        vulnManager.Add(azureADConnectVuln);

// Add SCCM Security Issues
        Vuln sccmVuln = VulnTemplatesGeneralEN.SCCMSecurityIssues;
        vulnManager.Add(sccmVuln);

// Add Remote Access Policy Issues
        Vuln remoteAccessVuln = VulnTemplatesGeneralEN.RemoteAccessPolicy;
        vulnManager.Add(remoteAccessVuln);

// Add ADCS Misconfiguration
        Vuln adcsVuln = VulnTemplatesGeneralEN.ADCSMisconfiguration;
        vulnManager.Add(adcsVuln);

// Add SQL Server Agent Jobs
        Vuln sqlAgentJobsVuln = VulnTemplatesGeneralEN.SQLServerAgentJobs;
        vulnManager.Add(sqlAgentJobsVuln);

// Add DHCP Misconfiguration
        Vuln dhcpVuln = VulnTemplatesGeneralEN.DHCPMisconfiguration;
        vulnManager.Add(dhcpVuln);

// Add Windows Firewall Policy
        Vuln firewallPolicyVuln = VulnTemplatesGeneralEN.WindowsFirewallPolicy;
        vulnManager.Add(firewallPolicyVuln);

// Add Password Filter DLL
        Vuln passwordFilterVuln = VulnTemplatesGeneralEN.PasswordFilterDLL;
        vulnManager.Add(passwordFilterVuln);

// Add Unauthorized Sudo Access
        Vuln sudoAccessVuln = VulnTemplatesGeneralEN.UnauthorizedSudoAccess;
        vulnManager.Add(sudoAccessVuln);

// Add Unencrypted Data Storage
        Vuln unencryptedDataVuln = VulnTemplatesGeneralEN.UnencryptedDataStorage;
        vulnManager.Add(unencryptedDataVuln);

// Add Insecure File Permissions
        Vuln insecureFileVuln = VulnTemplatesGeneralEN.InsecureFilePermissions;
        vulnManager.Add(insecureFileVuln);

// Add Insecure Service Configuration
        Vuln insecureServiceVuln = VulnTemplatesGeneralEN.InsecureServiceConfiguration;
        vulnManager.Add(insecureServiceVuln);

// Add Weak Password Storage
        Vuln weakPasswordStorageVuln = VulnTemplatesGeneralEN.WeakPasswordStorage;
        vulnManager.Add(weakPasswordStorageVuln);

// Add Unsecure Kernel Parameters
        Vuln unsecureKernelVuln = VulnTemplatesGeneralEN.UnsecureKernelParameters;
        vulnManager.Add(unsecureKernelVuln);

// Add Unsecured Cron Jobs
        Vuln unsecuredCronVuln = VulnTemplatesGeneralEN.UnsecuredCronJobs;
        vulnManager.Add(unsecuredCronVuln);

// Add Insecure Log Configuration
        Vuln insecureLogVuln = VulnTemplatesGeneralEN.InsecureLogConfiguration;
        vulnManager.Add(insecureLogVuln);

// Add Insecure Time Synchronization
        Vuln insecureTimeVuln = VulnTemplatesGeneralEN.InsecureTimeSynchronization;
        vulnManager.Add(insecureTimeVuln);

// Add Exposed Development Tools
        Vuln exposedDevToolsVuln = VulnTemplatesGeneralEN.ExposedDevelopmentTools;
        vulnManager.Add(exposedDevToolsVuln);

// Add Insecure Automount Configuration
        Vuln insecureAutomountVuln = VulnTemplatesGeneralEN.InsecureAutomountConfiguration;
        vulnManager.Add(insecureAutomountVuln);

// Add Unrestricted Core Dumps
        Vuln unrestrictedCoreDumpsVuln = VulnTemplatesGeneralEN.UnrestrictedCoreDumps;
        vulnManager.Add(unrestrictedCoreDumpsVuln);

// Add Insecure LDAP Configuration
        Vuln insecureLdapVuln = VulnTemplatesGeneralEN.InsecureLDAPConfiguration;
        vulnManager.Add(insecureLdapVuln);

// Add Missing Full Disk Encryption
        Vuln missingDiskEncryptionVuln = VulnTemplatesGeneralEN.MissingDiskEncryption;
        vulnManager.Add(missingDiskEncryptionVuln);

// Add Insecure SSH Configuration
        Vuln insecureSshVuln = VulnTemplatesGeneralEN.InsecureSSHConfiguration;
        vulnManager.Add(insecureSshVuln);

// Add Unrestricted USB Access
        Vuln unrestrictedUsbVuln = VulnTemplatesGeneralEN.UnrestrictedUSBAccess;
        vulnManager.Add(unrestrictedUsbVuln);

// Add Insecure Backup Configuration
        Vuln insecureBackupVuln = VulnTemplatesGeneralEN.InsecureBackupConfiguration;
        vulnManager.Add(insecureBackupVuln);

// Add Insecure Firewall Rules
        Vuln insecureFirewallVuln = VulnTemplatesGeneralEN.InsecureFirewallRules;
        vulnManager.Add(insecureFirewallVuln);

// Add Insecure Home Directories
        Vuln insecureHomeVuln = VulnTemplatesGeneralEN.InsecureHomeDirectories;
        vulnManager.Add(insecureHomeVuln);

// Add Insecure Keychain Configuration
        Vuln insecureKeychainVuln = VulnTemplatesGeneralEN.InsecureKeychainConfiguration;
        vulnManager.Add(insecureKeychainVuln);

// Add Disabled System Integrity Protection
        Vuln disabledSipVuln = VulnTemplatesGeneralEN.DisabledSystemIntegrityProtection;
        vulnManager.Add(disabledSipVuln);

// Add Unsecured Container Configuration
        Vuln unsecuredContainerVuln = VulnTemplatesGeneralEN.UnsecuredContainerConfiguration;
        vulnManager.Add(unsecuredContainerVuln);

// Add Insecure PAM Configuration
        Vuln insecurePamVuln = VulnTemplatesGeneralEN.InsecurePAMConfiguration;
        vulnManager.Add(insecurePamVuln);

// Add Insecure Audit Configuration
        Vuln insecureAuditVuln = VulnTemplatesGeneralEN.InsecureAuditConfiguration;
        vulnManager.Add(insecureAuditVuln);

// Add Insecure X Server Configuration
        Vuln insecureXServerVuln = VulnTemplatesGeneralEN.InsecureXServerConfiguration;
        vulnManager.Add(insecureXServerVuln);

// Add Insecure Resource Limits
        Vuln insecureResourceVuln = VulnTemplatesGeneralEN.InsecureResourceLimits;
        vulnManager.Add(insecureResourceVuln);

// Add Insecure Kernel Module Loading
        Vuln insecureKernelModuleVuln = VulnTemplatesGeneralEN.InsecureKernelModuleLoading;
        vulnManager.Add(insecureKernelModuleVuln);

// Add Insecure Syslog Configuration
        Vuln insecureSyslogVuln = VulnTemplatesGeneralEN.InsecureSyslogConfiguration;
        vulnManager.Add(insecureSyslogVuln);

// Add Insecure Gatekeeper Configuration
        Vuln insecureGatekeeperVuln = VulnTemplatesGeneralEN.InsecureGatekeeperConfiguration;
        vulnManager.Add(insecureGatekeeperVuln);

// Add Insecure Application Sandbox Configuration
        Vuln insecureSandboxVuln = VulnTemplatesGeneralEN.InsecureSandboxConfiguration;
        vulnManager.Add(insecureSandboxVuln);

// Add Insecure Systemd Service Configuration
        Vuln insecureSystemdVuln = VulnTemplatesGeneralEN.InsecureSystemdServiceConfiguration;
        vulnManager.Add(insecureSystemdVuln);

// Add Insecure XProtect Configuration
        Vuln insecureXProtectVuln = VulnTemplatesGeneralEN.InsecureXProtectConfiguration;
        vulnManager.Add(insecureXProtectVuln);

// Add Insecure Memory Protection
        Vuln insecureMemoryVuln = VulnTemplatesGeneralEN.InsecureMemoryProtection;
        vulnManager.Add(insecureMemoryVuln);

// Add Insecure IPC Configuration
        Vuln insecureIpcVuln = VulnTemplatesGeneralEN.InsecureIPCConfiguration;
        vulnManager.Add(insecureIpcVuln);

// Add Insecure Process Accounting
        Vuln insecureProcessVuln = VulnTemplatesGeneralEN.InsecureProcessAccounting;
        vulnManager.Add(insecureProcessVuln);

// Add AS-REP Roastable Accounts
        Vuln asRepRoastVuln = VulnTemplatesGeneralEN.ASREPRoastableAccounts;
        vulnManager.Add(asRepRoastVuln);

// Add Insecure Delegation Configuration
        Vuln insecureDelegationVuln = VulnTemplatesGeneralEN.InsecureDelegationConfiguration;
        vulnManager.Add(insecureDelegationVuln);

// Add Weak GPO Permissions
        Vuln weakGpoVuln = VulnTemplatesGeneralEN.WeakGPOPermissions;
        vulnManager.Add(weakGpoVuln);

// Add DCOM Exploitation
        Vuln dcomVuln = VulnTemplatesGeneralEN.DCOMExploitation;
        vulnManager.Add(dcomVuln);

// Add Weak Schema Permissions
        Vuln weakSchemaVuln = VulnTemplatesGeneralEN.WeakSchemaPermissions;
        vulnManager.Add(weakSchemaVuln);

// Add Insecure LDAPS Configuration
        Vuln insecureLdapsVuln = VulnTemplatesGeneralEN.InsecureLDAPSConfiguration;
        vulnManager.Add(insecureLdapsVuln);

// Add Privileged Group Membership
        Vuln privilegedGroupVuln = VulnTemplatesGeneralEN.PrivilegedGroupMembership;
        vulnManager.Add(privilegedGroupVuln);

// Save all changes
        await vulnManager.Context.SaveChangesAsync();
       
    }

    public async Task CreatePortugueseMastg()
    {
        Vuln insecureDataStorage = VulnTemplatesMastgPT.InsecureDataStorage;
        insecureDataStorage.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insecureDataStorage.Id });
        vulnManager.Add(insecureDataStorage);
        

        Vuln insecureCryptography = VulnTemplatesMastgPT.InsecureCryptography;
        insecureCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureCryptography.Id });
        vulnManager.Add(insecureCryptography);
        

        Vuln insecureAuthentication = VulnTemplatesMastgPT.InsecureAuthentication;
        insecureAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureAuthentication.Id });
        vulnManager.Add(insecureAuthentication);
        

        Vuln insecureNetworkCommunication = VulnTemplatesMastgPT.InsecureNetworkCommunication;
        insecureNetworkCommunication.VulnCwes.Add(new VulnCwe
            { CweId = 319, VulnId = insecureNetworkCommunication.Id });
        vulnManager.Add(insecureNetworkCommunication);
        

        Vuln privacyViolations = VulnTemplatesMastgPT.PrivacyViolation;
        privacyViolations.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = privacyViolations.Id });
        vulnManager.Add(privacyViolations);
        

        Vuln insecureDataLeakage = VulnTemplatesMastgPT.InsecureDataLeakage;
        insecureDataLeakage.VulnCwes.Add(new VulnCwe { CweId = 532, VulnId = insecureDataLeakage.Id });
        vulnManager.Add(insecureDataLeakage);
        

        Vuln insecureKeyManagement = VulnTemplatesMastgPT.InsecureKeyManagement;
        insecureKeyManagement.VulnCwes.Add(new VulnCwe { CweId = 321, VulnId = insecureKeyManagement.Id });
        vulnManager.Add(insecureKeyManagement);
        

        Vuln insecureLocalAuthentication = VulnTemplatesMastgPT.InsecureLocalAuthentication;
        insecureLocalAuthentication.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureLocalAuthentication.Id });
        vulnManager.Add(insecureLocalAuthentication);
        

        Vuln insecureCertificatePinning = VulnTemplatesMastgPT.InsecureCertificatePinning;
        insecureCertificatePinning.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureCertificatePinning.Id });
        vulnManager.Add(insecureCertificatePinning);
        

        Vuln insecureIPC = VulnTemplatesMastgPT.InsecureIPC;
        insecureIPC.VulnCwes.Add(new VulnCwe { CweId = 927, VulnId = insecureIPC.Id });
        vulnManager.Add(insecureIPC);
        

        Vuln insecureWebview = VulnTemplatesMastgPT.InsecureWebView;
        insecureWebview.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureWebview.Id });
        vulnManager.Add(insecureWebview);
        

        Vuln insecureDeepLinking = VulnTemplatesMastgPT.InsecureDeepLinking;
        insecureDeepLinking.VulnCwes.Add(new VulnCwe { CweId = 939, VulnId = insecureDeepLinking.Id });
        vulnManager.Add(insecureDeepLinking);
        

        Vuln insecureSessionHandling = VulnTemplatesMastgPT.InsecureSessionHandling;
        insecureSessionHandling.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionHandling.Id });
        vulnManager.Add(insecureSessionHandling);
        

        Vuln insecureTlsValidation = VulnTemplatesMastgPT.InsecureTlsValidation;
        insecureTlsValidation.VulnCwes.Add(new VulnCwe { CweId = 295, VulnId = insecureTlsValidation.Id });
        vulnManager.Add(insecureTlsValidation);
        

        Vuln insecureClipboardUsage = VulnTemplatesMastgPT.InsecureClipboardUsage;
        insecureClipboardUsage.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureClipboardUsage.Id });
        vulnManager.Add(insecureClipboardUsage);
        

        Vuln insecureDataCaching = VulnTemplatesMastgPT.InsecureDataCaching;
        insecureDataCaching.VulnCwes.Add(new VulnCwe { CweId = 524, VulnId = insecureDataCaching.Id });
        vulnManager.Add(insecureDataCaching);
        

        Vuln insecureBackupHandling = VulnTemplatesMastgPT.InsecureBackupHandling;
        insecureBackupHandling.VulnCwes.Add(new VulnCwe { CweId = 530, VulnId = insecureBackupHandling.Id });
        vulnManager.Add(insecureBackupHandling);
        

        Vuln insufficientInputValidation = VulnTemplatesMastgPT.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        

        Vuln insecureJailbreakRootDetection = VulnTemplatesMastgPT.InsecureJailbreakRootDetection;
        insecureJailbreakRootDetection.VulnCwes.Add(new VulnCwe
            { CweId = 919, VulnId = insecureJailbreakRootDetection.Id });
        vulnManager.Add(insecureJailbreakRootDetection);
        

        Vuln insecureCodeObfuscation = VulnTemplatesMastgPT.InsecureCodeObfuscation;
        insecureCodeObfuscation.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureCodeObfuscation.Id });
        vulnManager.Add(insecureCodeObfuscation);
        

        Vuln insecureRuntimeIntegrityChecks = VulnTemplatesMastgPT.InsecureRuntimeIntegrityChecks;
        insecureRuntimeIntegrityChecks.VulnCwes.Add(new VulnCwe
            { CweId = 693, VulnId = insecureRuntimeIntegrityChecks.Id });
        vulnManager.Add(insecureRuntimeIntegrityChecks);
        

        Vuln insecureAppPackaging = VulnTemplatesMastgPT.InsecureAppPackaging;
        insecureAppPackaging.VulnCwes.Add(new VulnCwe { CweId = 490, VulnId = insecureAppPackaging.Id });
        vulnManager.Add(insecureAppPackaging);
        

        Vuln insecureMemoryManagement = VulnTemplatesMastgPT.InsecureMemoryManagement;
        insecureMemoryManagement.VulnCwes.Add(new VulnCwe { CweId = 316, VulnId = insecureMemoryManagement.Id });
        vulnManager.Add(insecureMemoryManagement);
        

        Vuln insecureComponentUpgrade = VulnTemplatesMastgPT.InsecureComponentUpgrade;
        insecureComponentUpgrade.VulnCwes.Add(new VulnCwe { CweId = 494, VulnId = insecureComponentUpgrade.Id });
        vulnManager.Add(insecureComponentUpgrade);
        

        Vuln insecureDataResidency = VulnTemplatesMastgPT.InsecureDataResidency;
        insecureDataResidency.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insecureDataResidency.Id });
        vulnManager.Add(insecureDataResidency);
        

        Vuln insecureCloudSyncmechanism = VulnTemplatesMastgPT.InsecureCloudSyncMechanism;
        insecureCloudSyncmechanism.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insecureCloudSyncmechanism.Id });
        vulnManager.Add(insecureCloudSyncmechanism);
        

        Vuln vulnerableThirdPartyLibraries = VulnTemplatesMastgPT.VulnerableThirdPartyLibrary;
        vulnerableThirdPartyLibraries.VulnCwes.Add(new VulnCwe
            { CweId = 1035, VulnId = vulnerableThirdPartyLibraries.Id });
        vulnManager.Add(vulnerableThirdPartyLibraries);
        

        Vuln insecureDataExfiltration = VulnTemplatesMastgPT.InsecureDataExfiltration;
        insecureDataExfiltration.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureDataExfiltration.Id });
        vulnManager.Add(insecureDataExfiltration);
        

        Vuln insecureAPIVersioning = VulnTemplatesMastgPT.InsecureAPIVersioning;
        insecureAPIVersioning.VulnCwes.Add(new VulnCwe { CweId = 330, VulnId = insecureAPIVersioning.Id });
        vulnManager.Add(insecureAPIVersioning);
        

        Vuln insecureQRCodeHandling = VulnTemplatesMastgPT.InsecureQRCodeHandling;
        insecureQRCodeHandling.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insecureQRCodeHandling.Id });
        vulnManager.Add(insecureQRCodeHandling);
        

        Vuln insecureNFCImplementation = VulnTemplatesMastgPT.InsecureNFCImplementation;
        insecureNFCImplementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureNFCImplementation.Id });
        vulnManager.Add(insecureNFCImplementation);
        

        Vuln insecureARImpementation = VulnTemplatesMastgPT.InsecureARImplementation;
        insecureARImpementation.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureARImpementation.Id });
        vulnManager.Add(insecureARImpementation);
        

        Vuln insecureIoTImplementation = VulnTemplatesMastgPT.InsecureIoTIntegration;
        insecureIoTImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureIoTImplementation.Id });
        vulnManager.Add(insecureIoTImplementation);
        

        Vuln insecurePushNotification = VulnTemplatesMastgPT.InsecurePushNotifications;
        insecurePushNotification.VulnCwes.Add(new VulnCwe { CweId = 223, VulnId = insecurePushNotification.Id });
        vulnManager.Add(insecurePushNotification);
        

        Vuln insecureAppCloning = VulnTemplatesMastgPT.InsecureAppCloning;
        insecureAppCloning.VulnCwes.Add(new VulnCwe { CweId = 656, VulnId = insecureAppCloning.Id });
        vulnManager.Add(insecureAppCloning);
        

        Vuln insecureScreenOverlay = VulnTemplatesMastgPT.InsecureScreenOverlay;
        insecureScreenOverlay.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = insecureScreenOverlay.Id });
        vulnManager.Add(insecureScreenOverlay);
        

        Vuln insecureAppWidget = VulnTemplatesMastgPT.InsecureAppWidget;
        insecureAppWidget.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = insecureAppWidget.Id });
        vulnManager.Add(insecureAppWidget);
        

        Vuln insecureEdgeComputingIntegration = VulnTemplatesMastgPT.InsecureEdgeComputingIntegration;
        insecureEdgeComputingIntegration.VulnCwes.Add(new VulnCwe
            { CweId = 319, VulnId = insecureEdgeComputingIntegration.Id });
        vulnManager.Add(insecureEdgeComputingIntegration);
        

        Vuln insecureAIMLImplementation = VulnTemplatesMastgPT.InsecureAIMLImplementation;
        insecureAIMLImplementation.VulnCwes.Add(new VulnCwe { CweId = 306, VulnId = insecureAIMLImplementation.Id });
        vulnManager.Add(insecureAIMLImplementation);
        

        Vuln insecureQuantumResistantCrypto = VulnTemplatesMastgPT.InsecureQuantumResistantCrypto;
        insecureQuantumResistantCrypto.VulnCwes.Add(new VulnCwe
            { CweId = 327, VulnId = insecureQuantumResistantCrypto.Id });
        vulnManager.Add(insecureQuantumResistantCrypto);
        

        Vuln insecureVoiceIntegration = VulnTemplatesMastgPT.InsecureVoiceUIIntegration;
        insecureVoiceIntegration.VulnCwes.Add(new VulnCwe { CweId = 350, VulnId = insecureVoiceIntegration.Id });
        vulnManager.Add(insecureVoiceIntegration);
        

        Vuln insecureMultiDeviceSynchronization = VulnTemplatesMastgPT.InsecureMultiDeviceSynchronization;
        insecureMultiDeviceSynchronization.VulnCwes.Add(new VulnCwe
            { CweId = 319, VulnId = insecureMultiDeviceSynchronization.Id });
        vulnManager.Add(insecureMultiDeviceSynchronization);
        

        Vuln insecureBlockchainIntegration = VulnTemplatesMastgPT.InsecureBlockchainIntegration;
        insecureBlockchainIntegration.VulnCwes.Add(new VulnCwe
            { CweId = 320, VulnId = insecureBlockchainIntegration.Id });
        vulnManager.Add(insecureBlockchainIntegration);
        

        Vuln insecureKeychainKeystore = VulnTemplatesMastgPT.InsecureKeychainKeystore;
        insecureKeychainKeystore.VulnCwes.Add(new VulnCwe { CweId = 522, VulnId = insecureKeychainKeystore.Id });
        vulnManager.Add(insecureKeychainKeystore);
        

        Vuln insecureRandomNumberGeneration = VulnTemplatesMastgPT.InsecureRandomNumberGeneration;
        insecureRandomNumberGeneration.VulnCwes.Add(new VulnCwe
            { CweId = 338, VulnId = insecureRandomNumberGeneration.Id });
        vulnManager.Add(insecureRandomNumberGeneration);
        

        Vuln insecureSSOImplementation = VulnTemplatesMastgPT.InsecureSSOImplementation;
        insecureSSOImplementation.VulnCwes.Add(new VulnCwe { CweId = 287, VulnId = insecureSSOImplementation.Id });
        vulnManager.Add(insecureSSOImplementation);
        

        Vuln insecureVPNUsage = VulnTemplatesMastgPT.InsecureVPNUsage;
        insecureVPNUsage.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureVPNUsage.Id });
        vulnManager.Add(insecureVPNUsage);
        

        Vuln insecureCustomURLScheme = VulnTemplatesMastgPT.InsecureCustomURLScheme;
        insecureCustomURLScheme.VulnCwes.Add(new VulnCwe { CweId = 749, VulnId = insecureCustomURLScheme.Id });
        vulnManager.Add(insecureCustomURLScheme);
        

        Vuln timeOfCheckToTimeOfUse = VulnTemplatesMastgPT.TimeOfCheckToTimeOfUse;
        timeOfCheckToTimeOfUse.VulnCwes.Add(new VulnCwe { CweId = 367, VulnId = timeOfCheckToTimeOfUse.Id });
        vulnManager.Add(timeOfCheckToTimeOfUse);
        

        Vuln insecureAntiDebugging = VulnTemplatesMastgPT.InsecureAntiDebugging;
        insecureAntiDebugging.VulnCwes.Add(new VulnCwe { CweId = 388, VulnId = insecureAntiDebugging.Id });
        vulnManager.Add(insecureAntiDebugging);
        

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
        

        Vuln weakPasswordRequirements = VulnTemplatesWstgPT.WeakPasswordRequirements;
        weakPasswordRequirements.VulnCwes.Add(new VulnCwe { CweId = 521, VulnId = weakPasswordRequirements.Id });
        vulnManager.Add(weakPasswordRequirements);
        

        Vuln insecureSessionManagement = VulnTemplatesWstgPT.InsecureSessionManagement;
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 384, VulnId = insecureSessionManagement.Id });
        insecureSessionManagement.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insecureSessionManagement.Id });
        vulnManager.Add(insecureSessionManagement);
        

        Vuln idor = VulnTemplatesWstgPT.Idor;
        idor.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = idor.Id });
        vulnManager.Add(idor);
        

        Vuln crossSiteScripting = VulnTemplatesWstgPT.CrossSiteScripting;
        crossSiteScripting.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = crossSiteScripting.Id });
        vulnManager.Add(crossSiteScripting);
        

        Vuln insecureCryptographicStorage = VulnTemplatesWstgPT.InsecureCryptographicStorage;
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe
            { CweId = 327, VulnId = insecureCryptographicStorage.Id });
        insecureCryptographicStorage.VulnCwes.Add(new VulnCwe
            { CweId = 328, VulnId = insecureCryptographicStorage.Id });
        vulnManager.Add(insecureCryptographicStorage);
        

        Vuln insecureDeserialization = VulnTemplatesWstgPT.InsecureDeserialization;
        insecureDeserialization.VulnCwes.Add(new VulnCwe { CweId = 502, VulnId = insecureDeserialization.Id });
        vulnManager.Add(insecureDeserialization);
        

        Vuln sqlInjection = VulnTemplatesWstgPT.SqlInjection;
        sqlInjection.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = sqlInjection.Id });
        vulnManager.Add(sqlInjection);
        

        Vuln brokenAccessControl = VulnTemplatesWstgPT.BrokenAccessControl;
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 639, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = brokenAccessControl.Id });
        brokenAccessControl.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = brokenAccessControl.Id });
        vulnManager.Add(brokenAccessControl);
        

        Vuln securityMisconfiguration = VulnTemplatesWstgPT.SecurityMisconfiguration;
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 16, VulnId = securityMisconfiguration.Id });
        securityMisconfiguration.VulnCwes.Add(new VulnCwe { CweId = 2, VulnId = securityMisconfiguration.Id });
        vulnManager.Add(securityMisconfiguration);
        

        Vuln sensitiveDataExposure = VulnTemplatesWstgPT.SensitiveDataExposure;
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = sensitiveDataExposure.Id });
        sensitiveDataExposure.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = sensitiveDataExposure.Id });
        vulnManager.Add(sensitiveDataExposure);
        

        Vuln insufficientLoggingMonitoring = VulnTemplatesWstgPT.InsufficientLoggingAndMonitoring;
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe
            { CweId = 778, VulnId = insufficientLoggingMonitoring.Id });
        insufficientLoggingMonitoring.VulnCwes.Add(new VulnCwe
            { CweId = 223, VulnId = insufficientLoggingMonitoring.Id });
        vulnManager.Add(insufficientLoggingMonitoring);
        

        Vuln usingComponentsWithKnownVulnerabilities = VulnTemplatesWstgPT.UsingComponentsWithKnownVulnerabilities;
        usingComponentsWithKnownVulnerabilities.VulnCwes.Add(new VulnCwe
            { CweId = 1035, VulnId = usingComponentsWithKnownVulnerabilities.Id });
        vulnManager.Add(usingComponentsWithKnownVulnerabilities);
        

        Vuln serverSideRequestForgery = VulnTemplatesWstgPT.ServerSideRequestForgery;
        serverSideRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 918, VulnId = serverSideRequestForgery.Id });
        vulnManager.Add(serverSideRequestForgery);
        

        Vuln xmlExternalEntityProcessing = VulnTemplatesWstgPT.XmlExternalEntityProcessing;
        xmlExternalEntityProcessing.VulnCwes.Add(new VulnCwe { CweId = 611, VulnId = xmlExternalEntityProcessing.Id });
        vulnManager.Add(xmlExternalEntityProcessing);
        

        Vuln lackOfrateLimiting = VulnTemplatesWstgPT.LackOfRateLimiting;
        lackOfrateLimiting.VulnCwes.Add(new VulnCwe { CweId = 770, VulnId = lackOfrateLimiting.Id });
        vulnManager.Add(lackOfrateLimiting);
        

        Vuln inadequateOAuth2Implementation = VulnTemplatesWstgPT.InadequateOAuth2Implementation;
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe
            { CweId = 346, VulnId = inadequateOAuth2Implementation.Id });
        inadequateOAuth2Implementation.VulnCwes.Add(new VulnCwe
            { CweId = 601, VulnId = inadequateOAuth2Implementation.Id });
        vulnManager.Add(inadequateOAuth2Implementation);
        

        Vuln insufficientAntiAutomation = VulnTemplatesWstgPT.InsufficientAntiAutomation;
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 799, VulnId = insufficientAntiAutomation.Id });
        insufficientAntiAutomation.VulnCwes.Add(new VulnCwe { CweId = 837, VulnId = insufficientAntiAutomation.Id });
        vulnManager.Add(insufficientAntiAutomation);
        

        Vuln weakCryptographicAlgorithms = VulnTemplatesWstgPT.WeakCryptographicAlgorithms;
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = weakCryptographicAlgorithms.Id });
        weakCryptographicAlgorithms.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = weakCryptographicAlgorithms.Id });
        vulnManager.Add(weakCryptographicAlgorithms);
        

        Vuln improperCertificateValidation = VulnTemplatesWstgPT.ImproperCertificateValidation;
        improperCertificateValidation.VulnCwes.Add(new VulnCwe
            { CweId = 295, VulnId = improperCertificateValidation.Id });
        vulnManager.Add(improperCertificateValidation);
        

        Vuln crossSiteRequestForgery = VulnTemplatesWstgPT.CrossSiteRequestForgery;
        crossSiteRequestForgery.VulnCwes.Add(new VulnCwe { CweId = 352, VulnId = crossSiteRequestForgery.Id });
        vulnManager.Add(crossSiteRequestForgery);
        

        Vuln insufficientPasswordRecovery = VulnTemplatesWstgPT.InsufficientPasswordRecoveryMechanism;
        insufficientPasswordRecovery.VulnCwes.Add(new VulnCwe
            { CweId = 640, VulnId = insufficientPasswordRecovery.Id });
        vulnManager.Add(insufficientPasswordRecovery);
        

        Vuln lackOfInputSanitization = VulnTemplatesWstgPT.LackOfInputSanitization;
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = lackOfInputSanitization.Id });
        lackOfInputSanitization.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = lackOfInputSanitization.Id });
        vulnManager.Add(lackOfInputSanitization);
        

        Vuln insufficientSesssionTimeout = VulnTemplatesWstgPT.InsufficientSessionTimeout;
        insufficientSesssionTimeout.VulnCwes.Add(new VulnCwe { CweId = 613, VulnId = insufficientSesssionTimeout.Id });
        vulnManager.Add(insufficientSesssionTimeout);
        

        Vuln improperErrorHandling = VulnTemplatesWstgPT.ImproperErrorHandling;
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 209, VulnId = improperErrorHandling.Id });
        improperErrorHandling.VulnCwes.Add(new VulnCwe { CweId = 200, VulnId = improperErrorHandling.Id });
        vulnManager.Add(improperErrorHandling);
        

        Vuln missingSecurityHeaders = VulnTemplatesWstgPT.MissingSecurityHeaders;
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 693, VulnId = missingSecurityHeaders.Id });
        missingSecurityHeaders.VulnCwes.Add(new VulnCwe { CweId = 1021, VulnId = missingSecurityHeaders.Id });
        vulnManager.Add(missingSecurityHeaders);
        

        Vuln insecureUseOfCryptography = VulnTemplatesWstgPT.InsecureUseOfCryptography;
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 327, VulnId = insecureUseOfCryptography.Id });
        insecureUseOfCryptography.VulnCwes.Add(new VulnCwe { CweId = 320, VulnId = insecureUseOfCryptography.Id });
        vulnManager.Add(insecureUseOfCryptography);
        

        Vuln brokenFunctionLevelAuthorization = VulnTemplatesWstgPT.BrokenFunctionLevelAuthorization;
        brokenFunctionLevelAuthorization.VulnCwes.Add(new VulnCwe
            { CweId = 285, VulnId = brokenFunctionLevelAuthorization.Id });
        vulnManager.Add(brokenFunctionLevelAuthorization);
        

        Vuln insecureCommunication = VulnTemplatesWstgPT.InsecureCommunicationChannels;
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 319, VulnId = insecureCommunication.Id });
        insecureCommunication.VulnCwes.Add(new VulnCwe { CweId = 326, VulnId = insecureCommunication.Id });
        vulnManager.Add(insecureCommunication);
        

        Vuln lackOfSecureDefaults = VulnTemplatesWstgPT.LackOfSecureDefaults;
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 276, VulnId = lackOfSecureDefaults.Id });
        lackOfSecureDefaults.VulnCwes.Add(new VulnCwe { CweId = 1188, VulnId = lackOfSecureDefaults.Id });
        vulnManager.Add(lackOfSecureDefaults);
        

        Vuln insufficientProtectionFromAutomatedThreats =
            VulnTemplatesWstgPT.InsufficientProtectionFromAutomatedThreats;
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe
            { CweId = 799, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        insufficientProtectionFromAutomatedThreats.VulnCwes.Add(new VulnCwe
            { CweId = 307, VulnId = insufficientProtectionFromAutomatedThreats.Id });
        vulnManager.Add(insufficientProtectionFromAutomatedThreats);
        

        Vuln unvalidatedRedirectsAndForwards = VulnTemplatesWstgPT.UnvalidatedRedirectsAndForwards;
        unvalidatedRedirectsAndForwards.VulnCwes.Add(new VulnCwe
            { CweId = 601, VulnId = unvalidatedRedirectsAndForwards.Id });
        vulnManager.Add(unvalidatedRedirectsAndForwards);
        

        Vuln insecureAuthenticationMechanism = VulnTemplatesWstgPT.InsecureAuthenticationMechanism;
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe
            { CweId = 287, VulnId = insecureAuthenticationMechanism.Id });
        insecureAuthenticationMechanism.VulnCwes.Add(new VulnCwe
            { CweId = 384, VulnId = insecureAuthenticationMechanism.Id });
        vulnManager.Add(insecureAuthenticationMechanism);
        

        Vuln insuffificientAntiCachingHeaders = VulnTemplatesWstgPT.InsufficientAntiCachingHeaders;
        insuffificientAntiCachingHeaders.VulnCwes.Add(new VulnCwe
            { CweId = 525, VulnId = insuffificientAntiCachingHeaders.Id });
        vulnManager.Add(insuffificientAntiCachingHeaders);
        

        Vuln lackOfProperTLSConfiguration = VulnTemplatesWstgPT.LackOfProperTLSConfiguration;
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe
            { CweId = 326, VulnId = lackOfProperTLSConfiguration.Id });
        lackOfProperTLSConfiguration.VulnCwes.Add(new VulnCwe
            { CweId = 327, VulnId = lackOfProperTLSConfiguration.Id });
        vulnManager.Add(lackOfProperTLSConfiguration);
        

        Vuln insufficientDataProtection = VulnTemplatesWstgPT.InsufficientDataProtection;
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 311, VulnId = insufficientDataProtection.Id });
        insufficientDataProtection.VulnCwes.Add(new VulnCwe { CweId = 312, VulnId = insufficientDataProtection.Id });
        vulnManager.Add(insufficientDataProtection);
        

        Vuln improperAssetManagement = VulnTemplatesWstgPT.ImproperAssetManagement;
        improperAssetManagement.VulnCwes.Add(new VulnCwe { CweId = 1059, VulnId = improperAssetManagement.Id });
        vulnManager.Add(improperAssetManagement);
        

        Vuln lackOfSoftwareUpdate = VulnTemplatesWstgPT.LackOfSoftwareUpdates;
        lackOfSoftwareUpdate.VulnCwes.Add(new VulnCwe { CweId = 1104, VulnId = lackOfSoftwareUpdate.Id });
        vulnManager.Add(lackOfSoftwareUpdate);
        

        Vuln insufficientPrivacyControls = VulnTemplatesWstgPT.InsufficientPrivacyControls;
        insufficientPrivacyControls.VulnCwes.Add(new VulnCwe { CweId = 359, VulnId = insufficientPrivacyControls.Id });
        vulnManager.Add(insufficientPrivacyControls);
        

        Vuln insecureApiEndpoints = VulnTemplatesWstgPT.InsecureAPIEndpoints;
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 285, VulnId = insecureApiEndpoints.Id });
        insecureApiEndpoints.VulnCwes.Add(new VulnCwe { CweId = 284, VulnId = insecureApiEndpoints.Id });
        vulnManager.Add(insecureApiEndpoints);
        

        Vuln insufficientInputValidation = VulnTemplatesWstgPT.InsufficientInputValidation;
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 20, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = insufficientInputValidation.Id });
        insufficientInputValidation.VulnCwes.Add(new VulnCwe { CweId = 89, VulnId = insufficientInputValidation.Id });
        vulnManager.Add(insufficientInputValidation);
        

        Vuln improperOutputEncoding = VulnTemplatesWstgPT.ImproperOutputEncoding;
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 79, VulnId = improperOutputEncoding.Id });
        improperOutputEncoding.VulnCwes.Add(new VulnCwe { CweId = 116, VulnId = improperOutputEncoding.Id });
        vulnManager.Add(improperOutputEncoding);
        

        Vuln insecureFileHandling = VulnTemplatesWstgPT.InsecureFileHandling;
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 73, VulnId = insecureFileHandling.Id });
        insecureFileHandling.VulnCwes.Add(new VulnCwe { CweId = 434, VulnId = insecureFileHandling.Id });
        vulnManager.Add(insecureFileHandling);
        

        Vuln lackOfSecurePasswordStorage = VulnTemplatesWstgPT.LackOfSecurePasswordStorage;
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 256, VulnId = lackOfSecurePasswordStorage.Id });
        lackOfSecurePasswordStorage.VulnCwes.Add(new VulnCwe { CweId = 916, VulnId = lackOfSecurePasswordStorage.Id });
        vulnManager.Add(lackOfSecurePasswordStorage);
        

        Vuln insufficientProtectionAgainstDos = VulnTemplatesWstgPT.InsufficientProtectionAgainstDoS;
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe
            { CweId = 400, VulnId = insufficientProtectionAgainstDos.Id });
        insufficientProtectionAgainstDos.VulnCwes.Add(new VulnCwe
            { CweId = 770, VulnId = insufficientProtectionAgainstDos.Id });
        vulnManager.Add(insufficientProtectionAgainstDos);
        

        Vuln insufficientSubresourceIntegrity = VulnTemplatesWstgPT.InsufficientSubresourceIntegrity;
        insufficientSubresourceIntegrity.VulnCwes.Add(new VulnCwe
            { CweId = 353, VulnId = insufficientSubresourceIntegrity.Id });
        vulnManager.Add(insufficientSubresourceIntegrity);
        

        Vuln inadequateSecurityArchitecture = VulnTemplatesWstgPT.InadequateSecurityArchitecture;
        inadequateSecurityArchitecture.VulnCwes.Add(new VulnCwe
            { CweId = 1008, VulnId = inadequateSecurityArchitecture.Id });
        vulnManager.Add(inadequateSecurityArchitecture);
        

        Vuln flawedBusinessLogic = VulnTemplatesWstgPT.FlawedBusinessLogic;
        flawedBusinessLogic.VulnCwes.Add(new VulnCwe { CweId = 840, VulnId = flawedBusinessLogic.Id });
        vulnManager.Add(flawedBusinessLogic);
        

        Vuln insecureThirdPartyComponents = VulnTemplatesWstgPT.InsecureThirdPartyComponents;
        insecureThirdPartyComponents.VulnCwes.Add(
            new VulnCwe { CweId = 1104, VulnId = insecureThirdPartyComponents.Id });
        vulnManager.Add(insecureThirdPartyComponents);
        await vulnManager.Context.SaveChangesAsync();
    }
}