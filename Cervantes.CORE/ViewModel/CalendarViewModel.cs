using System.Collections.Generic;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModels;

public class CalendarViewModel
{
    public List<Project> Projects { get; set; }
    public List<CORE.Entities.Task> Tasks { get; set; }
}