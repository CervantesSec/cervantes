using System.Collections.Generic;
using Cervantes.CORE;

namespace Cervantes.Web.Models;

public class CalendarViewModel
{
    public IEnumerable<Project> Projects { get; set; }
    public IEnumerable<Task> Tasks { get; set; }
}