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
