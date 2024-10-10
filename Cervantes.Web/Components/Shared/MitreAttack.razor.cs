using Microsoft.AspNetCore.Components;

namespace Cervantes.Web.Components.Shared;

public partial class MitreAttack : ComponentBase
{
    private IEnumerable<string> selectedTechniques = new HashSet<string>();
    
    private List<Tactic> tactics = new List<Tactic>
    {
        new Tactic { Id = "TA0043", Name = "Reconnaissance" },
        new Tactic { Id = "TA0042", Name = "Resource Development" },
        new Tactic { Id = "TA0001", Name = "Initial Access" },
        new Tactic { Id = "TA0002", Name = "Execution" },
        new Tactic { Id = "TA0003", Name = "Persistence" },
        new Tactic { Id = "TA0004", Name = "Privilege Escalation" },
        new Tactic { Id = "TA0005", Name = "Defense Evasion" },
        new Tactic { Id = "TA0006", Name = "Credential Access" },
        new Tactic { Id = "TA0007", Name = "Discovery" },
        new Tactic { Id = "TA0008", Name = "Lateral Movement" },
        new Tactic { Id = "TA0009", Name = "Collection" },
        new Tactic { Id = "TA0011", Name = "Command and Control" },
        new Tactic { Id = "TA0010", Name = "Exfiltration" },
        new Tactic { Id = "TA0040", Name = "Impact" }
    };

