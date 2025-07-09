namespace Cervantes.IFR.ChecklistMigration;

/// <summary>
/// Models for deserializing WSTG JSON data
/// </summary>
public class WstgJsonRoot
{
    public Dictionary<string, WstgCategory> categories { get; set; } = new();
}

public class WstgCategory
{
    public string id { get; set; } = string.Empty;
    public List<WstgTest> tests { get; set; } = new();
}

public class WstgTest
{
    public string name { get; set; } = string.Empty;
    public string id { get; set; } = string.Empty;
    public string reference { get; set; } = string.Empty;
    public List<string> objectives { get; set; } = new();
}