using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.CORE;

public class TaskAttachment
{
    /// <summary>
    /// Task Attachment Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// TaskAttachment Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// TaskAttachment Path
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// User who created the attachment
    /// </summary>
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Project Associated
    /// </summary>
    [ForeignKey("TaskId")]
    public virtual Task Task { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid TaskId { get; set; }
}