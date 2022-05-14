using Cervantes.CORE;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cervantes.Web.Areas.Workspace.Models;

public class TaskCreateViewModel
{
    public int Id { get; set; }

    /// <summary>
    /// Is vuln template
    /// </summary>
    public bool Template { get; set; }

    /// <summary>
    /// User who created project
    /// </summary>
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// task created by
    /// </summary>
    public string CreatedUserId { get; set; }

    /// <summary>
    /// User asigned project
    /// </summary>
    public virtual ApplicationUser User2 { get; set; }

    /// <summary>
    /// Asined user id
    /// </summary>
    [Required]
    public string AsignedUserId { get; set; }

    /// <summary>
    /// Project Associated
    /// </summary>
    public virtual Project Project { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Task Start Date
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Task End Date
    /// </summary>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Task Name
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Task Description
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// Target Associated
    /// </summary>
    [Required]
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id Target
    /// </summary>
    [Required]
    public int TargetId { get; set; }

    /// <summary>
    /// Task status
    /// </summary>
    [Required]
    public TaskStatus Status { get; set; }

    public string TargetName { get; set; }
    public IList<SelectListItem> TargetList { get; set; }
    public IList<SelectListItem> UsersList { get; set; }
}