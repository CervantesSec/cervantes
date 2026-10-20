using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Helpers;

namespace Cervantes.Tests;

public class UserSecurityTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 20, 12, 0, 0, TimeSpan.Zero);
    private static readonly string[] UserRole = { "User" };

    private static ApplicationUser User() => new()
    {
        Id = "u1", Email = "qa@cervantes.local", FullName = "QA", Position = "Pentester", PhoneNumber = "1",
        Description = "d", TwoFactorEnabled = false, ExternalLogin = false, LockoutEnd = null
    };

    private static UserEditViewModel Model() => new()
    {
        Id = "u1", Email = "qa@cervantes.local", FullName = "QA", Position = "Pentester", PhoneNumber = "1",
        Description = "d", TwoFactorEnabled = false, ExternalLogin = false, Lockout = false, Role = ""
    };

    private static bool Changes(ApplicationUser user, UserEditViewModel model) => UserSecurity.ChangesSignIn(user, model, UserRole, Now);

    [Fact]
    public void ChangesSignIn_NothingSecurityRelated()
    {
        var model = Model();
        model.FullName = "Other";
        model.Position = "Lead";
        model.PhoneNumber = "2";
        model.Description = "other";

        Assert.False(Changes(User(), model));
    }

    [Fact]
    public void ChangesSignIn_EmailCaseOnly()
    {
        var user = User();
        user.Email = "A@x.local";
        var model = Model();
        model.Email = "a@x.local";

        Assert.False(Changes(user, model));
    }

    [Fact]
    public void ChangesSignIn_Email()
    {
        var model = Model();
        model.Email = "other@cervantes.local";

        Assert.True(Changes(User(), model));
    }

    [Fact]
    public void ChangesSignIn_Password()
    {
        var model = Model();
        model.Password = "N3w!password";

        Assert.True(Changes(User(), model));
    }

    [Fact]
    public void ChangesSignIn_TwoFactor()
    {
        var model = Model();
        model.TwoFactorEnabled = true;

        Assert.True(Changes(User(), model));
    }

    [Fact]
    public void ChangesSignIn_ExternalLogin()
    {
        var model = Model();
        model.ExternalLogin = true;

        Assert.True(Changes(User(), model));
    }

    [Fact]
    public void ChangesSignIn_Lock()
    {
        var model = Model();
        model.Lockout = true;

        Assert.True(Changes(User(), model));
    }

    [Fact]
    public void ChangesSignIn_Unlock()
    {
        var user = User();
        user.LockoutEnd = DateTimeOffset.MaxValue;

        Assert.True(Changes(user, Model()));
    }

    [Fact]
    public void ChangesSignIn_ExpiredLockIsUnlocked()
    {
        var user = User();
        user.LockoutEnd = Now.AddDays(-1);

        Assert.False(Changes(user, Model()));
    }

    [Fact]
    public void ChangesSignIn_SameRole()
    {
        var model = Model();
        model.Role = "User";

        Assert.False(Changes(User(), model));
    }

    [Fact]
    public void ChangesSignIn_NewRole()
    {
        var model = Model();
        model.Role = "Admin";

        Assert.True(Changes(User(), model));
    }

    [Fact]
    public void ChangesSignIn_EmptyRole()
    {
        var model = Model();
        model.Role = null;

        Assert.False(Changes(User(), model));
    }

    [Fact]
    public void IsLocked_ExpiredLockIsNotLocked()
    {
        var user = User();
        user.LockoutEnd = Now.AddMinutes(-1);

        Assert.False(UserSecurity.IsLocked(user, Now));
    }

    [Fact]
    public void IsLocked_FutureLockIsLocked()
    {
        var user = User();
        user.LockoutEnd = Now.AddMinutes(1);

        Assert.True(UserSecurity.IsLocked(user, Now));
    }

    [Fact]
    public void IsLocked_NoLock()
    {
        Assert.False(UserSecurity.IsLocked(User(), Now));
    }
}
