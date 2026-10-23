using Cervantes.Web.Helpers;
using iText.Kernel.Pdf;

namespace Cervantes.Tests.Web.Helpers;

public class ReportPdfTests
{
    private static PdfDocument Read(byte[] pdf) => new(new PdfReader(new MemoryStream(pdf)));

    [Fact]
    public void RenderCover_ShortCoverIsOneA4Page()
    {
        using var pdf = Read(ReportPdf.RenderCover("", "<h1>Web App Pentest</h1>"));

        Assert.Equal(1, pdf.GetNumberOfPages());
        var size = pdf.GetPage(1).GetPageSize();
        Assert.Equal(595, Math.Round(size.GetWidth()));
        Assert.Equal(842, Math.Round(size.GetHeight()));
    }

    // Only the first page used to be copied, so a cover longer than one page lost its end without a word.
    [Fact]
    public void RenderCover_KeepsEveryPageOfALongCover()
    {
        var cover = string.Concat(Enumerable.Range(0, 120).Select(i => $"<p>Line {i}</p>"));

        using var pdf = Read(ReportPdf.RenderCover("", cover));

        Assert.True(pdf.GetNumberOfPages() > 1);
        Assert.All(Enumerable.Range(1, pdf.GetNumberOfPages()), n => Assert.Equal(842, Math.Round(pdf.GetPage(n).GetPageSize().GetHeight())));
    }

    // The cover has the body's 0.60in page margin, so covers without inner spacing (the seeded ones) keep off the edge.
    [Fact]
    public void CoverDocument_HasTheBodyPageMarginAndCarriesTheReportStyles()
    {
        var html = ReportPdf.CoverDocument("<style>.band { color: red; }</style>", "<div class=\"band\">x</div>");

        Assert.Matches(@"@page\s*\{[^}]*margin:\s*0\.60in;", html);
        Assert.Contains("<style>.band { color: red; }</style>", html);
        Assert.Contains("<div class=\"band\">x</div>", html);
    }
}
