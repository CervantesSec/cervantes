using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class BackupUserMapTests
{
    private static BackupUserMap Map(params string[] existing)
    {
        var set = new HashSet<string>(existing);
        return new BackupUserMap("restorer", id => Task.FromResult(set.Contains(id)));
    }

    [Fact]
    public async Task Resolve_Mapped()
    {
        var map = Map("backup-u", "target-u");
        map.Map("backup-u", "target-u");

        Assert.Equal("target-u", await map.ResolveAsync("backup-u"));
    }

    [Fact]
    public async Task Resolve_Existing()
    {
        Assert.Equal("u1", await Map("u1").ResolveAsync("u1"));
    }

    [Fact]
    public async Task Resolve_Missing()
    {
        Assert.Equal("restorer", await Map("u1").ResolveAsync("gone"));
    }

    [Fact]
    public async Task Resolve_NullGivesFallback()
    {
        Assert.Equal("restorer", await Map("u1").ResolveAsync(null));
    }

    [Fact]
    public async Task ResolveOptional_NullStaysNull()
    {
        Assert.Null(await Map("u1").ResolveOptionalAsync(null));
        Assert.Null(await Map("u1").ResolveOptionalAsync(""));
    }

    [Fact]
    public async Task ResolveOptional_MissingIsNull()
    {
        Assert.Null(await Map("u1").ResolveOptionalAsync("gone"));
    }

    [Fact]
    public async Task ResolveOptional_Mapped()
    {
        var map = Map("target-u");
        map.Map("backup-u", "target-u");

        Assert.Equal("target-u", await map.ResolveOptionalAsync("backup-u"));
        Assert.Equal("target-u", await map.ResolveOptionalAsync("target-u"));
    }
}
