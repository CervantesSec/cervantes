namespace Cervantes.CORE.Entities;

public class KnowledgeBaseTags
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    // Relationships
    public List<KnowledgeBase> Notes { get; set; }
}