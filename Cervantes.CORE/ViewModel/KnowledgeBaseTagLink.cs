namespace Cervantes.CORE.ViewModel;

/// <summary>A row of the many-to-many table between knowledge base pages and tags, as a data backup carries it.</summary>
public class KnowledgeBaseTagLink
{
    public Guid NoteId { get; set; }
    public Guid TagId { get; set; }
}
