using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class TargetCreateViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public TargetType Type { get; set; }
    public string UserId { get; set; }
    public Guid? ProjectId { get; set; }
}