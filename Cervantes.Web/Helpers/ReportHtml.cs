using System.Text.RegularExpressions;

namespace Cervantes.Web.Helpers;

public static class ReportHtml
{
    /// <summary>
    /// A stored report as a complete HTML document. Editing a report stores a body fragment (the editor and the
    /// sanitizer drop &lt;html&gt;, &lt;head&gt; and &lt;body&gt;), and both the PDF layout and a downloaded HTML file need the
    /// document around it, the charset included. A report that still has its &lt;body&gt; is returned unchanged.
    /// </summary>
    public static string EnsureDocument(string html)
    {
        if (Regex.IsMatch(html, @"<body[\s>]", RegexOptions.IgnoreCase))
        {
            return html;
        }

        return "<!DOCTYPE html><html><head><meta charset=\"utf-8\"></head><body>" + html + "</body></html>";
    }

    /// <summary>
    /// Page CSS of the PDF body: A4 with no side margin, the report's header and footer run into the top and bottom
    /// margins, and the body container keeps 0.60in at the sides. The header and footer get the same side padding, or
    /// they would reach the edge of the paper.
    /// </summary>
    public const string PdfPageCss = @"
      <style>
        @page {
            margin: 0.60in 0 0.60in 0;
            size: A4;
        }
        body {
            margin: 0;
            padding: 0;
        }
        table { page-break-inside: avoid; }
        #header {
            position: running(header);
            padding: 0 0.60in;
        }
        #footer {
            position: running(footer);
            padding: 0 0.60in;
        }
        #document-body-container {
            margin: 0.10in 0.60in 0.10in 0.60in;
        }
        @page {
            @top-center {
                content: element(header);
            }
            @bottom-center {
                content: element(footer);
            }
        }
        #current-page::before {
            content: counter(page);
        }
        #total-pages::before {
            content: counter(pages);
        }
        h1 { bookmark-level: 1; bookmark-state: open; }
        h2 { bookmark-level: 2; bookmark-state: open; }
        h3 { bookmark-level: 3; bookmark-state: open; }
      </style>";

    /// <summary>
    /// Every &lt;style&gt; of the report outside &lt;cover&gt;, in document order. The PDF export converts the cover as a
    /// document of its own, which needs these stylesheets (the component CSS among them) to look like the HTML report.
    /// </summary>
    public static string Stylesheets(string html)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);
        var styles = doc.DocumentNode.SelectNodes("//style[not(ancestor::cover)]");
        return styles == null ? string.Empty : string.Concat(styles.Select(s => s.OuterHtml));
    }

    /// <summary>
    /// The body the PDF export converts: <paramref name="pageCss"/> first, then the report's &lt;header&gt; and &lt;footer&gt;
    /// content as <c>div#header</c> and <c>div#footer</c> (the page CSS runs them into the page margins), then everything
    /// else in <c>div#document-body-container</c>. &lt;cover&gt; is dropped: the export renders it on its own page. Built on
    /// the parsed document, so header and footer markup with nested elements keeps the rest of the report in the body.
    /// </summary>
    public static string BuildPdfBody(string html, string pageCss)
    {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(EnsureDocument(html));
        var body = doc.DocumentNode.SelectSingleNode("//body");

        foreach (var cover in body.SelectNodes(".//cover")?.ToList() ?? new List<HtmlAgilityPack.HtmlNode>())
        {
            cover.Remove();
        }

        var header = Collect(doc, body, "header");
        var footer = Collect(doc, body, "footer");

        var container = doc.CreateElement("div");
        container.Id = "document-body-container";
        foreach (var child in body.ChildNodes.ToList())
        {
            child.Remove();
            container.AppendChild(child);
        }

        var css = new HtmlAgilityPack.HtmlDocument();
        css.LoadHtml(pageCss);
        foreach (var node in css.DocumentNode.ChildNodes.ToList())
        {
            body.AppendChild(node);
        }
        body.AppendChild(header);
        body.AppendChild(footer);
        body.AppendChild(container);
        return doc.DocumentNode.OuterHtml;
    }

    // Moves the content of every <tag> element into one div#tag and removes the elements.
    private static HtmlAgilityPack.HtmlNode Collect(HtmlAgilityPack.HtmlDocument doc, HtmlAgilityPack.HtmlNode body, string tag)
    {
        var target = doc.CreateElement("div");
        target.Id = tag;
        foreach (var element in body.SelectNodes(".//" + tag)?.ToList() ?? new List<HtmlAgilityPack.HtmlNode>())
        {
            foreach (var child in element.ChildNodes.ToList())
            {
                child.Remove();
                target.AppendChild(child);
            }
            element.Remove();
        }
        return target;
    }

    /// <summary>
    /// Moves the &lt;style&gt; blocks of &lt;head&gt; to the start of &lt;body&gt;, in order. The report editor (TinyMCE) keeps
    /// only the body of a full document, so a style left in the head is lost on the first save; one in the body
    /// survives the editor and the sanitizer. A fragment, or a document without head styles, is returned unchanged.
    /// </summary>
    public static string MoveHeadStylesToBody(string html)
    {
        var head = Regex.Match(html, @"<head\b[^>]*>(.*?)</head>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (!head.Success)
        {
            return html;
        }

        // Comments are matched too so a style inside one is skipped and the comment is kept as it is.
        const string commentOrStyle = @"<!--.*?-->|<style\b[^>]*>.*?</style>";
        var styles = Regex.Matches(head.Groups[1].Value, commentOrStyle, RegexOptions.IgnoreCase | RegexOptions.Singleline)
            .Where(x => !x.Value.StartsWith("<!--", StringComparison.Ordinal)).ToList();
        var body = Regex.Match(html[(head.Index + head.Length)..], @"<body\b[^>]*>", RegexOptions.IgnoreCase);
        if (styles.Count == 0 || !body.Success)
        {
            return html;
        }

        var headContent = Regex.Replace(head.Groups[1].Value, commentOrStyle,
            m => m.Value.StartsWith("<!--", StringComparison.Ordinal) ? m.Value : "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        var bodyTagEnd = head.Index + head.Length + body.Index + body.Length;
        return html[..head.Groups[1].Index] + headContent + html[(head.Groups[1].Index + head.Groups[1].Length)..bodyTagEnd]
               + string.Concat(styles.Select(x => x.Value)) + html[bodyTagEnd..];
    }
}
