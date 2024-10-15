/*using Cervantes.CORE.Entities;

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
        OWASPRisk = "MASVS-STORAGE-1",
        MitreTechniques = "T1213",
        Impact = "Unauthorized access to sensitive user data, potential privacy violations, and compliance issues.",
        ProofOfConcept = "1. Obtain root/jailbreak access to the device.\n2. Navigate to the app's data directory.\n3. Locate and access unencrypted sensitive data files.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 312 } }, // CWE-312: Cleartext Storage of Sensitive Information
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-CRYPTO-1",
        MitreTechniques = "T1256",
        Impact = "Potential decryption of sensitive data, compromised integrity of encrypted communications.",
        ProofOfConcept = "1. Decompile the app and analyze the code.\n2. Identify usage of weak algorithms (e.g., MD5, SHA1) or small key sizes.\n3. Demonstrate the feasibility of breaking the encryption in a reasonable time frame.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 327 } }, // CWE-327: Use of a Broken or Risky Cryptographic Algorithm
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        RemediationPriority = RemediationPriority.Critical,
        OWASPRisk = "MASVS-AUTH-1",
        MitreTechniques = "T1111",
        Impact = "Unauthorized access to user accounts, potential data breach, and compromise of user privacy.",
        ProofOfConcept = "1. Analyze the app's authentication flow.\n2. Identify and exploit weaknesses (e.g., lack of rate limiting, weak password policies).\n3. Demonstrate unauthorized access to a user account.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 287 } }, // CWE-287: Improper Authentication
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-NETWORK-1",
        MitreTechniques = "T1071",
        Impact = "Interception of sensitive data in transit, potential man-in-the-middle attacks.",
        ProofOfConcept = "1. Set up a proxy to intercept app traffic.\n2. Identify unencrypted data transmissions or improper certificate validation.\n3. Demonstrate the ability to view or modify sensitive data in transit.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 319 } }, // CWE-319: Cleartext Transmission of Sensitive Information
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PRIVACY-1, MASVS-PRIVACY-2",
        MitreTechniques = "T1430",
        Impact = "Violation of user privacy, potential legal and regulatory consequences, loss of user trust.",
        ProofOfConcept = "1. Analyze the app's data collection and sharing practices.\n2. Identify instances of excessive data collection or sharing.\n3. Compare actual data handling with stated privacy policies and user consent.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 359 } }, // CWE-359: Exposure of Private Personal Information to an Unauthorized Actor
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-STORAGE-2",
        MitreTechniques = "T1145",
        Impact = "Exposure of sensitive user data, potential privacy violations, and security breaches.",
        ProofOfConcept = "1. Enable verbose logging on the device.\n2. Perform various actions in the app.\n3. Examine logs for sensitive data like passwords or session tokens.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 532 } }, // CWE-532: Insertion of Sensitive Information into Log File
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-CRYPTO-2",
        MitreTechniques = "T1145",
        Impact = "Compromise of encrypted data, potential for unauthorized decryption of sensitive information.",
        ProofOfConcept = "1. Decompile the app and analyze the code.\n2. Search for hardcoded cryptographic keys or insecure key storage methods.\n3. Demonstrate the ability to extract or use the keys unauthorized.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 321 } }, // CWE-321: Use of Hard-coded Cryptographic Key
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-AUTH-2",
        MitreTechniques = "T1111",
        Impact = "Unauthorized local access to sensitive app functionality or data.",
        ProofOfConcept = "1. Analyze the local authentication implementation.\n2. Attempt to bypass the authentication (e.g., by manipulating local data storage).\n3. Demonstrate unauthorized access to protected app features.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 287 } }, // CWE-287: Improper Authentication
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-NETWORK-2",
        MitreTechniques = "T1040",
        Impact = "Potential for man-in-the-middle attacks, interception of sensitive data in transit.",
        ProofOfConcept = "1. Set up a proxy with a self-signed certificate.\n2. Attempt to intercept the app's HTTPS traffic.\n3. If successful, demonstrate the ability to view or modify the intercepted data.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 295 } }, // CWE-295: Improper Certificate Validation
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PLATFORM-1",
        MitreTechniques = "T1559",
        Impact = "Potential exposure of sensitive data or functionality to unauthorized apps.",
        ProofOfConcept = "1. Analyze the app's IPC mechanisms (e.g., intents, content providers).\n2. Create a proof-of-concept app that attempts to access exposed components.\n3. Demonstrate unauthorized access to sensitive data or functionality.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 927 } }, // CWE-927: Use of Implicit Intent for Sensitive Communication
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PLATFORM-2",
        MitreTechniques = "T1559.001",
        Impact = "Potential execution of malicious scripts, unauthorized access to app data or functionality.",
        ProofOfConcept = "1. Analyze the WebView configuration in the app.\n2. If JavaScript is enabled, attempt to inject and execute malicious scripts.\n3. Demonstrate unauthorized access to sensitive app data or functionality via the WebView.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 749 } }, // CWE-749: Exposed Dangerous Method or Function
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PLATFORM-3",
        MitreTechniques = "T1541",
        Impact = "Potential unauthorized access to app functionality, data leakage, or execution of unintended actions.",
        ProofOfConcept = "1. Analyze the app's deep linking implementation.\n2. Craft malicious deep links targeting sensitive functionality.\n3. Demonstrate the ability to access restricted features or leak sensitive data via deep links.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 939 } }, // CWE-939: Improper Authorization in Handler for Custom URL Scheme
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };
    
    public static Vuln InsecureBiometricAuthentication => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Biometric Authentication Implementation",
        Description = "The app's biometric authentication mechanism is improperly implemented, potentially allowing bypass or unauthorized access.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement biometric authentication following platform best practices, use strong cryptographic techniques for key storage, and implement proper fallback mechanisms.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-AUTH-2",
        MitreTechniques = "T1111",
        Impact = "Unauthorized access to sensitive app functionality or data, potential for biometric spoofing attacks.",
        ProofOfConcept = "1. Analyze the biometric authentication implementation.\n2. Attempt to bypass the authentication (e.g., by manipulating local data storage or intercepting API calls).\n3. Demonstrate unauthorized access to protected app features using a biometric bypass.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 287 } }, // CWE-287: Improper Authentication
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-AUTH-3",
        MitreTechniques = "T1539",
        Impact = "Unauthorized access to user accounts, potential for session hijacking attacks.",
        ProofOfConcept = "1. Analyze the app's session management mechanism.\n2. Attempt to reuse old session tokens or manipulate session data.\n3. Demonstrate unauthorized access to a user's session.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 613 } }, // CWE-613: Insufficient Session Expiration
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-NETWORK-1",
        MitreTechniques = "T1040",
        Impact = "Potential for man-in-the-middle attacks, interception of sensitive data in transit.",
        ProofOfConcept = "1. Set up a proxy with an invalid TLS certificate.\n2. Attempt to intercept the app's HTTPS traffic.\n3. If successful, demonstrate the ability to view or modify the intercepted data.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 295 } }, // CWE-295: Improper Certificate Validation
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PLATFORM-3",
        MitreTechniques = "T1414",
        Impact = "Potential exposure of sensitive data to unauthorized apps or users.",
        ProofOfConcept = "1. Identify fields containing sensitive data in the app.\n2. Attempt to copy this data to the clipboard.\n3. Demonstrate the ability to paste the sensitive data in another app.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 200 } }, // CWE-200: Exposure of Sensitive Information to an Unauthorized Actor
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-STORAGE-1",
        MitreTechniques = "T1145",
        Impact = "Potential exposure of sensitive cached data to unauthorized users or apps.",
        ProofOfConcept = "1. Use the app and perform actions that involve sensitive data.\n2. Analyze the app's cache directories and files.\n3. Demonstrate the presence of sensitive data in unencrypted or easily accessible cache files.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 524 } }, // CWE-524: Use of Cache Containing Sensitive Information
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-STORAGE-2",
        MitreTechniques = "T1530",
        Impact = "Potential exposure of sensitive data through insecure backups.",
        ProofOfConcept = "1. Enable app data backup.\n2. Perform a backup of the device.\n3. Analyze the backup contents and demonstrate the presence of unencrypted sensitive data.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 530 } }, // CWE-530: Exposure of Backup File to an Unauthorized Control Sphere
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-CODE-4",
        MitreTechniques = "T1068",
        Impact = "Potential for injection attacks, data corruption, or unauthorized access to backend systems.",
        ProofOfConcept = "1. Identify input fields in the app.\n2. Test various malformed or malicious inputs.\n3. Demonstrate unexpected behavior or successful injection attack.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 20 } }, // CWE-20: Improper Input Validation
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-RESILIENCE-1",
        MitreTechniques = "T1480.001",
        Impact = "Potential exposure of sensitive app functionality or data on compromised devices.",
        ProofOfConcept = "1. Use a jailbroken/rooted device or emulator.\n2. Run the app and attempt to access sensitive features.\n3. Demonstrate that the app fails to detect the compromised environment and allows full functionality.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 919 } }, // CWE-919: Weaknesses in Mobile Applications
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-RESILIENCE-3",
        MitreTechniques = "T1027",
        Impact = "Easier reverse engineering of app logic, potential exposure of proprietary algorithms or security mechanisms.",
        ProofOfConcept = "1. Decompile the app using appropriate tools.\n2. Analyze the decompiled code for readability and understandability.\n3. Demonstrate the ease of understanding critical app logic or security mechanisms.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 656 } }, // CWE-656: Reliance on Security Through Obscurity
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-RESILIENCE-2",
        MitreTechniques = "T1407",
        Impact = "Potential for runtime code injection, bypassing of security controls, or manipulation of app behavior.",
        ProofOfConcept = "1. Use a tool like Frida to inject code into the running app.\n2. Modify critical app functionality or bypass security checks.\n3. Demonstrate that the app fails to detect or respond to the runtime modifications.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 693 } }, // CWE-693: Protection Mechanism Failure
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-CODE-2",
        MitreTechniques = "T1593.002",
        Impact = "Potential for app repackaging attacks, exposure of sensitive information in the app package.",
        ProofOfConcept = "1. Extract and analyze the app package.\n2. Identify sensitive information in the package or weaknesses in the signing process.\n3. Demonstrate the ability to modify and repackage the app without detection.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 490 } }, // CWE-490: Incomplete Mediation
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-CODE-3",
        MitreTechniques = "T1003",
        Impact = "Potential exposure of sensitive data through memory dumps or side-channel attacks.",
        ProofOfConcept = "1. Use debugging tools to analyze the app's memory during runtime.\n2. Identify instances of sensitive data (e.g., encryption keys, passwords) persisting in memory.\n3. Demonstrate the ability to extract this sensitive data from a memory dump.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 316 } }, // CWE-316: Cleartext Storage of Sensitive Information in Memory
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-CODE-2",
        MitreTechniques = "T1195.002",
        Impact = "Potential for installation of malicious updates, leading to code execution or data theft.",
        ProofOfConcept = "1. Intercept the app's update process.\n2. Replace a legitimate update with a modified version.\n3. Demonstrate that the app installs and executes the malicious update without proper verification.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 494 } }, // CWE-494: Download of Code Without Integrity Check
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PRIVACY-2",
        MitreTechniques = "T1530",
        Impact = "Potential legal and regulatory violations, loss of user trust due to improper data handling.",
        ProofOfConcept = "1. Analyze the app's data storage and processing mechanisms.\n2. Identify instances where data is stored or processed in non-compliant locations.\n3. Demonstrate the app's failure to adhere to specific data residency requirements or user preferences.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 359 } }, // CWE-359: Exposure of Private Personal Information to an Unauthorized Actor
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };
    
    public static Vuln InsecureDeepLinkHandling => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Deep Link Handling",
        Description = "The app does not properly validate or sanitize deep link parameters, potentially allowing attackers to inject malicious data or trigger unintended actions.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:H/A:N",
        Remediation = "Implement proper validation and sanitization of deep link parameters, use a allow list for accepted deep link schemes, and require user confirmation for sensitive actions triggered by deep links.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-PLATFORM-1",
        MitreTechniques = "T1649",
        Impact = "Potential for arbitrary code execution, unauthorized access to sensitive data, or triggering of unintended actions within the app.",
        ProofOfConcept = "1. Identify deep link schemes used by the app.\n2. Craft malicious deep links with injected parameters.\n3. Demonstrate unauthorized actions or data access through malformed deep links.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 749 } }, // CWE-749: Exposed Dangerous Method or Function
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-NETWORK-1, MASVS-STORAGE-1",
        MitreTechniques = "T1213",
        Impact = "Potential exposure of sensitive user data during sync processes or unauthorized access to cloud-stored data.",
        ProofOfConcept = "1. Intercept the app's cloud sync traffic.\n2. Analyze the data being transmitted for sensitive information.\n3. Attempt to access or modify synced data through unauthorized means.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 311 } }, // CWE-311: Missing Encryption of Sensitive Data
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-CODE-3",
        MitreTechniques = "T1195.001",
        Impact = "Potential for various attacks depending on the specific vulnerability, including data theft, code execution, or denial of service.",
        ProofOfConcept = "1. Identify third-party libraries used in the app.\n2. Check versions against known vulnerability databases.\n3. Demonstrate the impact of a specific vulnerability in an outdated library.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 1035 } }, // CWE-1035: 2020 CWE Top 25 - OWASP Top 10 2021 Category A06:2021 – Vulnerable and Outdated Components
        cve = "Specific CVE depends on the vulnerable library"
    };

    public static Vuln InsecureBiometricImplementation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Biometric Authentication Implementation",
        Description = "The app's implementation of biometric authentication is flawed, potentially allowing bypass or unauthorized access.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement biometric authentication using platform-provided APIs, ensure proper fallback mechanisms, and use additional factors for highly sensitive operations.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-AUTH-2",
        MitreTechniques = "T1078",
        Impact = "Potential unauthorized access to sensitive app functionalities or data through biometric authentication bypass.",
        ProofOfConcept = "1. Analyze the biometric authentication implementation.\n2. Attempt to bypass biometric checks through various means (e.g., API hooking, modifying app behavior).\n3. Demonstrate unauthorized access despite biometric protection.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 287 } }, // CWE-287: Improper Authentication
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureAppLinks => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure App Links Implementation",
        Description = "The app's implementation of App Links (Android) or Universal Links (iOS) is insecure, potentially allowing other apps to intercept sensitive deep links.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:C/C:L/I:L/A:N",
        Remediation = "Properly implement and verify App Links/Universal Links, use HTTPS for associated domains, and validate incoming link parameters.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        OWASPRisk = "MASVS-PLATFORM-1",
        MitreTechniques = "T1444",
        Impact = "Potential interception of deep links by malicious apps, leading to data theft or phishing attacks.",
        ProofOfConcept = "1. Analyze the app's manifest for App Links/Universal Links configuration.\n2. Attempt to register a malicious app with similar deep link patterns.\n3. Demonstrate interception of deep links intended for the target app.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 610 } }, // CWE-610: Externally Controlled Reference to a Resource in Another Sphere
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-STORAGE-1, MASVS-PLATFORM-1",
        MitreTechniques = "T1059",
        Impact = "Potential unauthorized extraction of sensitive data from the app, leading to data breaches or privacy violations.",
        ProofOfConcept = "1. Identify potential data exfiltration channels (e.g., screenshots, copy/paste, file sharing).\n2. Attempt to extract sensitive data through these channels.\n3. Demonstrate successful exfiltration of protected data.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 200 } }, // CWE-200: Exposure of Sensitive Information to an Unauthorized Actor
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureOfflineAuthentication => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Offline Authentication Mechanism",
        Description = "The app's offline authentication mechanism is weak or improperly implemented, potentially allowing unauthorized access when the device is offline.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure offline authentication using strong cryptographic techniques, limit the duration of offline access, and synchronize authentication state when online.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-AUTH-1",
        MitreTechniques = "T1556",
        Impact = "Potential unauthorized access to app functionality or data when the device is offline.",
        ProofOfConcept = "1. Put the device in offline mode.\n2. Analyze the app's offline authentication mechanism.\n3. Attempt to bypass or crack the offline authentication.\n4. Demonstrate unauthorized access in offline mode.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 308 } }, // CWE-308: Use of Single-factor Authentication
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-NETWORK-1",
        MitreTechniques = "T1190",
        Impact = "Potential exposure to vulnerabilities in outdated API endpoints, functionality issues due to version mismatches.",
        ProofOfConcept = "1. Analyze the app's API calls and versioning mechanism.\n2. Attempt to force the app to use an outdated API version.\n3. Demonstrate vulnerabilities or functionality issues with outdated API usage.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 330 } }, // CWE-330: Use of Insufficiently Random Values
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PLATFORM-1",
        MitreTechniques = "T1547",
        Impact = "Potential for arbitrary code execution, unauthorized data access, or triggering of unintended actions within the app.",
        ProofOfConcept = "1. Generate malicious QR codes with injected payloads.\n2. Scan the QR codes with the app.\n3. Demonstrate unauthorized actions or data access through malformed QR code data.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 20 } }, // CWE-20: Improper Input Validation
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-NETWORK-1",
        MitreTechniques = "T1442",
        Impact = "Potential unauthorized access to sensitive data or triggering of unintended actions through malicious NFC interactions.",
        ProofOfConcept = "1. Analyze the app's NFC implementation.\n2. Create malicious NFC tags or use an NFC emulator to send crafted data.\n3. Demonstrate data leakage or unauthorized actions through NFC communication.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 319 } }, // CWE-319: Cleartext Transmission of Sensitive Information
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PLATFORM-1",
        MitreTechniques = "T1430",
        Impact = "Potential privacy violations through unauthorized camera access or data collection, security breaches through malicious AR content.",
        ProofOfConcept = "1. Analyze the app's AR implementation and data handling.\n2. Attempt to bypass AR-related permissions or inject malicious AR content.\n3. Demonstrate privacy violations or security breaches through AR features.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 250 } }, // CWE-250: Execution with Unnecessary Privileges
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-NETWORK-1",
        MitreTechniques = "T1200",
        Impact = "Potential unauthorized control of IoT devices, data leakage from connected devices, or privacy violations.",
        ProofOfConcept = "1. Analyze the app's IoT device communication protocols.\n2. Attempt to intercept or manipulate communications between the app and IoT devices.\n3. Demonstrate unauthorized device control or data access.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 306 } }, // CWE-306: Missing Authentication for Critical Function
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PLATFORM-1",
        MitreTechniques = "T1444",
        Impact = "Potential exposure of sensitive data through push notifications or execution of malicious payloads.",
        ProofOfConcept = "1. Analyze the app's push notification handling mechanism.\n2. Send crafted push notifications with malicious payloads or sensitive data.\n3. Demonstrate data leakage or unauthorized actions through push notifications.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 223 } }, // CWE-223: Omission of Security-relevant Information
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-RESILIENCE-2",
        MitreTechniques = "T1632",
        Impact = "Potential unauthorized access to sensitive data or functionality through cloned apps, bypassing of licensing or access controls.",
        ProofOfConcept = "1. Attempt to clone the app using various tools or techniques.\n2. Run the cloned app on a different device.\n3. Demonstrate access to sensitive data or functionality through the cloned app.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 656 } }, // CWE-656: Reliance on Security Through Obscurity
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PLATFORM-3",
        MitreTechniques = "T1417",
        Impact = "Potential capture of sensitive user input such as passwords or credit card information.",
        ProofOfConcept = "1. Create a proof-of-concept overlay app.\n2. Attempt to capture user input in the target app using the overlay.\n3. Demonstrate successful capture of sensitive information.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 1021 } }, // CWE-1021: Improper Restriction of Rendered UI Layers or Frames
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PLATFORM-1",
        MitreTechniques = "T1446",
        Impact = "Potential exposure of sensitive data on the device's home screen or unauthorized access to app functionality through the widget.",
        ProofOfConcept = "1. Analyze the app's widget implementation.\n2. Attempt to access sensitive data or functionality through the widget without proper authentication.\n3. Demonstrate data leakage or unauthorized actions via the widget.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 200 } }, // CWE-200: Exposure of Sensitive Information to an Unauthorized Actor
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-NETWORK-1, MASVS-CRYPTO-1",
        MitreTechniques = "T1199",
        Impact = "Potential unauthorized access to sensitive data processed at the edge, data manipulation, or privacy violations.",
        ProofOfConcept = "1. Analyze the app's communication with edge computing services.\n2. Attempt to intercept or manipulate data between the app and edge services.\n3. Demonstrate unauthorized access to sensitive information or injection of malicious data.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 319 } }, // CWE-319: Cleartext Transmission of Sensitive Information
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-CODE-3",
        MitreTechniques = "T1580",
        Impact = "Potential extraction of sensitive training data, manipulation of AI/ML model outputs, or unauthorized inference about users.",
        ProofOfConcept = "1. Analyze the app's AI/ML model implementation.\n2. Attempt model inversion or membership inference attacks.\n3. Demonstrate extraction of sensitive information or successful adversarial examples.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 1041 } }, // CWE-1041: Use of Redundant Code
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln Insecure5GImplementation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure 5G Network Feature Implementation",
        Description = "The app's implementation of 5G network features is insecure, potentially exposing users to network-based attacks or privacy violations.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure 5G protocol usage, properly handle network slicing security, and protect against 5G-specific privacy leaks.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-NETWORK-1",
        MitreTechniques = "T1546",
        Impact = "Potential exposure to advanced network-based attacks, location privacy violations, or unauthorized access to high-bandwidth capabilities.",
        ProofOfConcept = "1. Analyze the app's usage of 5G network features.\n2. Attempt to exploit 5G-specific vulnerabilities or privacy leaks.\n3. Demonstrate unauthorized access to sensitive data or network capabilities.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 300 } }, // CWE-300: Channel Accessible by Non-Endpoint
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PLATFORM-1",
        MitreTechniques = "T1437",
        Impact = "Potential unauthorized access to app functionality, exposure of sensitive information through voice channels, or execution of malicious voice commands.",
        ProofOfConcept = "1. Analyze the app's voice UI implementation.\n2. Attempt to bypass voice authentication or inject malicious voice commands.\n3. Demonstrate unauthorized actions or data access through voice interactions.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 350 } }, // CWE-350: Reliance on Reverse DNS Resolution for a Security-Critical Action
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-STORAGE-1, MASVS-NETWORK-1",
        MitreTechniques = "T1442",
        Impact = "Potential unauthorized access to user data on multiple devices, interception of sensitive information during sync, or injection of malicious data across devices.",
        ProofOfConcept = "1. Analyze the app's multi-device sync mechanism.\n2. Attempt to intercept or manipulate sync data between devices.\n3. Demonstrate unauthorized access to synced data or injection of malicious content.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 319 } }, // CWE-319: Cleartext Transmission of Sensitive Information
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureBehavioralBiometrics => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Behavioral Biometrics Implementation",
        Description = "The app's implementation of behavioral biometrics (e.g., typing patterns, gesture recognition) is insecure, potentially allowing bypass of authentication or unauthorized profiling.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure storage and processing of behavioral biometric data, use additional factors for critical actions, and protect against replay or injection attacks.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-AUTH-2",
        MitreTechniques = "T1556",
        Impact = "Potential bypass of behavioral authentication, unauthorized access to user accounts, or privacy violations through behavioral data analysis.",
        ProofOfConcept = "1. Analyze the app's behavioral biometrics implementation.\n2. Attempt to replay captured behavioral data or inject simulated behavioral patterns.\n3. Demonstrate bypass of behavioral authentication or extraction of user behavioral profiles.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 287 } }, // CWE-287: Improper Authentication
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureZeroTrustImplementation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Implementation of Zero Trust Architecture",
        Description = "The app's implementation of zero trust principles is flawed, potentially allowing unauthorized access to resources or bypass of continuous authentication mechanisms.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.6,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H",
        Remediation = "Implement robust continuous authentication, fine-grained access controls, and encrypt all data in transit and at rest following zero trust principles.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-AUTH-1, MASVS-NETWORK-1",
        MitreTechniques = "T1199",
        Impact = "Potential bypass of zero trust controls, unauthorized access to sensitive resources, or compromise of the entire security model.",
        ProofOfConcept = "1. Analyze the app's implementation of zero trust principles.\n2. Attempt to bypass continuous authentication or exploit gaps in access controls.\n3. Demonstrate unauthorized access to resources that should be protected under zero trust architecture.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 602 } }, // CWE-602: Client-Side Enforcement of Server-Side Security
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        RemediationPriority = RemediationPriority.Critical,
        OWASPRisk = "MASVS-CRYPTO-2, MASVS-ARCH-1",
        MitreTechniques = "T1573",
        Impact = "Potential theft of cryptocurrency assets, unauthorized blockchain transactions, or compromise of decentralized app (DApp) integrity.",
        ProofOfConcept = "1. Analyze the app's blockchain integration, focusing on key management and transaction signing.\n2. Attempt to extract private keys or manipulate transaction data.\n3. Demonstrate unauthorized blockchain transactions or compromise of DApp functionality.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 320 } }, // CWE-320: Key Management Errors
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureQuantumKeyDistribution => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Quantum Key Distribution Implementation",
        Description = "The app's implementation of quantum key distribution (QKD) protocols is flawed, potentially compromising the security of quantum-resistant cryptographic keys.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.6,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H",
        Remediation = "Ensure proper implementation of QKD protocols, secure the classical channel used in QKD, and implement additional safeguards against side-channel attacks on quantum devices.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-CRYPTO-1",
        MitreTechniques = "T1600",
        Impact = "Potential compromise of quantum-resistant keys, leading to vulnerabilities in post-quantum cryptographic systems.",
        ProofOfConcept = "1. Analyze the app's QKD implementation.\n2. Attempt to exploit vulnerabilities in the classical channel or QKD protocol.\n3. Demonstrate theoretical compromise of quantum keys or successful man-in-the-middle attack on QKD.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 310 } }, // CWE-310: Cryptographic Issues
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureHapticAuthentication => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Haptic Authentication Implementation",
        Description = "The app's implementation of haptic-based authentication (e.g., specific touch patterns or vibrations) is insecure, potentially allowing bypass of this authentication method.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure storage and processing of haptic patterns, use additional factors for authentication, and protect against replay attacks of haptic inputs.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-AUTH-2",
        MitreTechniques = "T1556",
        Impact = "Potential bypass of haptic-based authentication, leading to unauthorized access to protected app features or sensitive data.",
        ProofOfConcept = "1. Analyze the app's haptic authentication mechanism.\n2. Attempt to replay captured haptic patterns or inject simulated haptic inputs.\n3. Demonstrate bypass of haptic authentication.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 287 } }, // CWE-287: Improper Authentication
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureBrainComputerInterface => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Brain-Computer Interface Integration",
        Description = "The app's integration with brain-computer interface (BCI) technology is insecure, potentially exposing highly sensitive neural data or allowing unauthorized control through brain signals.",
        Risk = VulnRisk.Critical,
        Status = VulnStatus.Open,
        CVSS3 = 9.8,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H",
        Remediation = "Implement end-to-end encryption for neural data transmission, use secure enclaves for processing brain signals, and implement strict access controls and user consent mechanisms for BCI functionalities.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Critical,
        OWASPRisk = "MASVS-PRIVACY-1, MASVS-NETWORK-1",
        MitreTechniques = "T1430",
        Impact = "Potential exposure of highly sensitive neural data, unauthorized control of app functions through brain signals, or severe privacy violations.",
        ProofOfConcept = "1. Analyze the app's BCI data handling and processing mechanisms.\n2. Attempt to intercept or manipulate neural data transmissions.\n3. Demonstrate unauthorized access to neural data or control of app functions via simulated brain signals.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 359 } }, // CWE-359: Exposure of Private Personal Information to an Unauthorized Actor
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureSmartDust => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Smart Dust Integration",
        Description = "The app's integration with smart dust (tiny wireless microelectromechanical sensors) is insecure, potentially allowing unauthorized access to sensor data or manipulation of smart dust networks.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure communication protocols for smart dust networks, use encryption for sensor data, and implement robust authentication mechanisms for smart dust interactions.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-NETWORK-1, MASVS-PRIVACY-2",
        MitreTechniques = "T1200",
        Impact = "Potential unauthorized access to widespread sensor data, manipulation of smart environments, or privacy violations through covert sensing.",
        ProofOfConcept = "1. Analyze the app's communication with smart dust networks.\n2. Attempt to intercept or inject false sensor data.\n3. Demonstrate unauthorized access to smart dust sensor data or manipulation of smart dust-controlled systems.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 319 } }, // CWE-319: Cleartext Transmission of Sensitive Information
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureNeuromorphicComputing => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Neuromorphic Computing Implementation",
        Description = "The app's implementation of neuromorphic computing (brain-inspired computing architecture) is insecure, potentially exposing the trained model or allowing adversarial attacks on the neuromorphic system.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.7,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement secure model storage and execution environments for neuromorphic systems, protect against side-channel attacks, and use adversarial training techniques to improve model robustness.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-RESILIENCE-1",
        MitreTechniques = "T1580",
        Impact = "Potential extraction of sensitive neuromorphic models, manipulation of AI decision-making processes, or unauthorized inference about the system's training data.",
        ProofOfConcept = "1. Analyze the app's neuromorphic computing implementation.\n2. Attempt to extract the neuromorphic model or perform model inversion attacks.\n3. Demonstrate successful adversarial examples that manipulate the neuromorphic system's outputs.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 311 } }, // CWE-311: Missing Encryption of Sensitive Data
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureMetaAIImplementation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Meta-AI Implementation",
        Description = "The app's implementation of meta-AI (AI systems that create or optimize other AI systems) is insecure, potentially allowing unauthorized manipulation of AI generation processes or exposure of proprietary AI development techniques.",
        Risk = VulnRisk.Critical,
        Status = VulnStatus.Open,
        CVSS3 = 9.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H",
        Remediation = "Implement secure enclaves for meta-AI operations, use strong access controls and audit logging for all meta-AI interactions, and implement safeguards against the generation of malicious or biased AI models.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Critical,
        OWASPRisk = "MASVS-CRYPTO-1, MASVS-RESILIENCE-2",
        MitreTechniques = "T1190",
        Impact = "Potential exposure of proprietary AI generation techniques, creation of malicious AI models, or unauthorized access to powerful AI capabilities.",
        ProofOfConcept = "1. Analyze the app's meta-AI system architecture and security controls.\n2. Attempt to manipulate the AI generation process or extract information about the meta-AI system.\n3. Demonstrate the creation of an unauthorized or potentially malicious AI model using the meta-AI system.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 284 } }, // CWE-284: Improper Access Control
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureAirGappedInteraction => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Air-Gapped System Interaction",
        Description = "The app's mechanism for interacting with air-gapped systems (physically isolated from unsecured networks) is flawed, potentially bridging the air gap and compromising the isolated system's security.",
        Risk = VulnRisk.Critical,
        Status = VulnStatus.Open,
        CVSS3 = 9.3,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:C/C:H/I:H/A:N",
        Remediation = "Implement stringent security measures for any data transfer mechanisms to air-gapped systems, use sophisticated encryption and authentication for all interactions, and implement comprehensive auditing and monitoring.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.Critical,
        OWASPRisk = "MASVS-NETWORK-1, MASVS-RESILIENCE-3",
        MitreTechniques = "T1030",
        Impact = "Potential compromise of air-gapped system integrity, unauthorized data exfiltration from isolated networks, or injection of malware into secure environments.",
        ProofOfConcept = "1. Analyze the app's mechanisms for interacting with air-gapped systems.\n2. Attempt to exploit vulnerabilities in the data transfer process or bypass air gap controls.\n3. Demonstrate unauthorized data transfer to or from the air-gapped system.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 668 } }, // CWE-668: Exposure of Resource to Wrong Sphere
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-STORAGE-1",
        MitreTechniques = "T1145",
        Impact = "Potential exposure of sensitive credentials or cryptographic keys, leading to unauthorized access or data decryption.",
        ProofOfConcept = "1. Analyze the app's keychain/keystore usage.\n2. Attempt to extract data from the keychain/keystore using a jailbroken/rooted device.\n3. Demonstrate access to sensitive information stored in the keychain/keystore.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 522 } }, // CWE-522: Insufficiently Protected Credentials
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-CRYPTO-1",
        MitreTechniques = "T1557",
        Impact = "Potential prediction of generated values, leading to compromised encryption, session tokens, or other security-critical data.",
        ProofOfConcept = "1. Identify usage of random number generation in the app's code.\n2. Analyze the randomness quality of generated values.\n3. Demonstrate predictability or bias in the generated random numbers.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 338 } }, // CWE-338: Use of Cryptographically Weak Pseudo-Random Number Generator (PRNG)
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-AUTH-1",
        MitreTechniques = "T1200",
        Impact = "Potential unauthorized access to user accounts, session hijacking, or bypass of authentication mechanisms.",
        ProofOfConcept = "1. Analyze the app's SSO implementation and token handling.\n2. Attempt to forge or manipulate SSO tokens.\n3. Demonstrate unauthorized access using manipulated SSO credentials.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 287 } }, // CWE-287: Improper Authentication
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-NETWORK-1",
        MitreTechniques = "T1133",
        Impact = "Potential exposure of user traffic, unauthorized access to protected networks, or compromise of VPN credentials.",
        ProofOfConcept = "1. Analyze the app's VPN implementation and configuration.\n2. Attempt to intercept or manipulate VPN traffic.\n3. Demonstrate data leakage or unauthorized access through VPN vulnerabilities.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 319 } }, // CWE-319: Cleartext Transmission of Sensitive Information
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PLATFORM-1",
        MitreTechniques = "T1547",
        Impact = "Potential unauthorized access to app functionality or data through maliciously crafted custom URLs.",
        ProofOfConcept = "1. Identify custom URL schemes used by the app.\n2. Craft malicious URLs exploiting these schemes.\n3. Demonstrate unauthorized actions or data access through custom URL scheme exploitation.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 749 } }, // CWE-749: Exposed Dangerous Method or Function
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-CODE-4",
        MitreTechniques = "T1499",
        Impact = "Potential privilege escalation, unauthorized access to resources, or data corruption due to race conditions.",
        ProofOfConcept = "1. Identify potential TOCTOU vulnerabilities in the app's code.\n2. Create a proof-of-concept exploit that races between the check and use.\n3. Demonstrate unauthorized actions or data access through successful exploitation of the race condition.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 367 } }, // CWE-367: Time-of-check Time-of-use (TOCTOU) Race Condition
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-RESILIENCE-2",
        MitreTechniques = "T1622",
        Impact = "Easier reverse engineering and tampering of the app, potentially exposing sensitive logic or bypassing security controls.",
        ProofOfConcept = "1. Attempt to attach a debugger to the running app.\n2. Analyze the app's anti-debugging measures.\n3. Demonstrate successful bypassing of existing anti-debugging techniques.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 388 } }, // CWE-388: 7PK - Error Handling
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
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
        OWASPRisk = "MASVS-PRIVACY-1",
        MitreTechniques = "T1430",
        Impact = "Potential privacy violations, unauthorized access to user data, or increased attack surface due to unnecessary permissions.",
        ProofOfConcept = "1. Analyze the app's manifest or info.plist for requested permissions.\n2. Identify permissions that are not essential for core functionality.\n3. Demonstrate potential misuse of over-privileged permissions.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 250 } }, // CWE-250: Execution with Unnecessary Privileges
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };
    
    public static Vuln InsecureFileEncryption => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure File Encryption Implementation",
        Description = "The app's implementation of file encryption is flawed, potentially allowing unauthorized access to sensitive data stored in encrypted files.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 7.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:N/A:N",
        Remediation = "Implement strong, industry-standard encryption algorithms for file encryption. Use proper key management practices and ensure that encryption keys are securely stored.",
        RemediationComplexity = RemediationComplexity.High,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-STORAGE-1, MASVS-CRYPTO-1",
        MitreTechniques = "T1486",
        Impact = "Potential unauthorized access to sensitive data stored in encrypted files, leading to data breaches or privacy violations.",
        ProofOfConcept = "1. Identify files that should be encrypted by the app.\n2. Analyze the encryption implementation and key management.\n3. Attempt to decrypt files using extracted or weak keys.\n4. Demonstrate access to sensitive data from supposedly encrypted files.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 311 } }, // CWE-311: Missing Encryption of Sensitive Data
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureIpcDataValidation => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insufficient Data Validation in IPC Mechanisms",
        Description = "The app does not properly validate data received through inter-process communication (IPC) mechanisms, potentially allowing injection attacks or unauthorized actions.",
        Risk = VulnRisk.High,
        Status = VulnStatus.Open,
        CVSS3 = 8.1,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:N",
        Remediation = "Implement strict input validation for all data received through IPC mechanisms. Use allow lists for accepted data formats and sanitize all inputs before processing.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.High,
        OWASPRisk = "MASVS-PLATFORM-1",
        MitreTechniques = "T1559",
        Impact = "Potential execution of unauthorized commands, data manipulation, or access to sensitive information through maliciously crafted IPC messages.",
        ProofOfConcept = "1. Identify IPC mechanisms used by the app (e.g., Intents, content providers).\n2. Craft malicious payloads to send via these IPC channels.\n3. Demonstrate unauthorized actions or data access through exploitation of insufficient IPC data validation.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 20 } }, // CWE-20: Improper Input Validation
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };

    public static Vuln InsecureDataDeletion => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure Data Deletion Practices",
        Description = "The app does not securely delete sensitive data when it's no longer needed, potentially leaving residual data that could be recovered by attackers.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 5.5,
        CVSSVector = "CVSS:3.1/AV:L/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implement secure data deletion practices, such as overwriting data before deletion, using platform-specific secure deletion APIs, and ensuring that all copies of sensitive data are properly removed.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        OWASPRisk = "MASVS-STORAGE-1",
        MitreTechniques = "T1485",
        Impact = "Potential recovery of sensitive data from device storage, leading to unauthorized access to user information or app secrets.",
        ProofOfConcept = "1. Identify sensitive data that should be deleted by the app.\n2. Trigger data deletion functions in the app.\n3. Analyze device storage (including free space) for residual data.\n4. Demonstrate recovery of supposedly deleted sensitive information.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 226 } }, // CWE-226: Sensitive Information Uncleared Before Release
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };
    

    public static Vuln InsecureAppPermissions => new Vuln
    {
        Template = true,
        Language = Language.English,
        Name = "Insecure App Permissions Management",
        Description = "The app does not properly manage or validate its permissions, potentially leading to unauthorized access to sensitive device features or user data.",
        Risk = VulnRisk.Medium,
        Status = VulnStatus.Open,
        CVSS3 = 6.5,
        CVSSVector = "CVSS:3.1/AV:N/AC:L/PR:N/UI:R/S:U/C:H/I:N/A:N",
        Remediation = "Implement proper permission requests and checks, use runtime permissions on Android, and ensure the app gracefully handles denied permissions.",
        RemediationComplexity = RemediationComplexity.Medium,
        RemediationPriority = RemediationPriority.Medium,
        OWASPRisk = "MASVS-PLATFORM-1",
        MitreTechniques = "T1404",
        Impact = "Potential unauthorized access to sensitive device features or user data, privacy violations, or app instability due to unexpected permission states.",
        ProofOfConcept = "1. Analyze the app's permission requests and usage.\n2. Attempt to access sensitive features without granting permissions.\n3. Demonstrate unauthorized access or app instability due to improper permission handling.",
        VulnCwes = new List<VulnCwe> { new VulnCwe { CweId = 732 } }, // CWE-732: Incorrect Permission Assignment for Critical Resource
        cve = "N/A (This is a general vulnerability pattern, not tied to a specific CVE)"
    };
}*/