using iText.Html2pdf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace Cervantes.Web.Helpers;

public static class ReportPdf
{
    /// <summary>
    /// The HTML document the PDF cover is converted from: the report's stylesheets, then the cover page CSS (A4 with the
    /// body's 0.60in page margin, as in the DOCX, so covers without inner spacing keep off the edge of the paper; the
    /// body box is the area inside that margin), then the cover.
    /// </summary>
    public static string CoverDocument(string reportStylesheets, string coverHtml)
    {
        return @"<!DOCTYPE HTML>
<html lang='en' xmlns='http://www.w3.org/1999/xhtml'>
<head>
    <meta charset='utf-8'/>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title></title>
    " + reportStylesheets + @"
    <style>
        @page {
            size: A4;
            margin: 0.60in;
        }
        html, body {
            width: 179mm;
            height: 266mm;
            margin: 0;
            padding: 0;
        }
        body {
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }
        * {
            box-sizing: border-box;
        }
    </style>
</head>
<body>" + coverHtml + @"</body>
</html>";
    }

    /// <summary>
    /// Converts the cover to A4 pages. Every page is kept: a cover longer than one page continues on the next one, as
    /// it does in the DOCX, instead of losing its end. Loads nothing outside the stored HTML (inline data: images render).
    /// </summary>
    public static byte[] RenderCover(string reportStylesheets, string coverHtml)
    {
        using var stream = new MemoryStream();
        using (var pdf = new PdfDocument(new PdfWriter(stream)))
        {
            pdf.SetDefaultPageSize(PageSize.A4);
            var properties = new ConverterProperties().SetResourceRetriever(new OfflineResourceRetriever());
            HtmlConverter.ConvertToPdf(CoverDocument(reportStylesheets, coverHtml), pdf, properties);
        }
        return stream.ToArray();
    }
}
