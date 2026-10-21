namespace Cervantes.Web.Helpers;

public static class ImagePaths
{
    /// <summary>
    /// True when a stored logo path points at a file. Clients without a logo store "None" (ClientsController,
    /// BackupController restore), and older rows can hold null or an empty string; rendering any of them as an image
    /// source requests a URL that does not exist.
    /// </summary>
    public static bool IsSet(string? path) => !string.IsNullOrWhiteSpace(path) && path != "None";
}
