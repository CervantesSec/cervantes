using System.Text;
using Cervantes.IFR.File;

namespace Cervantes.Tests.IFR;

public class FileCheckTests
{
    private readonly FileCheck check = new();

    private static byte[] Bytes(string s) => Encoding.UTF8.GetBytes(s);

    private const string Xhtml = "<?xml version=\"1.0\"?><!DOCTYPE html><html xmlns=\"http://www.w3.org/1999/xhtml\"><body><script>alert(1)</script></body></html>";
    private const string Svg = "<?xml version=\"1.0\"?><svg xmlns=\"http://www.w3.org/2000/svg\"><script>alert(1)</script></svg>";
    private const string HtmlNoNs = "<?xml version=\"1.0\"?><html><body/></html>";
    private const string PlainXml = "<?xml version=\"1.0\"?><config><item>a</item></config>";
    private const string NmapXml = "<?xml version=\"1.0\"?><nmaprun><host><ports><port portid=\"80\"><script id=\"http-title\" output=\"&lt;script&gt;x&lt;/script&gt;\"/></port></ports></host></nmaprun>";

    [Theory]
    [InlineData(Xhtml)]
    [InlineData(HtmlNoNs)]
    public void CheckAttachment_RejectsActiveXmlDocuments(string content)
    {
        var bytes = Bytes(content);
        Assert.Equal("xml", check.GetExtension(bytes), ignoreCase: true);
        Assert.False(check.CheckAttachment(bytes));
    }

    [Fact]
    public void CheckAttachment_RejectsSvg()
    {
        // MimeDetective reports this fixture as "svg" rather than "xml"; either way it must be refused.
        Assert.False(check.CheckAttachment(Bytes(Svg)));
    }

    [Fact]
    public void CheckAttachment_RejectsXhtmlWithBom()
    {
        var bytes = new byte[] { 0xEF, 0xBB, 0xBF }.Concat(Bytes(Xhtml)).ToArray();
        Assert.Equal("xml", check.GetExtension(bytes), ignoreCase: true);
        Assert.False(check.CheckAttachment(bytes));
    }

    [Theory]
    [InlineData(PlainXml)]
    [InlineData(NmapXml)]
    public void CheckAttachment_AcceptsNonActiveXml(string content)
    {
        var bytes = Bytes(content);
        Assert.Equal("xml", check.GetExtension(bytes), ignoreCase: true);
        Assert.True(check.CheckAttachment(bytes));
    }

    [Fact]
    public void CheckFile_AcceptsNmapXmlWithScripts()
    {
        var bytes = Bytes(NmapXml);
        Assert.Equal("xml", check.GetExtension(bytes), ignoreCase: true);
        Assert.True(check.CheckFile(bytes));
    }
}
