using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class Vault
{
    [Key]
    public int Id { get; set; }
    public virtual Project Project { get; set; }
    [ForeignKey("Project")]
    public int ProjectId { get; set; }
    public string Name { get; set; }
    public VaultType Type { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
    public DateTime CreatedDate { get; set; }
    public virtual ApplicationUser User { get; set; }
    [ForeignKey("User")]
    public string UserId { get; set; }
}