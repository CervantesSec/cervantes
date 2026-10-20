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

        var styles = Regex.Matches(head.Groups[1].Value, @"<style\b[^>]*>.*?</style>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        var body = Regex.Match(html[(head.Index + head.Length)..], @"<body\b[^>]*>", RegexOptions.IgnoreCase);
        if (styles.Count == 0 || !body.Success)
        {
            return html;
        }

        var headContent = Regex.Replace(head.Groups[1].Value, @"<style\b[^>]*>.*?</style>", "",RegexOptions.IgnoreCase | RegexOptions.Singleline);
        var bodyTagEnd = head.Index + head.Length + body.Index + body.Length;
        return html[..head.Groups[1].Index] + headContent + html[(head.Groups[1].Index + head.Groups[1].Length)..bodyTagEnd]
               + string.Concat(styles.Select(x => x.Value)) + html[bodyTagEnd..];
    }
}
