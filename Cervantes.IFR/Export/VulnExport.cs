namespace Cervantes.IFR.Export;

public class VulnExport
{
    public bool? Template { get; set; }
    public string? Language { get; set; }
    public string? Name { get; set; }
    public string? CreatedDate { get; set; }
    public string? ModifiedDate { get; set; }
    public string? CreatedUser { get; set; }
    public string? Project  { get; set; }
    public string? VulnCategory { get; set; }
    public string? Risk { get; set; }
    public string? Status { get; set; }
    public string? cve { get; set; }
    public string? Description { get; set; }
    public string? ProofOfConcept { get; set; }
    public string? Impact { get; set; }
    public double? CVSS3 { get; set; }
    public string? CVSSVector { get; set; }
    public string? Remediation { get; set; }
    public string? RemediationComplexity { get; set; }
    public string? RemediationPriority { get; set; }
    public bool? JiraCreated { get; set; }
    public string? OWASPRisk { get; set; }
    public string? OWASPImpact { get; set; }
    public string? OWASPLikehood { get; set; }
    public string? OWASPVector { get; set; }
}