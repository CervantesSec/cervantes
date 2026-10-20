using Cervantes.Web.Helpers;

namespace Cervantes.Tests;

public class AttachmentArchiveTests
{
    [Fact]
    public void IsRestorable_FileInFolder()
    {
        Assert.True(AttachmentArchive.IsRestorable("Vuln/1b2c/a.png"));
    }

    [Fact]
    public void IsRestorable_Backslashes()
    {
        Assert.True(AttachmentArchive.IsRestorable(@"Vuln\1b2c\a.png"));
    }

    [Theory]
    [InlineData("Temp/x.json")]
    [InlineData("Imports/x.xml")]
    [InlineData("Export/a.csv")]
    [InlineData("Vulns/a")]
    public void IsRestorable_PrivateFolders(string entry)
    {
        Assert.False(AttachmentArchive.IsRestorable(entry));
    }

    [Fact]
    public void IsRestorable_RootFile()
    {
        Assert.False(AttachmentArchive.IsRestorable("a.txt"));
    }

    [Theory]
    [InlineData("Vuln/")]
    [InlineData("Vuln/1b2c/")]
    public void IsRestorable_Directory(string entry)
    {
        Assert.False(AttachmentArchive.IsRestorable(entry));
    }

    [Theory]
    [InlineData("../a")]
    [InlineData("Vuln/../../a")]
    [InlineData("Vuln/./a")]
    public void IsRestorable_Traversal(string entry)
    {
        Assert.False(AttachmentArchive.IsRestorable(entry));
    }

    [Theory]
    [InlineData("/Vuln/a")]
    [InlineData("C:/Vuln/a")]
    public void IsRestorable_Rooted(string entry)
    {
        Assert.False(AttachmentArchive.IsRestorable(entry));
    }

    [Fact]
    public void IsRestorable_FolderCase()
    {
        Assert.False(AttachmentArchive.IsRestorable("vuln/a"));
    }
}
