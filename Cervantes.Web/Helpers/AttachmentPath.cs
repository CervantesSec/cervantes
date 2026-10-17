namespace Cervantes.Web.Helpers;

public static class AttachmentPath
{
    /// <summary>
    /// Resolves a stored attachment path ("Attachments/{folder}/...", relative to the web root) to a full path, only
    /// when it points to a file inside wwwroot/Attachments/{folder}. Same check as <see cref="Controllers.FileController"/>.
    /// </summary>
    public static bool TryResolve(string webRootPath, string? storedPath, string folder, out string fullPath)
    {
        fullPath = "";
        if (string.IsNullOrWhiteSpace(storedPath))
        {
            return false;
        }

        try
        {
            var baseDir = Path.GetFullPath(Path.Combine(webRootPath, "Attachments", folder));
            var candidate = Path.GetFullPath(Path.Combine(webRootPath, storedPath));

            if (!candidate.StartsWith(baseDir + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                || Path.GetFileName(candidate).Length == 0)
            {
                return false;
            }

            fullPath = candidate;
            return true;
        }
        catch (Exception e) when (e is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return false;
        }
    }
}
