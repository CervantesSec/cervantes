namespace Cervantes.Web.Helpers;

/// <summary>
/// Resolves the user ids of a data backup on the instance being restored. A backup user whose email (or user name)
/// already exists there under another id is the same person, so their rows are mapped to that user; an id that exists
/// is kept; anything else falls back to the restoring user, or to null for optional references.
/// </summary>
public sealed class BackupUserMap(string fallbackUserId, Func<string, Task<bool>> userExists)
{
    private readonly Dictionary<string, string> map = new();

    public void Map(string backupUserId, string existingUserId) => map[backupUserId] = existingUserId;

    public async Task<string> ResolveAsync(string? userId) => await ResolveOptionalAsync(userId) ?? fallbackUserId;

    public async Task<string?> ResolveOptionalAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        if (map.TryGetValue(userId, out var mapped))
        {
            return mapped;
        }

        return await userExists(userId) ? userId : null;
    }
}
