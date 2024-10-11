using Microsoft.AspNetCore.Components;

namespace Cervantes.Web.Components.Shared;

public partial class MitreAttack : ComponentBase
{
    [Parameter] public List<string> TechniquesValues{ get; set; }
    [Parameter] public EventCallback<List<string>> TechniquesValuesChanged { get; set; }
    
    [Parameter] public List<string> MitreTechniques{ get; set; }
    [Parameter] public EventCallback<List<string>> MitreTechniquesChanged { get; set; }
    private List<string> SelectedTechniques
    {
        get => TechniquesValues;
        set
        {
            if (value != TechniquesValues)
            {
                TechniquesValuesChanged.InvokeAsync(value);        
            }
        }
    }
    
    private List<string> SelectedMitreTechniques
    {
        get => MitreTechniques;
        set
        {
            if (value != MitreTechniques)
            {
                MitreTechniquesChanged.InvokeAsync(value);        
            }
        }
    }
    private async Task OnMitreTechniquesChanged(List<string> value)
        => await this.MitreTechniquesChanged.InvokeAsync(value);
    private async Task OnTechniquesChanged(List<string> value)
        => await this.TechniquesValuesChanged.InvokeAsync(value);
    
    private List<string> selectedTechniques = new List<string>();
    protected override void OnParametersSet()
    {
        //selectedTechniques = new List<string>(TechniquesValues);
        base.OnParametersSet();
    }
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
    new Technique { Id = "T1595", Name = "Active Scanning", Description = "Adversaries may execute active reconnaissance scans to gather information that can be used during targeting.", TacticId = "TA0043" },
    new Technique { Id = "T1592", Name = "Gather Victim Host Information", Description = "Adversaries may gather information about the victim's hosts that can be used during targeting.", TacticId = "TA0043" },
    new Technique { Id = "T1589", Name = "Gather Victim Identity Information", Description = "Adversaries may gather information about the victim's identity that can be used during targeting.", TacticId = "TA0043" },
    new Technique { Id = "T1590", Name = "Gather Victim Network Information", Description = "Adversaries may gather information about the victim's networks that can be used during targeting.", TacticId = "TA0043" },
    new Technique { Id = "T1591", Name = "Gather Victim Org Information", Description = "Adversaries may gather information about the victim's organization that can be used during targeting.", TacticId = "TA0043" },
    new Technique { Id = "T1598", Name = "Phishing for Information", Description = "Adversaries may send phishing messages to elicit sensitive information that can be used during targeting.", TacticId = "TA0043" },
    new Technique { Id = "T1597", Name = "Search Closed Sources", Description = "Adversaries may search closed sources for information about victims that can be used during targeting.", TacticId = "TA0043" },
    new Technique { Id = "T1596", Name = "Search Open Technical Databases", Description = "Adversaries may search freely available technical databases for information about victims that can be used during targeting.", TacticId = "TA0043" },
    new Technique { Id = "T1593", Name = "Search Open Websites/Domains", Description = "Adversaries may search freely available websites and/or domains for information about victims that can be used during targeting.", TacticId = "TA0043" },
    new Technique { Id = "T1594", Name = "Search Victim-Owned Websites", Description = "Adversaries may search websites owned by the victim for information that can be used during targeting.", TacticId = "TA0043" },

    // Resource Development (TA0042)
    new Technique { Id = "T1583", Name = "Acquire Infrastructure", Description = "Adversaries may buy, lease, or rent infrastructure that can be used during targeting.", TacticId = "TA0042" },
    new Technique { Id = "T1586", Name = "Compromise Accounts", Description = "Adversaries may compromise accounts with services that can be used during targeting.", TacticId = "TA0042" },
    new Technique { Id = "T1584", Name = "Compromise Infrastructure", Description = "Adversaries may compromise infrastructure that can be used during targeting.", TacticId = "TA0042" },
    new Technique { Id = "T1587", Name = "Develop Capabilities", Description = "Adversaries may build capabilities that can be used during targeting.", TacticId = "TA0042" },
    new Technique { Id = "T1585", Name = "Establish Accounts", Description = "Adversaries may create and cultivate accounts with services that can be used during targeting.", TacticId = "TA0042" },
    new Technique { Id = "T1588", Name = "Obtain Capabilities", Description = "Adversaries may buy, steal, or download capabilities that can be used during targeting.", TacticId = "TA0042" },
    new Technique { Id = "T1608", Name = "Stage Capabilities", Description = "Adversaries may upload, install, or configure capabilities that can be used during targeting.", TacticId = "TA0042" },
    new Technique { Id = "T1711", Name = "Acquire Access", Description = "Adversaries may purchase or steal access to systems, services, or credentials that can be used during targeting.", TacticId = "TA0042" },


