using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cervantes.CORE.Entities;

public class Client
{
    /// <summary>
    /// Client Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

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
    /// Created Date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Note Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Note Name
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    public string ContactName { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    public string ContactEmail { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    public string ContactPhone { get; set; }

    /// <summary>
    /// Client Image
    /// </summary>
    public string ImagePath { get; set; }
}