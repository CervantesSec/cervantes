using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cervantes.Tests.Web.Controllers;

public class ProjectListGuardTests
{
    // The caller is a member of no project; anything but VerifyUser means the guard ran too late.
    private sealed class NoMembership : IProjectUserManager
    {
        public ProjectUser VerifyUser(Guid project, string user) => null!;
        public IApplicationDbContext Context => throw new InvalidOperationException("queried before the membership check");
        public ProjectUser Add(ProjectUser entity) => throw new NotSupportedException();
        public Task<ProjectUser> AddAsync(ProjectUser entity) => throw new NotSupportedException();
        public ProjectUser Update(ProjectUser entity) => throw new NotSupportedException();
        public IQueryable<ProjectUser> GetAll() => throw new NotSupportedException();
        public ProjectUser GetById(object[] key) => throw new NotSupportedException();
        public ProjectUser GetById(Guid id) => throw new NotSupportedException();
        public ProjectUser Remove(ProjectUser entity) => throw new NotSupportedException();
    }

    private static IHttpContextAccessor Accessor()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "caller-id") }, "Test");
        return new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) } };
    }

    private static ChecklistController Checklists() => new(null!, new NoMembership(), null!,
        NullLogger<ChecklistController>.Instance, null!, null!, null!, null!, null!, Accessor(), null!, null!, null!, null!,
        null!, null!, null!, null!);

    [Fact]
    public void VaultGetByProject_NonMember_Returns403()
    {
        var controller = new VaultController(null!, null!, NullLogger<VaultController>.Instance, Accessor(), new NoMembership(), null!);

        var result = controller.GetByProject(Guid.NewGuid());

        Assert.Equal(403, Assert.IsType<ObjectResult>(result.Result).StatusCode);
    }

    [Fact]
    public void GetWSTG_NonMember_Returns403()
    {
        var result = Checklists().GetWSTG(Guid.NewGuid());

        Assert.Equal(403, Assert.IsType<ObjectResult>(result.Result).StatusCode);
    }

    [Fact]
    public void GetMSTG_NonMember_Returns403()
    {
        var result = Checklists().GetMSTG(Guid.NewGuid());

        Assert.Equal(403, Assert.IsType<ObjectResult>(result.Result).StatusCode);
    }
}
