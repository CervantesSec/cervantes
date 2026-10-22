using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class BackupKeyMatchTests
{
    private sealed record Row(Guid Id, string Key);

    private static Dictionary<Guid, Guid> Match(IEnumerable<Row> backup, IEnumerable<Row> existing)
        => BackupKeyMatch.Match(backup, existing, x => x.Id, x => x.Key);

    [Fact]
    public void Match_SameKeyDifferentId()
    {
        var b = new Row(Guid.NewGuid(), "SQL Injection|1");
        var e = new Row(Guid.NewGuid(), "SQL Injection|1");

        var map = Match(new[] { b }, new[] { e });

        Assert.Equal(e.Id, map[b.Id]);
    }

    [Fact]
    public void Match_SameIdNotMapped()
    {
        var id = Guid.NewGuid();
        Assert.Empty(Match(new[] { new Row(id, "a") }, new[] { new Row(id, "a") }));
    }

    [Fact]
    public void Match_NoKeyNotMapped()
    {
        Assert.Empty(Match(new[] { new Row(Guid.NewGuid(), "only in backup") }, new[] { new Row(Guid.NewGuid(), "other") }));
    }

    [Fact]
    public void Match_DuplicateExistingKeysPickFirst()
    {
        var b = new Row(Guid.NewGuid(), "dup");
        var low = new Row(Guid.Parse("00000000-0000-0000-0000-000000000001"), "dup");
        var high = new Row(Guid.Parse("00000000-0000-0000-0000-000000000002"), "dup");

        Assert.Equal(low.Id, Match(new[] { b }, new[] { high, low })[b.Id]);
    }

    [Fact]
    public void SameComponentContent_IgnoresLineEndings()
    {
        Assert.True(BackupKeyMatch.SameComponentContent("<p>a</p>\r\n<p>b</p> ", "p{}", "<p>a</p>\n<p>b</p>", "p{}\n"));
        Assert.True(BackupKeyMatch.SameComponentContent("<p>a</p>", null, "<p>a</p>", ""));
    }

    [Fact]
    public void SameComponentContent_DifferentCss()
    {
        Assert.False(BackupKeyMatch.SameComponentContent("<p>a</p>", "p{color:red}", "<p>a</p>", "p{}"));
        Assert.False(BackupKeyMatch.SameComponentContent("<p>a</p>", "", "<p>b</p>", ""));
    }
}
