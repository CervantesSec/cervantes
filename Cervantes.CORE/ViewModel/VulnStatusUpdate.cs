using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class VulnStatusUpdate
{
    public List<Guid> VulnIds { get; set; }
    public VulnStatus Status { get; set; }
}