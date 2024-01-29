using System;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class ChecklistCreateViewModel
{

    public Guid TargetId { get; set; }

    public ChecklistType Type { get; set; }
    
    public Guid ProjectId { get; set; }
    
    public MobileAppPlatform MobileAppPlatform { get; set; }
}