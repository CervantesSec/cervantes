using Cervantes.CORE.Entities;
using CsvHelper.Configuration.Attributes;

namespace Cervantes.CORE.ViewModel;

public class TargetImportCSV
{
    [Index(0)]
    public string Name { get; set; }
    [Index(1)]
    public string Description { get; set; }
    [Index(2)]
    public TargetType Type { get; set; }
}