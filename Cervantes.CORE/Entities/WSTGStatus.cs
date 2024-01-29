using System.ComponentModel.DataAnnotations;

namespace Cervantes.CORE.Entities;

public enum WSTGStatus
{
    [Display(Name="Not Started")]
    NotStarted = 0,
    Pass = 1,
    Issues = 2,
    [Display(Name="N/A")]
    NA = 3,
}