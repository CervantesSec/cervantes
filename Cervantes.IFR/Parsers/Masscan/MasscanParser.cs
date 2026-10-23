using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Collections.Generic;
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
    // The finding ids of the import in progress; the project's ids are read once per import, not once per vuln.
    private FindingIdSequence findingIds = FindingIdSequence.Empty;
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
        findingIds = FindingIdSequence.Empty;
        var sanitizer = new HtmlSanitizer().AllowDataImagesOnly();

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            findingIds = FindingIdGenerator.Start(vulnManager, pro);
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
        // Content that starts like JSON but is not throws and answers 400; the list format never starts with { or [
        foreach (var open in OpenPorts(Read(fileContent)))
        {
            ProcessOpenPort(open.Ip, open.Port.ToString(), open.Proto, "open", open.Service ?? "", pro, user, sanitizer, open.Banner);
        }
    }

    /// <summary>
    /// One entry per open ip/port/protocol. With <c>--banners</c> Masscan writes each grabbed banner as another record
    /// for the same port (without a status); those are merged into the open port: the first service name, and every
    /// banner as a "name: banner" line.
    /// </summary>
    public static IReadOnlyList<MasscanOpenPort> OpenPorts(IEnumerable<MasscanJsonResult> results)
    {
        var records = results
            .Where(x => !string.IsNullOrEmpty(x.Ip))
            .SelectMany(x => (x.Ports ?? Array.Empty<MasscanPort>()).Select(p => (x.Ip, p.Port, Proto: string.IsNullOrEmpty(p.Proto) ? "tcp" : p.Proto,
                Open: p.Status == "open" || p.Service != null, ServiceName: p.Service?.Name, p.Service?.Banner)));
        return Merge(records);
    }

    /// <summary>The open ports of a Masscan XML report, merged as <see cref="OpenPorts"/> does (banner hosts repeat the port).</summary>
    public static IReadOnlyList<MasscanOpenPort> ReadXml(XmlDocument doc)
    {
        var records = new List<(string Ip, int Port, string Proto, bool Open, string? ServiceName, string? Banner)>();
        foreach (XmlNode host in doc.GetElementsByTagName("host"))
        {
            var ip = host.SelectSingleNode("address[@addrtype='ipv4']")?.Attributes?["addr"]?.Value ?? "";
            foreach (XmlNode port in host.SelectNodes(".//port")!)
            {
                if (!int.TryParse(port.Attributes?["portid"]?.Value, out var portId) || ip.Length == 0)
                {
                    continue;
                }
                var service = port.SelectSingleNode("service");
                records.Add((ip, portId, port.Attributes?["protocol"]?.Value ?? "tcp",
                    port.SelectSingleNode("state")?.Attributes?["state"]?.Value == "open",
                    service?.Attributes?["name"]?.Value, service?.Attributes?["banner"]?.Value));
            }
        }
        return Merge(records);
    }

    private static IReadOnlyList<MasscanOpenPort> Merge(IEnumerable<(string Ip, int Port, string Proto, bool Open, string? ServiceName, string? Banner)> records)
    {
        return records
            .GroupBy(x => (x.Ip, x.Port, x.Proto))
            .Where(g => g.Any(x => x.Open))
            .Select(g =>
            {
                var services = g.Where(x => !string.IsNullOrEmpty(x.ServiceName)).ToList();
                var banners = services.Where(x => !string.IsNullOrEmpty(x.Banner)).Select(x => $"{x.ServiceName}: {x.Banner}").ToList();
                return new MasscanOpenPort(g.Key.Ip, g.Key.Port, g.Key.Proto, services.FirstOrDefault().ServiceName,
                    banners.Count > 0 ? string.Join("\n", banners) : null);
            })
            .ToList();
    }

    /// <summary>
    /// Reads Masscan JSON (`-oJ`): an array of hosts, or a single host object. Throws <see cref="JsonException"/> if it
    /// is not valid JSON.
    /// </summary>
    public static IReadOnlyList<MasscanJsonResult> Read(string content)
    {
        if (content.TrimStart().StartsWith("["))
        {
            return JsonSerializer.Deserialize<MasscanJsonResult[]>(content, ParserJson.Options) ?? Array.Empty<MasscanJsonResult>();
        }
        var single = JsonSerializer.Deserialize<MasscanJsonResult>(content, ParserJson.Options);
        return single == null ? Array.Empty<MasscanJsonResult>() : new[] { single };
    }

    private void ParseXmlFormat(string path, Project pro, string user, HtmlSanitizer sanitizer)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        foreach (var open in ReadXml(doc))
        {
            ProcessOpenPort(open.Ip, open.Port.ToString(), open.Proto, "open", open.Service ?? "", pro, user, sanitizer, open.Banner);
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

    private void ProcessOpenPort(string ip, string portNumber, string protocol, string state, string service,
                                Project pro, string user, HtmlSanitizer sanitizer, string? banner = null)
    {
        // Crear o encontrar target
        Target target = null;
        var targetName = ImportedTarget.Name(sanitizer, ip);
        var targets = targetManager.GetAll().Where(x => x.Project.Id == pro.Id).ToList();
        target = targets.FirstOrDefault(x => x.Name == targetName);

        if (target == null)
        {
            target = new Target
            {
                Id = Guid.NewGuid(),
                Name = targetName,
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
        if (!string.IsNullOrEmpty(banner))
        {
            poc += $"\nBanners:\n{banner}";
        }
        // Service names and banners come from the scanned host: plain text
        poc = WebUtility.HtmlEncode(poc);

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

            vulnerability.FindingId = findingIds.Next();
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
    // Masscan writes the reason as text ("syn-ack").
    [System.Text.Json.Serialization.JsonConverter(typeof(FlexibleStringConverter))]
    public string Reason { get; set; } = "";
    public int Ttl { get; set; }
    // With --banners Masscan writes {"name": ..., "banner": ...}; a plain string is read as the name
    [System.Text.Json.Serialization.JsonConverter(typeof(MasscanServiceConverter))]
    public MasscanService? Service { get; set; }
}

public class MasscanService
{
    public string Name { get; set; } = "";
    public string Banner { get; set; } = "";
}

// One open port of a host, with the banners Masscan grabbed for it merged in.
public sealed record MasscanOpenPort(string Ip, int Port, string Proto, string? Service, string? Banner);

public class MasscanServiceConverter : System.Text.Json.Serialization.JsonConverter<MasscanService?>
{
    public override MasscanService? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.String:
                return new MasscanService { Name = reader.GetString() ?? "" };
            case JsonTokenType.StartObject:
                using (var doc = JsonDocument.ParseValue(ref reader))
                {
                    string Text(string name) => doc.RootElement.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() ?? "" : "";
                    return new MasscanService { Name = Text("name"), Banner = Text("banner") };
                }
            default:
                throw new JsonException("Masscan service must be a string or an object");
        }
    }

    public override void Write(Utf8JsonWriter writer, MasscanService? value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value, options);
}