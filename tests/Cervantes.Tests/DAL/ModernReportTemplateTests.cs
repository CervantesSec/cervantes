using Cervantes.CORE.Entities;
using Cervantes.DAL;

namespace Cervantes.Tests.DAL;

public class ModernReportTemplateTests
{
    [Fact]
    public void Parts_LoadEveryComponentFromTheEmbeddedFiles()
    {
        var parts = ModernReportTemplate.Parts();

        Assert.Equal(11, parts.Count);
        Assert.All(parts, p =>
        {
            Assert.StartsWith("Modern - ", p.Name);
            Assert.False(string.IsNullOrWhiteSpace(p.Content), p.Name);
            Assert.False(string.IsNullOrWhiteSpace(p.ContentCss), p.Name);
        });
        Assert.Equal(parts.Count, parts.Select(p => p.Name).Distinct().Count());
    }

    [Fact]
    public void Parts_HaveOneCoverHeaderAndFooterAndBodyInReportOrder()
    {
        var parts = ModernReportTemplate.Parts();

        Assert.Equal(ReportPartType.Cover, Assert.Single(parts, p => p.Name == "Modern - Cover").Type);
        Assert.Equal(ReportPartType.Header, Assert.Single(parts, p => p.Name == "Modern - Header").Type);
        Assert.Equal(ReportPartType.Footer, Assert.Single(parts, p => p.Name == "Modern - Footer").Type);
        var body = parts.Where(p => p.Type == ReportPartType.Body).Select(p => p.Name).ToList();
        Assert.Equal(new[]
        {
            "Modern - Document Control", "Modern - Contents", "Modern - Executive Summary", "Modern - Introduction",
            "Modern - Methodology", "Modern - Findings Summary", "Modern - Findings Detail", "Modern - Disclaimer"
        }, body);
    }

    // Repeated table rows must use {{tablerow}}: the report sanitizer moves a bare {{for}} out of <tbody>.
    [Fact]
    public void Parts_RepeatTableRowsWithTablerow()
    {
        foreach (var part in ModernReportTemplate.Parts())
        {
            foreach (System.Text.RegularExpressions.Match tbody in System.Text.RegularExpressions.Regex.Matches(part.Content, "<tbody>(.*?)</tbody>", System.Text.RegularExpressions.RegexOptions.Singleline))
            {
                Assert.DoesNotContain("{{for", tbody.Value);
            }
        }
    }
}
