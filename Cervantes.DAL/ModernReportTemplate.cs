using Cervantes.Contracts;
using Cervantes.CORE.Entities;

namespace Cervantes.DAL;

/// <summary>
/// The "Modern Pentest Report (EN)" template. Its HTML and CSS live in Seed/ReportTemplates/Modern (embedded in this
/// assembly) and are laid out to render the same in HTML, PDF (iText) and DOCX (HtmlToOpenXml): tables for layout,
/// font sizes on cells, fixed heights in pt on rows and cells, {{tablerow}} for repeated rows.
/// </summary>
public static class ModernReportTemplate
{
    public const string TemplateName = "Modern Pentest Report (EN)";

    public sealed record Part(string Name, ReportPartType Type, string Content, string ContentCss);

    // Component name, embedded file name and part type, in report order.
    private static readonly (string Name, string File, ReportPartType Type)[] Definitions =
    {
        ("Modern - Cover", "cover", ReportPartType.Cover),
        ("Modern - Header", "header", ReportPartType.Header),
        ("Modern - Footer", "footer", ReportPartType.Footer),
        ("Modern - Document Control", "document-control", ReportPartType.Body),
        ("Modern - Contents", "contents", ReportPartType.Body),
        ("Modern - Executive Summary", "executive-summary", ReportPartType.Body),
        ("Modern - Introduction", "introduction", ReportPartType.Body),
        ("Modern - Methodology", "methodology", ReportPartType.Body),
        ("Modern - Findings Summary", "findings-summary", ReportPartType.Body),
        ("Modern - Findings Detail", "findings-detail", ReportPartType.Body),
        ("Modern - Disclaimer", "disclaimer", ReportPartType.Body),
    };

    public static IReadOnlyList<Part> Parts()
    {
        return Definitions.Select(d => new Part(d.Name, d.Type, Read(d.File + ".html"), Read(d.File + ".css"))).ToList();
    }

    private static string Read(string file)
    {
        using var stream = typeof(ModernReportTemplate).Assembly.GetManifestResourceStream("Cervantes.DAL.Seed.Modern." + file)
                           ?? throw new InvalidOperationException("Missing embedded report template file " + file);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Adds the template on new and existing installs. Nothing is done when the template or its cover component already
    /// exists (by name), so a template the user renamed, edited or deleted while keeping its components is left alone.
    /// </summary>
    public static void Seed(IReportTemplateManager templates, IReportComponentsManager components, IReportsPartsManager parts,
        string ownerUserId)
    {
        if (templates.GetAll().Any(t => t.Name == TemplateName) || components.GetAll().Any(c => c.Name == "Modern - Cover"))
        {
            return;
        }

        var now = DateTime.UtcNow;
        var template = new ReportTemplate
        {
            Id = Guid.NewGuid(),
            Name = TemplateName,
            Description = "Modern penetration test report: cover, document control, contents, executive summary, scope, methodology, findings and disclaimer",
            Language = Language.English,
            ReportType = ReportType.General,
            CreatedDate = now,
            UserId = ownerUserId
        };
        templates.Add(template);

        var order = 0;
        foreach (var part in Parts())
        {
            var component = new ReportComponents
            {
                Id = Guid.NewGuid(),
                Name = part.Name,
                Language = Language.English,
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
