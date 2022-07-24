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
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

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
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Task Associated
    /// </summary>
    [ForeignKey("TaskId")]
    public virtual Task Task { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Visibility of the task note
    /// </summary>
    public Visibility Visibility { get; set; }
}