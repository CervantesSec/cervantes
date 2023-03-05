using System.ComponentModel.DataAnnotations;

namespace Cervantes.CORE;

public enum ChecklistType
{
    [Display(Name="OWASP WSTG")]
    OWASPWSTG = 0,
    [Display(Name="OWASP MASTG")]
    OWASPMASVS = 1
}