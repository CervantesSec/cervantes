namespace Cervantes.CORE.ViewModel;

public class KnowledgeCategoryEditViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Icon { get; set; }
    public int Order { get; set; }
}