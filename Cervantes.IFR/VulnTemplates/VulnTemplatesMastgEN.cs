using Cervantes.CORE.Entities;

namespace Cervantes.IFR.VulnTemplates;

public class VulnTemplatesMastgEN
{
    public static Vuln InsecureDataStorage => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Data Storage",
        Description = "The app does not securely store sensitive data, potentially exposing it to unauthorized access.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure data storage mechanisms such as encryption for sensitive data stored locally.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Unauthorized access to sensitive user data, potential privacy violations, and compliance issues.",
        ProofOfConcept = "1. Obtain root/jailbreak access to the device.\n2. Navigate to the app's data directory.\n3. Locate and access unencrypted sensitive data files.",
        cve = "N/A"
    };

    public static Vuln InsecureCryptography => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Cryptography Implementation",
        Description = "The app uses weak or outdated cryptographic algorithms, or implements strong algorithms incorrectly.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement strong, up-to-date cryptographic algorithms and follow industry best practices for their usage.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential decryption of sensitive data, compromised integrity of encrypted communications.",
        ProofOfConcept = "1. Decompile the app and analyze the code.\n2. Identify usage of weak algorithms (e.g., MD5, SHA1) or small key sizes.\n3. Demonstrate the feasibility of breaking the encryption in a reasonable time frame.",
        cve = "N/A"
    };

    public static Vuln InsecureAuthentication => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Authentication Mechanism",
        Description = "The app's authentication mechanism is weak or improperly implemented, allowing unauthorized access.",
        Risk = VulnRisk.Critical,
        Status = VulnStatus.Open,
        CVSS3 = 9.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure authentication mechanisms, such as multi-factor authentication, and follow platform best practices for local authentication.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Unauthorized access to user accounts, potential data breach, and compromise of user privacy.",
        ProofOfConcept = "1. Analyze the app's authentication flow.\n2. Identify and exploit weaknesses (e.g., lack of rate limiting, weak password policies).\n3. Demonstrate unauthorized access to a user account.",
        cve = "N/A"
    };

    public static Vuln InsecureNetworkCommunication => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Network Communication",
        Description = "The app transmits sensitive data over insecure channels or does not properly validate server certificates.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure network communication protocols (e.g., TLS) and ensure proper certificate validation.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Interception of sensitive data in transit, potential man-in-the-middle attacks.",
        ProofOfConcept = "1. Set up a proxy to intercept app traffic.\n2. Identify unencrypted data transmissions or improper certificate validation.\n3. Demonstrate the ability to view or modify sensitive data in transit.",
        cve = "N/A"
    };
    
    public static Vuln PrivacyViolation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Privacy Violation",
        Description = "The app collects, processes, or shares user data without proper consent or beyond what's necessary for its functionality.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement data minimization practices, provide clear privacy policies, and offer user control over their data.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Violation of user privacy, potential legal and regulatory consequences, loss of user trust.",
        ProofOfConcept = "1. Analyze the app's data collection and sharing practices.\n2. Identify instances of excessive data collection or sharing.\n3. Compare actual data handling with stated privacy policies and user consent.",
        cve = "N/A"
    };
    
    public static Vuln InsecureDataLeakage => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Data Leakage",
        Description = "The app unintentionally leaks sensitive data through various channels such as logs, backups, or clipboard.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement proper data handling practices, avoid logging sensitive information, and secure app backups.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Exposure of sensitive user data, potential privacy violations, and security breaches.",
        ProofOfConcept = "1. Enable verbose logging on the device.\n2. Perform various actions in the app.\n3. Examine logs for sensitive data like passwords or session tokens.",
        cve = "N/A"
    };

    public static Vuln InsecureKeyManagement => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Cryptographic Key Management",
        Description = "The app does not properly manage cryptographic keys, potentially exposing them to unauthorized access.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure key management practices, use hardware-backed key storage when available, and avoid hardcoding keys.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Compromise of encrypted data, potential for unauthorized decryption of sensitive information.",
        ProofOfConcept = "1. Decompile the app and analyze the code.\n2. Search for hardcoded cryptographic keys or insecure key storage methods.\n3. Demonstrate the ability to extract or use the keys unauthorized.",
        cve = "N/A"
    };

    public static Vuln InsecureLocalAuthentication => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Local Authentication",
        Description = "The app's local authentication mechanism (e.g., biometrics, PIN) is improperly implemented or can be easily bypassed.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:P/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure local authentication mechanisms following platform best practices, use strong cryptographic techniques for storing authentication secrets.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Unauthorized local access to sensitive app functionality or data.",
        ProofOfConcept = "1. Analyze the local authentication implementation.\n2. Attempt to bypass the authentication (e.g., by manipulating local data storage).\n3. Demonstrate unauthorized access to protected app features.",
        cve = "N/A"
    };

    public static Vuln InsecureCertificatePinning => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Lack of Certificate Pinning",
        Description = "The app does not implement certificate pinning, making it vulnerable to man-in-the-middle attacks.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement certificate pinning for all remote endpoints under the developer's control.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential for man-in-the-middle attacks, interception of sensitive data in transit.",
        ProofOfConcept = "1. Set up a proxy with a self-signed certificate.\n2. Attempt to intercept the app's HTTPS traffic.\n3. If successful, demonstrate the ability to view or modify the intercepted data.",
        cve = "N/A"
    };

    public static Vuln InsecureIPC => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Inter-Process Communication",
        Description = "The app uses insecure IPC mechanisms, potentially exposing sensitive data or functionality to other apps.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure IPC mechanisms, use proper access controls, and validate all IPC inputs.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential exposure of sensitive data or functionality to unauthorized apps.",
        ProofOfConcept = "1. Analyze the app's IPC mechanisms (e.g., intents, content providers).\n2. Create a proof-of-concept app that attempts to access exposed components.\n3. Demonstrate unauthorized access to sensitive data or functionality.",
        cve = "N/A"
    };

    public static Vuln InsecureWebView => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure WebView Implementation",
        Description = "The app's WebView implementation is insecure, potentially allowing execution of malicious scripts or access to sensitive data.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:H",
        Remediation = "Secure WebView configuration, disable JavaScript if not needed, validate all loaded content, and use HTTPS for remote content.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential execution of malicious scripts, unauthorized access to app data or functionality.",
        ProofOfConcept = "1. Analyze the WebView configuration in the app.\n2. If JavaScript is enabled, attempt to inject and execute malicious scripts.\n3. Demonstrate unauthorized access to sensitive app data or functionality via the WebView.",
        cve = "N/A"
    };

    public static Vuln InsecureDeepLinking => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Deep Linking Implementation",
        Description = "The app's deep linking or URL scheme handling is insecure, potentially allowing unauthorized access to app functionality.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implement proper validation and sanitization of deep link parameters, use app-specific schemes, and require user confirmation for sensitive actions.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential unauthorized access to app functionality, data leakage, or execution of unintended actions.",
        ProofOfConcept = "1. Analyze the app's deep linking implementation.\n2. Craft malicious deep links targeting sensitive functionality.\n3. Demonstrate the ability to access restricted features or leak sensitive data via deep links.",
        cve = "N/A"
    };
    
    public static Vuln InsecureSessionHandling => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Session Handling",
        Description = "The app does not properly manage user sessions, potentially allowing session hijacking or unauthorized access.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.0,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure session management, use strong session identifiers, implement proper session expiration and invalidation mechanisms.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Unauthorized access to user accounts, potential for session hijacking attacks.",
        ProofOfConcept = "1. Analyze the app's session management mechanism.\n2. Attempt to reuse old session tokens or manipulate session data.\n3. Demonstrate unauthorized access to a user's session.",
        cve = "N/A"
    };

    public static Vuln InsecureTlsValidation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insufficient TLS Validation",
        Description = "The app does not properly validate TLS certificates, making it vulnerable to man-in-the-middle attacks.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement proper TLS certificate validation, including checking for certificate expiration, revocation, and correct hostname.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential for man-in-the-middle attacks, interception of sensitive data in transit.",
        ProofOfConcept = "1. Set up a proxy with an invalid TLS certificate.\n2. Attempt to intercept the app's HTTPS traffic.\n3. If successful, demonstrate the ability to view or modify the intercepted data.",
        cve = "N/A"
    };

    public static Vuln InsecureClipboardUsage => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Clipboard Usage",
        Description = "The app allows sensitive data to be copied to the clipboard, potentially exposing it to other apps.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 4.3,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:N",
        Remediation = "Prevent copying of sensitive data to the clipboard, or implement secure clipboard handling mechanisms.",
        RemediationComplexity = RemediationComplexity.Low,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential exposure of sensitive data to unauthorized apps or users.",
        ProofOfConcept = "1. Identify fields containing sensitive data in the app.\n2. Attempt to copy this data to the clipboard.\n3. Demonstrate the ability to paste the sensitive data in another app.",
        cve = "N/A"
    };

    public static Vuln InsecureDataCaching => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Data Caching",
        Description = "The app caches sensitive data insecurely, potentially exposing it to unauthorized access.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure caching mechanisms, avoid caching sensitive data, and clear caches properly when the app is closed or the user logs out.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential exposure of sensitive cached data to unauthorized users or apps.",
        ProofOfConcept = "1. Use the app and perform actions that involve sensitive data.\n2. Analyze the app's cache directories and files.\n3. Demonstrate the presence of sensitive data in unencrypted or easily accessible cache files.",
        cve = "N/A"
    };

    public static Vuln InsecureBackupHandling => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Backup Handling",
        Description = "The app does not properly secure its data during the backup process, potentially exposing sensitive information.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure backup mechanisms, exclude sensitive data from backups, or encrypt backup data.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential exposure of sensitive data through insecure backups.",
        ProofOfConcept = "1. Enable app data backup.\n2. Perform a backup of the device.\n3. Analyze the backup contents and demonstrate the presence of unencrypted sensitive data.",
        cve = "N/A"
    };

    public static Vuln InsufficientInputValidation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insufficient Input Validation",
        Description = "The app does not properly validate user input, potentially leading to injection attacks or unexpected behavior.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.6,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H",
        Remediation = "Implement thorough input validation for all user-supplied data, use parameterized queries, and sanitize input before use.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential for injection attacks, data corruption, or unauthorized access to backend systems.",
        ProofOfConcept = "1. Identify input fields in the app.\n2. Test various malformed or malicious inputs.\n3. Demonstrate unexpected behavior or successful injection attack.",
        cve = "N/A"
    };
    
    public static Vuln InsecureJailbreakRootDetection => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insufficient Jailbreak/Root Detection",
        Description = "The app does not properly detect or respond to jailbroken (iOS) or rooted (Android) devices, potentially exposing sensitive functionality or data.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implement robust jailbreak/root detection mechanisms and respond appropriately when a compromised environment is detected.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential exposure of sensitive app functionality or data on compromised devices.",
        ProofOfConcept = "1. Use a jailbroken/rooted device or emulator.\n2. Run the app and attempt to access sensitive features.\n3. Demonstrate that the app fails to detect the compromised environment and allows full functionality.",
        cve = "N/A"
    };

    public static Vuln InsecureCodeObfuscation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insufficient Code Obfuscation",
        Description = "The app's code is not adequately obfuscated, making it easier for attackers to reverse engineer and understand the app's logic.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.3,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:L",
        Remediation = "Implement strong code obfuscation techniques to make reverse engineering more difficult.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Easier reverse engineering of app logic, potential exposure of proprietary algorithms or security mechanisms.",
        ProofOfConcept = "1. Decompile the app using appropriate tools.\n2. Analyze the decompiled code for readability and understandability.\n3. Demonstrate the ease of understanding critical app logic or security mechanisms.",
        cve = "N/A"
    };

    public static Vuln InsecureRuntimeIntegrityChecks => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Lack of Runtime Integrity Checks",
        Description = "The app does not perform runtime integrity checks, making it vulnerable to code injection or tampering during execution.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement runtime integrity checks to detect and respond to code modifications or injections during app execution.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential for runtime code injection, bypassing of security controls, or manipulation of app behavior.",
        ProofOfConcept = "1. Use a tool like Frida to inject code into the running app.\n2. Modify critical app functionality or bypass security checks.\n3. Demonstrate that the app fails to detect or respond to the runtime modifications.",
        cve = "N/A"
    };

    public static Vuln InsecureAppPackaging => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure App Packaging",
        Description = "The app's package contains sensitive information or is not properly signed, making it vulnerable to tampering or information disclosure.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Ensure proper app signing, remove sensitive information from the app package, and implement additional integrity checks.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential for app repackaging attacks, exposure of sensitive information in the app package.",
        ProofOfConcept = "1. Extract and analyze the app package.\n2. Identify sensitive information in the package or weaknesses in the signing process.\n3. Demonstrate the ability to modify and repackage the app without detection.",
        cve = "N/A"
    };

    public static Vuln InsecureMemoryManagement => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Memory Management",
        Description = "The app does not properly handle sensitive data in memory, potentially leaving it vulnerable to memory dumps or side-channel attacks.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure memory management practices, such as zeroing out sensitive data after use and using secure memory allocations when available.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential exposure of sensitive data through memory dumps or side-channel attacks.",
        ProofOfConcept = "1. Use debugging tools to analyze the app's memory during runtime.\n2. Identify instances of sensitive data (e.g., encryption keys, passwords) persisting in memory.\n3. Demonstrate the ability to extract this sensitive data from a memory dump.",
        cve = "N/A"
    };

    public static Vuln InsecureComponentUpgrade => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Component Upgrade Mechanism",
        Description = "The app's mechanism for updating components or plugins is insecure, potentially allowing malicious updates to be installed.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure update mechanisms with proper integrity checks, use signed updates, and verify the source of updates.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential for installation of malicious updates, leading to code execution or data theft.",
        ProofOfConcept = "1. Intercept the app's update process.\n2. Replace a legitimate update with a modified version.\n3. Demonstrate that the app installs and executes the malicious update without proper verification.",
        cve = "N/A"
    };

    public static Vuln InsecureDataResidency => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Data Residency Practices",
        Description = "The app does not properly manage data residency, potentially violating legal or regulatory requirements for data storage and processing locations.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implement proper data residency controls, ensure data is stored and processed in compliance with relevant regulations, and provide user controls for data location preferences.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential legal and regulatory violations, loss of user trust due to improper data handling.",
        ProofOfConcept = "1. Analyze the app's data storage and processing mechanisms.\n2. Identify instances where data is stored or processed in non-compliant locations.\n3. Demonstrate the app's failure to adhere to specific data residency requirements or user preferences.",
        cve = "N/A"
    };
    
    public static Vuln InsecureCloudSyncMechanism => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Cloud Synchronization Mechanism",
        Description = "The app's cloud synchronization mechanism is not properly secured, potentially exposing sensitive data during the sync process or allowing unauthorized access to synced data.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement end-to-end encryption for synced data, use secure authentication for sync processes, and ensure proper access controls on cloud-stored data.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential exposure of sensitive user data during sync processes or unauthorized access to cloud-stored data.",
        ProofOfConcept = "1. Intercept the app's cloud sync traffic.\n2. Analyze the data being transmitted for sensitive information.\n3. Attempt to access or modify synced data through unauthorized means.",
        cve = "N/A"
    };

    public static Vuln VulnerableThirdPartyLibrary => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Vulnerable Third-Party Library",
        Description = "The app uses a third-party library with known security vulnerabilities, potentially exposing the app to various attacks.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:H",
        Remediation = "Regularly update third-party libraries to their latest secure versions, implement a process for tracking and addressing vulnerabilities in dependencies.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential for various attacks depending on the specific vulnerability, including data theft, code execution, or denial of service.",
        ProofOfConcept = "1. Identify third-party libraries used in the app.\n2. Check versions against known vulnerability databases.\n3. Demonstrate the impact of a specific vulnerability in an outdated library.",
        cve = "Specific CVE depends on the vulnerable library"
    };
    
    public static Vuln InsecureDataExfiltration => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Data Exfiltration Prevention",
        Description = "The app does not properly prevent unauthorized data exfiltration, allowing sensitive data to be extracted through various channels.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement proper data loss prevention techniques, restrict access to sensitive data, and monitor/control data output channels.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential unauthorized extraction of sensitive data from the app, leading to data breaches or privacy violations.",
        ProofOfConcept = "1. Identify potential data exfiltration channels (e.g., screenshots, copy/paste, file sharing).\n2. Attempt to extract sensitive data through these channels.\n3. Demonstrate successful exfiltration of protected data.",
        cve = "N/A"
    };
    

    public static Vuln InsecureAPIVersioning => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure API Versioning",
        Description = "The app does not properly handle API versioning, potentially exposing it to vulnerabilities in outdated API endpoints or causing functionality issues.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.3,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:L",
        Remediation = "Implement proper API versioning strategies, ensure the app uses the latest API version, and gracefully handle version incompatibilities.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential exposure to vulnerabilities in outdated API endpoints, functionality issues due to version mismatches.",
        ProofOfConcept = "1. Analyze the app's API calls and versioning mechanism.\n2. Attempt to force the app to use an outdated API version.\n3. Demonstrate vulnerabilities or functionality issues with outdated API usage.",
        cve = "N/A"
    };
    
    public static Vuln InsecureQRCodeHandling => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure QR Code Handling",
        Description = "The app does not properly validate or sanitize data from scanned QR codes, potentially allowing injection attacks or unintended actions.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation = "Implement strict validation and sanitization of QR code data, use allow lists for accepted data formats, and require user confirmation for sensitive actions triggered by QR codes.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential for arbitrary code execution, unauthorized data access, or triggering of unintended actions within the app.",
        ProofOfConcept = "1. Generate malicious QR codes with injected payloads.\n2. Scan the QR codes with the app.\n3. Demonstrate unauthorized actions or data access through malformed QR code data.",
        cve = "N/A"
    };

    public static Vuln InsecureNFCImplementation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure NFC Implementation",
        Description = "The app's NFC implementation is insecure, potentially allowing unauthorized data access or malicious actions through NFC communication.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:A/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure NFC communication protocols, validate and sanitize all NFC data, and use encryption for sensitive data transmission over NFC.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential unauthorized access to sensitive data or triggering of unintended actions through malicious NFC interactions.",
        ProofOfConcept = "1. Analyze the app's NFC implementation.\n2. Create malicious NFC tags or use an NFC emulator to send crafted data.\n3. Demonstrate data leakage or unauthorized actions through NFC communication.",
        cve = "N/A"
    };

    public static Vuln InsecureARImplementation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Augmented Reality (AR) Implementation",
        Description = "The app's AR features are implemented insecurely, potentially leading to privacy violations or security breaches through AR interactions.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure AR data handling, ensure proper permissions for camera and sensor access, and validate AR content sources.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential privacy violations through unauthorized camera access or data collection, security breaches through malicious AR content.",
        ProofOfConcept = "1. Analyze the app's AR implementation and data handling.\n2. Attempt to bypass AR-related permissions or inject malicious AR content.\n3. Demonstrate privacy violations or security breaches through AR features.",
        cve = "N/A"
    };

    public static Vuln InsecureIoTIntegration => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure IoT Device Integration",
        Description = "The app's integration with IoT devices is insecure, potentially allowing unauthorized control of connected devices or data leakage.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure communication protocols for IoT device interaction, use strong authentication for device pairing, and validate all commands sent to IoT devices.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential unauthorized control of IoT devices, data leakage from connected devices, or privacy violations.",
        ProofOfConcept = "1. Analyze the app's IoT device communication protocols.\n2. Attempt to intercept or manipulate communications between the app and IoT devices.\n3. Demonstrate unauthorized device control or data access.",
        cve = "N/A"
    };

    public static Vuln InsecurePushNotifications => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Push Notification Handling",
        Description = "The app's handling of push notifications is insecure, potentially allowing sensitive data leakage or execution of malicious payloads through notifications.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure handling of push notification payloads, validate and sanitize notification data, and avoid sending sensitive information in notifications.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential exposure of sensitive data through push notifications or execution of malicious payloads.",
        ProofOfConcept = "1. Analyze the app's push notification handling mechanism.\n2. Send crafted push notifications with malicious payloads or sensitive data.\n3. Demonstrate data leakage or unauthorized actions through push notifications.",
        cve = "N/A"
    };

    public static Vuln InsecureAppCloning => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Vulnerability to App Cloning",
        Description = "The app is vulnerable to cloning attacks, potentially allowing unauthorized copies of the app to access sensitive data or functionality.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement anti-cloning measures such as device binding, use hardware-backed key storage, and implement runtime integrity checks.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential unauthorized access to sensitive data or functionality through cloned apps, bypassing of licensing or access controls.",
        ProofOfConcept = "1. Attempt to clone the app using various tools or techniques.\n2. Run the cloned app on a different device.\n3. Demonstrate access to sensitive data or functionality through the cloned app.",
        cve = "N/A"
    };

    public static Vuln InsecureScreenOverlay => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Vulnerability to Screen Overlay Attacks",
        Description = "The app is vulnerable to screen overlay attacks, potentially allowing malicious apps to capture sensitive user input.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation = "Implement overlay detection mechanisms, use secure input methods for sensitive data, and educate users about the risks of granting overlay permissions to unknown apps.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential capture of sensitive user input such as passwords or credit card information.",
        ProofOfConcept = "1. Create a proof-of-concept overlay app.\n2. Attempt to capture user input in the target app using the overlay.\n3. Demonstrate successful capture of sensitive information.",
        cve = "N/A"
    };

    public static Vuln InsecureAppWidget => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure App Widget Implementation",
        Description = "The app's widget implementation is insecure, potentially exposing sensitive data or functionality on the device's home screen.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure data handling in app widgets, avoid displaying sensitive information, and use proper authentication for widget actions.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential exposure of sensitive data on the device's home screen or unauthorized access to app functionality through the widget.",
        ProofOfConcept = "1. Analyze the app's widget implementation.\n2. Attempt to access sensitive data or functionality through the widget without proper authentication.\n3. Demonstrate data leakage or unauthorized actions via the widget.",
        cve = "N/A"
    };
    
    public static Vuln InsecureEdgeComputingIntegration => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Edge Computing Integration",
        Description = "The app's integration with edge computing services is insecure, potentially exposing sensitive data or allowing unauthorized access to edge-processed information.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure communication protocols for edge computing interactions, use strong authentication and encryption, and validate all data received from edge services.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential unauthorized access to sensitive data processed at the edge, data manipulation, or privacy violations.",
        ProofOfConcept = "1. Analyze the app's communication with edge computing services.\n2. Attempt to intercept or manipulate data between the app and edge services.\n3. Demonstrate unauthorized access to sensitive information or injection of malicious data.",
        cve = "N/A"
    };

    public static Vuln InsecureAIMLImplementation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure AI/ML Model Implementation",
        Description = "The app's implementation of AI/ML models is insecure, potentially allowing model poisoning, data extraction, or adversarial attacks.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure AI/ML model deployment, use federated learning where appropriate, protect against model inversion and membership inference attacks, and regularly update models.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential extraction of sensitive training data, manipulation of AI/ML model outputs, or unauthorized inference about users.",
        ProofOfConcept = "1. Analyze the app's AI/ML model implementation.\n2. Attempt model inversion or membership inference attacks.\n3. Demonstrate extraction of sensitive information or successful adversarial examples.",
        cve = "N/A"
    };
    public static Vuln InsecureQuantumResistantCrypto => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Lack of Quantum-Resistant Cryptography",
        Description = "The app does not implement quantum-resistant cryptographic algorithms, potentially making it vulnerable to future quantum computing attacks.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.9,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement post-quantum cryptographic algorithms for sensitive data protection and key exchange mechanisms.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        OWASPRisk = "MASVS-CRYPTO-1",
        MitreTechniques = "T1600",
        Impact = "Potential future vulnerability to quantum computing attacks, possibly leading to decryption of sensitive data.",
        ProofOfConcept = "1. Analyze the app's cryptographic implementations.\n2. Identify usage of non-quantum-resistant algorithms for key sensitive operations.\n3. Demonstrate theoretical vulnerability to known quantum algorithms.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 327 } }, // CWE-327: Use of a Broken or Risky Cryptographic Algorithm
        cve = "N/A"
    };

    public static Vuln InsecureVoiceUIIntegration => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Voice User Interface Integration",
        Description = "The app's integration with voice user interfaces (VUI) is insecure, potentially allowing unauthorized voice commands or exposing sensitive information through voice interactions.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure voice authentication, validate and sanitize voice commands, and avoid exposing sensitive information through voice responses.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential unauthorized access to app functionality, exposure of sensitive information through voice channels, or execution of malicious voice commands.",
        ProofOfConcept = "1. Analyze the app's voice UI implementation.\n2. Attempt to bypass voice authentication or inject malicious voice commands.\n3. Demonstrate unauthorized actions or data access through voice interactions.",
        cve = "N/A"
    };

    public static Vuln InsecureMultiDeviceSynchronization => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Multi-Device Synchronization",
        Description = "The app's multi-device synchronization mechanism is insecure, potentially allowing unauthorized access to user data across devices or interception of sync data.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement end-to-end encryption for multi-device sync, use secure device pairing mechanisms, and validate the integrity of synced data.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential unauthorized access to user data on multiple devices, interception of sensitive information during sync, or injection of malicious data across devices.",
        ProofOfConcept = "1. Analyze the app's multi-device sync mechanism.\n2. Attempt to intercept or manipulate sync data between devices.\n3. Demonstrate unauthorized access to synced data or injection of malicious content.",
        cve = "N/A"
    };
    
    public static Vuln InsecureBlockchainIntegration => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Blockchain Integration",
        Description = "The app's integration with blockchain technology is insecure, potentially exposing cryptographic keys, allowing unauthorized transactions, or compromising the integrity of blockchain data.",
        Risk = VulnRisk.Critical,
        Status = VulnStatus.Open,
        CVSS3 = 9.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H",
        Remediation = "Implement secure key management for blockchain interactions, use hardware security modules where possible, validate all blockchain transactions, and ensure proper access controls to blockchain functionalities.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential theft of cryptocurrency assets, unauthorized blockchain transactions, or compromise of decentralized app (DApp) integrity.",
        ProofOfConcept = "1. Analyze the app's blockchain integration, focusing on key management and transaction signing.\n2. Attempt to extract private keys or manipulate transaction data.\n3. Demonstrate unauthorized blockchain transactions or compromise of DApp functionality.",
        cve = "N/A"
    };

    public static Vuln InsecureKeychainKeystore => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Keychain/Keystore Usage",
        Description = "The app's usage of the platform keychain (iOS) or keystore (Android) is insecure, potentially exposing sensitive credentials or cryptographic keys.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure keychain/keystore access, use appropriate protection classes or security levels, and avoid storing highly sensitive data in the keychain/keystore if possible.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential exposure of sensitive credentials or cryptographic keys, leading to unauthorized access or data decryption.",
        ProofOfConcept = "1. Analyze the app's keychain/keystore usage.\n2. Attempt to extract data from the keychain/keystore using a jailbroken/rooted device.\n3. Demonstrate access to sensitive information stored in the keychain/keystore.",
        cve = "N/A"
    };

    public static Vuln InsecureRandomNumberGeneration => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Random Number Generation",
        Description = "The app uses weak or predictable random number generation methods, potentially compromising cryptographic operations or security-critical functionalities.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Use cryptographically secure random number generators provided by the platform or well-vetted libraries. Avoid using Math.random() or similar weak PRNGs for security-critical operations.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential prediction of generated values, leading to compromised encryption, session tokens, or other security-critical data.",
        ProofOfConcept = "1. Identify usage of random number generation in the app's code.\n2. Analyze the randomness quality of generated values.\n3. Demonstrate predictability or bias in the generated random numbers.",
        cve = "N/A"
    };

    public static Vuln InsecureSSOImplementation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Single Sign-On (SSO) Implementation",
        Description = "The app's implementation of Single Sign-On (SSO) or federation is flawed, potentially allowing unauthorized access or account takeover.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure SSO practices, properly validate OAuth tokens, use secure redirect URIs, and implement proper session management after SSO authentication.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential unauthorized access to user accounts, session hijacking, or bypass of authentication mechanisms.",
        ProofOfConcept = "1. Analyze the app's SSO implementation and token handling.\n2. Attempt to forge or manipulate SSO tokens.\n3. Demonstrate unauthorized access using manipulated SSO credentials.",
        cve = "N/A"
    };

    public static Vuln InsecureVPNUsage => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure VPN Usage",
        Description = "The app's usage or implementation of VPN functionality is insecure, potentially exposing user traffic or allowing unauthorized access to protected networks.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure VPN protocols, properly validate server certificates, use strong encryption for VPN tunnels, and ensure proper handling of VPN credentials.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        Impact = "Potential exposure of user traffic, unauthorized access to protected networks, or compromise of VPN credentials.",
        ProofOfConcept = "1. Analyze the app's VPN implementation and configuration.\n2. Attempt to intercept or manipulate VPN traffic.\n3. Demonstrate data leakage or unauthorized access through VPN vulnerabilities.",
        cve = "N/A"
    };

    public static Vuln InsecureCustomURLScheme => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Custom URL Scheme Handling",
        Description = "The app's implementation of custom URL schemes is insecure, potentially allowing other apps to invoke sensitive functionality or access protected data.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implement proper validation and sanitization of custom URL scheme parameters, use app-specific schemes, and require user confirmation for sensitive actions triggered by URL schemes.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential unauthorized access to app functionality or data through maliciously crafted custom URLs.",
        ProofOfConcept = "1. Identify custom URL schemes used by the app.\n2. Craft malicious URLs exploiting these schemes.\n3. Demonstrate unauthorized actions or data access through custom URL scheme exploitation.",
        cve = "N/A"
    };

    public static Vuln TimeOfCheckToTimeOfUse => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Time-of-Check to Time-of-Use (TOCTOU) Vulnerability",
        Description = "The app contains race conditions between the checking of a condition and the use of the results of that check, potentially leading to security issues.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement proper synchronization mechanisms, use atomic operations where possible, and design code to minimize the time window between checks and usage.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential privilege escalation, unauthorized access to resources, or data corruption due to race conditions.",
        ProofOfConcept = "1. Identify potential TOCTOU vulnerabilities in the app's code.\n2. Create a proof-of-concept exploit that races between the check and use.\n3. Demonstrate unauthorized actions or data access through successful exploitation of the race condition.",
        cve = "N/A"
    };

    public static Vuln InsecureAntiDebugging => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insufficient Anti-Debugging Measures",
        Description = "The app lacks robust anti-debugging techniques, making it easier for attackers to analyze and modify the app's behavior at runtime.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.4,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:H/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement multiple layers of anti-debugging techniques, including native code checks, timing checks, and environment detection. Regularly update and obfuscate these protections.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Easier reverse engineering and tampering of the app, potentially exposing sensitive logic or bypassing security controls.",
        ProofOfConcept = "1. Attempt to attach a debugger to the running app.\n2. Analyze the app's anti-debugging measures.\n3. Demonstrate successful bypassing of existing anti-debugging techniques.",
        cve = "N/A"
    };

    public static Vuln OverPrivilegedApplication => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Over-Privileged Application",
        Description = "The app requests more permissions than necessary for its functionality, potentially exposing users to increased privacy and security risks.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.9,
        CVSSVector = "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Review and minimize the app's permission requests, implement runtime permission requests, and clearly explain to users why each permission is needed.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        Impact = "Potential privacy violations, unauthorized access to user data, or increased attack surface due to unnecessary permissions.",
        ProofOfConcept = "1. Analyze the app's manifest or info.plist for requested permissions.\n2. Identify permissions that are not essential for core functionality.\n3. Demonstrate potential misuse of over-privileged permissions.",
        cve = "N/A"
    };
}