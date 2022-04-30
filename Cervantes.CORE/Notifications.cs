using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class Notifications
{
    [Key]
    public int Id { get; set; }
    
    public virtual ApplicationUser User { get; set; }
    
    [ForeignKey("User")]
    public string UserId { get; set; }
    
    public DateTime Date { get; set; }
    
    public string Message { get; set; }
    
    public bool Read { get; set; }
}