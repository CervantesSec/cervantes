using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Cervantes.Tests.Web.Components;

public class LocalizationKeysTests
{
    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Cervantes.sln")))
        {
            dir = dir.Parent;
        }
        return dir?.FullName ?? throw new InvalidOperationException("Cervantes.sln not found above the test output.");
    }

    // A key missing from a culture file shows the raw key (e.g. "generate") in that language.
    [Fact]
    public void EveryLocalizerKeyExistsInEveryCulture()
    {
        var web = Path.Combine(RepoRoot(), "Cervantes.Web");
        var sources = Directory.EnumerateFiles(Path.Combine(web, "Components"), "*.razor*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".razor") || f.EndsWith(".razor.cs"))
            .Concat(Directory.EnumerateFiles(Path.Combine(web, "Helpers"), "*.cs", SearchOption.AllDirectories));
        // Validators take an optional localizer and index it with localizer?["key"].
        var keys = sources
            .SelectMany(f => Regex.Matches(File.ReadAllText(f), "localizer\\??\\[\"([^\"]+)\"").Select(m => m.Groups[1].Value))
            .ToHashSet();

        var missing = new List<string>();
        foreach (var resx in Directory.EnumerateFiles(Path.Combine(web, "Localization"), "Resource*.resx"))
        {
            var defined = XDocument.Load(resx).Root!.Elements("data").Select(d => (string)d.Attribute("name")!).ToHashSet();
            missing.AddRange(keys.Where(k => !defined.Contains(k)).Select(k => $"{Path.GetFileName(resx)}:{k}"));
        }

        Assert.True(missing.Count == 0, $"{missing.Count} missing: " + string.Join(", ", missing.OrderBy(x => x).Take(60)));
    }

    // The count goes in parentheses so the sentence reads the same for one item ("los 1 elementos" read wrong).
    [Fact]
    public void DeleteSelectedCountPutsTheCountInParentheses()
    {
        var web = Path.Combine(RepoRoot(), "Cervantes.Web");
        var wrong = Directory.EnumerateFiles(Path.Combine(web, "Localization"), "Resource*.resx")
            .Where(resx => !((string)XDocument.Load(resx).Root!.Elements("data")
                .Single(d => (string)d.Attribute("name")! == "deleteSelectedCount").Element("value")!).Contains("({0})"))
            .Select(Path.GetFileName)
            .ToList();

        Assert.True(wrong.Count == 0, "deleteSelectedCount without \"({0})\": " + string.Join(", ", wrong));
    }

    // The report sanitizer drops custom property declarations when a generated report is edited (FIXES 19), so the
    // component CSS help must say how to write CSS that survives it.
    [Fact]
    public void ReportComponentCssHelpWarnsAboutCssVariables()
    {
        var web = Path.Combine(RepoRoot(), "Cervantes.Web");
        var missing = Directory.EnumerateFiles(Path.Combine(web, "Localization"), "Resource*.resx")
            .Where(resx => !((string)XDocument.Load(resx).Root!.Elements("data")
                .Single(d => (string)d.Attribute("name")! == "reportComponentCssHelp").Element("value")!).Contains("var(--") || !((string)XDocument.Load(resx).Root!.Elements("data")
                .Single(d => (string)d.Attribute("name")! == "reportComponentCssHelp").Element("value")!).Contains("table-cell"))
            .Select(Path.GetFileName)
            .ToList();

        Assert.True(missing.Count == 0, "reportComponentCssHelp without the CSS variables and PDF header note: " + string.Join(", ", missing));
    }
}
