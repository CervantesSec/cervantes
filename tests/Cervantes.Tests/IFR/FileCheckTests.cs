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

    // The root decides: an XHTML document is refused even when the rest of it is not well-formed.
    [Fact]
    public void CheckAttachment_RejectsXhtmlRootWithBrokenBody()
    {
        Assert.False(check.CheckAttachment(Bytes("<?xml version=\"1.0\"?><html xmlns=\"http://www.w3.org/1999/xhtml\"><body><p></body>")));
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

    [Fact]
    public void CheckImport_AcceptsXmlReportsTheMimeLibraryMisreads()
    {
        // A real Acunetix export: MimeDetective calls it GPX, so CheckFile alone refuses it
        var bytes = File.ReadAllBytes(ParserFixtures.Path("Acunetix", "acunetix.xml"));

        Assert.NotEqual("xml", check.GetExtension(bytes), StringComparer.OrdinalIgnoreCase);
        Assert.True(check.CheckImport(bytes));
    }

    [Theory]
    [InlineData(Xhtml)]
    [InlineData(Svg)]
    [InlineData(HtmlNoNs)]
    [InlineData("<html><body><script>alert(1)</script></body></html>")]
    public void CheckImport_RejectsActiveDocuments(string content)
    {
        Assert.False(check.CheckImport(Bytes(content)));
    }

    [Theory]
    [InlineData(NmapXml)]
    [InlineData("{\"results\": []}")]
    [InlineData("Name;Template\nXSS;False\n")]
    public void CheckImport_AcceptsWhatCheckFileAccepts(string content)
    {
        Assert.True(check.CheckImport(Bytes(content)));
    }
}
