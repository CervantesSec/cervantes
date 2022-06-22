using System;
using Cervantes.CORE;

namespace Cervantes.Web.Areas.Workspace.Models;

public class VaultViewModel
{

    public int Id { get; set; }
    public virtual Project Project { get; set; }
    public int ProjectId { get; set; }
    public VaultType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
    public DateTime CreatedDate { get; set; }
    public virtual ApplicationUser User { get; set; }
    public string UserId { get; set; }

}