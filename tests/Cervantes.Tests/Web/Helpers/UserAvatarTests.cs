using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class UserAvatarTests
{
    [Fact]
    public void IsDefault_SeededLogo()
    {
        Assert.True(UserAvatar.IsDefault("Attachments/Users/logo.png"));
        Assert.True(UserAvatar.IsDefault("/Attachments/Users/LOGO.png"));
    }

    [Fact]
    public void IsDefault_UploadedAvatar()
    {
        Assert.False(UserAvatar.IsDefault("Attachments/Users/0b8a3c0e-1111-2222-3333-444455556666.png"));
    }

    [Fact]
    public void IsDefault_Null()
    {
        Assert.False(UserAvatar.IsDefault(null));
        Assert.False(UserAvatar.IsDefault(""));
    }
}
