using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cervantes.CORE;

public class VulnCategory
{
    /// <summary>
    /// Vuln Category Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Vuln Category Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Vuln category Description
    /// </summary>
    public string Description { get; set; }
}