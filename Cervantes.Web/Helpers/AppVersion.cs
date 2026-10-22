using System.Reflection;

namespace Cervantes.Web.Helpers;

public static class AppVersion
{
    /// <summary>The release shown in the footer: the &lt;Version&gt; of Cervantes.Web.csproj.</summary>
    public static string Display { get; } = Clean(typeof(AppVersion).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "");

    /// <summary>Drops the "+&lt;commit&gt;" suffix the SDK appends to the informational version.</summary>
    public static string Clean(string informationalVersion)
    {
        var plus = informationalVersion.IndexOf('+');
        return plus < 0 ? informationalVersion : informationalVersion[..plus];
    }
}
