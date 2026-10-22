using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Helpers;

public static class UploadResult
{
    // The body the upload actions answer when FileCheck rejects the file's type.
    public const string InvalidFileType = "Invalid file type";

    /// <summary>True when an upload action refused the file because of its type, so the dialog can say so.</summary>
    public static bool IsRejectedFileType(IActionResult? result)
    {
        return result is BadRequestObjectResult { Value: string body } && body == InvalidFileType;
    }
}
