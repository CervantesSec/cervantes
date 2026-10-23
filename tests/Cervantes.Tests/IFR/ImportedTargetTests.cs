using Cervantes.IFR.Parsers;
using Ganss.Xss;

namespace Cervantes.Tests.IFR;

public class ImportedTargetTests
{
    private static readonly HtmlSanitizer Sanitizer = new();

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Cervantes.sln")))
        {
            dir = dir.Parent;
        }
        return dir?.FullName ?? throw new InvalidOperationException("Cervantes.sln not found above the test output.");
    }

    [Fact]
    public void Name_StripsMarkup()
    {
        var name = ImportedTarget.Name(Sanitizer, "host<script>alert(1)</script>.local");

        Assert.DoesNotContain("<script", name);
        Assert.StartsWith("host", name);
    }

    [Theory]
    [InlineData("10.0.0.1")]
    [InlineData("app.example.com:8443")]
    [InlineData("httpd:2.4.49")]
    public void Name_KeepsPlainHosts(string host)
    {
        Assert.Equal(host, ImportedTarget.Name(Sanitizer, host));
    }

    [Fact]
    public void Name_IsStableForLookup()
    {
        var once = ImportedTarget.Name(Sanitizer, " a & b ");

        Assert.Equal(once, ImportedTarget.Name(Sanitizer, once));
    }

    [Fact]
    public void Name_OfNullIsEmpty()
    {
        Assert.Equal("", ImportedTarget.Name(Sanitizer, null));
    }

    // Report data names targets; a parser that stores it unsanitized lets markup from a scan reach the UI and reports.
    [Fact]
    public void EveryParserBuildsTargetNamesThroughImportedTarget()
    {
        var parsers = Path.Combine(RepoRoot(), "Cervantes.IFR", "Parsers");
        var offenders = Directory.EnumerateFiles(parsers, "*.cs", SearchOption.AllDirectories)
            // Nmap sanitizes its own target names; Dependency-Check names its single target with a fixed string.
            .Where(f => Path.GetFileName(f) is not ("NmapParser.cs" or "DependencyCheckParser.cs"))
            .Where(f =>
            {
                var text = File.ReadAllText(f);
                return text.Contains("new Target") && !text.Contains("ImportedTarget.Name(");
            })
            .Select(f => Path.GetRelativePath(parsers, f))
            .ToList();

        Assert.True(offenders.Count == 0, $"{offenders.Count} parsers name targets without ImportedTarget.Name: " + string.Join(", ", offenders));
    }
}
