using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Cervantes.Web.Helpers;

public static class PasswordRules
{
    /// <summary>
    /// The password rules of the user forms (length 8, upper and lower case, a digit and one of !?*.), with messages
    /// from <paramref name="localizer"/>. Without one, the messages stay in English.
    /// </summary>
    public static IRuleBuilderOptions<T, string> Strong<T>(this IRuleBuilder<T, string> rule, IStringLocalizer? localizer)
    {
        return rule
            .MinimumLength(8).WithMessage(localizer?["passwordMinLength"] ?? "Your password length must be at least 8.")
            .Matches(@"[A-Z]+").WithMessage(localizer?["passwordUppercase"] ?? "Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage(localizer?["passwordLowercase"] ?? "Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage(localizer?["passwordDigit"] ?? "Your password must contain at least one number.")
            .Matches(@"[\!\?\*\.]+").WithMessage(localizer?["passwordSymbol"] ?? "Your password must contain at least one (!? *.).");
    }
}
