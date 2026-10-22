using Scriban;

namespace Cervantes.Web.Helpers;

public static class ReportCss
{
    /// <summary>
    /// A report component's CSS as a &lt;style&gt; block for the generated report. Every "&lt;" becomes the CSS escape
    /// <c>\3C </c>, so the CSS cannot close the block and start markup. Blank CSS gives an empty string.
    /// </summary>
    public static string ToStyleBlock(string? css)
    {
        if (string.IsNullOrWhiteSpace(css))
        {
            return "";
        }

        return "<style>" + css.Replace("<", "\\3C ") + "</style>";
    }

    /// <summary>
    /// The style blocks of a report's components, each distinct CSS once, for the start of the report body. Kept out of
    /// &lt;header&gt; and &lt;footer&gt;, where the report editor does not allow &lt;style&gt; and drops it on the first save.
    /// </summary>
    public static string StyleBlocks(IEnumerable<string?> componentCss)
    {
        return string.Concat(componentCss.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().Select(ToStyleBlock));
    }

    /// <summary>The parse errors of a report template, each with its line and column.</summary>
    public static IReadOnlyList<string> TemplateErrors(Template template)
    {
        return template.HasErrors ? template.Messages.Select(m => m.ToString()).ToList() : Array.Empty<string>();
    }
}
