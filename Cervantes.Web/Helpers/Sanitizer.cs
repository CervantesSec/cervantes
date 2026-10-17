using System.Text.RegularExpressions;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Cervantes.IFR;
using Ganss.Xss;

namespace Cervantes.Web.Helpers;

public class Sanitizer
{
    private readonly HtmlSanitizer _sanitizer;
    private readonly HtmlSanitizer _reportSanitizer;
    private static readonly Regex _htmlEntityRegex = new Regex(@"&(?:#\d+|#x[a-fA-F0-9]+|[a-zA-Z]+);", RegexOptions.Compiled);
    private static readonly Regex _htmlTagRegex = new Regex(@"</?[a-zA-Z]+(?:\s+[a-zA-Z]+(?:\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)*\s*/?>", RegexOptions.Compiled);

    public Sanitizer()
    {
        _sanitizer = Create();

        // Report.HtmlCode is rendered only inside a sandboxed iframe (ReportDialog.razor) and read server-side by the
        // DOCX/PDF export, which looks for <cover>, <header> and <footer>. Generated reports start with a <style> block.
        // HtmlSanitizer sanitizes the CSS of a <style> that AngleSharp parsed as a stylesheet (@import, url(javascript:),
        // behavior, expression are dropped). Do not add CssRuleType.Page: HtmlSanitizer serializes @page into invalid CSS.
        _reportSanitizer = Create();
        _reportSanitizer.AllowedTags.Add("style");
        _reportSanitizer.AllowedTags.Add("cover");
        _reportSanitizer.AllowedAtRules.Add(CssRuleType.Media);
        // HtmlSanitizer only sanitizes the stylesheets in the document's StyleSheets. A <style> outside that list keeps
        // its raw CSS (SVG/MathML, a type AngleSharp ignores such as type="", a disabled sheet), so its text is dropped.
        _reportSanitizer.PostProcessNode += (_, e) =>
        {
            if (e.Node is IElement { LocalName: "style" } style
                && !(style is IHtmlStyleElement { Sheet: { } sheet } && e.Document.StyleSheets.Contains(sheet)))
            {
                style.TextContent = string.Empty;
            }
        };
    }

    private static HtmlSanitizer Create()
    {
        var sanitizer = new HtmlSanitizer
        {
            KeepChildNodes = true
        };
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedAttributes.Add("tabindex");
        sanitizer.AllowedAttributes.Add("data-language");
        // "style" and the "data" scheme are allowed on purpose; removing either breaks existing features.
        //  - style: TinyMCE stores alignment, colour, font and table layout as inline styles, and the
        //    seeded report templates and the DOCX/PDF report pipeline read them (e.g. image width/height).
        //    HtmlSanitizer already drops url(javascript:), expression(), behavior and -moz-binding from them.
        //  - data: with no TinyMCE upload handler configured, pasted/inserted screenshots and the Word
        //    import (Mammoth) are stored as base64 data:image URIs, as are the logos in the seeded cover
        //    pages; the Jira export and report generation read them back from the stored HTML.
        sanitizer.AllowedAttributes.Add("style");
        // HtmlSanitizer does not check the media type of a data: URI, so the scheme is limited to inline
        // images (shared with the scanner import parsers).
        sanitizer.AllowDataImagesOnly();
        sanitizer.AllowedTags.Add("pre");
        sanitizer.AllowedTags.Add("code");

        // Allow data attributes
        foreach (var attr in new[] { "data-line", "data-language", "data-prismjs-copy", "data-src" })
        {
            sanitizer.AllowedAttributes.Add(attr);
        }

        // With KeepChildNodes a removed <script> or <style> would leave its code as visible text.
        sanitizer.RemovingTag += (_, e) =>
        {
            if (e.Tag.LocalName is "style" or "script")
            {
                e.Tag.InnerHtml = string.Empty;
            }
        };

        return sanitizer;
    }

    /// <summary>Like <see cref="Sanitize"/> but keeps style blocks and the cover element. Only for Report.HtmlCode.</summary>
    public string SanitizeReportHtml(string? html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        return _reportSanitizer.Sanitize(html).Trim();
    }

    public string Sanitize(string? html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        // Run the whole input through HtmlSanitizer. <pre> and <code> are kept
        // because they are in AllowedTags, so code blocks survive, but nothing is
        // exempted from sanitization: event-handler attributes (onerror, onload,
        // onmouseover, ...) are stripped even inside <pre>/<code>. Do NOT reintroduce
        // any "extract raw block then reinsert verbatim" logic here; that bypasses
        // the sanitizer and allows stored XSS.
        return _sanitizer.Sanitize(html).Trim();
    }

    private static bool IsHtmlEncoded(string input)
    {
        return !string.IsNullOrEmpty(input) && _htmlEntityRegex.IsMatch(input);
    }

    public static bool IsHtmlCode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (_htmlTagRegex.IsMatch(input))
            return true;

        if (_htmlEntityRegex.IsMatch(input))
            return true;

        if (input.Trim().StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
            return true;

        if (input.Contains("<!--") && input.Contains("-->"))
            return true;

        return false;
    }
    
    private string ProcessCodeSamples(string content)
    {
        var languages = new[] { "markup", "javascript", "css", "php", "ruby", "python", "java", "c", "csharp", "cpp" };

        foreach (var lang in languages)
        {
            var pattern = $@"<pre class=""language-{lang}"">";
            var replacement = $@"<pre class=""language-{lang}"" style=""{GetLanguageSpecificStyle(lang)}"" data-language=""{lang}"">";
            content = Regex.Replace(content, pattern, replacement);
        }

        return content;
    }

    private string GetLanguageSpecificStyle(string language)
    {
        var baseStyle = "background-color: #f4f4f4; color: #666; page-break-inside: avoid; font-family: monospace; font-size: 15px; line-height: 1.6; margin-bottom: 1.6em; max-width: 100%; overflow: auto; padding: 1em 1.5em; display: block; word-wrap: break-word; border: 1px solid #ddd; border-left: 3px solid {0};";
        
        return string.Format(baseStyle, language switch
        {
            "markup" => "#e34c26",  // HTML/XML: Orange (HTML5 color)
            "javascript" => "#f0db4f",  // JavaScript: Yellow
            "css" => "#264de4",  // CSS: Blue
            "php" => "#777bb3",  // PHP: Purple
            "ruby" => "#cc342d",  // Ruby: Red
            "python" => "#306998",  // Python: Blue
            "java" => "#5382a1",  // Java: Blue
            "c" => "#a8b9cc",  // C: Light Blue
            "csharp" => "#68217a",  // C#: Purple
            "cpp" => "#00599c",  // C++: Dark Blue
            _ => "#f36d33"  // Default: Orange
        });
    }
}