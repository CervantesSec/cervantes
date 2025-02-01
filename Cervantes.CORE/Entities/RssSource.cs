using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

public class RssSource
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string ImagePath { get; set; }
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }
    public string UserId { get; set; }
    [ForeignKey("CategoryId")]
    [JsonIgnore]
    public virtual RssCategory Category { get; set; }
    public Guid CategoryId { get; set; }
}