    // Initial Access (TA0001)
    new Technique { Id = "T1659", Name = "Content Injection", Description = "Adversaries may gain access and continuously communicate with victims by injecting malicious content into systems through online network traffic. Rather than luring victims to malicious payloads hosted on a compromised website (i.e., Drive-by Target followed by Drive-by Compromise), adversaries may initially access victims through compromised data-transfer channels where they can manipulate traffic and/or inject their own content. These compromised online network channels may also be used to deliver additional payloads (i.e., Ingress Tool Transfer) and other data to already compromised systems.", TacticId = "TA0001" },
    new Technique { Id = "T1189", Name = "Drive-by Compromise", Description = "Adversaries may gain access to a system through a user visiting a website over the normal course of browsing.", TacticId = "TA0001" },
    new Technique { Id = "T1190", Name = "Exploit Public-Facing Application", Description = "Adversaries may attempt to take advantage of a weakness in an Internet-facing computer or program using software, data, or commands in order to cause unintended or unanticipated behavior.", TacticId = "TA0001" },
    new Technique { Id = "T1133", Name = "External Remote Services", Description = "Adversaries may leverage external-facing remote services to initially access and/or persist within a network.", TacticId = "TA0001" },
    new Technique { Id = "T1200", Name = "Hardware Additions", Description = "Adversaries may introduce computer accessories, computers, or networking hardware into a system or network that can be used as a vector to gain access.", TacticId = "TA0001" },
    new Technique { Id = "T1566", Name = "Phishing", Description = "Adversaries may send phishing messages to gain access to victim systems. All forms of phishing are electronically delivered social engineering. Phishing can be targeted, known as spearphishing. In spearphishing, a specific individual, company, or industry will be targeted by the adversary. More generally, adversaries can conduct non-targeted phishing, such as in mass malware spam campaigns.", TacticId = "TA0001" },
    new Technique { Id = "T1091", Name = "Replication Through Removable Media", Description = "Adversaries may move onto systems, possibly those on disconnected or air-gapped networks, by copying malware to removable media and taking advantage of Autorun features when the media is inserted into a system and executes.", TacticId = "TA0001" },
    new Technique { Id = "T1195", Name = "Supply Chain Compromise", Description = "Adversaries may manipulate products or product delivery mechanisms prior to receipt by a final consumer for the purpose of data or system compromise.", TacticId = "TA0001" },
    new Technique { Id = "T1199", Name = "Trusted Relationship", Description = "Adversaries may breach or otherwise leverage organizations who have access to intended victims.", TacticId = "TA0001" },
    new Technique { Id = "T1078", Name = "Valid Accounts", Description = "Adversaries may obtain and abuse credentials of existing accounts as a means of gaining Initial Access, Persistence, Privilege Escalation, or Defense Evasion.", TacticId = "TA0001" },

    // Execution (TA0002)
     new Technique { Id = "T1651", Name = "Cloud Administration Command", Description = "Adversaries may abuse cloud administration tools to execute commands or obtain sensitive information.", TacticId = "TA0002" },
    new Technique { Id = "T1059", Name = "Command and Scripting Interpreter", Description = "Adversaries may abuse command and script interpreters to execute commands, scripts, or binaries.", TacticId = "TA0002" },
    new Technique { Id = "T1609", Name = "Container Administration Command", Description = "Adversaries may abuse container administration commands to execute malicious commands or obtain sensitive information.", TacticId = "TA0002" },
    new Technique { Id = "T1610", Name = "Deploy Container", Description = "Adversaries may deploy a container into an environment to facilitate execution or evade defenses.", TacticId = "TA0002" },
    new Technique { Id = "T1203", Name = "Exploitation for Client Execution", Description = "Adversaries may exploit software vulnerabilities in client applications to execute code.", TacticId = "TA0002" },
    new Technique { Id = "T1559", Name = "Inter-Process Communication", Description = "Adversaries may abuse inter-process communication (IPC) mechanisms for local code or command execution.", TacticId = "TA0002" },
    new Technique { Id = "T1106", Name = "Native API", Description = "Adversaries may interact with the native OS application programming interface (API) to execute behaviors.", TacticId = "TA0002" },
    new Technique { Id = "T1053", Name = "Scheduled Task/Job", Description = "Adversaries may abuse task scheduling functionality to facilitate initial or recurring execution of malicious code.", TacticId = "TA0002" },
    new Technique { Id = "T1648", Name = "Serverless Execution", Description = "Adversaries may abuse serverless computing to execute malicious code in cloud environments.", TacticId = "TA0002" },
    new Technique { Id = "T1129", Name = "Shared Modules", Description = "Adversaries may execute malicious payloads via loading shared modules.", TacticId = "TA0002" },
    new Technique { Id = "T1072", Name = "Software Deployment Tools", Description = "Adversaries may gain access to and use third-party software suites installed within an enterprise network to move laterally through the network and execute malicious payloads.", TacticId = "TA0002" },
    new Technique { Id = "T1569", Name = "System Services", Description = "Adversaries may abuse system services or daemons to execute commands or programs.", TacticId = "TA0002" },
    new Technique { Id = "T1204", Name = "User Execution", Description = "An adversary may rely upon specific actions by a user in order to gain execution.", TacticId = "TA0002" },
    new Technique { Id = "T1047", Name = "Windows Management Instrumentation", Description = "Adversaries may abuse Windows Management Instrumentation (WMI) to execute malicious commands and payloads.", TacticId = "TA0002" },

