using System.ComponentModel.DataAnnotations;

namespace Cervantes.CORE.Entities;

public enum MASTGStatus
{
    [Display(Name="Not Started")]
    NotStarted = 0,
    Pass = 1,
    Fail = 2,
    [Display(Name="N/A")]
    NA = 3,
}