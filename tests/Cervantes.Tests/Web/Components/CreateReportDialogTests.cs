using Cervantes.Web.Components.Pages.Projects;

namespace Cervantes.Tests.Web.Components;

public class CreateReportDialogTests
{
    [Fact]
    public void TemplateError_ReadsTheFirstError()
    {
        IReadOnlyList<string> errors = new[] { "<input>(2,4) : error : unexpected", "second" };

        Assert.Equal("<input>(2,4) : error : unexpected", CreateReportDialog.TemplateError(new { errors }));
    }

    [Fact]
    public void TemplateError_OtherBodies()
    {
        Assert.Null(CreateReportDialog.TemplateError("Invalid request"));
        Assert.Null(CreateReportDialog.TemplateError(new { message = "x" }));
    }
}
