using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.Masscan;

public class MasscanParser : IMasscanParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public MasscanParser(ITargetManager targetManager, IVulnManager vulnManager,
        IVulnTargetManager vulnTargetManager, IProjectManager projectManager)
    {
        this.targetManager = targetManager;
        this.vulnManager = vulnManager;
        this.vulnTargetManager = vulnTargetManager;
        this.projectManager = projectManager;
    }

    public void Parse(Guid? project, string user, string path)
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedSchemes.Add("data");

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            var fileContent = System.IO.File.ReadAllText(path);
            
            // Detectar formato (XML, JSON o texto plano)
            if (fileContent.TrimStart().StartsWith("{") || fileContent.TrimStart().StartsWith("["))
            {
                ParseJsonFormat(fileContent, pro, user, sanitizer);
            }
            else if (fileContent.TrimStart().StartsWith("<"))
            {
                ParseXmlFormat(path, pro, user, sanitizer);
            }
            else
            {
                // Formato de texto plano de Masscan
                ParseTextFormat(fileContent, pro, user, sanitizer);
            }
        }
    }

    private void ParseJsonFormat(string fileContent, Project pro, string user, HtmlSanitizer sanitizer)
    {
        try
        {
            // Masscan JSON puede ser un array de hosts
            if (fileContent.TrimStart().StartsWith("["))
            {
                var masscanResults = JsonSerializer.Deserialize<MasscanJsonResult[]>(fileContent);
                foreach (var result in masscanResults ?? new MasscanJsonResult[0])
                {
                    ProcessMasscanResult(result, pro, user, sanitizer);
                }
            }
            else
            {
                var masscanResult = JsonSerializer.Deserialize<MasscanJsonResult>(fileContent);
                if (masscanResult != null)
                {
                    ProcessMasscanResult(masscanResult, pro, user, sanitizer);
                }
            }
        }
        catch (JsonException)
        {
            // Si falla JSON, intentar como texto plano
            ParseTextFormat(fileContent, pro, user, sanitizer);
        }
    }

    private void ParseXmlFormat(string path, Project pro, string user, HtmlSanitizer sanitizer)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList hosts = doc.GetElementsByTagName("host");
        
        foreach (XmlNode host in hosts)
        {
            string ip = "";
            var addressNode = host.SelectSingleNode("address[@addrtype='ipv4']");
            if (addressNode != null)
            {
                ip = addressNode.Attributes?["addr"]?.Value ?? "";
            }

            XmlNodeList ports = host.SelectNodes(".//port");
            foreach (XmlNode port in ports)
            {
                string portId = port.Attributes?["portid"]?.Value ?? "";
                string protocol = port.Attributes?["protocol"]?.Value ?? "tcp";
                
                var stateNode = port.SelectSingleNode("state");
                string state = stateNode?.Attributes?["state"]?.Value ?? "";

                if (state == "open" && !string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(portId))
                {
                    ProcessOpenPort(ip, portId, protocol, state, "", pro, user, sanitizer);
                }
            }
        }
    }

    private void ParseTextFormat(string fileContent, Project pro, string user, HtmlSanitizer sanitizer)
    {
        var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            // Formato típico de Masscan: "open tcp 80 192.168.1.1"
            // o "Discovered open port 80/tcp on 192.168.1.1"
            if (trimmedLine.Contains("open") && (trimmedLine.Contains("tcp") || trimmedLine.Contains("udp")))
            {
                ParseMasscanTextLine(trimmedLine, pro, user, sanitizer);
            }
        }
    }

    private void ParseMasscanTextLine(string line, Project pro, string user, HtmlSanitizer sanitizer)
    {
        // Parsear diferentes formatos de línea de Masscan
        string ip = "";
        string port = "";
        string protocol = "tcp";
        string state = "open";

        // Formato: "open tcp 80 192.168.1.1"
        if (line.StartsWith("open"))
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 4)
            {
                protocol = parts[1];
                port = parts[2];
                ip = parts[3];
            }
        }
        // Formato: "Discovered open port 80/tcp on 192.168.1.1"
        else if (line.Contains("Discovered open port") && line.Contains(" on "))
        {
            var onIndex = line.LastIndexOf(" on ");
            if (onIndex > 0)
            {
                ip = line.Substring(onIndex + 4).Trim();
                
                var portMatch = line.Substring(0, onIndex);
                var portIndex = portMatch.LastIndexOf("port ");
                if (portIndex >= 0)
                {
                    var portInfo = portMatch.Substring(portIndex + 5).Trim();
                    if (portInfo.Contains("/"))
                    {
                        var portParts = portInfo.Split('/');
                        port = portParts[0];
                        protocol = portParts[1];
                    }
                    else
                    {
                        port = portInfo;
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port))
        {
            ProcessOpenPort(ip, port, protocol, state, "", pro, user, sanitizer);
        }
    }

    private void ProcessMasscanResult(MasscanJsonResult result, Project pro, string user, HtmlSanitizer sanitizer)
    {
        if (string.IsNullOrEmpty(result.Ip)) return;

        foreach (var port in result.Ports ?? new MasscanPort[0])
        {
            if (port.Status == "open")
            {
                ProcessOpenPort(result.Ip, port.Port.ToString(), port.Proto, port.Status, port.Service, pro, user, sanitizer);
            }
        }
    }

    private void ProcessOpenPort(string ip, string portNumber, string protocol, string state, string service, 
                                Project pro, string user, HtmlSanitizer sanitizer)
    {
        // Crear o encontrar target
        Target target = null;
        var targets = targetManager.GetAll().Where(x => x.Project.Id == pro.Id).ToList();
        target = targets.FirstOrDefault(x => x.Name == ip);

        if (target == null)
        {
            target = new Target
            {
                Id = Guid.NewGuid(),
                Name = ip,
                Description = $"Host imported from Masscan scan",
                Type = TargetType.IP,
                Project = pro,
                UserId = user
            };
            targetManager.Add(target);
            targetManager.Context.SaveChanges();
        }

        // Determinar el riesgo basado en el puerto y servicio
        VulnRisk risk = DetermineRiskByPort(portNumber, service);

        // Construir descripción
        string description = $"Open {protocol.ToUpper()} port {portNumber} detected on {ip}";
        if (!string.IsNullOrEmpty(service))
        {
            description += $"\nService: {service}";
        }
        description += $"\nProtocol: {protocol.ToUpper()}";
        description += $"\nState: {state}";

        // Evaluar si el puerto representa un riesgo de seguridad
        string riskAssessment = EvaluatePortRisk(portNumber, service);
        if (!string.IsNullOrEmpty(riskAssessment))
        {
            description += $"\n\nSecurity Assessment: {riskAssessment}";
        }

        // Construir PoC
        string poc = $"Host: {ip}\nPort: {portNumber}/{protocol.ToUpper()}\nState: {state}";
        if (!string.IsNullOrEmpty(service))
        {
            poc += $"\nService: {service}";
        }

        // Construir remediación
        string remediation = GeneratePortRemediationAdvice(portNumber, service);

        // Generar nombre de vulnerabilidad
        string vulnName = $"Open Port {portNumber}/{protocol.ToUpper()}";
        if (!string.IsNullOrEmpty(service))
        {
            vulnName += $" ({service})";
        }
        vulnName += $" on {ip}";

        // Solo crear vulnerabilidad si el puerto representa un riesgo
        if (risk != VulnRisk.Info || IsHighRiskPort(portNumber))
        {
            var vulnerability = new Vuln
            {
                Id = Guid.NewGuid(),
                Name = sanitizer.Sanitize(vulnName),
                Description = sanitizer.Sanitize(description),
                Risk = risk,
                Status = VulnStatus.Open,
                Remediation = sanitizer.Sanitize(remediation),
                RemediationComplexity = RemediationComplexity.Low,
                RemediationPriority = RemediationPriority.Medium,
                Impact = sanitizer.Sanitize($"Exposed service on port {portNumber} may allow unauthorized access"),
                ProofOfConcept = sanitizer.Sanitize(poc),
                Project = pro,
                UserId = user,
                Template = false,
                Language = pro.Language,
                CreatedDate = DateTime.Now.ToUniversalTime()
            };

            vulnManager.Add(vulnerability);
            vulnManager.Context.SaveChanges();

            // Asociar vulnerabilidad con target
            var vulnTarget = new VulnTargets
            {
                Id = Guid.NewGuid(),
                VulnId = vulnerability.Id,
                TargetId = target.Id
            };
            vulnTargetManager.Add(vulnTarget);
            vulnTargetManager.Context.SaveChanges();
        }
    }

    private VulnRisk DetermineRiskByPort(string portNumber, string service)
    {
        if (!int.TryParse(portNumber, out int port)) return VulnRisk.Info;

        // Puertos de alto riesgo
        int[] highRiskPorts = { 21, 23, 135, 139, 445, 1433, 1521, 3306, 3389, 5432, 5984, 6379, 27017 };
        if (highRiskPorts.Contains(port)) return VulnRisk.High;

        // Puertos de riesgo medio
        int[] mediumRiskPorts = { 22, 25, 53, 110, 143, 993, 995, 1723, 2049, 5900, 5901, 5902 };
        if (mediumRiskPorts.Contains(port)) return VulnRisk.Medium;

        // Servicios conocidos peligrosos
        if (!string.IsNullOrEmpty(service))
        {
            string lowerService = service.ToLower();
            if (lowerService.Contains("telnet") || lowerService.Contains("ftp") || 
                lowerService.Contains("sql") || lowerService.Contains("rdp")) 
                return VulnRisk.High;
        }

        // Puertos de administración o desarrollo
        if (port >= 8000 && port <= 8999) return VulnRisk.Medium;
        if (port >= 9000 && port <= 9999) return VulnRisk.Medium;

        return VulnRisk.Low;
    }

    private bool IsHighRiskPort(string portNumber)
    {
        if (!int.TryParse(portNumber, out int port)) return false;
        
        // Considera puertos específicos como siempre reportables
        int[] alwaysReportPorts = { 21, 22, 23, 25, 53, 80, 110, 135, 139, 143, 443, 445, 993, 995, 1433, 1521, 3306, 3389, 5432, 5984, 6379, 27017 };
        return alwaysReportPorts.Contains(port);
    }

    private string EvaluatePortRisk(string portNumber, string service)
    {
        if (!int.TryParse(portNumber, out int port)) return "";

        switch (port)
        {
            case 21: return "FTP service may allow unauthorized file access if misconfigured";
            case 22: return "SSH service exposed - ensure strong authentication is configured";
            case 23: return "Telnet service sends credentials in clear text - consider disabling";
            case 25: return "SMTP service may be used for email relay attacks if misconfigured";
            case 53: return "DNS service exposed - verify it's not an open resolver";
            case 135: return "RPC service may expose sensitive Windows services";
            case 139: case 445: return "SMB/NetBIOS services may expose file shares and system information";
            case 1433: return "SQL Server exposed - ensure strong authentication and network restrictions";
            case 1521: return "Oracle Database exposed - verify access controls";
            case 3306: return "MySQL Database exposed - ensure proper authentication";
            case 3389: return "RDP service exposed - high risk for brute force attacks";
            case 5432: return "PostgreSQL Database exposed - verify access controls";
            case 6379: return "Redis database exposed - often lacks authentication by default";
            case 27017: return "MongoDB exposed - verify authentication is enabled";
            default: return "";
        }
    }

    private string GeneratePortRemediationAdvice(string portNumber, string service)
    {
        if (!int.TryParse(portNumber, out int port))
            return "Review if this service needs to be publicly accessible and implement appropriate security controls";

        switch (port)
        {
            case 21:
                return "Disable FTP if not needed, or use SFTP/FTPS. Implement strong authentication and restrict access";
            case 22:
                return "Ensure SSH uses key-based authentication, disable root login, and implement fail2ban";
            case 23:
                return "Disable Telnet service and use SSH instead for secure remote access";
            case 25:
                return "Secure SMTP configuration, implement authentication, and prevent open relay";
            case 53:
                return "Ensure DNS is not an open resolver and restrict recursive queries to authorized clients";
            case 135: case 139: case 445:
                return "Restrict SMB/RPC access to authorized networks and disable if not needed";
            case 1433: case 1521: case 3306: case 5432:
                return "Restrict database access to authorized IPs, use strong authentication, and encrypt connections";
            case 3389:
                return "Restrict RDP access, use VPN, implement account lockout policies, and enable NLA";
            case 6379:
                return "Enable Redis authentication, bind to localhost only, and use firewall rules";
            case 27017:
                return "Enable MongoDB authentication, bind to localhost, and implement proper access controls";
            default:
                return "Review if this service needs to be publicly accessible and implement appropriate security controls";
        }
    }
}

// Modelos para JSON de Masscan
public class MasscanJsonResult
{
    public string Ip { get; set; } = "";
    public long Timestamp { get; set; }
    public MasscanPort[] Ports { get; set; } = Array.Empty<MasscanPort>();
}

public class MasscanPort
{
    public int Port { get; set; }
    public string Proto { get; set; } = "";
    public string Status { get; set; } = "";
    public int Reason { get; set; }
    public int Ttl { get; set; }
    public string Service { get; set; } = "";
}