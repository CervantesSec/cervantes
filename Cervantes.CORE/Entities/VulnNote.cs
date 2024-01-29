using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cervantes.CORE.Entities;

public class VulnNote
{
    /// <summary>
    /// Porject Note Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public Guid Id { get; set; }

    /// <summary>
    /// Note Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// User who created project
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Project Associated
    /// </summary>
    [ForeignKey("VulnId")]
    [JsonIgnore]
    public virtual Vuln Vuln { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid VulnId { get; set; }

    /// <summary>
    /// Visibility of the vuln note
    /// </summary>
    public Visibility Visibility { get; set; }
}