    // Persistence (TA0003)
new Technique { Id = "T1098", Name = "Account Manipulation", Description = "Adversaries may manipulate accounts to maintain access to victim systems.", TacticId = "TA0003" },
    new Technique { Id = "T1197", Name = "BITS Jobs", Description = "Adversaries may abuse BITS jobs to persist and conduct network communications.", TacticId = "TA0003" },
    new Technique { Id = "T1547", Name = "Boot or Logon Autostart Execution", Description = "Adversaries may configure system settings to automatically execute a program during system boot or user login.", TacticId = "TA0003" },
    new Technique { Id = "T1037", Name = "Boot or Logon Initialization Scripts", Description = "Adversaries may use scripts automatically executed at boot or logon initialization to establish persistence.", TacticId = "TA0003" },
    new Technique { Id = "T1176", Name = "Browser Extensions", Description = "Adversaries may abuse Internet browser extensions to establish persistent access to victim systems.", TacticId = "TA0003" },
    new Technique { Id = "T1554", Name = "Compromise Host Software Binary", Description = "Adversaries may modify host software binaries to establish persistent access to systems.", TacticId = "TA0003" },
    new Technique { Id = "T1136", Name = "Create Account", Description = "Adversaries may create an account to maintain access to victim systems.", TacticId = "TA0003" },
    new Technique { Id = "T1543", Name = "Create or Modify System Process", Description = "Adversaries may create or modify system processes to repeatedly execute malicious payloads as part of persistence.", TacticId = "TA0003" },
    new Technique { Id = "T1546", Name = "Event Triggered Execution", Description = "Adversaries may establish persistence using system mechanisms that trigger execution based on specific events.", TacticId = "TA0003" },
    new Technique { Id = "T1133", Name = "External Remote Services", Description = "Adversaries may leverage external-facing remote services to initially access and/or persist within a network.", TacticId = "TA0003" },
    new Technique { Id = "T1574", Name = "Hijack Execution Flow", Description = "Adversaries may execute their own malicious payloads by hijacking the way operating systems run programs.", TacticId = "TA0003" },
    new Technique { Id = "T1525", Name = "Implant Internal Image", Description = "Adversaries may implant cloud or container images with malicious code to establish persistence after gaining access to an environment.", TacticId = "TA0003" },
    new Technique { Id = "T1556", Name = "Modify Authentication Process", Description = "Adversaries may modify authentication mechanisms and processes to access user credentials or enable otherwise unwarranted access to accounts.", TacticId = "TA0003" },
    new Technique { Id = "T1137", Name = "Office Application Startup", Description = "Adversaries may leverage Microsoft Office-based applications for persistence between startups.", TacticId = "TA0003" },
    new Technique { Id = "T1542", Name = "Pre-OS Boot", Description = "Adversaries may abuse Pre-OS Boot mechanisms as a way to establish persistence on a system.", TacticId = "TA0003" },
    new Technique { Id = "T1053", Name = "Scheduled Task/Job", Description = "Adversaries may abuse task scheduling functionality to facilitate initial or recurring execution of malicious code.", TacticId = "TA0003" },
    new Technique { Id = "T1505", Name = "Server Software Component", Description = "Adversaries may abuse legitimate extensible development features of servers to establish persistent access to systems.", TacticId = "TA0003" },
    new Technique { Id = "T1205", Name = "Traffic Signaling", Description = "Adversaries may use traffic signaling to hide open ports or other malicious functionality used for persistence or command and control.", TacticId = "TA0003" },
    new Technique { Id = "T1078", Name = "Valid Accounts", Description = "Adversaries may obtain and abuse credentials of existing accounts as a means of gaining Initial Access, Persistence, Privilege Escalation, or Defense Evasion.", TacticId = "TA0003" },
    new Technique { Id = "T1647", Name = "Power Settings", Description = "Adversaries may modify power settings to prevent systems from sleeping or hibernating to maintain access.", TacticId = "TA0003" },

    // Privilege Escalation (TA0004)
    new Technique { Id = "T1548", Name = "Abuse Elevation Control Mechanism", Description = "Adversaries may bypass mechanisms designed to control elevated privileges to gain higher-level permissions.", TacticId = "TA0004" },
    new Technique { Id = "T1134", Name = "Access Token Manipulation", Description = "Adversaries may modify access tokens to operate under a different user or system security context to perform actions and evade detection.", TacticId = "TA0004" },
    new Technique { Id = "T1098", Name = "Account Manipulation", Description = "Adversaries may manipulate accounts to maintain access to victim systems.", TacticId = "TA0004" },
    new Technique { Id = "T1547", Name = "Boot or Logon Autostart Execution", Description = "Adversaries may configure system settings to automatically execute a program during system boot or user login.", TacticId = "TA0004" },
    new Technique { Id = "T1037", Name = "Boot or Logon Initialization Scripts", Description = "Adversaries may use scripts automatically executed at boot or logon initialization to establish persistence.", TacticId = "TA0004" },
    new Technique { Id = "T1543", Name = "Create or Modify System Process", Description = "Adversaries may create or modify system processes to repeatedly execute malicious payloads as part of persistence.", TacticId = "TA0004" },
    new Technique { Id = "T1484", Name = "Domain or Tenant Policy Modification", Description = "Adversaries may modify the configuration settings of a domain or tenant to evade defenses and/or escalate privileges.", TacticId = "TA0004" },
    new Technique { Id = "T1611", Name = "Escape to Host", Description = "Adversaries may escape from a container to gain access to the underlying host.", TacticId = "TA0004" },
    new Technique { Id = "T1546", Name = "Event Triggered Execution", Description = "Adversaries may establish persistence using system mechanisms that trigger execution based on specific events.", TacticId = "TA0004" },
    new Technique { Id = "T1068", Name = "Exploitation for Privilege Escalation", Description = "Adversaries may exploit software vulnerabilities in an attempt to elevate privileges.", TacticId = "TA0004" },
    new Technique { Id = "T1574", Name = "Hijack Execution Flow", Description = "Adversaries may execute their own malicious payloads by hijacking the way operating systems run programs.", TacticId = "TA0004" },
    new Technique { Id = "T1055", Name = "Process Injection", Description = "Adversaries may inject code into processes in order to evade process-based defenses as well as possibly elevate privileges.", TacticId = "TA0004" },
    new Technique { Id = "T1053", Name = "Scheduled Task/Job", Description = "Adversaries may abuse task scheduling functionality to facilitate initial or recurring execution of malicious code.", TacticId = "TA0004" },
    new Technique { Id = "T1078", Name = "Valid Accounts", Description = "Adversaries may obtain and abuse credentials of existing accounts as a means of gaining Initial Access, Persistence, Privilege Escalation, or Defense Evasion.", TacticId = "TA0004" },

