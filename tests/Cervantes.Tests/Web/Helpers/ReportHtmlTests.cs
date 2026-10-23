using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class ReportHtmlTests
{
    [Fact]
    public void EnsureDocument_WrapsAFragment()
    {
        var html = ReportHtml.EnsureDocument("<style>p{}</style><p>Informe</p>");

        Assert.StartsWith("<!DOCTYPE html>", html);
        Assert.Contains("<meta charset=\"utf-8\">", html);
        Assert.Contains("<body><style>p{}</style><p>Informe</p></body>", html);
    }

    [Fact]
    public void EnsureDocument_KeepsAFullDocument()
    {
        const string full = "<!DOCTYPE HTML><html><head><style>p{}</style></head><body><p>x</p></body></html>";
        Assert.Equal(full, ReportHtml.EnsureDocument(full));
    }

    [Fact]
    public void EnsureDocument_BodyWithAttributes()
    {
        const string full = "<html><body class=\"x\"><p>x</p></body></html>";
        Assert.Equal(full, ReportHtml.EnsureDocument(full));
    }

    [Fact]
    public void EnsureDocument_DoesNotMatchBodyLikeTags()
    {
        Assert.StartsWith("<!DOCTYPE html>", ReportHtml.EnsureDocument("<bodyx>x</bodyx>"));
    }
    [Fact]
    public void MoveHeadStylesToBody_MovesTheStyle()
    {
        Assert.Equal("<html><head><meta charset=\"utf-8\"></head><body><style>pre{}</style><p>x</p></body></html>",
            ReportHtml.MoveHeadStylesToBody("<html><head><meta charset=\"utf-8\"><style>pre{}</style></head><body><p>x</p></body></html>"));
    }

    [Fact]
    public void MoveHeadStylesToBody_KeepsOrder()
    {
        Assert.Equal("<html><head></head><body><style>a{}</style><style>b{}</style><p>x</p></body></html>",
            ReportHtml.MoveHeadStylesToBody("<html><head><style>a{}</style><style>b{}</style></head><body><p>x</p></body></html>"));
    }

    [Fact]
    public void MoveHeadStylesToBody_BodyWithAttributes()
    {
        Assert.Equal("<html><head></head><body class=\"r\"><style>a{}</style><p>x</p></body></html>",
            ReportHtml.MoveHeadStylesToBody("<html><head><style>a{}</style></head><body class=\"r\"><p>x</p></body></html>"));
    }

    [Fact]
    public void MoveHeadStylesToBody_FragmentUnchanged()
    {
        const string fragment = "<header>h</header><style>a{}</style><p>x</p>";
        Assert.Equal(fragment, ReportHtml.MoveHeadStylesToBody(fragment));
    }

    [Fact]
    public void MoveHeadStylesToBody_IgnoresStyleInComment()
    {
        Assert.Equal(
            "<html><head><!-- <style>.x{}</style> --></head><body><style>.y{}</style><p>x</p></body></html>",
            ReportHtml.MoveHeadStylesToBody("<html><head><!-- <style>.x{}</style> --><style>.y{}</style></head><body><p>x</p></body></html>"));
    }

    [Fact]
    public void MoveHeadStylesToBody_NoHeadStyleUnchanged()
    {
        const string html = "<html><head><title>t</title></head><body><style>a{}</style><p>x</p></body></html>";
        Assert.Equal(html, ReportHtml.MoveHeadStylesToBody(html));
    }

    private const string PageCss = "<style>#header { position: running(header); }</style>";

    private static HtmlAgilityPack.HtmlNode Body(string html)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        return doc.DocumentNode.SelectSingleNode("//body");
    }

    [Fact]
    public void BuildPdfBody_NestedDivsStayInside()
    {
        const string report = "<html><head></head><body><header><div class=\"hdr\"><div>a</div></div></header>"
                              + "<cover><p>cover</p></cover><h1>Document Control</h1><p>x</p>"
                              + "<footer><div class=\"ftr\">b</div></footer></body></html>";

        var body = Body(ReportHtml.BuildPdfBody(report, PageCss));
        var children = body.ChildNodes.Where(n => n.NodeType == HtmlAgilityPack.HtmlNodeType.Element).ToList();

        Assert.Equal(new[] { "style", "div", "div", "div" }, children.Select(n => n.Name));
        Assert.Equal(new[] { "header", "footer", "document-body-container" }, children.Skip(1).Select(n => n.Id));
        Assert.NotNull(children[1].SelectSingleNode(".//div[@class='hdr']"));
        Assert.NotNull(children[2].SelectSingleNode(".//div[@class='ftr']"));
        Assert.NotNull(children[3].SelectSingleNode("./h1"));
        Assert.Null(body.SelectSingleNode("//cover"));
    }

    [Fact]
    public void BuildPdfBody_WithoutBodyTag()
    {
        var body = Body(ReportHtml.BuildPdfBody("<header><p>h</p></header><p>edited</p><footer><p>f</p></footer>", PageCss));

        Assert.Equal("edited", body.SelectSingleNode("./div[@id='document-body-container']/p").InnerText);
        Assert.NotNull(body.SelectSingleNode("./div[@id='header']"));
    }

    [Fact]
    public void BuildPdfBody_NoHeaderOrFooter()
    {
        var body = Body(ReportHtml.BuildPdfBody("<html><body><p>only</p></body></html>", PageCss));

        Assert.Equal("only", body.SelectSingleNode("./div[@id='document-body-container']/p").InnerText);
        Assert.Equal("style", body.ChildNodes.First(n => n.NodeType == HtmlAgilityPack.HtmlNodeType.Element).Name);
    }

    // The PDF cover is converted on its own, so it needs the report's stylesheets (component CSS included).
    [Fact]
    public void Stylesheets_CollectsEveryStyleInOrder()
    {
        const string html = "<html><head><style>.a{}</style></head><body><style>.b{}</style><header>h</header>" +
                            "<cover><div class=\"cover\">c</div></cover><p>x</p><style>.c{}</style></body></html>";

        Assert.Equal("<style>.a{}</style><style>.b{}</style><style>.c{}</style>", ReportHtml.Stylesheets(html));
    }

    // A style inside the cover already travels with the cover's own HTML.
    [Fact]
    public void Stylesheets_SkipsStylesInsideTheCover()
    {
        const string html = "<html><body><style>.b{}</style><cover><style>.in{}</style><div>c</div></cover></body></html>";

        Assert.Equal("<style>.b{}</style>", ReportHtml.Stylesheets(html));
    }

    [Fact]
    public void Stylesheets_NoneIsEmpty()
    {
        Assert.Equal("", ReportHtml.Stylesheets("<p>no styles</p>"));
    }

    // The PDF page has no side margin, so the running header and footer would touch the paper edge; they take the same
    // side padding as the body container.
    [Fact]
    public void PdfPageCss_HeaderAndFooterAlignWithTheBody()
    {
        var css = ReportHtml.PdfPageCss;

        Assert.Matches(@"#document-body-container\s*\{[^}]*margin:\s*0\.10in 0\.60in 0\.10in 0\.60in", css);
        Assert.Matches(@"#header\s*\{[^}]*position:\s*running\(header\);[^}]*padding:\s*0 0\.60in", css);
        Assert.Matches(@"#footer\s*\{[^}]*position:\s*running\(footer\);[^}]*padding:\s*0 0\.60in", css);
    }

    // {{PageBreak}} used page-break-after, which HtmlToOpenXml ignores: Word reports had no page breaks at all.
    [Fact]
    public async System.Threading.Tasks.Task PageBreak_BreaksTheDocxPage()
    {
        using var stream = new MemoryStream();
        using var docx = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
        var main = docx.AddMainDocumentPart();
        new DocumentFormat.OpenXml.Wordprocessing.Document(new DocumentFormat.OpenXml.Wordprocessing.Body()).Save(main);

        await new HtmlToOpenXml.HtmlConverter(main).ParseBody("<p>one</p>" + ReportHtml.PageBreak + "<p>two</p>");

        Assert.Single(main.Document.Body!.Descendants<DocumentFormat.OpenXml.Wordprocessing.Break>(),
            b => b.Type?.Value == DocumentFormat.OpenXml.Wordprocessing.BreakValues.Page);
    }

    [Fact]
    public void PageBreak_BreaksThePdfPage()
    {
        using var stream = new MemoryStream();
        iText.Html2pdf.HtmlConverter.ConvertToPdf("<html><body><p>one</p>" + ReportHtml.PageBreak + "<p>two</p></body></html>", stream);

        using var pdf = new iText.Kernel.Pdf.PdfDocument(new iText.Kernel.Pdf.PdfReader(new MemoryStream(stream.ToArray())));
        Assert.Equal(2, pdf.GetNumberOfPages());
    }
}
