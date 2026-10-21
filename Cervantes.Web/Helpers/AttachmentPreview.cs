using System.IO;

namespace Cervantes.Web.Helpers;

public enum AttachmentPreviewKind { None, Image, Pdf, Video }

/// <summary>
/// Decides how an attachment is previewed. Only passive content types are previewed or rendered inline
/// (FileController uses the same rule); everything else (html, svg, xml, ...) is a download only.
/// </summary>
public static class AttachmentPreview
{
    private static readonly HashSet<string> Images = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp", ".ico", ".tif", ".tiff", ".heic", ".heif", ".avif"
    };

    private static readonly HashSet<string> Videos = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".m4v", ".webm", ".mov", ".mkv"
    };

    public static AttachmentPreviewKind KindOf(string? path)
    {
        if (string.IsNullOrEmpty(path)) return AttachmentPreviewKind.None;

        var query = path.IndexOf('?');
        if (query >= 0) path = path[..query];

        var ext = Path.GetExtension(path);
        if (Images.Contains(ext)) return AttachmentPreviewKind.Image;
        if (string.Equals(ext, ".pdf", StringComparison.OrdinalIgnoreCase)) return AttachmentPreviewKind.Pdf;
        if (Videos.Contains(ext)) return AttachmentPreviewKind.Video;
        return AttachmentPreviewKind.None;
    }
}
