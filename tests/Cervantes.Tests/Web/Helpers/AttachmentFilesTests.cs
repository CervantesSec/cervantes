using Cervantes.Web.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cervantes.Tests.Web.Helpers;

public class AttachmentFilesTests : IDisposable
{
    private readonly string root = Path.Combine(Path.GetTempPath(), "cervantes-tests-files-" + Guid.NewGuid());
    private readonly Guid ownerA = Guid.NewGuid();
    private readonly Guid ownerB = Guid.NewGuid();

    public void Dispose()
    {
        Directory.Delete(root, true);
    }

    private string Write(string stored)
    {
        var full = Path.Combine(root, stored);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        File.WriteAllText(full, "x");
        return full;
    }

    private void Delete(string? stored)
    {
        AttachmentFiles.DeleteFile(root, stored, "Vuln", ownerA, NullLogger.Instance, "VulnAttachment", Guid.NewGuid(), "user");
    }

    [Fact]
    public void DeleteFile_RemovesOwnFile()
    {
        var full = Write($"Attachments/Vuln/{ownerA}/f.png");

        Delete($"Attachments/Vuln/{ownerA}/f.png");

        Assert.False(File.Exists(full));
    }

    [Fact]
    public void DeleteFile_KeepsAFileOfAnotherOwner()
    {
        var full = Write($"Attachments/Vuln/{ownerB}/f.png");

        Delete($"Attachments/Vuln/{ownerB}/f.png");

        Assert.True(File.Exists(full));
    }

    [Fact]
    public void DeleteFile_LeavesASubfolderPath()
    {
        Write($"Attachments/Vuln/{ownerA}/sub/f.png");

        Delete($"Attachments/Vuln/{ownerA}/sub");

        Assert.True(Directory.Exists(Path.Combine(root, $"Attachments/Vuln/{ownerA}/sub")));
    }

    [Fact]
    public void DeleteFile_IgnoresAMissingFile()
    {
        Directory.CreateDirectory(root);

        Delete($"Attachments/Vuln/{ownerA}/missing.png");
    }
    private void DeleteFolder(Guid owner)
    {
        AttachmentFiles.DeleteOwnerFolder(root, "Vuln", owner, NullLogger.Instance, "Vuln", "user");
    }

    [Fact]
    public void DeleteOwnerFolder_RemovesNestedFiles()
    {
        Write($"Attachments/Vuln/{ownerA}/f.png");
        Write($"Attachments/Vuln/{ownerA}/sub/g.png");

        DeleteFolder(ownerA);

        Assert.False(Directory.Exists(Path.Combine(root, $"Attachments/Vuln/{ownerA}")));
    }

    [Fact]
    public void DeleteOwnerFolder_IgnoresAMissingFolder()
    {
        Directory.CreateDirectory(root);

        DeleteFolder(ownerA);
    }

    [Fact]
    public void DeleteOwnerFolder_KeepsSiblingOwners()
    {
        Write($"Attachments/Vuln/{ownerA}/f.png");
        var other = Write($"Attachments/Vuln/{ownerB}/f.png");

        DeleteFolder(ownerA);

        Assert.True(File.Exists(other));
    }

    [Fact]
    public void DeleteOwnerFolder_LeavesALockedFolder()
    {
        if (!OperatingSystem.IsWindows())
        {
            return; // Only Windows refuses to delete a file that is open.
        }

        var full = Write($"Attachments/Vuln/{ownerA}/f.png");
        using (new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            DeleteFolder(ownerA);
        }

        Assert.True(File.Exists(full));
    }
}
