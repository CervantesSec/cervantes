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
}
