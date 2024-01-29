using System;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModels;

public class TargetViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public TargetType Type { get; set; }

    public virtual ApplicationUser User { get; set; }
    
    public string UserId { get; set; }
    public Guid? ProjectId { get; set; }

}