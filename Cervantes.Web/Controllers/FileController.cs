using System.IO;
using Cervantes.Web.Helpers;
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

        // Everything but a PDF is served under CSP "sandbox" (the global OnStarting callback reads this flag; setting
        // the header here would be overwritten). PDFs are left out because Chrome does not render a PDF in a sandbox.
        var kind = AttachmentPreview.KindOfFile(full);
        if (kind != AttachmentPreviewKind.Pdf)
        {
            HttpContext.Items[SecurityHeaders.SandboxItem] = true;
        }

        // Only passive content types (see AttachmentPreview) are rendered inline. Everything else (html, svg, xml, ...)
        // is sent as an octet-stream download so the browser never executes it in our origin.
        if (kind != AttachmentPreviewKind.None)
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
