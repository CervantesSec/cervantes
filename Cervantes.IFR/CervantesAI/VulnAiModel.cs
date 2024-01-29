using Cervantes.CORE.Entities;

namespace Cervantes.IFR.CervantesAI;

public class VulnAiModel
{
    public string Name { get; set; }
    public Language Language { get; set; }
    public string Description { get; set; }
    public string Impact { get; set; }
    public VulnRisk Risk { get; set; }
    public string ProofOfConcept { get; set; }
    public string Remediation { get; set; }
    
    
}