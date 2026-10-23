using Cervantes.Web.Helpers;
using Cervantes.Web.Localization;
using Microsoft.Extensions.Localization;

namespace Cervantes.Tests.Web.Helpers;

public class LocalizedIdentityErrorDescriberTests
{
    // Returns "T:<key>" so a test sees which resource key each description comes from.
    private sealed class KeyLocalizer : IStringLocalizer<Resource>
    {
        public LocalizedString this[string name] => new(name, "T:" + name);
        public LocalizedString this[string name, params object[] arguments] => this[name];
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
    }

    private readonly LocalizedIdentityErrorDescriber describer = new(new KeyLocalizer());

    [Fact]
    public void PasswordMismatch_IsLocalizedAndKeepsItsCode()
    {
        var error = describer.PasswordMismatch();

        Assert.Equal("T:passwordIncorrect", error.Description);
        Assert.Equal("PasswordMismatch", error.Code);
    }

    [Fact]
    public void PasswordRuleErrors_UseThePasswordRulesKeys()
    {
        Assert.Equal("T:passwordMinLength", describer.PasswordTooShort(8).Description);
        Assert.Equal("T:passwordUppercase", describer.PasswordRequiresUpper().Description);
        Assert.Equal("T:passwordLowercase", describer.PasswordRequiresLower().Description);
        Assert.Equal("T:passwordDigit", describer.PasswordRequiresDigit().Description);
        Assert.Equal("T:passwordSymbol", describer.PasswordRequiresNonAlphanumeric().Description);
    }
}
