using Cervantes.CORE.Entities;
using Cervantes.DAL;

namespace Cervantes.Tests.DAL;

public class ModernReportTemplateTests
{
    public static TheoryData<string> Variants => new() { "English", "Spanish" };

    private static ModernReportTemplate.Variant VariantNamed(string name) =>
        name == "English" ? ModernReportTemplate.English : ModernReportTemplate.Spanish;

    [Theory]
    [MemberData(nameof(Variants))]
    public void Parts_LoadEveryComponentFromTheEmbeddedFiles(string variantName)
    {
        var variant = VariantNamed(variantName);
        var parts = ModernReportTemplate.Parts(variant);

        Assert.Equal(11, parts.Count);
        Assert.All(parts, p =>
        {
            Assert.False(string.IsNullOrWhiteSpace(p.Content), p.Name);
            Assert.False(string.IsNullOrWhiteSpace(p.ContentCss), p.Name);
        });
        Assert.Equal(parts.Count, parts.Select(p => p.Name).Distinct().Count());
        Assert.Equal(new[] { ReportPartType.Cover, ReportPartType.Header, ReportPartType.Footer },
            parts.Take(3).Select(p => p.Type));
        Assert.All(parts.Skip(3), p => Assert.Equal(ReportPartType.Body, p.Type));
    }

    [Fact]
    public void Variants_ShareTheLayoutAndHaveTheirOwnNamesAndLanguage()
    {
        var english = ModernReportTemplate.Parts(ModernReportTemplate.English);
        var spanish = ModernReportTemplate.Parts(ModernReportTemplate.Spanish);

        Assert.Equal(english.Select(p => p.ContentCss), spanish.Select(p => p.ContentCss));
        Assert.Empty(english.Select(p => p.Name).Intersect(spanish.Select(p => p.Name)));
        Assert.Equal(Language.English, ModernReportTemplate.English.Language);
        Assert.Equal(Language.Español, ModernReportTemplate.Spanish.Language);
        Assert.NotEqual(ModernReportTemplate.English.TemplateName, ModernReportTemplate.Spanish.TemplateName);
        Assert.Contains("Informe de Test de Intrusión", spanish[0].Content);
    }

    // Repeated table rows must use {{tablerow}}: the report sanitizer moves a bare {{for}} out of <tbody>. And
    // {{tablerow}} strips every {{end}} of its row, so a row cannot hold an {{if}}.
    [Theory]
    [MemberData(nameof(Variants))]
    public void Parts_RepeatTableRowsWithTablerowAndNoIfInsideThem(string variantName)
    {
        foreach (var part in ModernReportTemplate.Parts(VariantNamed(variantName)))
        {
            foreach (System.Text.RegularExpressions.Match tbody in System.Text.RegularExpressions.Regex.Matches(part.Content, "<tbody>(.*?)</tbody>", System.Text.RegularExpressions.RegexOptions.Singleline))
            {
                Assert.DoesNotContain("{{for", tbody.Value);
                Assert.DoesNotContain("{{if", tbody.Value);
            }
        }
    }
}
