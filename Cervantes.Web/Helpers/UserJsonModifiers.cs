using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Identity;

namespace Cervantes.Web.Helpers;

/// <summary>
/// JSON contract changes for API responses. Only MVC uses them: backups serialize users with their own
/// options and keep these fields, because a restore needs the password hashes.
/// </summary>
public static class UserJsonModifiers
{
    // Credential material inherited from IdentityUser, and the user's API key rows (they carry key hashes).
    private static readonly HashSet<string> HiddenMembers = new(StringComparer.Ordinal)
    {
        nameof(IdentityUser.PasswordHash),
        nameof(IdentityUser.SecurityStamp),
        nameof(IdentityUser.ConcurrencyStamp),
        nameof(CORE.Entities.ApplicationUser.ApiKeys),
    };

    /// <summary>Removes credential fields from the JSON contract of every IdentityUser type.</summary>
    public static void HideUserSecrets(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object || !typeof(IdentityUser).IsAssignableFrom(typeInfo.Type))
        {
            return;
        }

        for (var i = typeInfo.Properties.Count - 1; i >= 0; i--)
        {
            // Match on the C# member name: the JSON name is camelCase under the web defaults.
            if (typeInfo.Properties[i].AttributeProvider is MemberInfo member && HiddenMembers.Contains(member.Name))
            {
                typeInfo.Properties.RemoveAt(i);
            }
        }
    }
}
