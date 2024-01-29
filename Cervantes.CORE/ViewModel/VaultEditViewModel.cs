using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class VaultEditViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public VaultType Type { get; set; }
    public string Value { get; set; }
    public Guid ProjectId { get; set; }
}