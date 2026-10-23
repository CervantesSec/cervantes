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

    // A sticky toolbar works out its visibility when the editor starts. Dialogs slide in, so at that moment the editor is
    // off screen: TinyMCE fades the toolbar out (tox-editor-dock-fadeout) and only shows it again on the next scroll.
    // Every editor has a fixed height and scrolls inside itself, so its toolbar is always on top anyway.
    [Fact]
    public void NoEditorUsesAStickyToolbar()
    {
        var components = Path.Combine(RepoRoot(), "Cervantes.Web", "Components");
        var offenders = Directory.EnumerateFiles(components, "*.razor*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".razor") || f.EndsWith(".razor.cs"))
            .Where(f => Regex.IsMatch(File.ReadAllText(f), "\"toolbar_sticky\"\\s*,\\s*true"))
            .Select(f => Path.GetRelativePath(components, f))
            .ToList();

        Assert.True(offenders.Count == 0, $"{offenders.Count} TinyMCE configs with a sticky toolbar: " + string.Join(", ", offenders));
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
