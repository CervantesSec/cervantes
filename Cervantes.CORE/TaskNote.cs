using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.CORE;

public class TaskNote
{
    /// <summary>
    /// Task Note Id
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Task Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Task Note description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// User who created project
    /// </summary>
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    [ForeignKey("User")]
    public string UserId { get; set; }

    /// <summary>
    /// Task Associated
    /// </summary>
    public virtual Task Task { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    [ForeignKey("Task")]
    public int TaskId { get; set; }

    /// <summary>
    /// Visibility of the task note
    /// </summary>
    public Visibility Visibility { get; set; }
}