    private List<Technique> techniques = new List<Technique>
    {
       // Reconnaissance (TA0043)
    new Technique { Id = "T1595", Name = "Active Scanning", Description = "Adversaries may execute active reconnaissance scans to gather information that can be used during targeting. Active scans are those where the adversary probes victim infrastructure via network traffic, as opposed to other forms of reconnaissance that do not involve direct interaction.", TacticId = "TA0043" },
    new Technique { Id = "T1592", Name = "Gather Victim Host Information", Description = "Adversaries may gather information about the victim's hosts that can be used during targeting. Information about hosts may include a variety of details, including administrative data (ex: name, assigned IP, functionality, etc.) as well as specifics regarding its configuration (ex: operating system, language, etc.).", TacticId = "TA0043" },
    new Technique { Id = "T1589", Name = "Gather Victim Identity Information", Description = "Adversaries may gather information about the victim's identity that can be used during targeting. Information about identities may include a variety of details, including personal data (ex: employee names, email addresses, etc.) as well as sensitive credentials (ex: usernames and passwords, API keys, etc.).", TacticId = "TA0043" },
    new Technique { Id = "T1590", Name = "Gather Victim Network Information", Description = "Adversaries may gather information about the victim's networks that can be used during targeting. Information about networks may include a variety of details, including administrative data (ex: IP ranges, domain names, etc.) as well as specifics regarding its topology and operations.", TacticId = "TA0043" },
    new Technique { Id = "T1591", Name = "Gather Victim Org Information", Description = "Adversaries may gather information about the victim's organization that can be used during targeting. Information about an organization may include a variety of details, including the names of divisions/departments, physical locations, and the roles and responsibilities of specific individuals.", TacticId = "TA0043" },
    new Technique { Id = "T1598", Name = "Phishing for Information", Description = "Adversaries may send phishing messages to elicit sensitive information that can be used during targeting. Phishing for information is an attempt to trick targets into divulging information, frequently credentials or other actionable information.", TacticId = "TA0043" },
    new Technique { Id = "T1597", Name = "Search Closed Sources", Description = "Adversaries may search closed sources for information about victims that can be used during targeting. Information about victims may be available for purchase from reputable private sources and databases, such as paid subscriptions to business listings and similar services.", TacticId = "TA0043" },
    new Technique { Id = "T1596", Name = "Search Open Technical Databases", Description = "Adversaries may search freely available technical databases for information about victims that can be used during targeting. Information about victims may be available in online databases and repositories, such as registrations of domains/certificates, public collections of network data, and other freely available data generated from network interactions.", TacticId = "TA0043" },
    new Technique { Id = "T1593", Name = "Search Open Websites/Domains", Description = "Adversaries may search freely available websites and/or domains for information about victims that can be used during targeting. Information about victims may be available in various online sites, such as social media, news sites, or those hosting information about business operations such as hiring or requested/rewarded contracts.", TacticId = "TA0043" },
    new Technique { Id = "T1594", Name = "Search Victim-Owned Websites", Description = "Adversaries may search websites owned by the victim for information that can be used during targeting. Victim-owned websites may contain a variety of details, including names of departments/divisions, physical locations, and data about key employees such as names, roles, and contact info.", TacticId = "TA0043" },

    // Resource Development (TA0042)
    new Technique { Id = "T1583", Name = "Acquire Infrastructure", Description = "Adversaries may buy, lease, or rent infrastructure that can be used during targeting. A wide variety of infrastructure exists for hosting and orchestrating adversary operations.", TacticId = "TA0042" },
    new Technique { Id = "T1586", Name = "Compromise Accounts", Description = "Adversaries may compromise accounts with services that can be used during targeting. For operations incorporating social engineering, the utilization of an online persona may be important.", TacticId = "TA0042" },
    new Technique { Id = "T1584", Name = "Compromise Infrastructure", Description = "Adversaries may compromise infrastructure that can be used during targeting. Infrastructure solutions include physical or cloud servers, domains, and third-party systems and services.", TacticId = "TA0042" },
    new Technique { Id = "T1587", Name = "Develop Capabilities", Description = "Adversaries may build capabilities that can be used during targeting. Rather than purchasing, freely downloading, or stealing capabilities, adversaries may develop their own capabilities in-house.", TacticId = "TA0042" },
    new Technique { Id = "T1585", Name = "Establish Accounts", Description = "Adversaries may create and cultivate accounts with services that can be used during targeting. Adversaries can create accounts that can be used to build a persona to further operations.", TacticId = "TA0042" },
    new Technique { Id = "T1588", Name = "Obtain Capabilities", Description = "Adversaries may buy, steal, or download capabilities that can be used during targeting. Rather than developing their own capabilities in-house, adversaries may purchase, freely download, or steal them.", TacticId = "TA0042" },
    new Technique { Id = "T1608", Name = "Stage Capabilities", Description = "Adversaries may upload, install, or configure capabilities that can be used during targeting. To support their operations, adversaries may need to take capabilities they have developed or obtained and stage them on infrastructure under their control.", TacticId = "TA0042" },
    new Technique { Id = "T1598", Name = "Phishing for Information", Description = "Adversaries may send phishing messages to elicit sensitive information that can be used during targeting. Phishing for information is an attempt to trick targets into divulging information, frequently credentials or other actionable information.", TacticId = "TA0042" },

    // Initial Access (TA0001)
    new Technique { Id = "T1189", Name = "Drive-by Compromise", Description = "Adversaries may gain access to a system through a user visiting a website over the normal course of browsing. With this technique, the user's web browser is typically targeted for exploitation, but adversaries may also use compromised websites for non-exploitation behavior such as acquiring application access tokens.", TacticId = "TA0001" },
    new Technique { Id = "T1190", Name = "Exploit Public-Facing Application", Description = "Adversaries may attempt to take advantage of a weakness in an Internet-facing computer or program using software, data, or commands in order to cause unintended or unanticipated behavior. The weakness in the system can be a bug, a glitch, or a design vulnerability.", TacticId = "TA0001" },
    new Technique { Id = "T1133", Name = "External Remote Services", Description = "Adversaries may leverage external-facing remote services to initially access and/or persist within a network. Remote services such as VPNs, Citrix, and other access mechanisms allow users to connect to internal enterprise network resources from external locations.", TacticId = "TA0001" },
    new Technique { Id = "T1200", Name = "Hardware Additions", Description = "Adversaries may introduce computer accessories, computers, or networking hardware into a system or network that can be used as a vector to gain access. While public references of usage by APT groups are scarce, many penetration testers leverage hardware additions for initial access.", TacticId = "TA0001" },
    new Technique { Id = "T1566", Name = "Phishing", Description = "Adversaries may send phishing messages to gain access to victim systems. All forms of phishing are electronically delivered social engineering. Phishing can be targeted, known as spearphishing. In spearphishing, a specific individual, company, or industry will be targeted by the adversary.", TacticId = "TA0001" },
    new Technique { Id = "T1091", Name = "Replication Through Removable Media", Description = "Adversaries may move onto systems, possibly those on disconnected or air-gapped networks, by copying malware to removable media and taking advantage of Autorun features when the media is inserted into a system and executes.", TacticId = "TA0001" },
    new Technique { Id = "T1195", Name = "Supply Chain Compromise", Description = "Adversaries may manipulate products or product delivery mechanisms prior to receipt by a final consumer for the purpose of data or system compromise. Supply chain compromise can take place at any stage of the supply chain including: manipulation of development tools, manipulation of a development environment, manipulation of source code repositories, manipulation of software update/distribution mechanisms, compromised/infected system images, replacement of legitimate software with modified versions, etc.", TacticId = "TA0001" },
    new Technique { Id = "T1199", Name = "Trusted Relationship", Description = "Adversaries may breach or otherwise leverage organizations who have access to intended victims. Access through trusted third party relationship exploits an existing connection that may not be protected or receives less scrutiny than standard mechanisms of gaining access to a network.", TacticId = "TA0001" },
    new Technique { Id = "T1078", Name = "Valid Accounts", Description = "Adversaries may obtain and abuse credentials of existing accounts as a means of gaining Initial Access, Persistence, Privilege Escalation, or Defense Evasion. Compromised credentials may be used to bypass access controls placed on various resources on systems within the network and may even be used for persistent access to remote systems and externally available services, such as VPNs, Outlook Web Access and remote desktop.", TacticId = "TA0001" },
    new Technique { Id = "T1204", Name = "User Execution", Description = "An adversary may rely upon specific actions by a user in order to gain execution. Users may be subjected to social engineering to get them to execute malicious code by clicking links, opening documents, accepting prompts, etc.", TacticId = "TA0001" },

    // Execution (TA0002)
    new Technique { Id = "T1059", Name = "Command and Scripting Interpreter", Description = "Adversaries may abuse command and script interpreters to execute commands, scripts, or binaries. These interfaces and languages provide ways of interacting with computer systems and are a common feature across many different platforms.", TacticId = "TA0002" },
    new Technique { Id = "T1106", Name = "Native API", Description = "Adversaries may interact with the native OS application programming interface (API) to execute behaviors. Native APIs provide a controlled means of calling low-level OS services to interact with processes, manipulate the filesystem, and access system resources.", TacticId = "TA0002" },
    new Technique { Id = "T1053", Name = "Scheduled Task/Job", Description = "Adversaries may abuse task scheduling functionality to facilitate initial or recurring execution of malicious code. Utilities exist within all major operating systems to schedule programs or scripts to be executed at a specified date and time.", TacticId = "TA0002" },
    new Technique { Id = "T1129", Name = "Shared Modules", Description = "Adversaries may execute malicious payloads via loading shared modules. The Windows module loader can be instructed to load DLLs from arbitrary local paths and arbitrary Universal Naming Convention (UNC) network paths.", TacticId = "TA0002" },
    new Technique { Id = "T1072", Name = "Software Deployment Tools", Description = "Adversaries may gain access to and use third-party software suites installed within an enterprise network, such as administration, monitoring, and deployment systems, to move laterally through the network and execute malicious payloads.", TacticId = "TA0002" },
    new Technique { Id = "T1569", Name = "System Services", Description = "Adversaries may abuse system services or daemons to execute commands or programs. Adversaries can execute malicious content by interacting with or creating services. Many services are set to run at boot, which can aid in achieving persistence via Persistence/Create or Modify System Process.", TacticId = "TA0002" },
    new Technique { Id = "T1204", Name = "User Execution", Description = "An adversary may rely upon specific actions by a user in order to gain execution. Users may be subjected to social engineering to get them to execute malicious code by clicking links, opening documents, accepting prompts, etc.", TacticId = "TA0002" },
    new Technique { Id = "T1047", Name = "Windows Management Instrumentation", Description = "Adversaries may abuse Windows Management Instrumentation (WMI) to execute malicious commands and payloads. WMI is a Windows administration feature that provides a uniform environment for local and remote access to Windows system components.", TacticId = "TA0002" },
    new Technique { Id = "T1559", Name = "Inter-Process Communication", Description = "Adversaries may abuse inter-process communication (IPC) mechanisms for local code or command execution.", TacticId = "TA0002" },
    new Technique { Id = "T1609", Name = "Container Administration Command", Description = "Adversaries may abuse container administration commands to execute malicious commands or obtain sensitive information.", TacticId = "TA0002" },
    new Technique { Id = "T1559.001", Name = "Component Object Model", Description = "Adversaries may use the Windows Component Object Model (COM) for local code execution. COM is a system within Windows that enables interaction between software components through the operating system.", TacticId = "TA0002" },
    new Technique { Id = "T1559.002", Name = "Dynamic Data Exchange", Description = "Adversaries may use Windows Dynamic Data Exchange (DDE) to execute arbitrary commands. DDE is a client-server protocol for one-time and/or continuous inter-process communication (IPC) between applications.", TacticId = "TA0002" },
    new Technique { Id = "T1203", Name = "Exploitation for Client Execution", Description = "Adversaries may exploit software vulnerabilities in client applications to execute code. Vulnerabilities can exist in software due to unsecure coding practices that can lead to unanticipated behavior.", TacticId = "TA0002" },
    new Technique { Id = "T1559", Name = "Inter-Process Communication", Description = "Adversaries may abuse inter-process communication (IPC) mechanisms for local code or command execution. IPC is typically used by processes to share data, communicate with each other, or synchronize execution.", TacticId = "TA0002" },

    // Persistence (TA0003)
    new Technique {
        Id = "T1098",
        Name = "Account Manipulation",
        Description = "Adversaries may manipulate accounts to maintain access to victim systems. Account manipulation may consist of any action that preserves adversary access to a compromised account, such as modifying credentials or permission groups.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1197",
        Name = "BITS Jobs",
        Description = "Adversaries may abuse BITS jobs to persist and conduct network communications. The Background Intelligent Transfer Service (BITS) is a low-bandwidth, asynchronous file transfer mechanism exposed through Component Object Model (COM).",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1547",
        Name = "Boot or Logon Autostart Execution",
        Description = "Adversaries may configure system settings to automatically execute a program during system boot or user login. Many legitimate programs come configured to start automatically, so these techniques may be difficult to detect.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1037",
        Name = "Boot or Logon Initialization Scripts",
        Description = "Adversaries may use scripts automatically executed at boot or logon initialization to establish persistence. Initialization scripts can be used to perform administrative functions, which may often execute with higher privileges.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1176",
        Name = "Browser Extensions",
        Description = "Adversaries may abuse Internet browser extensions to establish persistent access to victim systems. Browser extensions or plugins are small programs that can add functionality and customize aspects of Internet browsers.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1554",
        Name = "Compromise Client Software Binary",
        Description = "Adversaries may modify client software binaries to establish persistent access to systems. Client software enables users to access services provided by a server. Common client software types are SSH clients, FTP clients, email clients, and web browsers.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1136",
        Name = "Create Account",
        Description = "Adversaries may create an account to maintain access to victim systems. With a sufficient level of access, creating such accounts may be used to establish secondary credentialed access that does not require persistent remote access tools to be deployed on the system.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1543",
        Name = "Create or Modify System Process",
        Description = "Adversaries may create or modify system processes to repeatedly execute malicious payloads as part of persistence. Creation or modification of a system process allows for persistent execution of malicious code and may avoid detection.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1546",
        Name = "Event Triggered Execution",
        Description = "Adversaries may establish persistence using system mechanisms that trigger execution based on specific events. Various operating systems have means to monitor and subscribe to events such as logons or other user activity.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1133",
        Name = "External Remote Services",
        Description = "Adversaries may leverage external-facing remote services to initially access and/or persist within a network. Remote services such as VPNs, Citrix, and other access mechanisms allow users to connect to internal enterprise network resources from external locations.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1574",
        Name = "Hijack Execution Flow",
        Description = "Adversaries may execute their own malicious payloads by hijacking the way operating systems run programs. Hijacking execution flow can be for the purposes of persistence, since this hijacked execution may reoccur over time.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1525",
        Name = "Implant Internal Image",
        Description = "Adversaries may implant cloud or container images with malicious code to establish persistence after gaining access to an environment. Amazon Web Services (AWS) Amazon Machine Images (AMIs), Google Cloud Platform (GCP) Images, and Azure Images as well as popular container runtimes such as Docker can be implanted or backdoored.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1556",
        Name = "Modify Authentication Process",
        Description = "Adversaries may modify authentication mechanisms and processes to access user credentials or enable otherwise unwarranted access to accounts. The authentication process is handled by mechanisms, such as the Local Security Authentication Server (LSASS) process and the Security Accounts Manager (SAM) on Windows, pluggable authentication modules (PAM) on Unix-based systems, and authorization plugins on MacOS systems, responsible for gathering, storing, and validating credentials.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1137",
        Name = "Office Application Startup",
        Description = "Adversaries may leverage Microsoft Office-based applications for persistence between startups. Microsoft Office is a fairly common application suite on Windows-based operating systems within an enterprise network. There are multiple mechanisms that can be used with Office for persistence.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1542",
        Name = "Pre-OS Boot",
        Description = "Adversaries may abuse Pre-OS Boot mechanisms as a way to establish persistence on a system. During the booting process of a computer, firmware and various startup services are loaded before the operating system. These programs control flow of execution before the operating system takes control.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1053",
        Name = "Scheduled Task/Job",
        Description = "Adversaries may abuse task scheduling functionality to facilitate initial or recurring execution of malicious code. Utilities exist within all major operating systems to schedule programs or scripts to be executed at a specified date and time.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1505",
        Name = "Server Software Component",
        Description = "Adversaries may abuse legitimate extensible development features of servers to establish persistent access to systems. Enterprise server applications may include features that allow developers to write and install software or scripts to extend the functionality of the main application.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1205",
        Name = "Traffic Signaling",
        Description = "Adversaries may use traffic signaling to hide open ports or other malicious functionality used for persistence or command and control. Traffic signaling involves the use of a magic value or sequence that must be sent to a system to trigger a special response, such as opening a closed port or executing a malicious task.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1078",
        Name = "Valid Accounts",
        Description = "Adversaries may obtain and abuse credentials of existing accounts as a means of gaining Initial Access, Persistence, Privilege Escalation, or Defense Evasion. Compromised credentials may be used to bypass access controls placed on various resources on systems within the network and may even be used for persistent access to remote systems and externally available services, such as VPNs, Outlook Web Access and remote desktop.",
        TacticId = "TA0003"
    },
    new Technique {
        Id = "T1612",
        Name = "Build Image on Host",
        Description = "Adversaries may build a container image directly on a host to bypass defenses that monitor for the retrieval of malicious images from a public registry.",
        TacticId = "TA0003"
    },
    
    // Privilege Escalation (TA0004)
    new Technique {
        Id = "T1548",
        Name = "Abuse Elevation Control Mechanism",
        Description = "Adversaries may bypass mechanisms designed to control elevated privileges to gain higher-level permissions. Most modern systems contain native elevation control mechanisms that are intended to limit privileges that running applications can use, as well as limit the privileges of users that are logged into systems.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1134",
        Name = "Access Token Manipulation",
        Description = "Adversaries may modify access tokens to operate under a different user or system security context to perform actions and evade detection. An adversary can use built-in Windows API functions to copy access tokens from existing processes; this is known as token stealing.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1547",
        Name = "Boot or Logon Autostart Execution",
        Description = "Adversaries may configure system settings to automatically execute a program during system boot or user login to maintain persistence or gain higher-level privileges on compromised systems. Many programs may be set to execute at login or system startup through various mechanisms.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1037",
        Name = "Boot or Logon Initialization Scripts",
        Description = "Adversaries may use scripts automatically executed at boot or logon initialization to establish persistence. Initialization scripts can be used to perform administrative functions, which may often execute with higher privileges.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1543",
        Name = "Create or Modify System Process",
        Description = "Adversaries may create or modify system processes to repeatedly execute malicious payloads as part of persistence. Creation or modification of system processes may require privileged access.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1484",
        Name = "Domain Policy Modification",
        Description = "Adversaries may modify the configuration settings of a domain to evade defenses and/or escalate privileges in domain environments. Domains provide a centralized means of managing how computer resources can be accessed by users and systems in the domain.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1611",
        Name = "Escape to Host",
        Description = "Adversaries may escape from a container to gain access to the underlying host. This can allow an adversary access to other containerized resources from the host level or to the host itself. In principle, containerized resources should provide a clear separation of application functionality and be isolated from the host environment.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1546",
        Name = "Event Triggered Execution",
        Description = "Adversaries may establish persistence and/or elevate privileges using system mechanisms that trigger execution based on specific events. Various operating systems have means to monitor and subscribe to events such as logons or other user activity such as running specific applications/binaries.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1068",
        Name = "Exploitation for Privilege Escalation",
        Description = "Adversaries may exploit software vulnerabilities in an attempt to elevate privileges. Exploitation of a software vulnerability occurs when an adversary takes advantage of a programming error in a program, service, or within the operating system software or kernel itself to execute adversary-controlled code.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1574",
        Name = "Hijack Execution Flow",
        Description = "Adversaries may execute their own malicious payloads by hijacking the way operating systems run programs. Hijacking execution flow can be for the purposes of persistence, since this hijacked execution may reoccur over time. Adversaries may also use these mechanisms to elevate privileges if registration of executables or loading of modules requires higher permissions than those of the current process.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1055",
        Name = "Process Injection",
        Description = "Adversaries may inject code into processes in order to evade process-based defenses as well as possibly elevate privileges. Process injection is a method of executing arbitrary code in the address space of a separate live process.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1053",
        Name = "Scheduled Task/Job",
        Description = "Adversaries may abuse task scheduling functionality to facilitate initial or recurring execution of malicious code. Utilities exist within all major operating systems to schedule programs or scripts to be executed at a specified date and time. A task can also be scheduled on a remote system, provided the proper authentication is met.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1078",
        Name = "Valid Accounts",
        Description = "Adversaries may obtain and abuse credentials of existing accounts as a means of gaining Initial Access, Persistence, Privilege Escalation, or Defense Evasion. Compromised credentials may be used to bypass access controls placed on various resources on systems within the network and may even be used for persistent access to remote systems and externally available services, such as VPNs, Outlook Web Access and remote desktop.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1558",
        Name = "Steal or Forge Kerberos Tickets",
        Description = "Adversaries may attempt to subvert Kerberos authentication by stealing or forging Kerberos tickets to enable Pass the Ticket. Kerberos is an authentication protocol widely used in modern Windows domain environments. In Kerberos authentication, Kerberos tickets are used to authenticate users and machines in the domain.",
        TacticId = "TA0004"
    },
    new Technique {
        Id = "T1548.001",
        Name = "Setuid and Setgid",
        Description = "An adversary may perform shell escapes or exploit vulnerabilities in an application with the setuid or setgid bits to get code running in a different user's context.",
        TacticId = "TA0004"
    },
    
    // Defense Evasion (TA0005)
    new Technique {
        Id = "T1548",
        Name = "Abuse Elevation Control Mechanism",
        Description = "Adversaries may bypass mechanisms designed to control elevated privileges to gain higher-level permissions. Most modern systems contain native elevation control mechanisms that are intended to limit privileges that running applications can use, as well as limit the privileges of users that are logged into systems.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1134",
        Name = "Access Token Manipulation",
        Description = "Adversaries may modify access tokens to operate under a different user or system security context to perform actions and evade detection. An adversary can use built-in Windows API functions to copy access tokens from existing processes; this is known as token stealing.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1197",
        Name = "BITS Jobs",
        Description = "Adversaries may abuse BITS jobs to avoid detection and circumvent defenses. Windows Background Intelligent Transfer Service (BITS) is a low-bandwidth, asynchronous file transfer mechanism exposed through Component Object Model (COM).",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1140",
        Name = "Deobfuscate/Decode Files or Information",
        Description = "Adversaries may use obfuscated files or information to hide artifacts of an intrusion from analysis. They may require separate mechanisms to decode or deobfuscate that information depending on how they intend to use it.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1006",
        Name = "Direct Volume Access",
        Description = "Adversaries may directly access a volume to bypass file access controls and file system monitoring. Windows allows programs to have direct access to logical volumes. Programs with direct access may read and write files directly from the drive by analyzing file system data structures.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1484",
        Name = "Domain Policy Modification",
        Description = "Adversaries may modify the configuration settings of a domain to evade defenses and/or escalate privileges in domain environments. Domains provide a centralized means of managing how computer resources can be accessed by users and systems in the domain.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1480",
        Name = "Execution Guardrails",
        Description = "Adversaries may use execution guardrails to constrain execution or actions based on adversary-supplied environmental conditions that are expected to be present on the target. Guardrails ensure that a payload only executes against an intended target and reduces collateral damage from an adversary's campaign.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1211",
        Name = "Exploitation for Defense Evasion",
        Description = "Adversaries may exploit a system or application vulnerability to bypass security features. Exploitation of a software vulnerability occurs when an adversary takes advantage of a programming error in a program, service, or within the operating system software or kernel itself to execute adversary-controlled code.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1222",
        Name = "File and Directory Permissions Modification",
        Description = "Adversaries may modify file or directory permissions/attributes to evade access control lists (ACLs) and access protected files. File and directory permissions are commonly managed by ACLs configured by the file or directory owner, or users with the appropriate permissions.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1564",
        Name = "Hide Artifacts",
        Description = "Adversaries may attempt to hide artifacts associated with their behaviors to evade detection. Operating systems may have features to hide various artifacts, such as important system files and administrative task execution, to avoid disrupting user work environments and prevent users from changing files or features on the system.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1574",
        Name = "Hijack Execution Flow",
        Description = "Adversaries may execute their own malicious payloads by hijacking the way operating systems run programs. Hijacking execution flow can be for the purposes of persistence, since this hijacked execution may reoccur over time.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1562",
        Name = "Impair Defenses",
        Description = "Adversaries may maliciously modify components of a victim environment in order to hinder or disable defensive mechanisms. This can include actions to disable or modify system firewalls, anti-virus, or other defensive software, as well as mechanisms native to the operating system, such as Windows Defender.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1070",
        Name = "Indicator Removal on Host",
        Description = "Adversaries may delete or modify artifacts generated on a host system to remove evidence of their presence or hinder defenses. This includes the deletion or modification of files or other system artifacts.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1202",
        Name = "Indirect Command Execution",
        Description = "Adversaries may abuse utilities that allow for command execution to bypass security restrictions that limit the use of command-line interpreters. Various Windows utilities may be used to execute commands, possibly without invoking cmd.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1036",
        Name = "Masquerading",
        Description = "Adversaries may attempt to manipulate features of their artifacts to make them appear legitimate or benign to users and/or security tools. Masquerading occurs when the name or location of an object, legitimate or malicious, is manipulated or abused for the sake of evading defenses and observation.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1556",
        Name = "Modify Authentication Process",
        Description = "Adversaries may modify authentication mechanisms and processes to access user credentials or enable otherwise unwarranted access to accounts. The authentication process is handled by mechanisms, such as the Local Security Authentication Server (LSASS) process and the Security Accounts Manager (SAM) on Windows, pluggable authentication modules (PAM) on Unix-based systems, and authorization plugins on MacOS systems, responsible for gathering, storing, and validating credentials.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1578",
        Name = "Modify Cloud Compute Infrastructure",
        Description = "An adversary may attempt to modify cloud compute infrastructure to evade defenses. This can include the creation, deletion, or modification of public cloud infrastructure, such as compute instances, storage devices, and networking.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1112",
        Name = "Modify Registry",
        Description = "Adversaries may interact with the Windows Registry to hide configuration information within Registry keys, remove information as part of cleaning up, or as part of other techniques to aid in persistence and execution.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1601",
        Name = "Modify System Image",
        Description = "Adversaries may make changes to the operating system of embedded network devices to weaken defenses and provide new capabilities for themselves. By modifying the operating system of a network device, adversaries can gain a persistence mechanism in the network and subvert security controls that exist in the device.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1599",
        Name = "Network Boundary Bridging",
        Description = "Adversaries may bridge network boundaries by compromising perimeter network devices. Breaching these devices may enable an adversary to bypass restrictions on traffic routing that otherwise separate trusted and untrusted networks.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1027",
        Name = "Obfuscated Files or Information",
        Description = "Adversaries may attempt to make an executable or file difficult to discover or analyze by encrypting, encoding, or otherwise obfuscating its contents on the system or in transit. This is common behavior that can be used across different platforms and the network to evade defenses.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1542",
        Name = "Pre-OS Boot",
        Description = "Adversaries may abuse Pre-OS Boot mechanisms as a way to establish persistence on a system. During the booting process of a computer, firmware and various startup services are loaded before the operating system. These programs control flow of execution before the operating system takes control.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1055",
        Name = "Process Injection",
        Description = "Adversaries may inject code into processes in order to evade process-based defenses as well as possibly elevate privileges. Process injection is a method of executing arbitrary code in the address space of a separate live process.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1207",
        Name = "Rogue Domain Controller",
        Description = "Adversaries may register a rogue Domain Controller to enable manipulation of Active Directory data. Rogue Domain Controllers can be used to inject false authentication data into the domain, manipulate Group Policy Objects, or subvert any number of other domain-specific functionality.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1014",
        Name = "Rootkit",
        Description = "Adversaries may use rootkits to hide the presence of programs, files, network connections, services, drivers, and other system components. Rootkits are programs that hide the existence of malware by intercepting and modifying operating system API calls that supply system information.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1218",
        Name = "Signed Binary Proxy Execution",
        Description = "Adversaries may bypass process and/or signature-based defenses by proxying execution of malicious content with signed binaries. Binaries signed with trusted digital certificates can execute on Windows systems protected by digital signature validation.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1216",
        Name = "Signed Script Proxy Execution",
        Description = "Adversaries may use scripts signed with trusted certificates to proxy execution of malicious files. Several Microsoft signed scripts that are default on Windows installations can be used to proxy execution of other files.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1553",
        Name = "Subvert Trust Controls",
        Description = "Adversaries may undermine security controls that will either warn users of untrusted activity or prevent execution of untrusted programs. Operating systems and security products may contain mechanisms to identify or block the execution of files and other programs that do not meet a set of trust criteria.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1221",
        Name = "Template Injection",
        Description = "Adversaries may create or modify references in Office document templates to conceal malicious code or force authentication attempts. Microsoft's Office Open XML (OOXML) specification defines an XML-based format for Office documents (.docx, xlsx, .pptx) to replace older binary formats (.doc, .xls, .ppt).",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1205",
        Name = "Traffic Signaling",
        Description = "Adversaries may use traffic signaling to hide open ports or other malicious functionality used for persistence or command and control. Traffic signaling involves the use of a magic value or sequence that must be sent to a system to trigger a special response, such as opening a closed port or executing a malicious task.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1127",
        Name = "Trusted Developer Utilities Proxy Execution",
        Description = "Adversaries may take advantage of trusted developer utilities to proxy execution of malicious payloads. There are many utilities used for software development related tasks that can be used to execute code in various forms to assist in development, debugging, and reverse engineering.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1535",
        Name = "Unused/Unsupported Cloud Regions",
        Description = "Adversaries may create cloud instances in unused geographic service regions to evade detection. Access is usually obtained through compromising accounts used to manage cloud infrastructure.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1550",
        Name = "Use Alternate Authentication Material",
        Description = "Adversaries may use alternate authentication material, such as password hashes, Kerberos tickets, and application access tokens, in order to move laterally within an environment and bypass normal system access controls.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1078",
        Name = "Valid Accounts",
        Description = "Adversaries may obtain and abuse credentials of existing accounts as a means of gaining Initial Access, Persistence, Privilege Escalation, or Defense Evasion. Compromised credentials may be used to bypass access controls placed on various resources on systems within the network and may even be used for persistent access to remote systems and externally available services, such as VPNs, Outlook Web Access and remote desktop.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1497",
        Name = "Virtualization/Sandbox Evasion",
        Description = "Adversaries may employ various means to detect and avoid virtualization and analysis environments. This may include changing behaviors based on the results of checks for the presence of artifacts indicative of a virtual machine environment (VME) or sandbox. If the adversary detects a VME, they may alter their malware to disengage from the victim or conceal the core functions of the implant.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1600",
        Name = "Weaken Encryption",
        Description = "Adversaries may compromise a network device's encryption capability in order to bypass encryption that would otherwise protect data communications. Encryption can be used to protect information in transit over a network.",
        TacticId = "TA0005"
    },
    new Technique {
        Id = "T1220",
        Name = "XSL Script Processing",
        Description = "Adversaries may bypass application control and obscure execution of code by embedding scripts inside XSL files. Extensible Stylesheet Language (XSL) files are commonly used to describe the processing and rendering of data within XML files.",
        TacticId = "TA0005"
    },
    
    // Credential Access (TA0006)
    new Technique {
        Id = "T1110",
        Name = "Brute Force",
        Description = "Adversaries may attempt to gain unauthorized access to accounts by systematically guessing passwords. This can include trying common passwords, dictionary words, or exhaustively trying all possible character combinations.",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1555",
        Name = "Credentials from Password Stores",
        Description = "Adversaries may search for common password storage locations to obtain user credentials. Passwords are stored in several places on a system, depending on the operating system or application holding the credentials.",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1212",
        Name = "Exploitation for Credential Access",
        Description = "Adversaries may exploit software vulnerabilities in an attempt to collect credentials. Exploitation of a software vulnerability occurs when an adversary takes advantage of a programming error in a program, service, or within the operating system software or kernel itself to execute adversary-controlled code.",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1187",
        Name = "Forced Authentication",
        Description = "Adversaries may gather credential material by invoking or forcing a user to automatically provide authentication information through a mechanism in which they can intercept.",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1056",
        Name = "Input Capture",
        Description = "Adversaries may use methods of capturing user input to obtain credentials or collect information. During normal system usage, users often provide credentials to various systems as part of their daily activities. Input capture mechanisms may be transparent to the user (e.g. keylogging) or rely on deceiving the user into providing inputs into what they believe to be a genuine service (e.g. phishing).",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1557",
        Name = "Man-in-the-Middle",
        Description = "Adversaries may attempt to position themselves between two or more networked devices using an adversary-in-the-middle (AiTM) technique to support follow-on behaviors such as Network Sniffing or Transmitted Data Manipulation. By abusing features of common networking protocols that can determine the flow of network traffic (e.g. ARP, DNS, LLMNR, etc.), adversaries may force a device to communicate through an adversary controlled system so they can collect information or perform additional actions.",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1556",
        Name = "Modify Authentication Process",
        Description = "Adversaries may modify authentication mechanisms and processes to access user credentials or enable otherwise unwarranted access to accounts. The authentication process is handled by mechanisms, such as the Local Security Authentication Server (LSASS) process and the Security Accounts Manager (SAM) on Windows, pluggable authentication modules (PAM) on Unix-based systems, and authorization plugins on MacOS systems, responsible for gathering, storing, and validating credentials.",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1040",
        Name = "Network Sniffing",
        Description = "Adversaries may sniff network traffic to capture information about an environment, including authentication material passed over the network. Network sniffing refers to using the network interface on a system to monitor or capture information sent over a wired or wireless connection.",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1003",
        Name = "OS Credential Dumping",
        Description = "Adversaries may attempt to dump credentials to obtain account login and credential material, normally in the form of a hash or a clear text password, from the operating system and software. Credentials can then be used to perform Lateral Movement and access restricted information.",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1528",
        Name = "Steal Application Access Token",
        Description = "Adversaries may steal application access tokens as a means of acquiring credentials to access remote systems and resources. Application access tokens are used to make authorized API requests on behalf of a user or service and are commonly used as a way to access resources in cloud and container-based applications and software-as-a-service (SaaS).",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1558",
        Name = "Steal or Forge Kerberos Tickets",
        Description = "Adversaries may attempt to subvert Kerberos authentication by stealing or forging Kerberos tickets to enable Pass the Ticket. Kerberos is an authentication protocol widely used in modern Windows domain environments. In Kerberos authentication, Kerberos tickets are used to authenticate users and machines in the domain.",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1539",
        Name = "Steal Web Session Cookie",
        Description = "An adversary may steal web application or service session cookies and use them to gain unauthorized access to user accounts. Web applications and services often use session cookies as an authentication token after a user has authenticated to a website.",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1111",
        Name = "Two-Factor Authentication Interception",
        Description = "Adversaries may target two-factor authentication mechanisms to gain access to credentials that can be used to access systems, services, and network resources. Two-factor authentication (2FA) is a security process in which the user provides two authentication factors to verify their identity.",
        TacticId = "TA0006"
    },
    new Technique {
        Id = "T1552",
        Name = "Unsecured Credentials",
        Description = "Adversaries may search compromised systems to find and obtain insecurely stored credentials. These credentials can be stored and/or misplaced in many locations on a system, including plaintext files, configuration files, or other specialized files.",
        TacticId = "TA0006"
    },
    new Technique { Id = "T1606", Name = "Forge Web Credentials", Description = "Adversaries may forge web credentials to access protected web resources. This may involve creating or modifying authentication tokens, cookies, or other web session identifiers to bypass authentication mechanisms.", TacticId = "TA0006" },
    new Technique { Id = "T1613", Name = "Container API", Description = "Adversaries may use container administration APIs to access sensitive information or execute commands on container hosts. Container APIs, such as the Docker API, can provide a wide range of functionality for managing containers and accessing host resources.", TacticId = "TA0006" },

    // Discovery (TA0007)
    new Technique {
        Id = "T1087",
        Name = "Account Discovery",
        Description = "Adversaries may attempt to get a listing of accounts on a system or within an environment. This information can help adversaries determine which accounts exist to aid in follow-on behavior.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1010",
        Name = "Application Window Discovery",
        Description = "Adversaries may attempt to get information about open windows on a system. Window listings could convey information about how the system is used or give context to information displayed on the screen.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1217",
        Name = "Browser Bookmark Discovery",
        Description = "Adversaries may enumerate browser bookmarks to learn more about compromised hosts. Browser bookmarks may reveal personal information about users (ex: banking sites, interests, social media, etc.) as well as details about internal network resources such as servers, tools/dashboards, or other related infrastructure.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1580",
        Name = "Cloud Infrastructure Discovery",
        Description = "An adversary may attempt to discover infrastructure and resources that are available within an infrastructure-as-a-service (IaaS) environment. This includes compute service resources such as instances, virtual machines, and snapshots as well as resources of other services including the storage and database services.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1538",
        Name = "Cloud Service Dashboard",
        Description = "An adversary may use a cloud service dashboard GUI with stolen credentials to gain useful information from an operational cloud environment, such as specific services, resources, and features enabled.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1526",
        Name = "Cloud Service Discovery",
        Description = "An adversary may attempt to enumerate the cloud services running on a system after gaining access. These methods can differ from platform-as-a-service (PaaS), to infrastructure-as-a-service (IaaS), or software-as-a-service (SaaS).",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1613",
        Name = "Container and Resource Discovery",
        Description = "Adversaries may attempt to discover containers and other resources that are available within a containers environment. Other resources may include images, deployments, pods, nodes, and other information such as the status of a cluster.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1482",
        Name = "Domain Trust Discovery",
        Description = "Adversaries may attempt to gather information on domain trust relationships that may be used to identify lateral movement opportunities in Windows multi-domain/forest environments. Domain trusts provide a mechanism for a domain to allow access to resources based on the authentication procedures of another domain.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1083",
        Name = "File and Directory Discovery",
        Description = "Adversaries may enumerate files and directories or may search in specific locations of a host or network share for certain information within a file system. Adversaries may use the information from File and Directory Discovery during automated discovery to shape follow-on behaviors, including whether or not the adversary fully infects the target and/or attempts specific actions.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1046",
        Name = "Network Service Scanning",
        Description = "Adversaries may attempt to get a listing of services running on remote hosts, including those that may be vulnerable to remote software exploitation. Methods to acquire this information include port scans and vulnerability scans using tools that are brought onto a system.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1135",
        Name = "Network Share Discovery",
        Description = "Adversaries may look for folders and drives shared on remote systems as a means of identifying sources of information to gather as a precursor for Collection and to identify potential systems of interest for Lateral Movement. Networks often contain shared network drives and folders that enable users to access file directories on various systems across a network.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1040",
        Name = "Network Sniffing",
        Description = "Adversaries may sniff network traffic to capture information about an environment, including authentication material passed over the network. Network sniffing refers to using the network interface on a system to monitor or capture information sent over a wired or wireless connection.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1201",
        Name = "Password Policy Discovery",
        Description = "Adversaries may attempt to access detailed information about the password policy used within an enterprise network. Password policies for networks are a way to enforce complex passwords that are difficult to guess or crack through strict requirements.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1120",
        Name = "Peripheral Device Discovery",
        Description = "Adversaries may attempt to gather information about attached peripheral devices and components connected to a computer system. Peripheral devices could include auxiliary resources that support a variety of functionalities such as keyboards, printers, cameras, smart card readers, or removable storage.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1069",
        Name = "Permission Groups Discovery",
        Description = "Adversaries may attempt to find group and permission settings. This information can help adversaries determine which user accounts and groups are available, the membership of users in particular groups, and which users and groups have elevated permissions.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1057",
        Name = "Process Discovery",
        Description = "Adversaries may attempt to get information about running processes on a system. Information obtained could be used to gain an understanding of common software/applications running on systems within the network.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1012",
        Name = "Query Registry",
        Description = "Adversaries may interact with the Windows Registry to gather information about the system, configuration, and installed software. The Registry contains a significant amount of information about the operating system, configuration, software, and security.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1018",
        Name = "Remote System Discovery",
        Description = "Adversaries may attempt to get a listing of other systems by IP address, hostname, or other logical identifier on a network that may be used for Lateral Movement from the current system. Functionality could exist within remote access tools to enable this, but utilities available on the operating system could also be used.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1518",
        Name = "Software Discovery",
        Description = "Adversaries may attempt to get a listing of software and software versions that are installed on a system or in a cloud environment. Adversaries may use the information from Software Discovery during automated discovery to shape follow-on behaviors, including whether or not the adversary fully infects the target and/or attempts specific actions.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1082",
        Name = "System Information Discovery",
        Description = "An adversary may attempt to get detailed information about the operating system and hardware, including version, patches, hotfixes, service packs, and architecture. Adversaries may use the information from System Information Discovery during automated discovery to shape follow-on behaviors, including whether or not the adversary fully infects the target and/or attempts specific actions.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1614",
        Name = "System Location Discovery",
        Description = "Adversaries may gather information in an attempt to calculate the geographical location of a victim host. Adversaries may use the information from System Location Discovery during automated discovery to shape follow-on behaviors, including whether or not the adversary fully infects the target and/or attempts specific actions.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1016",
        Name = "System Network Configuration Discovery",
        Description = "Adversaries may look for details about the network configuration and settings of systems they access or through information discovery of remote systems. Several operating system administration utilities exist that can be used to gather this information.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1049",
        Name = "System Network Connections Discovery",
        Description = "Adversaries may attempt to get a listing of network connections to or from the compromised system they are currently accessing or from remote systems by querying for information over the network.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1033",
        Name = "System Owner/User Discovery",
        Description = "Adversaries may attempt to identify the primary user, currently logged in user, set of users that commonly uses a system, or whether a user is actively using the system. They may do this, for example, by retrieving account usernames or by using OS tooling to understand currently logged in users.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1007",
        Name = "System Service Discovery",
        Description = "Adversaries may try to get information about registered services. Commands that may obtain information about services using operating system utilities are 'sc,' 'tasklist /svc' using Tasklist, and 'net start' using Net, but adversaries may also use other tools as well.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1124",
        Name = "System Time Discovery",
        Description = "An adversary may gather the system time and/or time zone from a local or remote system. The system time is set and stored by the Windows Time Service within a domain to maintain time synchronization between systems and services in an enterprise network.",
        TacticId = "TA0007"
    },
    new Technique {
        Id = "T1497",
        Name = "Virtualization/Sandbox Evasion",
        Description = "Adversaries may employ various means to detect and avoid virtualization and analysis environments. This may include changing behaviors based on the results of checks for the presence of artifacts indicative of a virtual machine environment (VME) or sandbox. If the adversary detects a VME, they may alter their malware to disengage from the victim or conceal the core functions of the implant.",
        TacticId = "TA0007"
    },
    new Technique { Id = "T1601.001", Name = "Patch System Image", Description = "Adversaries may modify the operating system of embedded network devices to weaken defenses and provide new capabilities for themselves. By modifying the operating system of a network device, adversaries can gain a persistence mechanism in the network and subvert security controls that exist in the device.", TacticId = "TA0005" },
    new Technique { Id = "T1601.002", Name = "Downgrade System Image", Description = "Adversaries may downgrade the operating system of an embedded network device to a version that is vulnerable to specific exploits or has fewer security features. By downgrading the operating system, adversaries can gain a persistence mechanism in the network and subvert security controls that exist in newer versions.", TacticId = "TA0005" },
    new Technique { Id = "T1578", Name = "Modify Cloud Compute Infrastructure", Description = "Adversaries may modify cloud compute infrastructure to evade defenses and/or escalate privileges. This can include modifying virtual machines, serverless functions, or other cloud-based compute resources.", TacticId = "TA0005" },
    new Technique { Id = "T1619", Name = "Cloud Storage Object Discovery", Description = "Adversaries may enumerate objects in cloud storage infrastructure to better understand the cloud environment and the data that is present. This information may be used to inform future actions or to identify valuable information.", TacticId = "TA0007" },
    new Technique { Id = "T1622", Name = "Debugger Evasion", Description = "Adversaries may employ various means to detect and avoid debuggers. Debuggers are typically used by defenders to trace and analyze the execution of potential malware payloads.", TacticId = "TA0007" },
    new Technique { Id = "T1614", Name = "System Location Discovery", Description = "Adversaries may attempt to identify the physical location of a victim system. This information could be used to determine the geographic location of the target organization or to inform follow-on behaviors.", TacticId = "TA0007" },
    new Technique { Id = "T1613", Name = "Container and Resource Discovery", Description = "Adversaries may attempt to discover containers and other resources that are available within a containers environment. This may include enumerating containers, images, volumes, and other container-related resources.", TacticId = "TA0007" },

    // Lateral Movement (TA0008)
    new Technique { Id = "T1210", Name = "Exploitation of Remote Services", Description = "Adversaries may exploit remote services to gain unauthorized access to internal systems. This can be done through exploitation of vulnerabilities in services that are exposed to the internet or internal network.", TacticId = "TA0008" },
    new Technique { Id = "T1534", Name = "Internal Spearphishing", Description = "Adversaries may use internal spearphishing to gain access to additional information or systems within the target network. This involves sending malicious emails from compromised internal accounts to other users within the organization.", TacticId = "TA0008" },
    new Technique { Id = "T1570", Name = "Lateral Tool Transfer", Description = "Adversaries may transfer tools or other files between systems in a compromised environment. This may be done to stage tools for lateral movement or to accomplish other objectives.", TacticId = "TA0008" },
    new Technique { Id = "T1563", Name = "Remote Service Session Hijacking", Description = "Adversaries may hijack legitimate remote sessions to move laterally within a network. This can involve taking over a disconnected session or creating a new session masquerading as the legitimate user.", TacticId = "TA0008" },
    new Technique { Id = "T1021", Name = "Remote Services", Description = "Adversaries may use legitimate remote services to move laterally within a network. This can include using remote desktop protocols, SSH, VNC, or other remote access tools.", TacticId = "TA0008" },
    new Technique { Id = "T1091", Name = "Replication Through Removable Media", Description = "Adversaries may move onto systems by copying malware to removable media and taking advantage of autorun features when the media is inserted into a system.", TacticId = "TA0008" },
    new Technique { Id = "T1072", Name = "Software Deployment Tools", Description = "Adversaries may use legitimate software deployment tools to move laterally within a network. This can involve using system management tools to deploy malicious payloads to multiple systems simultaneously.", TacticId = "TA0008" },
    new Technique { Id = "T1080", Name = "Taint Shared Content", Description = "Adversaries may taint shared content to move laterally within a network. This can involve inserting malicious content into legitimate files that are shared between systems or users.", TacticId = "TA0008" },
    new Technique { Id = "T1550", Name = "Use Alternate Authentication Material", Description = "Adversaries may use alternate authentication material, such as password hashes or Kerberos tickets, to move laterally within a network without needing to know the plaintext password.", TacticId = "TA0008" },
    // Collection (TA0008)
    new Technique { Id = "T1560", Name = "Archive Collected Data", Description = "Adversaries may compress and/or encrypt data that is collected prior to exfiltration. This can be done to minimize the amount of data sent over the network and to evade detection.", TacticId = "TA0009" },
    new Technique { Id = "T1123", Name = "Audio Capture", Description = "Adversaries may capture audio recordings from compromised systems. This can include capturing microphone input to gather sensitive information or to monitor the victim's activities.", TacticId = "TA0009" },
    new Technique { Id = "T1119", Name = "Automated Collection", Description = "Adversaries may use automated techniques to collect data from a system. This can involve using scripts or tools to gather files matching certain criteria or to extract specific types of information.", TacticId = "TA0009" },
    new Technique { Id = "T1115", Name = "Clipboard Data", Description = "Adversaries may collect data stored in the clipboard from users copying and pasting information. This can capture sensitive information such as passwords or other credentials.", TacticId = "TA0009" },
    new Technique { Id = "T1530", Name = "Data from Cloud Storage Object", Description = "Adversaries may access data objects from cloud storage services to collect sensitive information. This can involve accessing and downloading files from services like Amazon S3, Google Cloud Storage, or Azure Blob Storage.", TacticId = "TA0009" },
    new Technique { Id = "T1602", Name = "Data from Configuration Repository", Description = "Adversaries may collect data from configuration repositories, which can contain sensitive information about the network and its systems. This can include accessing configuration management databases or version control systems.", TacticId = "TA0009" },
    new Technique { Id = "T1213", Name = "Data from Information Repositories", Description = "Adversaries may collect data from information repositories within a victim's network. This can include accessing internal wikis, document management systems, or other centralized information stores.", TacticId = "TA0009" },
    new Technique { Id = "T1005", Name = "Data from Local System", Description = "Adversaries may collect data stored on local systems, including files, documents, and other sensitive information. This can involve searching for and copying specific file types or accessing user directories.", TacticId = "TA0009" },
    new Technique { Id = "T1039", Name = "Data from Network Shared Drive", Description = "Adversaries may collect data from shared network drives that are accessible from the compromised system. This can provide access to a wide range of organizational data.", TacticId = "TA0009" },
    new Technique { Id = "T1025", Name = "Data from Removable Media", Description = "Adversaries may collect data from removable media connected to compromised systems. This can include copying files from USB drives, external hard drives, or other portable storage devices.", TacticId = "TA0009" },
    new Technique { Id = "T1074", Name = "Data Staged", Description = "Adversaries may stage collected data in a central location or directory prior to exfiltration. This can involve moving and aggregating data from multiple sources to prepare for efficient exfiltration.", TacticId = "TA0009" },
    new Technique { Id = "T1114", Name = "Email Collection", Description = "Adversaries may collect email data from compromised user accounts or email servers. This can provide access to sensitive communications and attachments.", TacticId = "TA0009" },
    new Technique { Id = "T1056", Name = "Input Capture", Description = "Adversaries may use various methods to capture user input on a system. This can include keylogging, capturing touch input on mobile devices, or recording other forms of user interaction.", TacticId = "TA0009" },
    new Technique { Id = "T1185", Name = "Man in the Browser", Description = "Adversaries may use malware to intercept and collect data from web browsers. This can involve modifying browser functionality to capture form inputs, steal credentials, or manipulate web sessions.", TacticId = "TA0009" },
    new Technique { Id = "T1557", Name = "Man-in-the-Middle", Description = "Adversaries may attempt to intercept network traffic between systems to collect sensitive information. This can involve techniques like ARP spoofing or DNS hijacking to redirect traffic through a controlled system.", TacticId = "TA0009" },
    new Technique { Id = "T1113", Name = "Screen Capture", Description = "Adversaries may capture screenshots of the compromised system to gather sensitive information. This can provide visual data about the system's current state and any displayed information.", TacticId = "TA0009" },
    new Technique { Id = "T1125", Name = "Video Capture", Description = "Adversaries may capture video recordings from compromised systems. This can include recording the screen or accessing connected cameras to monitor the victim's activities.", TacticId = "TA0009" },
    // Command and Control (TA0011)
    new Technique { Id = "T1071", Name = "Application Layer Protocol", Description = "Adversaries may communicate using application layer protocols to avoid detection/network filtering by blending in with existing traffic. Commands to the remote system, and often the results of those commands, will be embedded within the protocol traffic between the client and server.", TacticId = "TA0011" },
    new Technique { Id = "T1092", Name = "Communication Through Removable Media", Description = "Adversaries may use removable media to communicate with systems that are not connected to networks. This can be used to evade typical network defenses and to bridge air-gapped networks.", TacticId = "TA0011" },
    new Technique { Id = "T1132", Name = "Data Encoding", Description = "Adversaries may encode data to make the content of command and control traffic more difficult to detect. Command and control (C2) information can be encoded using a standard data encoding system.", TacticId = "TA0011" },
    new Technique { Id = "T1001", Name = "Data Obfuscation", Description = "Adversaries may obfuscate command and control traffic to make it more difficult to detect. This may include adding junk data to protocol traffic, using steganography, or other tricks to hide the true nature of the traffic.", TacticId = "TA0011" },
    new Technique { Id = "T1568", Name = "Dynamic Resolution", Description = "Adversaries may dynamically establish connections to command and control infrastructure to evade common detections and remediations. This may include using dynamic DNS or algorithmically generated domains.", TacticId = "TA0011" },
    new Technique { Id = "T1573", Name = "Encrypted Channel", Description = "Adversaries may use encrypted channels to hide command and control traffic from detection. By utilizing encryption, adversaries can make it difficult for defenders to decipher the content of command and control communications.", TacticId = "TA0011" },
    new Technique { Id = "T1008", Name = "Fallback Channels", Description = "Adversaries may use fallback or alternate communication channels if the primary channel is compromised or inaccessible. This helps to ensure continued command and control even if the main method is discovered or blocked.", TacticId = "TA0011" },
    new Technique { Id = "T1105", Name = "Ingress Tool Transfer", Description = "Adversaries may transfer tools or other files from an external system into a compromised environment. This may include downloading payloads directly from the internet or copying tools from an adversary-controlled system.", TacticId = "TA0011" },
    new Technique { Id = "T1104", Name = "Multi-Stage Channels", Description = "Adversaries may create multiple stages for command and control that are employed under different conditions or for certain functions. This may involve dropping new code or using a series of protocols to establish a connection with controller nodes.", TacticId = "TA0011" },
    new Technique { Id = "T1095", Name = "Non-Application Layer Protocol", Description = "Adversaries may use a non-application layer protocol for communication between host and C2 server or among infected hosts within a network. This could include using network layer protocols or other lower-level protocols that are not typically used for command and control.", TacticId = "TA0011" },
    new Technique { Id = "T1571", Name = "Non-Standard Port", Description = "Adversaries may communicate using a protocol and port paring that are typically not associated. For example, HTTPS over port 8088 or port 587 when traditional HTTPS traffic uses port 443.", TacticId = "TA0011" },
    new Technique { Id = "T1572", Name = "Protocol Tunneling", Description = "Adversaries may tunnel network communications to and from a victim system within a separate protocol to avoid detection/network filtering and/or to reduce data attribution. This may include encapsulating the traffic within a different protocol.", TacticId = "TA0011" },
    new Technique { Id = "T1090", Name = "Proxy", Description = "Adversaries may use a connection proxy to direct network traffic between systems or act as an intermediary for network communications. This can be used to manage command and control communications, reduce the number of simultaneous outbound network connections, or disguise the source of traffic.", TacticId = "TA0011" },
    new Technique { Id = "T1219", Name = "Remote Access Software", Description = "Adversaries may use legitimate desktop support and remote access software, such as TeamViewer, AnyDesk, LogMeIn, etc., to establish an interactive command and control channel to target systems within networks.", TacticId = "TA0011" },
    new Technique { Id = "T1205", Name = "Traffic Signaling", Description = "Adversaries may use specific traffic patterns or signals to hide open ports or other malicious functionality used for persistence or command and control. This may involve port knocking sequences or other predetermined patterns that need to be sent before a port will be opened or a service will respond.", TacticId = "TA0011" },
    new Technique { Id = "T1102", Name = "Web Service", Description = "Adversaries may use an existing, legitimate external Web service as a means for relaying data to/from a compromised system. Popular websites and social media platforms can be used as a mechanism for command and control to blend in with traffic that is consistently expected.", TacticId = "TA0011" },
    new Technique { Id = "T1095", Name = "Non-Application Layer Protocol", Description = "Adversaries may use a non-application layer protocol for communication between host and C2 server or among infected hosts within a network. This could include using network layer protocols or other lower-level protocols that are not typically used for command and control.", TacticId = "TA0011" },
    new Technique { Id = "T1571", Name = "Non-Standard Port", Description = "Adversaries may communicate using a protocol and port paring that are typically not associated. For example, HTTPS over port 8088 or port 587 when traditional HTTPS traffic uses port 443.", TacticId = "TA0011" },

    // Exfiltration (TA0010)
    new Technique { Id = "T1020", Name = "Automated Exfiltration", Description = "Adversaries may use automated techniques to exfiltrate data from a victim network. This can involve using scripts or tools to automatically locate and transmit data of interest to the adversary.", TacticId = "TA0010" },
    new Technique { Id = "T1029", Name = "Scheduled Transfer", Description = "Adversaries may schedule data exfiltration to occur at certain times of day or at certain intervals. This can help blend in with normal network activity patterns and avoid detection.", TacticId = "TA0010" },
    new Technique { Id = "T1048", Name = "Exfiltration Over Alternative Protocol", Description = "Adversaries may steal data by exfiltrating it over a different protocol than that of the existing command and control channel. The data may also be sent to an alternate network location from the main command and control server.", TacticId = "TA0010" },
    new Technique { Id = "T1041", Name = "Exfiltration Over C2 Channel", Description = "Adversaries may steal data by exfiltrating it over an existing command and control channel. Stolen data is encoded into the normal communications channel using the same protocol as command and control communications.", TacticId = "TA0010" },
    new Technique { Id = "T1011", Name = "Exfiltration Over Other Network Medium", Description = "Adversaries may attempt to exfiltrate data over a different network medium than the command and control channel. If the command and control network is a wired Internet connection, the exfiltration may occur, for example, over a WiFi connection, modem, cellular data connection, Bluetooth, or another radio frequency (RF) channel.", TacticId = "TA0010" },
    new Technique { Id = "T1052", Name = "Exfiltration Over Physical Medium", Description = "Adversaries may attempt to exfiltrate data via a physical medium, such as a removable drive. In certain circumstances, such as an air-gapped network compromise, exfiltration could occur via a physical medium or device introduced by a user.", TacticId = "TA0010" },
    new Technique { Id = "T1567", Name = "Exfiltration Over Web Service", Description = "Adversaries may use an existing, legitimate external web service to exfiltrate data from a network. Popular websites and social media platforms can act as a mechanism for exfiltration to blend in with normal network traffic.", TacticId = "TA0010" },
    new Technique { Id = "T1022", Name = "Data Encrypted", Description = "Adversaries may encrypt data that is being exfiltrated to hide the information that is being stolen. This can be done using built-in encryption functions provided by the command and control protocol or known encryption algorithms.", TacticId = "TA0010" },
    new Technique { Id = "T1030", Name = "Data Transfer Size Limits", Description = "Adversaries may exfiltrate data in fixed size chunks instead of whole files or limit packet sizes below certain thresholds. This approach may be used to avoid triggering network data transfer threshold alerts.", TacticId = "TA0010" },
    // Impact (TA0040)
    new Technique { Id = "T1485", Name = "Data Destruction", Description = "Adversaries may destroy data and files on specific systems or in large numbers on a network to interrupt availability to systems, services, and network resources. Data destruction is likely to render stored data irrecoverable by forensic techniques.", TacticId = "TA0040" },
    new Technique { Id = "T1486", Name = "Data Encrypted for Impact", Description = "Adversaries may encrypt data on target systems or on large numbers of systems in a network to interrupt availability to system and network resources. This is often done as part of a ransomware attack.", TacticId = "TA0040" },
    new Technique { Id = "T1565", Name = "Data Manipulation", Description = "Adversaries may insert, delete, or manipulate data in order to manipulate external outcomes or hide activity. By manipulating data, adversaries may attempt to affect a business process, organizational understanding, or decision making.", TacticId = "TA0040" },
    new Technique { Id = "T1491", Name = "Defacement", Description = "Adversaries may modify visual content available internally or externally to an enterprise network. Reasons for defacement include delivering messaging, intimidation, or claiming (possibly false) credit for an intrusion.", TacticId = "TA0040" },
    new Technique { Id = "T1561", Name = "Disk Wipe", Description = "Adversaries may wipe or corrupt raw disk data on specific systems or in large numbers in a network to interrupt availability to system and network resources. This may be done in order to damage data, disrupt system operations, or wipe evidence of activity.", TacticId = "TA0040" },
    new Technique { Id = "T1499", Name = "Endpoint Denial of Service", Description = "Adversaries may perform Endpoint Denial of Service (DoS) attacks to degrade or block the availability of services to users. Endpoint DoS can be performed by exhausting the system resources those services are hosted on or exploiting the system to cause a persistent crash condition.", TacticId = "TA0040" },
    new Technique { Id = "T1495", Name = "Firmware Corruption", Description = "Adversaries may overwrite or corrupt the flash memory contents of system BIOS or other firmware in devices attached to a system in order to render them inoperable or unable to boot, thus denying the availability of the system or network resources.", TacticId = "TA0040" },
    new Technique { Id = "T1490", Name = "Inhibit System Recovery", Description = "Adversaries may delete or remove built-in operating system data and turn off services designed to aid in the recovery of a corrupted system to prevent recovery. This may deny access to available backups and recovery options.", TacticId = "TA0040" },
    new Technique { Id = "T1498", Name = "Network Denial of Service", Description = "Adversaries may perform Network Denial of Service (DoS) attacks to degrade or block the availability of targeted resources to users. Network DoS can be performed by exhausting the network bandwidth services rely on.", TacticId = "TA0040" },
    new Technique { Id = "T1496", Name = "Resource Hijacking", Description = "Adversaries may leverage the resources of co-opted systems in order to solve resource intensive problems which may impact system and/or hosted service availability. One common purpose for Resource Hijacking is to validate transactions of cryptocurrency networks and earn virtual currency.", TacticId = "TA0040" },
    new Technique { Id = "T1489", Name = "Service Stop", Description = "Adversaries may stop or disable services on a system to render those services unavailable to legitimate users. Stopping critical services can inhibit or stop response to an incident or aid in the adversary's overall objectives to cause damage to the environment.", TacticId = "TA0040" },
    new Technique { Id = "T1529", Name = "System Shutdown/Reboot", Description = "Adversaries may shutdown/reboot systems to interrupt access to, or aid in the destruction of, those systems. Operating systems may contain commands to initiate a shutdown/reboot of a machine. Shutting down or rebooting systems may disrupt access to computer resources for legitimate users.", TacticId = "TA0040" },
    new Technique { Id = "T1491", Name = "Defacement", Description = "Adversaries may modify visual content available internally or externally to an enterprise network. Reasons for defacement include delivering messaging, intimidation, or claiming (possibly false) credit for an intrusion.", TacticId = "TA0040" },
    new Technique { Id = "T1531", Name = "Account Access Removal", Description = "Adversaries may delete or disable user accounts to remove access to applications and systems. This technique can be used to deny access to accounts that may be used by legitimate users or administrators.", TacticId = "TA0040" },

    };


    private void OnSelectedTechniquesChanged(IEnumerable<string> values)
    {
        selectedTechniques = values;
        StateHasChanged();
    }

    public class Tactic
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Technique
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TacticId { get; set; }
    }  
}