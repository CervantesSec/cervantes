using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class VulnCreateViewModel
{
    public Guid Id { get; set; }
    
    public string FindingId { get; set; }

    /// <summary>
    /// Is vuln template
    /// </summary>
    public bool Template { get; set; }

    /// <summary>
    /// Vuln Name
    /// </summary>
    public string Name { get; set; }
    

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid? ProjectId { get; set; }
   

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid? VulnCategoryId { get; set; }

    /// <summary>
    /// Vuln Risk
    /// </summary>
    public VulnRisk Risk { get; set; }

    /// <summary>
    /// Vulnerability Status
    /// </summary>
    public VulnStatus Status { get; set; }
    
    /// <summary>
    /// CVE Associated to vuln
    /// </summary>
    public string cve { get; set; }

    /// <summary>
    /// Vuln Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Vuln POC
    /// </summary>
    public string ProofOfConcept { get; set; }

    /// <summary>
    /// Vuln Impact
    /// </summary>
    public string Impact { get; set; }

    /// <summary>
    /// Vuln CVSS3 Score
    /// </summary>
    public double CVSS3 { get; set; }

    /// <summary>
    /// Vuln CVSS Vector
    /// </summary>
    public string CVSSVector { get; set; }

    /// <summary>
    /// Vuln Remediation Description
    /// </summary>
    public string Remediation { get; set; }

    /// <summary>
    /// Vuln Remediation Complexity
    /// </summary>
    public RemediationComplexity RemediationComplexity { get; set; }

    /// <summary>
    /// Vuln Remediation Priority
    /// </summary>
    public RemediationPriority RemediationPriority { get; set; }
    
    public string OWASPRisk { get; set; }
    public string OWASPImpact { get; set; }
    public string OWASPLikehood { get; set; }
    public string OWASPVector { get; set; }
    
    public List<int> CweId { get; set; }
    public List<Guid> TargetId { get; set; }
    public List<string> MitreValues { get; set; }
    public List<string> MitreTechniques { get; set; }
    public Language Language { get; set; }
    
    /// <summary>
    /// Custom field values for this vulnerability
    /// Key: CustomFieldId, Value: Field value as string
    /// </summary>
    public Dictionary<Guid, string> CustomFieldValues { get; set; } = new Dictionary<Guid, string>();
}