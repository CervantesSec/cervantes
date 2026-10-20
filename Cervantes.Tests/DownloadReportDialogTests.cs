using Cervantes.Web.Components.Pages.Projects;

namespace Cervantes.Tests;

public class DownloadReportDialogTests
{
    [Theory]
    [InlineData("text/html; charset=utf-8", "text/html")]
    [InlineData("text/html", "text/html")]
    [InlineData("application/pdf", "application/pdf")]
    [InlineData(" TEXT/HTML ;charset=utf-8", "text/html")]
    public void MediaType_DropsParameters(string contentType, string expected)
    {
        Assert.Equal(expected, DownloadReportDialog.MediaType(contentType));
    }
}
