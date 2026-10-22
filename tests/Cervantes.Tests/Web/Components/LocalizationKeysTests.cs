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
        var keys = Directory.EnumerateFiles(Path.Combine(web, "Components"), "*.razor*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".razor") || f.EndsWith(".razor.cs"))
            .SelectMany(f => Regex.Matches(File.ReadAllText(f), "localizer\\[\"([^\"]+)\"").Select(m => m.Groups[1].Value))
            .ToHashSet();

        var missing = new List<string>();
        foreach (var resx in Directory.EnumerateFiles(Path.Combine(web, "Localization"), "Resource*.resx"))
        {
            var defined = XDocument.Load(resx).Root!.Elements("data").Select(d => (string)d.Attribute("name")!).ToHashSet();
            missing.AddRange(keys.Where(k => !defined.Contains(k)).Select(k => $"{Path.GetFileName(resx)}:{k}"));
        }

        Assert.True(missing.Count == 0, $"{missing.Count} missing: " + string.Join(", ", missing.OrderBy(x => x).Take(60)));
    }
}
