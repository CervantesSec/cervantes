using Cervantes.Web.Helpers;

namespace Cervantes.Tests;

public class AttachmentPathTests
{
    private static readonly string Root = Path.Combine(Path.GetTempPath(), "cervantes-tests", "wwwroot");

    [Theory]
    [InlineData("Documents", "Attachments/Documents/a.pdf")]
    [InlineData("Task", "Attachments/Task/3f2c9a1e-0000-0000-0000-000000000001/b.txt")]
    [InlineData("Organization", "Attachments/Organization/cervantes.png")]
    [InlineData("Users", "Attachments/Users/logo.png")]
    public void TryResolve_AcceptsFilesInTheirFolder(string folder, string stored)
    {
        Assert.True(AttachmentPath.TryResolve(Root, stored, folder, out var fullPath));
        Assert.Equal(Path.GetFullPath(Path.Combine(Root, stored)), fullPath);
    }

    [Fact]
    public void TryResolve_NormalizesDotDotThatStaysInside()
    {
        Assert.True(AttachmentPath.TryResolve(Root, "Attachments/Documents/../Documents/a.pdf", "Documents", out var fullPath));
        Assert.Equal(Path.GetFullPath(Path.Combine(Root, "Attachments/Documents/a.pdf")), fullPath);
    }

    [Theory]
    [InlineData("Documents", "../appsettings.json")]
    [InlineData("Documents", "Attachments/Documents/../../../appsettings.json")]
    [InlineData("Organization", "../wwwroot/decoy.txt")]
    [InlineData("Documents", "Attachments/Clients/x.png")]
    [InlineData("Documents", "Attachments/DocumentsX/a.pdf")]
    [InlineData("Documents", "Attachments/Documents/")]
    [InlineData("Documents", "Attachments/Documents")]
    [InlineData("Documents", "/etc/passwd")]
    [InlineData("Documents", @"C:\Windows\win.ini")]
    [InlineData("Documents", @"\\server\share\a.pdf")]
    [InlineData("Documents", @"Attachments\Documents\..\..\x.json")]
    [InlineData("Clients", "None")]
    [InlineData("Documents", null)]
    [InlineData("Documents", "")]
    [InlineData("Documents", "   ")]
    [InlineData("Documents", "Attachments/Documents/a\0b.pdf")]
    [InlineData("Documents", "attachments/documents/a.pdf")]
    public void TryResolve_RejectsPathsOutsideTheFolder(string folder, string? stored)
    {
        Assert.False(AttachmentPath.TryResolve(Root, stored, folder, out var fullPath));
        Assert.Equal("", fullPath);
    }
}
