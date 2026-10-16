using System.Text.RegularExpressions;
using Ganss.Xss;

namespace Cervantes.IFR;

public static class HtmlSanitizerExtensions
{
    // An inline image data URI: data:image/<subtype>;... or data:image/<subtype>,...
    private static readonly Regex DataImageUri = new(@"^data:image/[a-z0-9.+-]+[;,]",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// Allows the data: scheme for inline images only. HtmlSanitizer does not check the media type of a
    /// data: URI, so allowing the scheme alone also lets data:text/html and data:application/* through.
    /// </summary>
    public static HtmlSanitizer AllowDataImagesOnly(this HtmlSanitizer sanitizer)
    {
        sanitizer.AllowedSchemes.Add("data");
        sanitizer.FilterUrl += (_, e) =>
        {
            var url = e.SanitizedUrl?.TrimStart();
            if (url != null && url.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && !DataImageUri.IsMatch(url))
                e.SanitizedUrl = null;
        };
        return sanitizer;
    }
}
