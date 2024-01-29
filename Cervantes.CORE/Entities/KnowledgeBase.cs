using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

public class KnowledgeBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Order { get; set; }
    [ForeignKey("CreatedUserId")]
    [JsonIgnore]
    public virtual ApplicationUser CreatedUser { get; set; }
    public string CreatedUserId { get; set; }
    [ForeignKey("UpdatedUserId")]
    [JsonIgnore]
    public virtual ApplicationUser UpdatedUser { get; set; }
    public string UpdatedUserId { get; set; }
    // Relationships
    [ForeignKey("CategoryId")]
    [JsonIgnore]
    public virtual KnowledgeBaseCategories Category { get; set; }
    public Guid CategoryId { get; set; }
    public List<KnowledgeBaseTags> Tags { get; set; }

}