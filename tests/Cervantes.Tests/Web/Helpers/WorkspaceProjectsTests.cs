using Cervantes.CORE.Entities;
using Cervantes.Web.Helpers;

namespace Cervantes.Tests.Web.Helpers;

public class WorkspaceProjectsTests
{
    private static readonly Guid Mine = Guid.NewGuid();
    private static readonly Guid Other = Guid.NewGuid();

    private static bool Listed(Guid id, ProjectStatus status) =>
        WorkspaceProjects.ListedFor(new List<Guid> { Mine }).Compile()(new Project { Id = id, Status = status });

    [Fact]
    public void ListedFor_OnlyTheUsersProjects()
    {
        Assert.True(Listed(Mine, ProjectStatus.Active));
        Assert.True(Listed(Mine, ProjectStatus.PendingReview));
        Assert.False(Listed(Other, ProjectStatus.PendingReview)); // was listed to everyone
        Assert.False(Listed(Other, ProjectStatus.Active));
    }

    [Fact]
    public void ListedFor_SkipsArchivedAndWaiting()
    {
        Assert.False(Listed(Mine, ProjectStatus.Archived));
        Assert.False(Listed(Mine, ProjectStatus.Waiting));
    }
}
