using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class AiMarkdownTests
{
    [Fact]
    public void ToSafeHtml_Bold()
    {
        Assert.Contains("<strong>high</strong>", AiMarkdown.ToSafeHtml("Overall risk is **high**."));
    }

    [Fact]
    public void ToSafeHtml_DropsRawHtml()
    {
        var html = AiMarkdown.ToSafeHtml("<script>alert(1)</script><img src=x onerror=alert(1)>");

        Assert.DoesNotContain("<script", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<img", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToSafeHtml_Null()
    {
        Assert.Equal("", AiMarkdown.ToSafeHtml(null));
    }
}