    // Defense Evasion (TA0005)
       new Technique { Id = "T1548", Name = "Abuse Elevation Control Mechanism", Description = "Adversaries may bypass mechanisms designed to control elevated privileges to gain higher-level permissions.", TacticId = "TA0005" },
    new Technique { Id = "T1134", Name = "Access Token Manipulation", Description = "Adversaries may modify access tokens to operate under a different user or system security context to perform actions and evade detection.", TacticId = "TA0005" },
    new Technique { Id = "T1197", Name = "BITS Jobs", Description = "Adversaries may abuse BITS jobs to avoid detection and circumvent defenses.", TacticId = "TA0005" },
    new Technique { Id = "T1656", Name = "Build Image on Host", Description = "Adversaries may build a container image directly on a host to bypass defenses that monitor for the retrieval of malicious images from a public registry.", TacticId = "TA0005" },
    new Technique { Id = "T1622", Name = "Debugger Evasion", Description = "Adversaries may employ various means to detect and avoid debuggers.", TacticId = "TA0005" },
    new Technique { Id = "T1140", Name = "Deobfuscate/Decode Files or Information", Description = "Adversaries may use obfuscated files or information to hide artifacts of an intrusion from analysis.", TacticId = "TA0005" },
    new Technique { Id = "T1610", Name = "Deploy Container", Description = "Adversaries may deploy a container into an environment to facilitate execution or evade defenses.", TacticId = "TA0005" },
    new Technique { Id = "T1006", Name = "Direct Volume Access", Description = "Adversaries may directly access a volume to bypass file access controls and file system monitoring.", TacticId = "TA0005" },
    new Technique { Id = "T1484", Name = "Domain or Tenant Policy Modification", Description = "Adversaries may modify domain or tenant policies to evade defenses and/or escalate privileges.", TacticId = "TA0005" },
    new Technique { Id = "T1480", Name = "Execution Guardrails", Description = "Adversaries may use execution guardrails to constrain execution or actions based on environmental conditions.", TacticId = "TA0005" },
    new Technique { Id = "T1211", Name = "Exploitation for Defense Evasion", Description = "Adversaries may exploit a system or application vulnerability to bypass security features.", TacticId = "TA0005" },
    new Technique { Id = "T1222", Name = "File and Directory Permissions Modification", Description = "Adversaries may modify file or directory permissions/attributes to evade access control lists (ACLs) and access protected files.", TacticId = "TA0005" },
    new Technique { Id = "T1564", Name = "Hide Artifacts", Description = "Adversaries may attempt to hide artifacts associated with their behaviors to evade detection.", TacticId = "TA0005" },
    new Technique { Id = "T1574", Name = "Hijack Execution Flow", Description = "Adversaries may execute their own malicious payloads by hijacking the way operating systems run programs.", TacticId = "TA0005" },
    new Technique { Id = "T1562", Name = "Impair Defenses", Description = "Adversaries may maliciously modify components of a victim environment in order to hinder or disable defensive mechanisms.", TacticId = "TA0005" },
    new Technique { Id = "T1070", Name = "Indicator Removal", Description = "Adversaries may delete or modify artifacts generated on a host system to remove evidence of their presence or hinder defenses.", TacticId = "TA0005" },
    new Technique { Id = "T1202", Name = "Indirect Command Execution", Description = "Adversaries may abuse utilities that allow for command execution to bypass security restrictions that limit the use of command-line interpreters.", TacticId = "TA0005" },
    new Technique { Id = "T1036", Name = "Masquerading", Description = "Adversaries may attempt to manipulate features of their artifacts to make them appear legitimate or benign to users and/or security tools.", TacticId = "TA0005" },
    new Technique { Id = "T1556", Name = "Modify Authentication Process", Description = "Adversaries may modify authentication mechanisms and processes to access user credentials or enable otherwise unwarranted access to accounts.", TacticId = "TA0005" },
    new Technique { Id = "T1578", Name = "Modify Cloud Compute Infrastructure", Description = "Adversaries may modify cloud compute infrastructure to evade defenses and/or escalate privileges.", TacticId = "TA0005" },
    new Technique { Id = "T1112", Name = "Modify Registry", Description = "Adversaries may interact with the Windows Registry to hide configuration information within Registry keys, remove information as part of cleaning up, or as part of other techniques to aid in persistence and execution.", TacticId = "TA0005" },
    new Technique { Id = "T1601", Name = "Modify System Image", Description = "Adversaries may make changes to the operating system of embedded network devices to weaken defenses and provide new capabilities for themselves.", TacticId = "TA0005" },
    new Technique { Id = "T1599", Name = "Network Boundary Bridging", Description = "Adversaries may bridge network boundaries by compromising perimeter network devices.", TacticId = "TA0005" },
    new Technique { Id = "T1027", Name = "Obfuscated Files or Information", Description = "Adversaries may attempt to make an executable or file difficult to discover or analyze by encrypting, encoding, or otherwise obfuscating its contents on the system or in transit.", TacticId = "TA0005" },
    new Technique { Id = "T1647", Name = "Plist File Modification", Description = "Adversaries may modify plist files to hide or manipulate malicious activity.", TacticId = "TA0005" },
    new Technique { Id = "T1542", Name = "Pre-OS Boot", Description = "Adversaries may abuse Pre-OS Boot mechanisms as a way to establish persistence on a system.", TacticId = "TA0005" },
    new Technique { Id = "T1055", Name = "Process Injection", Description = "Adversaries may inject code into processes in order to evade process-based defenses as well as possibly elevate privileges.", TacticId = "TA0005" },
    new Technique { Id = "T1620", Name = "Reflective Code Loading", Description = "Adversaries may reflectively load code into a process in order to conceal the execution of malicious payloads.", TacticId = "TA0005" },
    new Technique { Id = "T1207", Name = "Rogue Domain Controller", Description = "Adversaries may register a rogue Domain Controller to enable manipulation of Active Directory data.", TacticId = "TA0005" },
    new Technique { Id = "T1014", Name = "Rootkit", Description = "Adversaries may use rootkits to hide the presence of programs, files, network connections, services, drivers, and other system components.", TacticId = "TA0005" },
    new Technique { Id = "T1553", Name = "Subvert Trust Controls", Description = "Adversaries may undermine security controls that will either warn users of untrusted activity or prevent execution of untrusted programs.", TacticId = "TA0005" },
    new Technique { Id = "T1218", Name = "System Binary Proxy Execution", Description = "Adversaries may bypass process and/or signature-based defenses by proxying execution of malicious content with signed binaries.", TacticId = "TA0005" },
    new Technique { Id = "T1216", Name = "System Script Proxy Execution", Description = "Adversaries may use scripts signed with trusted certificates to proxy execution of malicious files.", TacticId = "TA0005" },
    new Technique { Id = "T1221", Name = "Template Injection", Description = "Adversaries may create or modify references in Office document templates to conceal malicious code or force authentication attempts.", TacticId = "TA0005" },
    new Technique { Id = "T1205", Name = "Traffic Signaling", Description = "Adversaries may use traffic signaling to hide open ports or other malicious functionality used for persistence or command and control.", TacticId = "TA0005" },
    new Technique { Id = "T1127", Name = "Trusted Developer Utilities Proxy Execution", Description = "Adversaries may take advantage of trusted developer utilities to proxy execution of malicious payloads.", TacticId = "TA0005" },
    new Technique { Id = "T1535", Name = "Unused/Unsupported Cloud Regions", Description = "Adversaries may create cloud instances in unused geographic service regions to evade detection.", TacticId = "TA0005" },
    new Technique { Id = "T1550", Name = "Use Alternate Authentication Material", Description = "Adversaries may use alternate authentication material, such as password hashes, Kerberos tickets, and application access tokens, in order to move laterally within an environment and bypass normal system access controls.", TacticId = "TA0005" },
    new Technique { Id = "T1078", Name = "Valid Accounts", Description = "Adversaries may obtain and abuse credentials of existing accounts as a means of gaining Initial Access, Persistence, Privilege Escalation, or Defense Evasion.", TacticId = "TA0005" },
    new Technique { Id = "T1497", Name = "Virtualization/Sandbox Evasion", Description = "Adversaries may employ various means to detect and avoid virtualization and analysis environments.", TacticId = "TA0005" },
    new Technique { Id = "T1600", Name = "Weaken Encryption", Description = "Adversaries may compromise a network device's encryption capability in order to bypass encryption that would otherwise protect data communications.", TacticId = "TA0005" },
    new Technique { Id = "T1220", Name = "XSL Script Processing", Description = "Adversaries may bypass application control and obscure execution of code by embedding scripts inside XSL files.", TacticId = "TA0005" },
    
