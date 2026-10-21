using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class ImagePathsTests
{
    [Fact]
    public void IsSet_RejectsTheNoLogoMarker()
    {
        Assert.False(ImagePaths.IsSet(null));
        Assert.False(ImagePaths.IsSet(" "));
        Assert.False(ImagePaths.IsSet("None"));
        Assert.True(ImagePaths.IsSet("Attachments/Clients/x.png"));
    }
}
