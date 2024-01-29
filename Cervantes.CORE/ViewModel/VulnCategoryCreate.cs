using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class VulnCategoryCreate
{
    public string Name { get; set; }
    
    public string Description { get; set; }

    public VulnCategoryType Type { get; set; }
}