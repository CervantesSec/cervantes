using System.Reflection;
using Cervantes.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Tests.Web.Controllers;

public class ActionResultTypeTests
{
    // ActionResult<T> throws at run time when T is itself an IActionResult (e.g. FileContentResult).
    [Fact]
    public void NoActionWrapsAnActionResultInActionResultOfT()
    {
        var offenders = typeof(ReportHtml).Assembly.GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t))
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => WrapsActionResult(m.ReturnType))
                .Select(m => $"{t.Name}.{m.Name}"))
            .ToList();

        Assert.True(offenders.Count == 0, "ActionResult<T> with an IActionResult T: " + string.Join(", ", offenders));
    }

    private static bool WrapsActionResult(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            type = type.GetGenericArguments()[0];
        }
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ActionResult<>)
               && typeof(IActionResult).IsAssignableFrom(type.GetGenericArguments()[0]);
    }
}
