namespace Cervantes.Web.Helpers;

public static class UserAvatar
{
    // The avatar the first admin gets on install (DataInitializer); the file ships with the app.
    private const string DefaultAvatar = "Attachments/Users/logo.png";

    /// <summary>True for the shipped default avatar, whose file must never be deleted with a user's avatar.</summary>
    public static bool IsDefault(string? avatar)
    {
        return !string.IsNullOrWhiteSpace(avatar)
               && string.Equals(avatar.Trim().TrimStart('/'), DefaultAvatar, StringComparison.OrdinalIgnoreCase);
    }
}
