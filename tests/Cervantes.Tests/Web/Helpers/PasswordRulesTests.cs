using Cervantes.Web.Helpers;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Cervantes.Tests.Web.Helpers;

public class PasswordRulesTests
{
    // Returns "T:<key>" so a test sees which resource key each message comes from.
    private sealed class KeyLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name] => new(name, "T:" + name);
        public LocalizedString this[string name, params object[] arguments] => this[name];
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
    }

    private sealed class Model
    {
        public string Password { get; set; } = "";
    }

    private sealed class Validator : AbstractValidator<Model>
    {
        public Validator(IStringLocalizer? localizer) => RuleFor(x => x.Password).Strong(localizer);
    }

    [Fact]
    public void Strong_UsesLocalizerMessages()
    {
        var errors = new Validator(new KeyLocalizer()).Validate(new Model { Password = "abc" }).Errors.Select(e => e.ErrorMessage).ToList();

        Assert.Equal(new[] { "T:passwordMinLength", "T:passwordUppercase", "T:passwordDigit", "T:passwordSymbol" }, errors);
    }

    [Fact]
    public void Strong_WithoutLocalizerKeepsEnglish()
    {
        var errors = new Validator(null).Validate(new Model { Password = "ABCDEFGH1!" }).Errors.Select(e => e.ErrorMessage);

        Assert.Equal("Your password must contain at least one lowercase letter.", Assert.Single(errors));
    }

    [Fact]
    public void Strong_AcceptsAStrongPassword()
    {
        Assert.True(new Validator(null).Validate(new Model { Password = "Abcdefg1!" }).IsValid);
    }
}
