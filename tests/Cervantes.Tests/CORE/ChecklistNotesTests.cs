using System.Reflection;
using Cervantes.CORE.ViewModel.Mastg;
using Cervantes.CORE.ViewModel.Wstg;

namespace Cervantes.Tests.CORE;

public class ChecklistNotesTests
{
    // A new WSTG/MASTG checklist has null notes; non-nullable note properties made the PUT answer 400.
    [Fact]
    public void EveryWstgAndMastgNoteIsNullable()
    {
        var context = new NullabilityInfoContext();
        var types = new[] { typeof(WSTGViewModel), typeof(MastgViewModel) }
            .SelectMany(t => t.Assembly.GetTypes().Where(x => x.Namespace == t.Namespace));
        var offenders = types
            .SelectMany(t => t.GetProperties())
            .Where(p => p.PropertyType == typeof(string) && p.Name.EndsWith("Note"))
            .Where(p => context.Create(p).WriteState != NullabilityState.Nullable)
            .Select(p => $"{p.DeclaringType!.Name}.{p.Name}")
            .ToList();

        Assert.True(offenders.Count == 0, $"{offenders.Count} non-nullable notes, e.g. " + string.Join(", ", offenders.Take(5)));
    }
}
