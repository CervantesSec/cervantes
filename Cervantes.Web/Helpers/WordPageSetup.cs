using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Cervantes.Web.Helpers;

/// <summary>
/// Page setup of the DOCX report, matching the PDF: A4, and 0.60in at the sides of the cover and the body (covers
/// without inner spacing keep off the edge of the paper). HtmlToOpenXml writes no page size, so without this Word falls
/// back to the default of its install (Letter on many).
/// </summary>
public static class WordPageSetup
{
    // Sizes in twentieths of a point (dxa): A4 is 210 x 297 mm; 0.60in = 864, 0.70in = 1008, 0.30in = 432.
    private const uint A4Width = 11906;
    private const uint A4Height = 16838;

    private static PageSize A4() => new() { Width = A4Width, Height = A4Height };

    /// <summary>The section break that ends the cover: next page, A4, 0.60in margins.</summary>
    public static SectionProperties CoverSection()
    {
        return new SectionProperties(
            new SectionType { Val = SectionMarkValues.NextPage },
            A4(),
            new PageMargin { Top = 864, Right = 864U, Bottom = 864, Left = 864U, Header = 432U, Footer = 432U, Gutter = 0U });
    }

    /// <summary>
    /// Gives the body's last section (created when missing) A4 and the body margins, after the header and footer
    /// references HtmlToOpenXml writes there, as the schema orders them.
    /// </summary>
    public static void ApplyBodyPage(Body body)
    {
        var section = body.Elements<SectionProperties>().LastOrDefault();
        if (section == null)
        {
            section = new SectionProperties();
            body.AppendChild(section);
        }

        section.RemoveAllChildren<PageSize>();
        section.RemoveAllChildren<PageMargin>();

        var margin = new PageMargin { Top = 1008, Right = 864U, Bottom = 1008, Left = 864U, Header = 432U, Footer = 432U, Gutter = 0U };
        OpenXmlElement? anchor = section.ChildElements.LastOrDefault(e =>
            e is HeaderReference or FooterReference or FootnoteProperties or EndnoteProperties or SectionType);
        if (anchor == null)
        {
            section.PrependChild(margin);
            section.PrependChild(A4());
        }
        else
        {
            anchor.InsertAfterSelf(margin);
            anchor.InsertAfterSelf(A4());
        }
    }
}
