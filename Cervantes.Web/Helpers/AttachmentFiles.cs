using Microsoft.Extensions.Logging;

namespace Cervantes.Web.Helpers;

public static class AttachmentFiles
{
    /// <summary>
    /// Removes an attachment's file once its row is gone. Only a file inside the owner's attachments folder is
    /// deleted, because an old restore may have stored a path that points elsewhere, another owner's file included.
    /// A file that cannot be deleted is left behind: the attachment is already gone for the user, and a stray file
    /// only takes disk space.
    /// </summary>
    public static void DeleteFile(string webRootPath, string? storedPath, string folder, Guid ownerId, ILogger logger,
        string entity, Guid id, string userId)
    {
        if (!AttachmentPath.TryResolve(webRootPath, storedPath, AttachmentPath.OwnerFolder(folder, ownerId), out var path))
        {
            logger.LogWarning("Stored file of {Entity} {Id} is outside its attachments folder, file not deleted. User: {UserId}",
                entity, id, userId);
            return;
        }

        try
        {
            // False for a directory as well, so a stored path that names a folder deletes nothing.
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(e, "File of {Entity} {Id} could not be deleted. User: {UserId}", entity, id, userId);
        }
    }
}
