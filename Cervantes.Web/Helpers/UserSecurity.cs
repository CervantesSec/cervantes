using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;

namespace Cervantes.Web.Helpers;

public static class UserSecurity
{
    /// <summary>
    /// Whether an admin edit changes how the user signs in: email, password, two-factor, external login, lock state or
    /// role. Only those edits change the security stamp, because a new stamp signs the user out of every session within
    /// the validation interval; a profile edit (name, position, avatar) must not.
    /// </summary>
    public static bool ChangesSignIn(ApplicationUser user, UserEditViewModel model, IReadOnlyCollection<string> currentRoles,
        DateTimeOffset now)
    {
        var locked = IsLocked(user, now);
        return !string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase)
               || !string.IsNullOrWhiteSpace(model.Password)
               || user.TwoFactorEnabled != model.TwoFactorEnabled
               || user.ExternalLogin != model.ExternalLogin
               || locked != model.Lockout
               || (!string.IsNullOrEmpty(model.Role) && !currentRoles.Contains(model.Role));
    }

    /// <summary>
    /// Whether the user is locked now. A lock from failed logins leaves its past end date behind, so a set LockoutEnd
    /// alone does not mean locked.
    /// </summary>
    public static bool IsLocked(ApplicationUser user, DateTimeOffset now) => user.LockoutEnd != null && user.LockoutEnd > now;
}
