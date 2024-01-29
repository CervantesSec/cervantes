namespace Cervantes.IFR.Export;

public class TaskExport
{
    public string Name { get; set; }
    public bool Template { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string AsignedUser { get; set; }
    public string? Project { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
}