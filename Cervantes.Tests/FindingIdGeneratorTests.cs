using Cervantes.IFR.Parsers;

namespace Cervantes.Tests;

public class FindingIdGeneratorTests
{
    [Fact]
    public void Next_EmptyProject()
    {
        Assert.Equal("P-01", FindingIdGenerator.Next(Array.Empty<string?>(), "P"));
    }

    [Fact]
    public void Next_AfterGap()
    {
        // A deleted finding leaves a gap; counting would hand out P-03 a second time.
        Assert.Equal("P-04", FindingIdGenerator.Next(new[] { "P-01", "P-03" }, "P"));
    }

    [Fact]
    public void Next_SkipsUnparsableAndForeignIds()
    {
        Assert.Equal("P-03", FindingIdGenerator.Next(new[] { "P-02", "F1", "X-09", "P-abc", "", null }, "P"));
    }

    [Fact]
    public void Next_MixedPadding()
    {
        // Manual vulns used to get no zero padding.
        Assert.Equal("P-08", FindingIdGenerator.Next(new[] { "P-5", "P-07" }, "P"));
    }

    [Fact]
    public void ProjectFindingIds_CountsOldTemplateRowsInTheProject()
    {
        // Older builds let a project vuln be a template and numbered it; unticking the flag later brings its number back.
        var project = Guid.NewGuid();
        var vulns = new List<Cervantes.CORE.Entities.Vuln>
        {
            new() { ProjectId = project, FindingId = "P-01", Template = false },
            new() { ProjectId = project, FindingId = "P-02", Template = true },
            new() { ProjectId = Guid.NewGuid(), FindingId = "P-09", Template = false },
        };

        var ids = FindingIdGenerator.ProjectFindingIds(vulns.AsQueryable(), project);

        Assert.Equal("P-03", FindingIdGenerator.Next(ids, "P"));
    }

    [Fact]
    public void Next_PrefixWithDash()
    {
        Assert.Equal("ACME-2026-05", FindingIdGenerator.Next(new[] { "ACME-2026-04" }, "ACME-2026"));
    }
}