    // Discovery (TA0007)
    new Technique { Id = "T1087", Name = "Account Discovery", Description = "Adversaries may attempt to get a listing of accounts on a system or within an environment.", TacticId = "TA0007" },
    new Technique { Id = "T1010", Name = "Application Window Discovery", Description = "Adversaries may attempt to get information about open windows on a system.", TacticId = "TA0007" },
    new Technique { Id = "T1217", Name = "Browser Information Discovery", Description = "Adversaries may enumerate information about the browser to inform follow-on behaviors.", TacticId = "TA0007" },
    new Technique { Id = "T1580", Name = "Cloud Infrastructure Discovery", Description = "Adversaries may attempt to discover infrastructure and resources that are available within an infrastructure-as-a-service (IaaS) environment.", TacticId = "TA0007" },
    new Technique { Id = "T1538", Name = "Cloud Service Dashboard", Description = "Adversaries may use a cloud service dashboard GUI with stolen credentials to gain useful information from an operational cloud environment.", TacticId = "TA0007" },
    new Technique { Id = "T1526", Name = "Cloud Service Discovery", Description = "Adversaries may attempt to enumerate the cloud services running on a system after gaining access.", TacticId = "TA0007" },
    new Technique { Id = "T1613", Name = "Container and Resource Discovery", Description = "Adversaries may attempt to discover containers and other resources that are available within a containers environment.", TacticId = "TA0007" },
    new Technique { Id = "T1482", Name = "Domain Trust Discovery", Description = "Adversaries may attempt to gather information on domain trust relationships that may be used to identify lateral movement opportunities in Windows multi-domain/forest environments.", TacticId = "TA0007" },
    new Technique { Id = "T1083", Name = "File and Directory Discovery", Description = "Adversaries may enumerate files and directories or may search in specific locations of a host or network share for certain information within a file system.", TacticId = "TA0007" },
    new Technique { Id = "T1046", Name = "Network Service Discovery", Description = "Adversaries may attempt to get a listing of services running on remote hosts, including those that may be vulnerable to remote software exploitation.", TacticId = "TA0007" },
    new Technique { Id = "T1135", Name = "Network Share Discovery", Description = "Adversaries may look for folders and drives shared on remote systems as a means of identifying sources of information to gather as a precursor for Collection and to identify potential systems of interest for Lateral Movement.", TacticId = "TA0007" },
    new Technique { Id = "T1040", Name = "Network Sniffing", Description = "Adversaries may sniff network traffic to capture information about an environment, including authentication material passed over the network.", TacticId = "TA0007" },
    new Technique { Id = "T1201", Name = "Password Policy Discovery", Description = "Adversaries may attempt to access detailed information about the password policy used within an enterprise network.", TacticId = "TA0007" },
    new Technique { Id = "T1120", Name = "Peripheral Device Discovery", Description = "Adversaries may attempt to gather information about attached peripheral devices and components connected to a computer system.", TacticId = "TA0007" },
    new Technique { Id = "T1069", Name = "Permission Groups Discovery", Description = "Adversaries may attempt to find group and permission settings.", TacticId = "TA0007" },
    new Technique { Id = "T1057", Name = "Process Discovery", Description = "Adversaries may attempt to get information about running processes on a system.", TacticId = "TA0007" },
    new Technique { Id = "T1012", Name = "Query Registry", Description = "Adversaries may interact with the Windows Registry to gather information about the system, configuration, and installed software.", TacticId = "TA0007" },
    new Technique { Id = "T1018", Name = "Remote System Discovery", Description = "Adversaries may attempt to get a listing of other systems by IP address, hostname, or other logical identifier on a network that may be used for Lateral Movement from the current system.", TacticId = "TA0007" },
    new Technique { Id = "T1518", Name = "Software Discovery", Description = "Adversaries may attempt to get a listing of software and software versions that are installed on a system or in a cloud environment.", TacticId = "TA0007" },
    new Technique { Id = "T1082", Name = "System Information Discovery", Description = "An adversary may attempt to get detailed information about the operating system and hardware, including version, patches, hotfixes, service packs, and architecture.", TacticId = "TA0007" },
    new Technique { Id = "T1016", Name = "System Network Configuration Discovery", Description = "Adversaries may look for details about the network configuration and settings of systems they access or through information discovery of remote systems.", TacticId = "TA0007" },
    new Technique { Id = "T1049", Name = "System Network Connections Discovery", Description = "Adversaries may attempt to get a listing of network connections to or from the compromised system they are currently accessing or from remote systems by querying for information over the network.", TacticId = "TA0007" },
    new Technique { Id = "T1033", Name = "System Owner/User Discovery", Description = "Adversaries may attempt to identify the primary user, currently logged in user, set of users that commonly uses a system, or whether a user is actively using the system.", TacticId = "TA0007" },
    new Technique { Id = "T1007", Name = "System Service Discovery", Description = "Adversaries may try to get information about registered services.", TacticId = "TA0007" },
    new Technique { Id = "T1124", Name = "System Time Discovery", Description = "An adversary may gather the system time and/or time zone from a local or remote system.", TacticId = "TA0007" },
    new Technique { Id = "T1497", Name = "Virtualization/Sandbox Evasion", Description = "Adversaries may employ various means to detect and avoid virtualization and analysis environments.", TacticId = "TA0007" },
    new Technique { Id = "T1619", Name = "Cloud Storage Object Discovery", Description = "Adversaries may enumerate objects in cloud storage infrastructure to better understand the cloud environment and the data that is present.", TacticId = "TA0007" },
    new Technique { Id = "T1622", Name = "Debugger Evasion", Description = "Adversaries may employ various means to detect and avoid debuggers.", TacticId = "TA0007" },
    new Technique { Id = "T1614", Name = "System Location Discovery", Description = "Adversaries may gather information in an attempt to calculate the geographical location of a victim host.", TacticId = "TA0007" },
    new Technique { Id = "T1592", Name = "Gather Victim Host Information", Description = "Adversaries may gather information about the victim's hosts that can be used during targeting.", TacticId = "TA0007" },
    new Technique { Id = "T1652", Name = "Device Driver Discovery", Description = "Adversaries may attempt to identify device drivers installed on a system.", TacticId = "TA0007" },
    new Technique { Id = "T1653", Name = "Log Enumeration", Description = "Adversaries may enumerate log files to better understand the environment and identify potential weaknesses.", TacticId = "TA0007" },

