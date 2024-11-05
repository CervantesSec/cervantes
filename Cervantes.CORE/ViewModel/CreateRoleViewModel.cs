namespace Cervantes.CORE.ViewModel;

public class CreateRoleViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<string> Permissions { get; set; }
 }