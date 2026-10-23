using Ganss.Xss;

namespace Cervantes.IFR.Parsers;

public static class ImportedTarget
{
    /// <summary>
    /// The name of a target an import creates or reuses. Report data is untrusted, so the name is sanitized like the
    /// ones users type; parsers look targets up by this value, so a re-import finds the target it created before.
    /// </summary>
    public static string Name(HtmlSanitizer sanitizer, string? raw)
    {
        return sanitizer.Sanitize(raw ?? "").Trim();
    }
}
