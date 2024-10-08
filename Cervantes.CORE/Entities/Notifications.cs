using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE.Entities;

public class Notifications
{
    [Key] public Guid Id { get; set; }

    [ForeignKey("User")] 
    public virtual ApplicationUser User { get; set; }
    public string UserId { get; set; }

    public DateTime Date { get; set; }

    public string Message { get; set; }

    public bool Read { get; set; }
}