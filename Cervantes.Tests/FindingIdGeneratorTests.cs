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
    [Fact]
    public void Next_AfterIntMax()
    {
        // A restored backup can hold any number; the next one must still be larger.
        Assert.Equal("P-2147483648", FindingIdGenerator.Next(new[] { "P-2147483647" }, "P"));
    }

    [Fact]
    public void Next_IgnoresLongMax()
    {
        Assert.Equal("P-04", FindingIdGenerator.Next(new[] { "P-9223372036854775807", "P-03" }, "P"));
    }

    [Fact]
    public void Next_NullPrefix()
    {
        Assert.Equal("-04", FindingIdGenerator.Next(new[] { "-03" }, null));
    }

    [Fact]
    public void Next_LeadingZeros()
    {
        Assert.Equal("P-08", FindingIdGenerator.Next(new[] { "P-007" }, "P"));
    }

    [Fact]
    public void Next_IgnoresSignAndSpaces()
    {
        Assert.Equal("P-01", FindingIdGenerator.Next(new[] { "P-+5", "P- 6", "P-7 " }, "P"));
    }

    [Fact]
    public void Sequence_NumbersConsecutively()
    {
        var sequence = FindingIdGenerator.Start(new[] { "P-03" }, "P");

        Assert.Equal("P-04", sequence.Next());
        Assert.Equal("P-05", sequence.Next());
    }

    [Fact]
    public void Sequence_WithoutProjectIsEmpty()
    {
        // The manager is not touched when there is no project, so null stands in for it.
        var sequence = FindingIdGenerator.Start(vulnManager: null!, project: null);

        Assert.Equal("", sequence.Next());
        Assert.Equal("", sequence.Next());
    }
}
