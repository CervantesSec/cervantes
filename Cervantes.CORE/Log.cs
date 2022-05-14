using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class Log
{
    /// <summary>
    /// Porject Note Id
    /// </summary>
    [Key]
    public int id { get; set; }

    /// <summary>
    /// Created Date
    /// </summary>
    public DateTime created_on { get; set; }

    public string level { get; set; }
    public string message { get; set; }
    public string stack_trace { get; set; }
    public string exception { get; set; }
    public string logger { get; set; }

    /// <summary>
    /// Note Name
    /// </summary>
    public string url { get; set; }
}