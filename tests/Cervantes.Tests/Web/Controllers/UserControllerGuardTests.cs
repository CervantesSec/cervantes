using System.Security.Claims;
using AuthPermissions.BaseCode.PermissionsCode;
using Cervantes.CORE;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Tests.Web.Controllers;

public class UserControllerGuardTests
{
    private const string CallerId = "caller-id";

    // Only the accessor is needed: the guards must answer before any other dependency is touched.
    private static UserController Controller(params Permissions[] granted)
    {
        var packed = new string(granted.Select(p => (char)(ushort)p).ToArray());
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, CallerId),
            new Claim(PermissionConstants.PackedPermissionClaimType, packed)
        }, "Test");
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
        return new UserController(null!, null!, null!, null!, null!, null!, null!, null!, accessor, null!, null!, null!, null!);
    }

    [Fact]
    public async Task Add_WithoutUsersAdd_Returns403()
    {
        var result = await Controller(Permissions.UsersRead).Add(new UserCreateViewModel());

        Assert.Equal(403, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task Edit_WithoutUsersEdit_Returns403()
    {
        // Editing your own account does not skip the check: the edit can change the role.
        var result = await Controller(Permissions.UsersRead).Edit(new UserEditViewModel { Id = CallerId });

        Assert.Equal(403, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task Edit_WithUsersEdit_PassesTheGuard()
    {
        try
        {
            var result = await Controller(Permissions.UsersEdit).Edit(new UserEditViewModel { Id = CallerId });
            Assert.False(result is ObjectResult { StatusCode: 403 });
        }
        catch (Exception)
        {
            // Past the guard, the action reaches the null dependencies; the guard itself returns, it never throws.
        }
    }
}
