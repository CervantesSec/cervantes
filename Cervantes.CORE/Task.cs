using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class Task
{
    /// <summary>
    /// Id Task
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Is vuln template
    /// </summary>
    public bool Template { get; set; }

    /// <summary>
    /// User who created project
    /// </summary>
    public virtual ApplicationUser CreatedUser { get; set; }

    /// <summary>
    /// task created by
    /// </summary>
    [ForeignKey("CreatedUser")]
    public string CreatedUserId { get; set; }

    /// <summary>
    /// User asigned project
    /// </summary>
    public virtual ApplicationUser AsignedUser { get; set; }

    /// <summary>
    /// Asined user id
    /// </summary>
    [ForeignKey("AsignedUser")]
    public string AsignedUserId { get; set; }

    /// <summary>
    /// Project Associated
    /// </summary>
    public virtual Project Project { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    [ForeignKey("Project")]
    public int ProjectId { get; set; }

    /// <summary>
    /// Task Start Date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Task End Date
    /// </summary>
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
    [ForeignKey("Target")]
    public int TargetId { get; set; }

    /// <summary>
    /// Task status
    /// </summary>
    public TaskStatus Status { get; set; }
}