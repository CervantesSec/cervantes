using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using Ganss.Xss;

namespace Cervantes.IFR.Parsers.Prowler;

public class ProwlerParser : IProwlerParser
{
    private ITargetManager targetManager = null;
    private IVulnManager vulnManager = null;
    // The finding ids of the import in progress; the project's ids are read once per import, not once per vuln.
    private FindingIdSequence findingIds = FindingIdSequence.Empty;
    private IVulnTargetManager vulnTargetManager = null;
    private IProjectManager projectManager = null;

    public ProwlerParser(ITargetManager targetManager, IVulnManager vulnManager,
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
            // Read the whole file first: a file that is not a Prowler report throws before anything is saved
            foreach (var result in Read(System.IO.File.ReadAllText(path)).Where(IsFinding))
            {
                ProcessProwlerFinding(result, pro, user, sanitizer);
            }
        }
    }

    /// <summary>
    /// Reads every check result of a Prowler report: v3 JSON (array of objects with <c>CheckID</c>), v4/v5 JSON-OCSF
    /// (array of objects with <c>status_code</c>) or the CSV of v3, v4 and v5 (<c>;</c>-separated, with a
    /// <c>CHECK_ID</c> column). Malformed JSON throws <see cref="JsonException"/>; anything else throws
    /// <see cref="InvalidDataException"/>.
    /// </summary>
    public static IReadOnlyList<ProwlerResult> Read(string content)
    {
        var trimmed = content.TrimStart('﻿', ' ', '\t', '\r', '\n');
        if (trimmed.StartsWith("[") || trimmed.StartsWith("{"))
        {
            using var doc = JsonDocument.Parse(trimmed);
            var items = doc.RootElement.ValueKind == JsonValueKind.Array
                ? doc.RootElement.EnumerateArray().ToList()
                : new List<JsonElement> { doc.RootElement };
            if (items.Count == 0)
            {
                return Array.Empty<ProwlerResult>();
            }
            if (items.All(x => Field(x, "CheckID") is { ValueKind: JsonValueKind.String }))
            {
                return items.Select(FromV3Json).ToList();
            }
            if (items.All(x => Field(x, "status_code") is { ValueKind: JsonValueKind.String }))
            {
                return items.Select(FromOcsf).ToList();
            }
            throw new InvalidDataException("Not a Prowler v3 JSON, OCSF JSON or CSV report");
        }

        var firstLine = trimmed.Split('\n', 2)[0];
        if (firstLine.Contains(';') && firstLine.Split(';').Any(x => x.Trim().Trim('"') == "CHECK_ID"))
        {
            return FromCsv(trimmed);
        }
        throw new InvalidDataException("Not a Prowler v3 JSON, OCSF JSON or CSV report");
    }

    /// <summary>A failed check that is not muted; passed, manual and muted results are not findings.</summary>
    public static bool IsFinding(ProwlerResult result)
    {
        return result.Status.Equals("FAIL", StringComparison.OrdinalIgnoreCase) && !result.Muted;
    }

    private static ProwlerResult FromV3Json(JsonElement e)
    {
        var recommendation = Field(e, "Remediation") is { } remediation ? Field(remediation, "Recommendation") : null;
        return new ProwlerResult
        {
            CheckID = Text(e, "CheckID"),
            CheckTitle = Text(e, "CheckTitle"),
            Status = Text(e, "Status"),
            Severity = Text(e, "Severity"),
            Service = Text(e, "ServiceName"),
            Region = Text(e, "Region"),
            Account = Text(e, "AccountId"),
            Resource = FirstNonEmpty(Text(e, "ResourceId"), Text(e, "ResourceArn")),
            Description = Text(e, "Description"),
            Risk = Text(e, "Risk"),
            Remediation = recommendation is { } r ? Text(r, "Text") : "",
            RemediationUrl = recommendation is { } u ? Text(u, "Url") : "",
        };
    }

    private static ProwlerResult FromOcsf(JsonElement e)
    {
        var resource = Field(e, "resources") is { ValueKind: JsonValueKind.Array } resources && resources.GetArrayLength() > 0
            ? resources[0]
            : (JsonElement?)null;
        var cloud = Field(e, "cloud");
        var account = cloud is { } c ? Field(c, "account") : null;
        var remediation = Field(e, "remediation");
        var references = remediation is { } rem ? Field(rem, "references") : null;
        return new ProwlerResult
        {
            CheckID = Field(e, "metadata") is { } metadata ? Text(metadata, "event_code") : "",
            CheckTitle = Field(e, "finding_info") is { } title ? Text(title, "title") : "",
            Status = Text(e, "status_code"),
            Severity = Text(e, "severity"),
            Service = resource is { } res && Field(res, "group") is { } group ? Text(group, "name") : "",
            Region = cloud is { } cl ? Text(cl, "region") : "",
            Account = account is { } a ? Text(a, "uid") : "",
            Resource = resource is { } r ? Text(r, "uid") : "",
            Description = Field(e, "finding_info") is { } info ? Text(info, "desc") : "",
            Risk = Text(e, "risk_details"),
            Remediation = remediation is { } d ? Text(d, "desc") : "",
            RemediationUrl = references is { ValueKind: JsonValueKind.Array } refs
                ? refs.EnumerateArray().Select(x => x.ValueKind == JsonValueKind.String ? x.GetString() ?? "" : "")
                    .FirstOrDefault(x => x.StartsWith("http", StringComparison.OrdinalIgnoreCase)) ?? ""
                : "",
        };
    }

    private static IReadOnlyList<ProwlerResult> FromCsv(string content)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            BadDataFound = null,
            MissingFieldFound = null,
        };
        using var csv = new CsvReader(new StringReader(content), config);
        csv.Read();
        csv.ReadHeader();
        string Column(params string[] names) =>
            names.Select(name => csv.HeaderRecord!.Contains(name) ? csv.GetField(name) : null)
                .FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? "";

        var results = new List<ProwlerResult>();
        while (csv.Read())
        {
            results.Add(new ProwlerResult
            {
                CheckID = Column("CHECK_ID"),
                CheckTitle = Column("CHECK_TITLE"),
                Status = Column("STATUS"),
                Severity = Column("SEVERITY"),
                Service = Column("SERVICE_NAME"),
                Region = Column("REGION"),
                Account = Column("ACCOUNT_UID", "ACCOUNT_ID"),
                Resource = Column("RESOURCE_UID", "RESOURCE_ID", "RESOURCE_ARN"),
                Description = Column("DESCRIPTION"),
                Risk = Column("RISK"),
                Remediation = Column("REMEDIATION_RECOMMENDATION_TEXT"),
                RemediationUrl = Column("REMEDIATION_RECOMMENDATION_URL"),
                Muted = Column("MUTED").Equals("True", StringComparison.OrdinalIgnoreCase),
            });
        }
        return results;
    }

    private static JsonElement? Field(JsonElement obj, string name)
    {
        return obj.ValueKind == JsonValueKind.Object && obj.TryGetProperty(name, out var value) ? value : null;
    }

    private static string Text(JsonElement obj, string name)
    {
        return Field(obj, name) is { ValueKind: JsonValueKind.String } value ? value.GetString() ?? "" : "";
    }

    private static string FirstNonEmpty(params string[] values) => values.FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? "";

    private void ProcessProwlerFinding(ProwlerResult result, Project pro, string user, HtmlSanitizer sanitizer)
    {
        if (string.IsNullOrEmpty(result.Description) && string.IsNullOrEmpty(result.CheckID))
            return;

        // Mapear severidad a riesgo
        VulnRisk risk = VulnRisk.Medium; // Default para hallazgos de cloud security

        if (!string.IsNullOrEmpty(result.Severity))
        {
            switch (result.Severity.ToLower())
            {
                case "critical":
                    risk = VulnRisk.Critical;
                    break;
                case "high":
                    risk = VulnRisk.High;
                    break;
                case "medium":
                    risk = VulnRisk.Medium;
                    break;
                case "low":
                    risk = VulnRisk.Low;
                    break;
                case "info":
                case "informational":
                    risk = VulnRisk.Info;
                    break;
            }
        }

        // Crear target basado en la región/cuenta
        Target target = null;
        string targetName = "";

        if (!string.IsNullOrEmpty(result.Region))
        {
            targetName = result.Region;
            if (!string.IsNullOrEmpty(result.Account))
            {
                targetName = $"{result.Account} ({result.Region})";
            }
        }
        else if (!string.IsNullOrEmpty(result.Account))
        {
            targetName = result.Account;
        }

        targetName = ImportedTarget.Name(sanitizer, targetName);

        if (!string.IsNullOrEmpty(targetName))
        {
            var targets = targetManager.GetAll().Where(x => x.Project.Id == pro.Id).ToList();
            target = targets.FirstOrDefault(x => x.Name == targetName);

            if (target == null)
            {
                target = new Target
                {
                    Id = Guid.NewGuid(),
                    Name = targetName,
                    Description = $"Cloud account/region imported from Prowler scan",
                    Type = TargetType.Hostname, // Usamos hostname para entidades cloud
                    Project = pro,
                    UserId = user
                };
                targetManager.Add(target);
                targetManager.Context.SaveChanges();
            }
        }

        // The report fields are plain text; encode them before they go into the rich-text fields
        string Html(string text) => WebUtility.HtmlEncode(text);

        // Construir descripción detallada
        string detailedDescription = Html(result.Description ?? "");
        if (!string.IsNullOrEmpty(result.CheckID))
        {
            detailedDescription += $"\n\nProwler Check ID: {Html(result.CheckID)}";
        }
        if (!string.IsNullOrEmpty(result.Service))
        {
            detailedDescription += $"\nCloud Service: {Html(result.Service)}";
        }
        if (!string.IsNullOrEmpty(result.Region))
        {
            detailedDescription += $"\nRegion: {Html(result.Region)}";
        }
        if (!string.IsNullOrEmpty(result.Account))
        {
            detailedDescription += $"\nAccount: {Html(result.Account)}";
        }

        // Construir PoC
        string poc = "";
        if (!string.IsNullOrEmpty(result.Resource))
        {
            poc += $"Affected Resource: {Html(result.Resource)}\n";
        }
        if (!string.IsNullOrEmpty(result.Service))
        {
            poc += $"Service: {Html(result.Service)}\n";
        }
        if (!string.IsNullOrEmpty(result.Region))
        {
            poc += $"Region: {Html(result.Region)}\n";
        }
        if (!string.IsNullOrEmpty(result.Account))
        {
            poc += $"Account: {Html(result.Account)}\n";
        }
        if (!string.IsNullOrEmpty(result.Status))
        {
            poc += $"Status: {Html(result.Status)}\n";
        }

        // Generar nombre de vulnerabilidad: the check title when the report has one
        string vulnName = !string.IsNullOrEmpty(result.CheckTitle) ? result.CheckTitle : "Prowler Cloud Security Finding";
        if (string.IsNullOrEmpty(result.CheckTitle) && !string.IsNullOrEmpty(result.CheckID))
        {
            vulnName = $"Prowler {result.CheckID}";
            if (!string.IsNullOrEmpty(result.Service))
            {
                vulnName += $" - {result.Service}";
            }
        }

        string remediation = !string.IsNullOrEmpty(result.Remediation)
            ? Html(result.Remediation)
            : "Review and remediate the cloud security misconfiguration according to best practices";
        if (!string.IsNullOrEmpty(result.RemediationUrl))
        {
            remediation += $"\n\n{Html(result.RemediationUrl)}";
        }

        // Crear vulnerabilidad
        var vulnerability = new Vuln
        {
            Id = Guid.NewGuid(),
            Name = sanitizer.Sanitize(vulnName),
            Description = sanitizer.Sanitize(detailedDescription),
            Risk = risk,
            Status = VulnStatus.Open,
            Remediation = sanitizer.Sanitize(remediation),
            RemediationComplexity = RemediationComplexity.Medium,
            RemediationPriority = RemediationPriority.Medium,
            Impact = sanitizer.Sanitize(!string.IsNullOrEmpty(result.Risk) ? Html(result.Risk)
                : Html(result.Description ?? "Cloud security misconfiguration detected")),
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

// One check result of a Prowler report, whatever the format it was read from
public class ProwlerResult
{
    public string CheckID { get; set; } = "";
    public string CheckTitle { get; set; } = "";
    public string Status { get; set; } = "";
    public string Severity { get; set; } = "";
    public string Region { get; set; } = "";
    public string Account { get; set; } = "";
    public string Service { get; set; } = "";
    public string Description { get; set; } = "";
    public string Resource { get; set; } = "";
    public string Risk { get; set; } = "";
    public string Remediation { get; set; } = "";
    public string RemediationUrl { get; set; } = "";
    public bool Muted { get; set; }
}
