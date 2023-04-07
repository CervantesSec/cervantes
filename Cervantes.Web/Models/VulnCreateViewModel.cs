using Cervantes.CORE;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cervantes.Web.Models;

public class VulnCreateViewModel
{
    /// <summary>
    /// Id Vuln
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Is vuln template
    /// </summary>
    public bool Template { get; set; }

    /// <summary>
    /// Vuln Name
    /// </summary>
    [Required]
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
    /// Target Associated
    /// </summary>
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id Target
    /// </summary>
    public Guid TargetId { get; set; }
    
    public List<Guid> SelectedTargets { get; set; }

    public string TargetName { get; set; }

    /// <summary>
    /// VulnCategory Associated
    /// </summary>
    public virtual VulnCategory VulnCategory { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid? VulnCategoryId { get; set; }

    public string VulnCategoryName { get; set; }

    /// <summary>
    /// Vuln Risk
    /// </summary>
    [Required]
    public VulnRisk Risk { get; set; }

    /// <summary>
    /// Vulnerability Status
    /// </summary>
    [Required]
    public VulnStatus Status { get; set; }

    /// <summary>
    /// CVE Associated to vuln
    /// </summary>
    [Required]
    public string cve { get; set; }

    /// <summary>
    /// Vuln Description
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// Vuln POC
    /// </summary>
    [Required]
    public string ProofOfConcept { get; set; }

    /// <summary>
    /// Vuln Impact
    /// </summary>
    [Required]
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
    [Required]
    public string Remediation { get; set; }

    /// <summary>
    /// Vuln Remediation Complexity
    /// </summary>
    [Required]
    public RemediationComplexity RemediationComplexity { get; set; }

    /// <summary>
    /// Vuln Remediation Priority
    /// </summary>
    [Required]
    public RemediationPriority RemediationPriority { get; set; }

    public IList<SelectListItem> TargetList { get; set; }
    public IList<SelectListItem> VulnCatList { get; set; }
    
    public string OwaspRisk { get; set; }
    public string OwaspVector { get; set; }
    public string OwaspLikehood { get; set; }
    public string OwaspImpact { get; set; }
    public List<VulnCategory> VulnCategories { get; set; }
    public List<Project> Projects { get; set; }
}