using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class AppVersionTests
{
    [Fact]
    public void Clean_DropsTheSourceRevision()
    {
        Assert.Equal("1.4.1", AppVersion.Clean("1.4.1+82ea7b1a28fe5201d4c0dce341c6ed6e8c261790"));
        Assert.Equal("1.4.1", AppVersion.Clean("1.4.1"));
    }

    [Fact]
    public void Display_IsTheWebAssemblyVersion()
    {
        Assert.Equal("1.4.1", AppVersion.Display);
    }
}