    // Lateral Movement (TA0008)
    new Technique { Id = "T1210", Name = "Exploitation of Remote Services", Description = "Adversaries may exploit remote services to gain unauthorized access to internal systems.", TacticId = "TA0008" },
    new Technique { Id = "T1534", Name = "Internal Spearphishing", Description = "Adversaries may use internal spearphishing to gain access to additional information or exploit other users within the same organization.", TacticId = "TA0008" },
    new Technique { Id = "T1570", Name = "Lateral Tool Transfer", Description = "Adversaries may transfer tools or other files between systems in a compromised environment.", TacticId = "TA0008" },
    new Technique { Id = "T1563", Name = "Remote Service Session Hijacking", Description = "Adversaries may hijack legitimate remote sessions to move laterally within a network.", TacticId = "TA0008" },
    new Technique { Id = "T1021", Name = "Remote Services", Description = "Adversaries may use legitimate remote services to move laterally within a network.", TacticId = "TA0008" },
    new Technique { Id = "T1091", Name = "Replication Through Removable Media", Description = "Adversaries may move onto systems by copying malware to removable media and taking advantage of autorun features when the media is inserted into a system.", TacticId = "TA0008" },
    new Technique { Id = "T1072", Name = "Software Deployment Tools", Description = "Adversaries may use legitimate software deployment tools to move laterally within a network.", TacticId = "TA0008" },
    new Technique { Id = "T1080", Name = "Taint Shared Content", Description = "Adversaries may taint shared content to move laterally within a network.", TacticId = "TA0008" },
    new Technique { Id = "T1550", Name = "Use Alternate Authentication Material", Description = "Adversaries may use alternate authentication material, such as password hashes or Kerberos tickets, to move laterally within a network without needing to know the plaintext password.", TacticId = "TA0008" },

    
    // Collection (TA0008)
   new Technique { Id = "T1557", Name = "Adversary-in-the-Middle", Description = "Adversaries may attempt to position themselves between two or more networked devices using an adversary-in-the-middle (AiTM) technique to support follow-on behaviors such as Network Sniffing or Transmitted Data Manipulation.", TacticId = "TA0009" },
    new Technique { Id = "T1560", Name = "Archive Collected Data", Description = "Adversaries may compress and/or encrypt data that is collected prior to exfiltration.", TacticId = "TA0009" },
    new Technique { Id = "T1123", Name = "Audio Capture", Description = "Adversaries may capture audio recordings from compromised systems to gather information.", TacticId = "TA0009" },
    new Technique { Id = "T1119", Name = "Automated Collection", Description = "Adversaries may use automated techniques to collect data from a system.", TacticId = "TA0009" },
    new Technique { Id = "T1115", Name = "Clipboard Data", Description = "Adversaries may collect data stored in the clipboard from users copying information.", TacticId = "TA0009" },
    new Technique { Id = "T1530", Name = "Data from Cloud Storage Object", Description = "Adversaries may access data objects from cloud storage services to collect sensitive information.", TacticId = "TA0009" },
    new Technique { Id = "T1602", Name = "Data from Configuration Repository", Description = "Adversaries may collect data from configuration repositories, which can contain sensitive information about the network and its systems.", TacticId = "TA0009" },
    new Technique { Id = "T1213", Name = "Data from Information Repositories", Description = "Adversaries may collect data from information repositories within a victim's network.", TacticId = "TA0009" },
    new Technique { Id = "T1005", Name = "Data from Local System", Description = "Adversaries may collect data stored on local systems, including files, documents, and other sensitive information.", TacticId = "TA0009" },
    new Technique { Id = "T1039", Name = "Data from Network Shared Drive", Description = "Adversaries may collect data from shared network drives that are accessible from the compromised system.", TacticId = "TA0009" },
    new Technique { Id = "T1025", Name = "Data from Removable Media", Description = "Adversaries may collect data from removable media connected to compromised systems.", TacticId = "TA0009" },
    new Technique { Id = "T1074", Name = "Data Staged", Description = "Adversaries may stage collected data in a central location or directory prior to exfiltration.", TacticId = "TA0009" },
    new Technique { Id = "T1114", Name = "Email Collection", Description = "Adversaries may collect email data from compromised user accounts or email servers.", TacticId = "TA0009" },
    new Technique { Id = "T1056", Name = "Input Capture", Description = "Adversaries may use various methods to capture user input on a system.", TacticId = "TA0009" },
    new Technique { Id = "T1185", Name = "Man in the Browser", Description = "Adversaries may use malware to intercept and collect data from web browsers.", TacticId = "TA0009" },
    new Technique { Id = "T1113", Name = "Screen Capture", Description = "Adversaries may capture screenshots of the compromised system to gather sensitive information.", TacticId = "TA0009" },
    new Technique { Id = "T1125", Name = "Video Capture", Description = "Adversaries may capture video recordings from compromised systems.", TacticId = "TA0009" },
    
    
    // Command and Control (TA0011)
    new Technique { Id = "T1071", Name = "Application Layer Protocol", Description = "Adversaries may communicate using application layer protocols to avoid detection/network filtering by blending in with existing traffic.", TacticId = "TA0011" },
    new Technique { Id = "T1092", Name = "Communication Through Removable Media", Description = "Adversaries may use removable media to communicate with systems that are not connected to networks.", TacticId = "TA0011" },
    new Technique { Id = "T1132", Name = "Data Encoding", Description = "Adversaries may encode data to make the content of command and control traffic more difficult to detect.", TacticId = "TA0011" },
    new Technique { Id = "T1001", Name = "Data Obfuscation", Description = "Adversaries may obfuscate command and control traffic to make it more difficult to detect.", TacticId = "TA0011" },
    new Technique { Id = "T1568", Name = "Dynamic Resolution", Description = "Adversaries may dynamically establish connections to command and control infrastructure to evade common detections and remediations.", TacticId = "TA0011" },
    new Technique { Id = "T1573", Name = "Encrypted Channel", Description = "Adversaries may use encrypted channels to hide command and control traffic from detection.", TacticId = "TA0011" },
    new Technique { Id = "T1008", Name = "Fallback Channels", Description = "Adversaries may use fallback or alternate communication channels if the primary channel is compromised or inaccessible.", TacticId = "TA0011" },
    new Technique { Id = "T1105", Name = "Ingress Tool Transfer", Description = "Adversaries may transfer tools or other files from an external system into a compromised environment.", TacticId = "TA0011" },
    new Technique { Id = "T1104", Name = "Multi-Stage Channels", Description = "Adversaries may create multiple stages for command and control that are employed under different conditions or for certain functions.", TacticId = "TA0011" },
    new Technique { Id = "T1095", Name = "Non-Application Layer Protocol", Description = "Adversaries may use a non-application layer protocol for communication between host and C2 server or among infected hosts within a network.", TacticId = "TA0011" },
    new Technique { Id = "T1571", Name = "Non-Standard Port", Description = "Adversaries may communicate using a protocol and port paring that are typically not associated.", TacticId = "TA0011" },
    new Technique { Id = "T1572", Name = "Protocol Tunneling", Description = "Adversaries may tunnel network communications to and from a victim system within a separate protocol to avoid detection/network filtering and/or to reduce data attribution.", TacticId = "TA0011" },
    new Technique { Id = "T1090", Name = "Proxy", Description = "Adversaries may use a connection proxy to direct network traffic between systems or act as an intermediary for network communications.", TacticId = "TA0011" },
    new Technique { Id = "T1219", Name = "Remote Access Software", Description = "Adversaries may use legitimate desktop support and remote access software to establish an interactive command and control channel to target systems within networks.", TacticId = "TA0011" },
    new Technique { Id = "T1205", Name = "Traffic Signaling", Description = "Adversaries may use specific traffic patterns or signals to hide open ports or other malicious functionality used for persistence or command and control.", TacticId = "TA0011" },
    new Technique { Id = "T1102", Name = "Web Service", Description = "Adversaries may use an existing, legitimate external Web service as a means for relaying data to/from a compromised system.", TacticId = "TA0011" },
    new Technique { Id = "T1499", Name = "Endpoint Denial of Service", Description = "Adversaries may perform Endpoint Denial of Service (DoS) attacks to degrade or block the availability of services to users.", TacticId = "TA0011" },
    new Technique { Id = "T1657", Name = "Hide Infrastructure", Description = "Adversaries may hide their command and control infrastructure to make it harder for defenders to detect and mitigate malicious activity.", TacticId = "TA0011" },

