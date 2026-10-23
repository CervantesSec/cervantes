using System.Text.RegularExpressions;

namespace Cervantes.Tests.Web.Components;

public class EditorConfigTests
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

    // TinyMCE's sticky toolbar docks against the window by default; inside a scrolling dialog it fades the toolbar out
    // and never brings it back. "split" makes it dock on the container's scroll too.
    [Fact]
    public void EveryStickyEditorUsesSplitUiMode()
    {
        var components = Path.Combine(RepoRoot(), "Cervantes.Web", "Components");
        var offenders = Directory.EnumerateFiles(components, "*.razor*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".razor") || f.EndsWith(".razor.cs"))
            .Where(f =>
            {
                var text = File.ReadAllText(f);
                return text.Contains("\"toolbar_sticky\", true") && !text.Contains("\"ui_mode\", \"split\"");
            })
            .Select(f => Path.GetRelativePath(components, f))
            .ToList();

        Assert.True(offenders.Count == 0, "Sticky TinyMCE configs without ui_mode split: " + string.Join(", ", offenders));
    }

    // Self-hosted TinyMCE 7 runs in evaluation mode, with a console warning, unless the config declares the GPL key.
    [Fact]
    public void EveryEditorDeclaresGplLicenseKey()
    {
        var components = Path.Combine(RepoRoot(), "Cervantes.Web", "Components");
        var offenders = Directory.EnumerateFiles(components, "*.razor*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".razor") || f.EndsWith(".razor.cs"))
            .Where(f =>
            {
                var text = File.ReadAllText(f);
                return text.Contains("\"toolbar_sticky\"") &&
                       !Regex.IsMatch(text, "\\{\\s*\"license_key\"\\s*,\\s*\"gpl\"\\s*\\}");
            })
            .Select(f => Path.GetRelativePath(components, f))
            .ToList();

        Assert.True(offenders.Count == 0, $"{offenders.Count} TinyMCE configs without the GPL licence key: " + string.Join(", ", offenders));
    }
}
