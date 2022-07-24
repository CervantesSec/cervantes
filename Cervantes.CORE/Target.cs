using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.CORE;

public class Target
{
    /// <summary>
    /// Id Vuln
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

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
    /// Project Associated
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Target Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Target Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Target Type
    /// </summary>
    public TargetType Type { get; set; }
}