using System.Security.Claims;
using AuthPermissions.BaseCode.PermissionsCode;
using Cervantes.CORE;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Tests.Web.Controllers;

public class TargetControllerGuardTests
{
    // Only the accessor is needed: the guards must answer before any other dependency is touched.
    private static TargetController Controller(params Permissions[] granted)
    {
        var packed = new string(granted.Select(p => (char)(ushort)p).ToArray());
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "caller-id"),
            new Claim(PermissionConstants.PackedPermissionClaimType, packed)
        }, "Test");
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
        return new TargetController(null!, null!, null!, null!, null!, null!, null!, null!, accessor, null!, null!, null!, null!);
    }

    [Fact]
    public async Task Edit_WithoutTargetsEdit_Returns403()
    {
        var result = await Controller(Permissions.TargetsRead).Edit(new TargetEditViewModel());

        Assert.Equal(403, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task Delete_WithoutTargetsDelete_Returns403()
    {
        var result = await Controller(Permissions.TargetsRead).Delete(Guid.NewGuid());

        Assert.Equal(403, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task EditService_WithoutTargetsServicesEdit_Returns403()
    {
        var result = await Controller(Permissions.TargetsRead).EditService(new TargetServiceEditViewModel());

        Assert.Equal(403, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Fact]
    public async Task UpdateCustomFieldValues_WithoutTargetCustomFieldsEdit_Returns403()
    {
        var result = await Controller(Permissions.TargetsRead).UpdateCustomFieldValues(Guid.NewGuid(), new Dictionary<Guid, string>());

        Assert.Equal(403, Assert.IsType<ObjectResult>(result).StatusCode);
    }
}
