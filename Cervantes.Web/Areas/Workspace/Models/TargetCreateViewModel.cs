using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cervantes.CORE;

namespace Cervantes.Web.Areas.Workspace.Models;

public class TargetCreateViewModel
{
    /// <summary>
    /// Id Vuln
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// User who created project
    /// </summary>
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Project Associated
    /// </summary>
    public virtual Project Project { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Target Name
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Target Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Target Type
    /// </summary>
    [Required]
    public TargetType Type { get; set; }
}