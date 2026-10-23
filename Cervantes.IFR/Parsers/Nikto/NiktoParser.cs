using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
    // The finding ids of the import in progress; the project's ids are read once per import, not once per vuln.
    private FindingIdSequence findingIds = FindingIdSequence.Empty;
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
        findingIds = FindingIdSequence.Empty;
        var sanitizer = new HtmlSanitizer().AllowDataImagesOnly();

        if (project != null)
        {
            var pro = projectManager.GetById((Guid)project);
            findingIds = FindingIdGenerator.Start(vulnManager, pro);
            
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
        // Content that starts like JSON but is not (Nikto 2.5.0 writes broken JSON) throws and answers 400;
        // the text report never starts with { or [, so falling back to it would only import nothing
        foreach (var host in Read(fileContent))
        {
            foreach (var vulnerability in host.Vulnerabilities ?? Array.Empty<NiktoVulnerability>())
            {
                ProcessVulnerability(vulnerability.Id, vulnerability.Method, vulnerability.Url,
                                   vulnerability.Msg, host.Ip, host.Host,
                                   host.Port.ToString(), pro, user, sanitizer);
            }
        }
    }

    /// <summary>
    /// Reads the items of a Nikto XML report (2.5 and 2.6): test id, method, URI, description and the scan's target IP,
    /// host name and port. Nikto 2.6 starts some descriptions with "&lt;uri&gt;: "; that prefix is removed.
    /// </summary>
    public static System.Collections.Generic.IReadOnlyList<(string Id, string Method, string Uri, string Description, string Ip, string Host, string Port)> ReadXml(XmlDocument doc)
    {
        var items = new System.Collections.Generic.List<(string, string, string, string, string, string, string)>();
        foreach (XmlNode scanDetail in doc.GetElementsByTagName("scandetails"))
        {
            string ip = scanDetail.Attributes?["targetip"]?.Value ?? "";
            string host = scanDetail.Attributes?["targethostname"]?.Value ?? "";
            string port = scanDetail.Attributes?["targetport"]?.Value ?? "80";
            foreach (XmlNode item in scanDetail.SelectNodes(".//item")!)
            {
                string uri = item.SelectSingleNode("uri")?.InnerText.Trim() ?? "";
                string description = item.SelectSingleNode("description")?.InnerText.Trim() ?? "";
                if (uri.Length > 0 && description.StartsWith(uri + ": ", StringComparison.Ordinal))
                {
                    description = description[(uri.Length + 2)..];
                }
                items.Add((item.Attributes?["id"]?.Value ?? "", item.Attributes?["method"]?.Value ?? "", uri, description, ip, host, port));
            }
        }
        return items;
    }

    /// <summary>
    /// Reads Nikto JSON (`-Format json`): an array of hosts, or a single host object. Throws <see cref="JsonException"/>
    /// if it is not valid JSON.
    /// </summary>
    public static System.Collections.Generic.IReadOnlyList<NiktoJsonResult> Read(string content)
    {
        if (content.TrimStart().StartsWith("["))
        {
            return JsonSerializer.Deserialize<NiktoJsonResult[]>(content, ParserJson.Options) ?? Array.Empty<NiktoJsonResult>();
        }
        var single = JsonSerializer.Deserialize<NiktoJsonResult>(content, ParserJson.Options);
        return single == null ? Array.Empty<NiktoJsonResult>() : new[] { single };
    }

    private void ParseXmlFormat(string path, Project pro, string user, HtmlSanitizer sanitizer)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        foreach (var item in ReadXml(doc))
        {
            ProcessVulnerability(item.Id, item.Method, item.Uri, item.Description, item.Ip,
                               item.Host, item.Port, pro, user, sanitizer);
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
        string hostToUse = ImportedTarget.Name(sanitizer, !string.IsNullOrEmpty(targetHostname) ? targetHostname : targetHost);
        
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
            Description = sanitizer.Sanitize(WebUtility.HtmlEncode(detailedDescription)),
            Risk = risk,
            Status = VulnStatus.Open,
            Remediation = sanitizer.Sanitize("Review and remediate the identified web vulnerability"),
            RemediationComplexity = RemediationComplexity.Medium,
            RemediationPriority = RemediationPriority.Medium,
            Impact = sanitizer.Sanitize(WebUtility.HtmlEncode(description)),
            ProofOfConcept = sanitizer.Sanitize(WebUtility.HtmlEncode(poc)),
            Project = pro,
            UserId = user,
            Template = false,
            Language = pro.Language,
            CreatedDate = DateTime.Now.ToUniversalTime()
        };

        vulnerability.FindingId = findingIds.Next();
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

// Modelos para JSON de Nikto (`-Format json`): one object per scanned host, port written as a string.
public class NiktoJsonResult
{
    public string Host { get; set; } = "";
    public string Ip { get; set; } = "";
    public int Port { get; set; } = 80;
    public string Banner { get; set; } = "";
    // Nikto 2.6 writes the banner as server_banner (often null)
    [System.Text.Json.Serialization.JsonPropertyName("server_banner")]
    public string? ServerBanner { get; set; }
    public NiktoVulnerability[] Vulnerabilities { get; set; } = Array.Empty<NiktoVulnerability>();
}

public class NiktoVulnerability
{
    public string Id { get; set; } = "";
    public string Method { get; set; } = "";
    public string Url { get; set; } = "";
    public string Msg { get; set; } = "";
    // A string, or a list in some Nikto versions; not used by the import.
    public JsonElement References { get; set; }
}