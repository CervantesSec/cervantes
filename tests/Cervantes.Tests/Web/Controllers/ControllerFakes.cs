using System.Security.Claims;
using AuthPermissions.BaseCode.PermissionsCode;
using Cervantes.Contracts;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Http;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Tests.Web.Controllers;

// In-memory managers for controller tests that must answer before anything is saved: Context throws, so a test
// fails if the action reaches the database.
internal class ListManager<T> : IGenericManager<T> where T : class
{
    public List<T> Items { get; } = new();
    public IApplicationDbContext Context => throw new InvalidOperationException("the action reached the database");
    public T Add(T entity) => throw new NotSupportedException();
    public Task<T> AddAsync(T entity) => throw new NotSupportedException();
    public T Update(T entity) => throw new NotSupportedException();
    public IQueryable<T> GetAll() => Items.AsQueryable();
    public T GetById(object[] key) => throw new NotSupportedException();
    public T GetById(Guid id) => Items.FirstOrDefault(x => (Guid)typeof(T).GetProperty("Id")!.GetValue(x)! == id)!;
    public T Remove(T entity) => throw new NotSupportedException();
}

internal sealed class ProjectFieldManager : ListManager<ProjectCustomField>, IProjectCustomFieldManager { }
internal sealed class TargetFieldManager : ListManager<TargetCustomField>, ITargetCustomFieldManager { }
internal sealed class ClientFieldManager : ListManager<ClientCustomField>, IClientCustomFieldManager { }
internal sealed class VulnFieldManager : ListManager<VulnCustomField>, IVulnCustomFieldManager { }
internal sealed class VulnListManager : ListManager<Vuln>, IVulnManager { }
internal sealed class VulnValueManager : ListManager<VulnCustomFieldValue>, IVulnCustomFieldValueManager { }

internal sealed class TargetListManager : ListManager<Target>, ITargetManager
{
    public Target GetByName(string name, Guid? projectId) => throw new NotSupportedException();
}

// The caller is a member of every project.
internal sealed class EveryMembership : ListManager<ProjectUser>, IProjectUserManager
{
    public ProjectUser VerifyUser(Guid project, string user) => new() { ProjectId = project, UserId = user };
}

internal static class ControllerFakes
{
    public static IHttpContextAccessor Accessor(params Permissions[] granted)
    {
        var packed = new string(granted.Select(p => (char)(ushort)p).ToArray());
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "caller-id"),
            new Claim(PermissionConstants.PackedPermissionClaimType, packed)
        }, "Test");
        return new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
    }
}
