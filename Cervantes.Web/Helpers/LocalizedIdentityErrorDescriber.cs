using Cervantes.Web.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Cervantes.Web.Helpers;

/// <summary>
/// Identity's password errors in the user's language, with the same keys as <see cref="PasswordRules"/>. The other
/// errors keep Identity's English text.
/// </summary>
internal sealed class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
    private readonly IStringLocalizer<Resource> localizer;

    public LocalizedIdentityErrorDescriber(IStringLocalizer<Resource> localizer)
    {
        this.localizer = localizer;
    }

    private IdentityError Localized(IdentityError error, string key) => new() { Code = error.Code, Description = localizer[key] };

    public override IdentityError PasswordMismatch() => Localized(base.PasswordMismatch(), "passwordIncorrect");

    public override IdentityError PasswordTooShort(int length) => Localized(base.PasswordTooShort(length), "passwordMinLength");

    public override IdentityError PasswordRequiresUpper() => Localized(base.PasswordRequiresUpper(), "passwordUppercase");

    public override IdentityError PasswordRequiresLower() => Localized(base.PasswordRequiresLower(), "passwordLowercase");

    public override IdentityError PasswordRequiresDigit() => Localized(base.PasswordRequiresDigit(), "passwordDigit");

    public override IdentityError PasswordRequiresNonAlphanumeric() => Localized(base.PasswordRequiresNonAlphanumeric(), "passwordSymbol");
}
