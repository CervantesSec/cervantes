using System.Collections.Generic;
using Cervantes.CORE.Entities;

namespace Cervantes.CORE.ViewModel;

public class LogViewModel
{
    public int Id { get; set; }
    public string CreatedOn { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public string Exception { get; set; }
    public string Logger { get; set; }
    public string Url { get; set; }
}