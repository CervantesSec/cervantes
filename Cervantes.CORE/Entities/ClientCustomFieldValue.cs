using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cervantes.CORE.Entities;

/// <summary>
/// Stores the actual value of a custom field for a specific client
/// </summary>
public class ClientCustomFieldValue
{
    /// <summary>
    /// Unique identifier for the custom field value
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// The client this value belongs to
    /// </summary>
    [ForeignKey("ClientId")]
    [JsonIgnore]
    public virtual Client Client { get; set; }

    /// <summary>
    /// ID of the client this value belongs to
    /// </summary>
    [Required]
    public Guid ClientId { get; set; }

    /// <summary>
    /// The custom field definition this value is for
    /// </summary>
    [ForeignKey("ClientCustomFieldId")]
    [JsonIgnore]
    public virtual ClientCustomField ClientCustomField { get; set; }

    /// <summary>
    /// ID of the custom field definition this value is for
    /// </summary>
    [Required]
    public Guid ClientCustomFieldId { get; set; }

    /// <summary>
    /// The actual value stored as string (will be converted based on field type)
    /// - For Input/Textarea: plain text
    /// - For Select: selected option value
    /// - For Number: string representation of number
    /// - For Date: ISO 8601 date string
    /// - For Boolean: "true" or "false"
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// When this value was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// When this value was last modified
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// User who created/last modified this value
    /// </summary>
    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual ApplicationUser User { get; set; }

    /// <summary>
    /// ID of the user who created/last modified this value
    /// </summary>
    public string UserId { get; set; }
}