using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cervantes.CORE;

public class Document
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
    /// File path location
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// User who created project
    /// </summary>
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// Id user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Visibility of the document
    /// </summary>
    public Visibility Visibility { get; set; }

    public DateTime CreatedDate { get; set; }
}