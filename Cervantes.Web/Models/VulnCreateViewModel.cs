using Cervantes.CORE;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Cervantes.Web.Models;

public class VulnCreateViewModel
{
    /// <summary>
    /// Id Vuln
    /// </summary>
    public int Id { get; set; }

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
    public int ProjectId { get; set; }

    /// <summary>
    /// Target Associated
    /// </summary>
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id Target
    /// </summary>
    public int TargetId { get; set; }

    public string TargetName { get; set; }

    /// <summary>
    /// VulnCategory Associated
    /// </summary>
    public virtual VulnCategory VulnCategory { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public int VulnCategoryId { get; set; }

    public string VulnCategoryName { get; set; }

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

    public IList<SelectListItem> TargetList { get; set; }
    public IList<SelectListItem> VulnCatList { get; set; }
}