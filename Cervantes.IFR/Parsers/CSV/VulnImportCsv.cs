using Cervantes.CORE;
using CsvHelper.Configuration.Attributes;

namespace Cervantes.IFR.Parsers.CSV;

public class VulnImportCsv
{
    
    
    [Index(0)]
    public string Name { get; set; }
    [Index(1)]
    public bool? Template { get; set; }
    [Index(2)]
    public string VulnCategory { get; set; }
    [Index(3)]
    public VulnRisk? Risk { get; set; }
    [Index(4)]
    public VulnStatus? Status { get; set; }
    [Index(5)]
    public string CVE { get; set; }
    [Index(6)]
    public string Description { get; set; }
    [Index(7)]
    public string Poc { get; set; }
    [Index(8)]
    public string Impact { get; set; }
    [Index(9)]
    public float? CVSS3 { get; set; }
    [Index(10)]
    public string CVSSVector { get; set; }
    [Index(11)]
    public string Remediation { get; set; }
    [Index(12)]
    public RemediationComplexity? RemediationComplexity { get; set; }
    [Index(13)]
    public RemediationPriority? RemediationPriority { get; set; }
    [Index(14)]
    public string OwaspRisk { get; set; }
    [Index(15)]
    public string OwaspVector { get; set; }
    [Index(16)]
    public string OwaspLikehood { get; set; }
    [Index(17)]
    public string OwaspImpact { get; set; }
}