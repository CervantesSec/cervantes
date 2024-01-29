using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE.Entities;

public class Log
{

    [Key]
    public int Id { get; set; }
    public string CreatedOn { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public string Exception { get; set; }
    public string Logger { get; set; }

    /// <summary>
    /// Note Name
    /// </summary>
    public string Url { get; set; }
}