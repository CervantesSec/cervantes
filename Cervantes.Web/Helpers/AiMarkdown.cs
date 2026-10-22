using Markdig;

namespace Cervantes.Web.Helpers;

public static class AiMarkdown
{
    // AI replies are Markdown. HTML the model writes is shown as text (DisableHtml), and the generated HTML is
    // sanitized because Markdown links can still carry javascript: URLs.
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().DisableHtml().Build();
    private static readonly Lazy<Sanitizer> HtmlSanitizer = new(() => new Sanitizer());

    /// <summary>AI Markdown as sanitized HTML, ready for a rich-text field or a MarkupString. Null gives "".</summary>
    public static string ToSafeHtml(string? markdown)
    {
        return string.IsNullOrEmpty(markdown) ? "" : HtmlSanitizer.Value.Sanitize(Markdown.ToHtml(markdown, Pipeline));
    }
}
