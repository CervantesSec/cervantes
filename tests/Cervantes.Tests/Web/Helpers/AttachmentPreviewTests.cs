using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class AttachmentPreviewTests
{
    [Fact]
    public void KindOf_Image() =>
        Assert.Equal(AttachmentPreviewKind.Image, AttachmentPreview.KindOf("Attachments/Vuln/1/a.PNG"));

    [Fact]
    public void KindOf_Pdf() =>
        Assert.Equal(AttachmentPreviewKind.Pdf, AttachmentPreview.KindOf("a.pdf"));

    [Fact]
    public void KindOf_Video() =>
        Assert.Equal(AttachmentPreviewKind.Video, AttachmentPreview.KindOf("a.webm"));

    [Theory]
    [InlineData("a.xml")]
    [InlineData("a.txt")]
    [InlineData("a.svg")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("Attachments/Vuln/1/")]
    public void KindOf_None(string? path) =>
        Assert.Equal(AttachmentPreviewKind.None, AttachmentPreview.KindOf(path));

    [Fact]
    public void KindOf_IgnoresQuery() =>
        Assert.Equal(AttachmentPreviewKind.Pdf, AttachmentPreview.KindOf("a.pdf?x=1"));
}
