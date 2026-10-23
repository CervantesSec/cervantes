using Cervantes.Web.Helpers;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Cervantes.Tests.Web.Helpers;

public class WordPageSetupTests
{
    // A4 in twentieths of a point; without it Word falls back to its own default size (Letter on many installs).
    private const uint A4Width = 11906, A4Height = 16838;

    [Fact]
    public void CoverSection_IsA4WithTheBodySideMargins()
    {
        var section = WordPageSetup.CoverSection();

        Assert.Equal(SectionMarkValues.NextPage, section.GetFirstChild<SectionType>()!.Val!.Value);
        var size = section.GetFirstChild<PageSize>()!;
        Assert.Equal(A4Width, size.Width!.Value);
        Assert.Equal(A4Height, size.Height!.Value);
        var margin = section.GetFirstChild<PageMargin>()!;
        Assert.Equal(864, margin.Top!.Value);
        Assert.Equal(864u, margin.Left!.Value);
        Assert.Equal(864u, margin.Right!.Value);
        Assert.Equal(864, margin.Bottom!.Value);
    }

    // The body matches the PDF: 0.60in at the sides, room for the header and footer at the top and bottom.
    [Fact]
    public void ApplyBodyPage_AddsA4AndMarginsAfterTheHeaderAndFooterReferences()
    {
        var body = new Body(new Paragraph(), new SectionProperties(new HeaderReference { Id = "h" }, new FooterReference { Id = "f" }));

        WordPageSetup.ApplyBodyPage(body);

        var section = body.Elements<SectionProperties>().Single();
        var children = section.ChildElements.ToList();
        Assert.IsType<HeaderReference>(children[0]);
        Assert.IsType<FooterReference>(children[1]);
        Assert.IsType<PageSize>(children[2]);
        Assert.IsType<PageMargin>(children[3]);
        Assert.Equal(A4Width, ((PageSize)children[2]).Width!.Value);
        var margin = (PageMargin)children[3];
        Assert.Equal(864u, margin.Left!.Value);
        Assert.Equal(864u, margin.Right!.Value);
    }

    [Fact]
    public void ApplyBodyPage_CreatesTheSectionWhenTheBodyHasNone()
    {
        var body = new Body(new Paragraph());

        WordPageSetup.ApplyBodyPage(body);

        Assert.IsType<SectionProperties>(body.LastChild);
        Assert.Equal(A4Height, body.Elements<SectionProperties>().Single().GetFirstChild<PageSize>()!.Height!.Value);
    }

    [Fact]
    public void ApplyBodyPage_ReplacesAnExistingPageSetup()
    {
        var body = new Body(new SectionProperties(new PageSize { Width = 12240, Height = 15840 }, new PageMargin { Left = 1440 }));

        WordPageSetup.ApplyBodyPage(body);

        var section = body.Elements<SectionProperties>().Single();
        Assert.Single(section.Elements<PageSize>());
        Assert.Single(section.Elements<PageMargin>());
        Assert.Equal(A4Width, section.GetFirstChild<PageSize>()!.Width!.Value);
    }
}
