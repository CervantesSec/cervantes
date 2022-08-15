using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class ReportTemplate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public Language Language { get; set; }
    
    public DateTime CreatedDate { get; set; }
   
    
    
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }
    
    public string UserId { get; set; }
    
    public string FilePath { get; set; }
}