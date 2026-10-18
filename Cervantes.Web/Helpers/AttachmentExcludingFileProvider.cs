using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Cervantes.Web.Helpers;

/// <summary>
/// Web root file provider for the static file middleware that hides everything inside wwwroot/Attachments, so stored
/// files are only delivered by the authenticated <see cref="Controllers.FileController"/>. The check runs on the
/// physical path the request resolves to, not on the URL: the provider normalizes extra slashes, backslashes, "..",
/// trailing dots and 8.3 short names, so a URL check cannot see every way of reaching the folder.
/// </summary>
public sealed class AttachmentExcludingFileProvider : IFileProvider
{
    private readonly IFileProvider inner;
    private readonly string attachmentsDir;

    public AttachmentExcludingFileProvider(IFileProvider inner, string webRootPath)
    {
        this.inner = inner;
        attachmentsDir = Path.GetFullPath(Path.Combine(webRootPath, "Attachments"));
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        var info = inner.GetFileInfo(subpath);
        // Ignore case on every OS: a miss here serves a raw attachment, so the comparison errs toward refusing.
        if (info.Exists && info.PhysicalPath is { } physicalPath &&
            Path.GetFullPath(physicalPath).StartsWith(attachmentsDir + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase))
        {
            return new NotFoundFileInfo(subpath);
        }

        return info;
    }

    // The static file middleware never lists directories (no UseDirectoryBrowser or UseDefaultFiles).
    public IDirectoryContents GetDirectoryContents(string subpath) => inner.GetDirectoryContents(subpath);

    public IChangeToken Watch(string filter) => inner.Watch(filter);
}
