using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.DAL;

/// <summary>
/// The "Modern" report templates, in English and Spanish. Their HTML lives in Seed/ReportTemplates/Modern (English) and
/// Seed/ReportTemplates/Moderno (Spanish); both share the CSS in Seed/ReportTemplates/Modern. All of it is embedded in
/// this assembly and laid out to render the same in HTML, PDF (iText) and DOCX (HtmlToOpenXml): tables for layout, font
/// sizes on cells, fixed heights in pt on rows and cells, {{tablerow}} for repeated rows.
/// </summary>
public static class ModernReportTemplate
{
    public sealed record Part(string Name, ReportPartType Type, string Content, string ContentCss);

    /// <summary>One language of the template: its name, language, HTML folder and component names in report order.</summary>
    public sealed record Variant(string TemplateName, string Description, Language Language, string HtmlFolder, string[] ComponentNames);

    public static readonly Variant English = new(
        "Modern Pentest Report (EN)",
        "Modern penetration test report: cover, document control, contents, executive summary, scope, methodology, findings and disclaimer",
        Language.English, "Modern",
        new[]
        {
            "Modern - Cover", "Modern - Header", "Modern - Footer", "Modern - Document Control", "Modern - Contents",
            "Modern - Executive Summary", "Modern - Introduction", "Modern - Methodology", "Modern - Findings Summary",
            "Modern - Findings Detail", "Modern - Disclaimer"
        });

    public static readonly Variant Spanish = new(
        "Informe de Pentest Moderno (ES)",
        "Informe de test de intrusión moderno: portada, control del documento, índice, resumen ejecutivo, alcance, metodología, hallazgos y aviso legal",
        Language.Español, "Moderno",
        new[]
        {
            "Moderno - Portada", "Moderno - Cabecera", "Moderno - Pie", "Moderno - Control del Documento", "Moderno - Índice",
            "Moderno - Resumen Ejecutivo", "Moderno - Introducción", "Moderno - Metodología", "Moderno - Resumen de Hallazgos",
            "Moderno - Detalle de Hallazgos", "Moderno - Aviso Legal"
        });

    // Embedded file name and part type of each component, in report order (same order as ComponentNames).
    private static readonly (string File, ReportPartType Type)[] Layout =
    {
        ("cover", ReportPartType.Cover), ("header", ReportPartType.Header), ("footer", ReportPartType.Footer),
        ("document-control", ReportPartType.Body), ("contents", ReportPartType.Body), ("executive-summary", ReportPartType.Body),
        ("introduction", ReportPartType.Body), ("methodology", ReportPartType.Body), ("findings-summary", ReportPartType.Body),
        ("findings-detail", ReportPartType.Body), ("disclaimer", ReportPartType.Body),
    };

    public static IReadOnlyList<Part> Parts(Variant variant)
    {
        return Layout.Select((l, i) => new Part(variant.ComponentNames[i], l.Type,
            Read(variant.HtmlFolder + "." + l.File + ".html"), Read("Modern." + l.File + ".css"))).ToList();
    }

    private static string Read(string file)
    {
        using var stream = typeof(ModernReportTemplate).Assembly.GetManifestResourceStream("Cervantes.DAL.Seed." + file)
                           ?? throw new InvalidOperationException("Missing embedded report template file " + file);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Adds both languages on new and existing installs. A language is skipped when its template or its cover component
    /// already exists (by name), so a template the user renamed, edited or deleted while keeping its components is left alone.
    /// </summary>
    public static void Seed(IReportTemplateManager templates, IReportComponentsManager components, IReportsPartsManager parts,
        string ownerUserId)
    {
        foreach (var variant in new[] { English, Spanish })
        {
            Seed(variant, templates, components, parts, ownerUserId);
        }
    }

    private static void Seed(Variant variant, IReportTemplateManager templates, IReportComponentsManager components,
        IReportsPartsManager parts, string ownerUserId)
    {
        var coverName = variant.ComponentNames[0];
        if (templates.GetAll().Any(t => t.Name == variant.TemplateName) || components.GetAll().Any(c => c.Name == coverName))
        {
            return;
        }

        var now = DateTime.UtcNow;
        var template = new ReportTemplate
        {
            Id = Guid.NewGuid(),
            Name = variant.TemplateName,
            Description = variant.Description,
            Language = variant.Language,
            ReportType = ReportType.General,
            CreatedDate = now,
            UserId = ownerUserId
        };
        templates.Add(template);

        var order = 0;
        foreach (var part in Parts(variant))
        {
            var component = new ReportComponents
            {
                Id = Guid.NewGuid(),
                Name = part.Name,
                Language = variant.Language,
                ComponentType = part.Type,
                Content = part.Content,
                ContentCss = part.ContentCss,
                Created = now,
                Updated = now
            };
            components.Add(component);
            parts.Add(new ReportParts
            {
                Id = Guid.NewGuid(),
                TemplateId = template.Id,
                ComponentId = component.Id,
                PartType = part.Type,
                Order = order++
            });
        }

        templates.Context.SaveChanges();
    }
}
