namespace Cervantes.CORE.ViewModel;

public class CreateRoleViewModel
{
    public string Name { get; set; }
    /// <summary>Optional. On edit, an omitted (null) description keeps the stored one.</summary>
    public string? Description { get; set; }
    public IEnumerable<string> Permissions { get; set; }
 }