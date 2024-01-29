using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cervantes.CORE.Entities;

public enum ReportType
{
    General = 0,
    [Display(Name = "OWASP WSTG")]
    WSTG = 1,
    [Display(Name = "OWASP MSTG")]
    MASTG = 2,
}