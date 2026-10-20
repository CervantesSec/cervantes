using Cervantes.CORE.ViewModels;
using Cervantes.Web.Components.Pages.Admin.Users;

namespace Cervantes.Tests;

public class UserSearchTests
{
    private static readonly UserViewModel Admin = new() { FullName = "Admin", Email = "admin@cervantes.local" };

    [Fact]
    public void MatchesSearch_Email()
    {
        Assert.True(Users.MatchesSearch(Admin, "ADMIN@cervantes"));
    }

    [Fact]
    public void MatchesSearch_Name()
    {
        Assert.True(Users.MatchesSearch(Admin, "adm"));
        Assert.False(Users.MatchesSearch(Admin, "someone else"));
    }

    [Fact]
    public void MatchesSearch_Blank()
    {
        Assert.True(Users.MatchesSearch(Admin, null));
        Assert.True(Users.MatchesSearch(Admin, "  "));
    }

    [Fact]
    public void MatchesSearch_NullFields()
    {
        Assert.False(Users.MatchesSearch(new UserViewModel(), "x"));
    }
}
