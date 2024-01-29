using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cervantes.CORE.Entities;
using TaskStatus = Cervantes.CORE.Entities.TaskStatus;

namespace Cervantes.CORE.ViewModel;

public class TaskCreateViewModel
{
    //public Guid Id { get; set; }

    /// <summary>
    /// Is vuln template
    /// </summary>
    //public bool Template { get; set; }
    
    /// <summary>
    /// task created by
    /// </summary>
    public string CreatedUserId { get; set; }

    /// <summary>
    /// Asined user id
    /// </summary>
    public string AsignedUserId { get; set; }

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
    /// Id Target
    /// </summary>
    //public Guid TargetId { get; set; }

    /// <summary>
    /// Task status
    /// </summary>
    public TaskStatus Status { get; set; }
    
 
}