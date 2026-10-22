namespace Cervantes.Web.Helpers;

public static class ImportFile
{
    /// <summary>
    /// True when an import failed because the uploaded file could not be read as the chosen format (bad CSV, XML,
    /// JSON or YAML, or content no parser recognizes), so the answer is 400 and not 500.
    /// </summary>
    public static bool IsUnreadable(Exception e)
    {
        return e is CsvHelper.CsvHelperException or System.Xml.XmlException or System.Text.Json.JsonException
            or YamlDotNet.Core.YamlException or InvalidDataException;
    }
}
