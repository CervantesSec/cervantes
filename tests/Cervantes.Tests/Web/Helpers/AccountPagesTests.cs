using Cervantes.Web.Helpers;
using Microsoft.AspNetCore.Http;

namespace Cervantes.Tests.Web.Helpers;

public class AccountPagesTests
{
    [Theory]
    [InlineData("/Account/Login")]
    [InlineData("/Account/LoginWith2fa")]
    [InlineData("/Account/Lockout")]
    [InlineData("/account/login")]
    public void UsesStaticRender_AccountPages(string path)
    {
        Assert.True(AccountPages.UsesStaticRender(new PathString(path)));
    }

    [Theory]
    [InlineData("/Account/Profile")]
    [InlineData("/Account/Profile/")]
    [InlineData("/account/profile/")]
    [InlineData("/Account/Profile/x")]
    public void UsesStaticRender_Profile(string path)
    {
        Assert.False(AccountPages.UsesStaticRender(new PathString(path)));
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/vulns")]
    [InlineData("/Accounts")]
    public void UsesStaticRender_OtherPaths(string path)
    {
        Assert.False(AccountPages.UsesStaticRender(new PathString(path)));
    }
}
