using System;
using System.ComponentModel.DataAnnotations;
using Cervantes.CORE;

namespace Cervantes.Web.Areas.Workspace.Models;

public class VaultViewModel
{

    public Guid Id { get; set; }
    public virtual Project Project { get; set; }
    public Guid ProjectId { get; set; }
    public VaultType Type { get; set; }
    
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    
    [Required]
    public string Value { get; set; }
    public DateTime CreatedDate { get; set; }
    public virtual ApplicationUser User { get; set; }
    public string UserId { get; set; }

}