using Cervantes.CORE;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cervantes.Web.Models;

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
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Project Description
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// Project Start Date
    /// </summary>
    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Project End Date
    /// </summary>
    [Required]
    [DataType(DataType.Date)]
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
    [Required]
    public virtual Client Client { get; set; }

    /// <summary>
    /// Client ID
    /// </summary>
    [Required]
    public Guid ClientId { get; set; }

    /// <summary>
    /// Is project a template
    /// </summary>
    public bool Template { get; set; }

    public IList<SelectListItem> ItemList { get; set; }
    public IList<SelectListItem> StatusList { get; set; }
    public IList<SelectListItem> TypeList { get; set; }

    /// <summary>
    /// Project Status
    /// </summary>
    [Required]
    public ProjectStatus Status { get; set; }

    /// <summary>
    /// Project Type
    /// </summary>
    [Required]
    public ProjectType ProjectType { get; set; }
    
    /// <summary>
    /// Project Language
    /// </summary>
    [Required]
    public Language Language { get; set; }
    
    /// <summary>
    /// Project Score
    /// </summary>
    [Required]
    public Score Score { get; set; }

    [Required]
    public string FindingsId { get; set; }
}