using AuthPermissions.BaseCode.PermissionsCode;
using Cervantes.CORE;

namespace Cervantes.Web.Helpers;

public static class PermissionExtensions
{
    /// <summary>
    /// True when the signed-in user holds <paramref name="permission"/>. Controllers call this inside an action:
    /// [HasPermission] is an MVC filter, so it does not run when a Blazor component calls the controller in-process.
    /// ControllerBase.User is not populated in-process either, hence the accessor.
    /// </summary>
    public static bool CurrentUserHas(this IHttpContextAccessor? accessor, Permissions permission) =>
        accessor?.HttpContext?.User.HasPermission(permission) == true;
}
