using Cervantes.CORE;
using CsvHelper.Configuration.Attributes;

namespace Cervantes.Web.Areas.Workspace.Models;

public class TargetImportCsv
{
    [Index(0)]
    public string Name { get; set; }
    [Index(1)]
    public string Description { get; set; }
    [Index(2)]
    public TargetType Type { get; set; }
    
}