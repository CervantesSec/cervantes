using System;
using System.Collections.Generic;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class ChecklistViewModel
{

    public Guid Id { get; set; }
    public string Target { get; set; }
    public DateTime CreatedDate { get; set; }
    public string User { get; set; }
    public ChecklistType Type { get; set; }
    public MobileAppPlatform Platform { get; set; }
}