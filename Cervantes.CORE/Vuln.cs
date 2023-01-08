using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class Vuln
{
    /// <summary>
    /// Id Vuln
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

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
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Project Associated
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// VulnCategory Associated
    /// </summary>
    [ForeignKey("VulnCategoryId")]
    public virtual VulnCategory VulnCategory { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid VulnCategoryId { get; set; }

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
    public float CVSS3 { get; set; }

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
}