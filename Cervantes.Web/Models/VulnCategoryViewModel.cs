using System;
using Cervantes.CORE;

namespace Cervantes.Web.Models;

public class VulnCategoryViewModel
{
    /// <summary>
    /// Vuln Category Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Vuln Category Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Vuln category Description
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Vuln category Type
    /// </summary>
    public VulnCategoryType Type { get; set; }
}