    // Exfiltration (TA0010)
    new Technique { Id = "T1020", Name = "Automated Exfiltration", Description = "Adversaries may use automated techniques to exfiltrate data from a victim network.", TacticId = "TA0010" },
    new Technique { Id = "T1030", Name = "Data Transfer Size Limits", Description = "Adversaries may exfiltrate data in fixed size chunks instead of whole files or limit packet sizes below certain thresholds.", TacticId = "TA0010" },
    new Technique { Id = "T1048", Name = "Exfiltration Over Alternative Protocol", Description = "Adversaries may steal data by exfiltrating it over a different protocol than that of the existing command and control channel.", TacticId = "TA0010" },
    new Technique { Id = "T1041", Name = "Exfiltration Over C2 Channel", Description = "Adversaries may steal data by exfiltrating it over an existing command and control channel.", TacticId = "TA0010" },
    new Technique { Id = "T1011", Name = "Exfiltration Over Other Network Medium", Description = "Adversaries may attempt to exfiltrate data over a different network medium than the command and control channel.", TacticId = "TA0010" },
    new Technique { Id = "T1052", Name = "Exfiltration Over Physical Medium", Description = "Adversaries may attempt to exfiltrate data via a physical medium, such as a removable drive.", TacticId = "TA0010" },
    new Technique { Id = "T1567", Name = "Exfiltration Over Web Service", Description = "Adversaries may use an existing, legitimate external web service to exfiltrate data from a network.", TacticId = "TA0010" },
    new Technique { Id = "T1029", Name = "Scheduled Transfer", Description = "Adversaries may schedule data exfiltration to occur at certain times of day or at certain intervals.", TacticId = "TA0010" },
    new Technique { Id = "T1537", Name = "Transfer Data to Cloud Account", Description = "Adversaries may exfiltrate data by transferring it to a cloud storage account they control.", TacticId = "TA0010" },

