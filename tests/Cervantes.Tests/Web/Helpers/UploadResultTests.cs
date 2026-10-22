using Cervantes.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Tests.Web.Helpers;

public class UploadResultTests
{
    [Fact]
    public void IsRejectedFileType_InvalidFileTypeBadRequest()
    {
        Assert.True(UploadResult.IsRejectedFileType(new BadRequestObjectResult("Invalid file type")));
    }

    [Fact]
    public void IsRejectedFileType_OtherResults()
    {
        Assert.False(UploadResult.IsRejectedFileType(new BadRequestObjectResult("Invalid request")));
        Assert.False(UploadResult.IsRejectedFileType(new CreatedResult("/x", null)));
        Assert.False(UploadResult.IsRejectedFileType(null));
    }
}
