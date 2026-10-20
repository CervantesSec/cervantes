namespace Cervantes.Web.Helpers;

/// <summary>
/// What the attachment backup holds: the folders under wwwroot/Attachments that keep user files. Temp, Imports and
/// Export only hold short-lived files (uploads being processed, generated exports) and Vulns is an empty legacy folder,
/// so none of them is backed up or restored.
/// </summary>
public static class AttachmentArchive
{
    public static readonly string[] Folders = { "Clients", "Documents", "Organization", "Projects", "Reports", "Task", "Users", "Vuln" };

    /// <summary>
    /// Whether a zip entry may be written by an attachment restore: a file (not a directory) inside one of
    /// <see cref="Folders"/>, with a relative path and no "." or ".." segment. Backslashes count as separators.
    /// </summary>
    public static bool IsRestorable(string entryFullName)
    {
        var segments = entryFullName.Replace('\\', '/').Split('/');
        return segments.Length >= 2
               && Folders.Contains(segments[0], StringComparer.Ordinal)
               && segments.All(x => x.Length > 0 && x != "." && x != ".." && !x.Contains(':'));
    }
}