    // Impact (TA0040)
    new Technique { Id = "T1531", Name = "Account Access Removal", Description = "Adversaries may interrupt availability of system and network resources by inhibiting access to accounts utilized by legitimate users.", TacticId = "TA0040" },
    new Technique { Id = "T1485", Name = "Data Destruction", Description = "Adversaries may destroy data and files on specific systems or in large numbers on a network to interrupt availability to systems, services, and network resources.", TacticId = "TA0040" },
    new Technique { Id = "T1486", Name = "Data Encrypted for Impact", Description = "Adversaries may encrypt data on target systems or on large numbers of systems in a network to interrupt availability to system and network resources.", TacticId = "TA0040" },
    new Technique { Id = "T1565", Name = "Data Manipulation", Description = "Adversaries may insert, delete, or manipulate data in order to manipulate external outcomes or hide activity.", TacticId = "TA0040" },
    new Technique { Id = "T1491", Name = "Defacement", Description = "Adversaries may modify visual content available internally or externally to an enterprise network.", TacticId = "TA0040" },
    new Technique { Id = "T1561", Name = "Disk Wipe", Description = "Adversaries may wipe or corrupt raw disk data on specific systems or in large numbers in a network to interrupt availability to system and network resources.", TacticId = "TA0040" },
    new Technique { Id = "T1499", Name = "Endpoint Denial of Service", Description = "Adversaries may perform Endpoint Denial of Service (DoS) attacks to degrade or block the availability of services to users.", TacticId = "TA0040" },
    new Technique { Id = "T1495", Name = "Firmware Corruption", Description = "Adversaries may overwrite or corrupt the flash memory contents of system BIOS or other firmware in devices attached to a system in order to render them inoperable or unable to boot.", TacticId = "TA0040" },
    new Technique { Id = "T1490", Name = "Inhibit System Recovery", Description = "Adversaries may delete or remove built-in operating system data and turn off services designed to aid in the recovery of a corrupted system to prevent recovery.", TacticId = "TA0040" },
    new Technique { Id = "T1498", Name = "Network Denial of Service", Description = "Adversaries may perform Network Denial of Service (DoS) attacks to degrade or block the availability of targeted resources to users.", TacticId = "TA0040" },
    new Technique { Id = "T1496", Name = "Resource Hijacking", Description = "Adversaries may leverage the resources of co-opted systems in order to solve resource intensive problems which may impact system and/or hosted service availability.", TacticId = "TA0040" },
    new Technique { Id = "T1489", Name = "Service Stop", Description = "Adversaries may stop or disable services on a system to render those services unavailable to legitimate users.", TacticId = "TA0040" },
    new Technique { Id = "T1529", Name = "System Shutdown/Reboot", Description = "Adversaries may shutdown/reboot systems to interrupt access to, or aid in the destruction of, those systems.", TacticId = "TA0040" },
    new Technique { Id = "T1654", Name = "Financial Theft", Description = "Adversaries may attempt to steal financial resources from an organization or its customers.", TacticId = "TA0040" },
    };


    private async Task OnSelectedTechniquesChanged(List<string> values)
    {
        selectedTechniques = new List<string>(values);
        await TechniquesValuesChanged.InvokeAsync(selectedTechniques);
        StateHasChanged();
    }
    
    private IEnumerable<Technique> GetSelectedTechniques()
    {
        return techniques.Where(t => selectedTechniques.Contains(t.Id));
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