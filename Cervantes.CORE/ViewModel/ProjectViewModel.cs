using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cervantes.CORE;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class ProjectViewModel
{
    /// <summary>
    /// Project Id
    /// </summary>
    ///
    public Guid Id { get; set; }

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
    /// User who created project
    /// </summary>
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Client asssociated to project
    /// </summary>
    public virtual CORE.Entities.Client Client { get; set; }

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
    public string ExecutiveSummary { get; set; }
    public List<Client> Clients { get; set; }
    public DateTime? dateStart = DateTime.Today;
    public DateTime? dateEnd = DateTime.Today;
    public int BusinessImpact { get; set; }

}