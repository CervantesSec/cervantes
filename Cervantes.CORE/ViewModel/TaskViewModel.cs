using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cervantes.CORE.Entities;
using TaskStatus = Cervantes.CORE.Entities.TaskStatus;

namespace Cervantes.CORE.ViewModels;

public class TaskViewModel
{
    public Guid Id { get; set; }

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
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// Task Start Date
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Task End Date
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Task Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Task Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Target Associated
    /// </summary>
    public virtual Target Target { get; set; }

    /// <summary>
    /// Id Target
    /// </summary>
    public Guid TargetId { get; set; }
    
    public IEnumerable<Guid> SelectedTargets { get; set; }


    /// <summary>
    /// Task status
    /// </summary>
    public TaskStatus Status { get; set; }

    public string TargetName { get; set; }
    public List<Target> Targets { get; set; }
    public List<ApplicationUser> Users { get; set; }
    public DateTime? dateStart = DateTime.Today;
    public DateTime? dateEnd = DateTime.Today;
}