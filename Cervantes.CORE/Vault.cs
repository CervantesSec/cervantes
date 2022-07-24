using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class Vault
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; }
    public VaultType Type { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
    public DateTime CreatedDate { get; set; }
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }
    public string UserId { get; set; }
    
}