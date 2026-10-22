using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class CustomFieldRulesTests
{
    private static readonly Guid Severity = Guid.NewGuid();
    private static readonly Guid Owner = Guid.NewGuid();
    private static readonly Guid Retired = Guid.NewGuid();

    private static readonly (Guid Id, string Label, bool IsRequired, bool IsActive)[] Fields =
    {
        (Severity, "Severity override", true, true),
        (Owner, "Owner", false, true),
        (Retired, "Retired field", true, false),
    };

    [Fact]
    public void MissingRequired_ListsBlankRequired()
    {
        Assert.Equal(new[] { "Severity override" }, CustomFieldRules.MissingRequired(Fields, new Dictionary<Guid, string> { [Severity] = "  " }));
        Assert.Equal(new[] { "Severity override" }, CustomFieldRules.MissingRequired(Fields, new Dictionary<Guid, string> { [Owner] = "me" }));
    }

    [Fact]
    public void MissingRequired_IgnoresInactiveAndOptional()
    {
        Assert.Empty(CustomFieldRules.MissingRequired(Fields, new Dictionary<Guid, string> { [Severity] = "Raise" }));
    }

    [Fact]
    public void MissingRequired_NullValues()
    {
        Assert.Equal(new[] { "Severity override" }, CustomFieldRules.MissingRequired(Fields, null));
    }
}
