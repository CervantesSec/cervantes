using System.Xml;

namespace Cervantes.Tests.IFR;

// Real tool output under IFR/Fixtures/Parsers/<Tool>/ (see NOTICE.md there), copied to the test output folder.
internal static class ParserFixtures
{
    public static string Path(string tool, string file) =>
        System.IO.Path.Combine(AppContext.BaseDirectory, "IFR", "Fixtures", "Parsers", tool, file);

    public static string Text(string tool, string file) => File.ReadAllText(Path(tool, file));

    public static XmlDocument Xml(string tool, string file)
    {
        var doc = new XmlDocument();
        doc.Load(Path(tool, file));
        return doc;
    }
}
