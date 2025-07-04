﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

public class Task
{
    /// <summary>
    /// Id Task
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Is vuln template
    /// </summary>
    public bool Template { get; set; }

    /// <summary>
    /// User who created project
    /// </summary>
    [JsonIgnore]
    public virtual ApplicationUser CreatedUser { get; set; }

    /// <summary>
    /// task created by
    /// </summary>
    public string CreatedUserId { get; set; }

    /// <summary>
    /// User asigned project
    /// </summary>
    [ForeignKey("AsignedUserId")]
    [JsonIgnore]
    public virtual ApplicationUser AsignedUser { get; set; }

    /// <summary>
    /// Asined user id
    /// </summary>
    public string? AsignedUserId { get; set; }

    /// <summary>
    /// Project Associated
    /// </summary>
    [ForeignKey("ProjectId")]
    [JsonIgnore]
    public virtual Project Project { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// Task Start Date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Task End Date
    /// </summary>
    public DateTime EndDate { get; set; }
    public DateTime? ClosedDate { get; set; }
    
    /// <summary>
    /// Task Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Task Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Task status
    /// </summary>
    public TaskStatus Status { get; set; }
}