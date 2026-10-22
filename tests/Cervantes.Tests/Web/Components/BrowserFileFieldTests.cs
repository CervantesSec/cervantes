using System.Reflection;
using Cervantes.Web.Helpers;
using Microsoft.AspNetCore.Components.Forms;

namespace Cervantes.Tests.Web.Components;

public class BrowserFileFieldTests
{
    // A static IBrowserFile is shared by every circuit: one user's chosen file leaks into another user's dialog.
    [Fact]
    public void NoComponentKeepsABrowserFileInAStaticField()
    {
        var offenders = typeof(ReportHtml).Assembly.GetTypes()
            .SelectMany(t => t.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(IBrowserFile))
                .Select(f => $"{t.FullName}.{f.Name}"))
            .ToList();

        Assert.True(offenders.Count == 0, "Static IBrowserFile fields: " + string.Join(", ", offenders));
    }
}
