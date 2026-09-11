using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Cervantes.Web.Controllers;

/// <summary>
/// Serves files stored under wwwroot/Attachments to authenticated users only.
/// Static file serving is disabled for /Attachments in Program.cs, so every stored
/// "Attachments/..." path keeps working but now goes through this controller.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]
public class FileController : Controller
{
    private readonly IWebHostEnvironment env;

    // Only passive content types are rendered inline. Everything else (html, svg, xml, ...)
    // is sent as an octet-stream download so the browser never executes it in our origin.
    private static readonly HashSet<string> InlineSafe = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp", ".ico", ".tif", ".tiff",
        ".heic", ".heif", ".avif", ".pdf",
        ".mp4", ".m4v", ".webm", ".mov", ".mkv"
    };

    private static readonly string[] PrivateFolders = { "Temp", "Imports" };

    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    public FileController(IWebHostEnvironment env)
    {
        this.env = env;
    }

    [HttpGet]
    [Route("Attachments/{**path}")]
    public IActionResult Get(string path)
    {
        var baseDir = Path.GetFullPath(Path.Combine(env.WebRootPath, "Attachments"));
        var full = Path.GetFullPath(Path.Combine(baseDir, path ?? string.Empty));

        // Block path traversal outside the Attachments folder.
        if (!full.StartsWith(baseDir + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            || !System.IO.File.Exists(full))
        {
            return NotFound();
        }

        // Internal working folders (scanner imports, backup restores) are never served.
        foreach (var folder in PrivateFolders)
        {
            if (full.StartsWith(Path.Combine(baseDir, folder) + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }
        }

        Response.Headers["X-Content-Type-Options"] = "nosniff";

        if (InlineSafe.Contains(Path.GetExtension(full)))
        {
            var contentType = ContentTypeProvider.TryGetContentType(full, out var mime)
                ? mime
                : "application/octet-stream";
            // Range requests let the browser seek inside video attachments.
            return PhysicalFile(full, contentType, enableRangeProcessing: true);
        }

        return PhysicalFile(full, "application/octet-stream", Path.GetFileName(full), enableRangeProcessing: true);
    }
}
