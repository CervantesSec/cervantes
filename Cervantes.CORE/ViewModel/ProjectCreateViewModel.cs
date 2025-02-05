using System;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class ProjectCreateViewModel
{
    /// <summary>
    /// Project Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Project Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Project Start Date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Project End Date
    /// </summary>
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// Client ID
    /// </summary>
    public Guid ClientId { get; set; }

    /// <summary>
    /// Is project a template
    /// </summary>
    public bool Template { get; set; }

    /// <summary>
    /// Project Status
    /// </summary>
    public ProjectStatus Status { get; set; }

    /// <summary>
    /// Project Type
    /// </summary>
    public ProjectType ProjectType { get; set; }
    
    /// <summary>
    /// Project Language
    /// </summary>
    public Language Language { get; set; }
    
    /// <summary>
    /// Project Scoreing type
    /// </summary>
    public Score Score { get; set; }
    
    public string FindingsId { get; set; }
    public int BusinessImpact { get; set; }

}