using Cervantes.Web.Helpers;
using Scriban;

namespace Cervantes.Tests.Web.Helpers;

public class ReportCssTests
{
    [Fact]
    public void ToStyleBlock_EscapesClosingTag()
    {
        var block = ReportCss.ToStyleBlock("a{}</style><script>x</script>");

        Assert.StartsWith("<style>", block);
        Assert.EndsWith("</style>", block);
        Assert.Equal(block.Length - "</style>".Length, block.IndexOf("</style>", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain("<script", block, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToStyleBlock_KeepsCss()
    {
        Assert.Equal("<style>.badge.risk-High{background:#dc2626}</style>", ReportCss.ToStyleBlock(".badge.risk-High{background:#dc2626}"));
    }

    [Fact]
    public void ToStyleBlock_Blank()
    {
        Assert.Equal("", ReportCss.ToStyleBlock(null));
        Assert.Equal("", ReportCss.ToStyleBlock("  "));
    }

    [Fact]
    public void TemplateErrors_ReportsLine()
    {
        var errors = ReportCss.TemplateErrors(Template.Parse("<p>\n{{ for x in }}\n</p>"));

        Assert.NotEmpty(errors);
        Assert.Contains("(2,", errors[0]);
    }

    [Fact]
    public void StyleBlocks_OncePerDistinctCss()
    {
        var blocks = ReportCss.StyleBlocks(new[] { ".hdr{}", null, ".cover{}", ".hdr{}", " " });

        Assert.Equal("<style>.hdr{}</style><style>.cover{}</style>", blocks);
    }
}
