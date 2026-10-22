using Cervantes.Web.Components.Pages.Admin.Roles;

namespace Cervantes.Tests.Web.Components;

public class RolePermissionsTests
{
    [Fact]
    public void GroupPermissions_ByDisplayGroup()
    {
        var groups = RoleDialog.GroupPermissions(new[] { "VulnsRead", "VulnsAdd", "WorkspacesRead" });

        var vulns = Assert.Single(groups, g => g.Names.Contains("VulnsRead"));
        Assert.Contains("VulnsAdd", vulns.Names);
        Assert.DoesNotContain("WorkspacesRead", vulns.Names);
        Assert.Contains(groups, g => g.Group == "Workspaces" && g.Names.SequenceEqual(new[] { "WorkspacesRead" }));
    }

    [Fact]
    public void GroupPermissions_UnknownNamesGoToOther()
    {
        var groups = RoleDialog.GroupPermissions(new[] { "NotAPermission" });

        Assert.Equal("Other", Assert.Single(groups).Group);
    }
}
