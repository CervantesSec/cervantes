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

namespace Cervantes.IFR.Parsers.Nikto;

public class NiktoParser : INiktoParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public NiktoParser(ITargetManager targetManager, IVulnManager vulnManager,
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
            
            // Detectar formato (XML o JSON)
            var fileContent = System.IO.File.ReadAllText(path);
            
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
                // Formato de texto plano de Nikto
                ParseTextFormat(fileContent, pro, user, sanitizer);
            }
        }
    }

    private void ParseJsonFormat(string fileContent, Project pro, string user, HtmlSanitizer sanitizer)
    {
        try
        {
            var niktoResult = JsonSerializer.Deserialize<NiktoJsonResult>(fileContent);
            if (niktoResult?.host != null)
            {
                foreach (var vulnerability in niktoResult.vulnerabilities ?? new NiktoVulnerability[0])
                {
                    ProcessVulnerability(vulnerability.id, vulnerability.method, vulnerability.url, 
                                       vulnerability.msg, niktoResult.host.ip, niktoResult.host.hostname, 
                                       niktoResult.host.port.ToString(), pro, user, sanitizer);
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

        XmlNodeList scanDetails = doc.GetElementsByTagName("scandetails");
        
        foreach (XmlNode scanDetail in scanDetails)
        {
            string targetHost = scanDetail.Attributes?["targetip"]?.Value ?? "";
            string targetHostname = scanDetail.Attributes?["targethostname"]?.Value ?? "";
            string targetPort = scanDetail.Attributes?["targetport"]?.Value ?? "80";

            XmlNodeList items = scanDetail.SelectNodes(".//item");
            foreach (XmlNode item in items)
            {
                string id = item.Attributes?["id"]?.Value ?? "";
                string method = item.Attributes?["method"]?.Value ?? "";
                string uri = item.SelectSingleNode("uri")?.InnerText ?? "";
                string description = item.SelectSingleNode("description")?.InnerText ?? "";

                ProcessVulnerability(id, method, uri, description, targetHost, 
                                   targetHostname, targetPort, pro, user, sanitizer);
            }
        }
    }

    private void ParseTextFormat(string fileContent, Project pro, string user, HtmlSanitizer sanitizer)
    {
        var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        string currentHost = "";
        string currentPort = "80";
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            // Detectar información del target
            if (trimmedLine.StartsWith("- Nikto v") || trimmedLine.StartsWith("+ Target"))
            {
                // Extraer host y puerto de líneas como "Target IP: 192.168.1.1" o "Target Host: example.com"
                if (trimmedLine.Contains("Target") && trimmedLine.Contains(":"))
                {
                    var parts = trimmedLine.Split(':');
                    if (parts.Length >= 2)
                    {
                        var hostPart = string.Join(":", parts[1..]).Trim();
                        if (hostPart.Contains(":"))
                        {
                            var hostPortParts = hostPart.Split(':');
                            currentHost = hostPortParts[0];
                            currentPort = hostPortParts[1];
                        }
                        else
                        {
                            currentHost = hostPart;
                        }
                    }
                }
                continue;
            }

            // Detectar vulnerabilidades (líneas que empiezan con +)
            if (trimmedLine.StartsWith("+") && trimmedLine.Contains(":"))
            {
                var vulnParts = trimmedLine.Substring(1).Split(':', 2);
                if (vulnParts.Length >= 2)
                {
                    string uri = vulnParts[0].Trim();
                    string description = vulnParts[1].Trim();
                    
                    ProcessVulnerability("", "GET", uri, description, currentHost, 
                                       "", currentPort, pro, user, sanitizer);
                }
            }
        }
    }

    private void ProcessVulnerability(string id, string method, string uri, string description, 
                                    string targetHost, string targetHostname, string targetPort, 
                                    Project pro, string user, HtmlSanitizer sanitizer)
    {
        if (string.IsNullOrEmpty(description) && string.IsNullOrEmpty(uri))
            return;

        // Determinar el riesgo basado en palabras clave en la descripción
        VulnRisk risk = VulnRisk.Info;
        var lowerDesc = description.ToLower();
        
        if (lowerDesc.Contains("sql injection") || lowerDesc.Contains("xss") || 
            lowerDesc.Contains("command injection") || lowerDesc.Contains("file inclusion"))
        {
            risk = VulnRisk.High;
        }
        else if (lowerDesc.Contains("disclosure") || lowerDesc.Contains("exposed") || 
                 lowerDesc.Contains("vulnerable") || lowerDesc.Contains("outdated"))
        {
            risk = VulnRisk.Medium;
        }
        else if (lowerDesc.Contains("misconfiguration") || lowerDesc.Contains("header") || 
                 lowerDesc.Contains("cookie"))
        {
            risk = VulnRisk.Low;
        }

        // Crear o encontrar target
        Target target = null;
        string hostToUse = !string.IsNullOrEmpty(targetHostname) ? targetHostname : targetHost;
        
        if (!string.IsNullOrEmpty(hostToUse))
        {
            var targets = targetManager.GetAll().Where(x => x.Project.Id == pro.Id).ToList();
            target = targets.FirstOrDefault(x => x.Name == hostToUse);

            if (target == null)
            {
                target = new Target
                {
                    Id = Guid.NewGuid(),
                    Name = hostToUse,
                    Description = $"Target imported from Nikto scan",
                    Type = Uri.CheckHostName(hostToUse) == UriHostNameType.IPv4 || 
                           Uri.CheckHostName(hostToUse) == UriHostNameType.IPv6 ? 
                           TargetType.IP : TargetType.Hostname,
                    Project = pro,
                    UserId = user
                };
                targetManager.Add(target);
                targetManager.Context.SaveChanges();
            }
        }

        // Construir descripción detallada
        string detailedDescription = description;
        if (!string.IsNullOrEmpty(id))
        {
            detailedDescription += $"\n\nNikto Test ID: {id}";
        }
        if (!string.IsNullOrEmpty(method))
        {
            detailedDescription += $"\nHTTP Method: {method}";
        }

        // Construir PoC
        string poc = "";
        if (!string.IsNullOrEmpty(uri))
        {
            poc += $"URI: {uri}\n";
        }
        if (!string.IsNullOrEmpty(method))
        {
            poc += $"Method: {method}\n";
        }
        if (!string.IsNullOrEmpty(targetHost))
        {
            poc += $"Target IP: {targetHost}\n";
        }
        if (!string.IsNullOrEmpty(targetPort))
        {
            poc += $"Port: {targetPort}\n";
        }

        // Generar nombre de vulnerabilidad
        string vulnName = "Nikto Finding";
        if (!string.IsNullOrEmpty(uri))
        {
            vulnName = $"Nikto Finding on {uri}";
        }
        else if (!string.IsNullOrEmpty(description) && description.Length > 50)
        {
            vulnName = description.Substring(0, 50) + "...";
        }
        else if (!string.IsNullOrEmpty(description))
        {
            vulnName = description;
        }

        // Crear vulnerabilidad
        var vulnerability = new Vuln
        {
            Id = Guid.NewGuid(),
            Name = sanitizer.Sanitize(vulnName),
            Description = sanitizer.Sanitize(detailedDescription),
            Risk = risk,
            Status = VulnStatus.Open,
            Remediation = sanitizer.Sanitize("Review and remediate the identified web vulnerability"),
            RemediationComplexity = RemediationComplexity.Medium,
            RemediationPriority = RemediationPriority.Medium,
            Impact = sanitizer.Sanitize(description),
            ProofOfConcept = sanitizer.Sanitize(poc),
            Project = pro,
            UserId = user,
            Template = false,
            Language = pro.Language,
            CreatedDate = DateTime.Now.ToUniversalTime()
        };

        vulnManager.Add(vulnerability);
        vulnManager.Context.SaveChanges();

        // Asociar vulnerabilidad con target si está disponible
        if (target != null)
        {
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
}

// Modelos para JSON de Nikto
public class NiktoJsonResult
{
    public NiktoHost host { get; set; } = new();
    public NiktoVulnerability[] vulnerabilities { get; set; } = Array.Empty<NiktoVulnerability>();
}

public class NiktoHost
{
    public string ip { get; set; } = "";
    public string hostname { get; set; } = "";
    public int port { get; set; } = 80;
}

public class NiktoVulnerability
{
    public string id { get; set; } = "";
    public string method { get; set; } = "";
    public string url { get; set; } = "";
    public string msg { get; set; } = "";
}