using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class VulnViewModel
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
    /// Vuln Created Date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// User who created project
    /// </summary>
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Project Associated
    /// </summary>
    public virtual Project Project { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid? ProjectId { get; set; }
    
    /// <summary>
    /// VulnCategory Associated
    /// </summary>
    public virtual VulnCategory VulnCategory { get; set; }

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
    
    /// <summary>
    /// Vuln have a Jira Ticket
    /// </summary>
    public bool JiraCreated { get; set; }
    
    public string OWASPRisk { get; set; }
    public string OWASPImpact { get; set; }
    public string OWASPLikehood { get; set; }
    public string OWASPVector { get; set; }
    public Language Language { get; set; }
    public DateTime ModifiedDate { get; set; }
    public List<string> MitreValues { get; set; }
    public List<string> MitreTechniques { get; set; }
}