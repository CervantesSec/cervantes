namespace Cervantes.CORE.Entities;

/// <summary>
/// Represents a mapping between a CVE and a CWE (Common Weakness Enumeration)
/// </summary>
public class CveCweMapping
{
    /// <summary>
    /// Unique identifier for the mapping
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the CVE
    /// </summary>
    public Guid CveId { get; set; }

    /// <summary>
    /// Navigation property to the CVE
    /// </summary>
    public virtual Cve Cve { get; set; } = null!;

    /// <summary>
    /// Foreign key to the CWE
    /// </summary>
    public int CweId { get; set; }

    /// <summary>
    /// Navigation property to the CWE
    /// </summary>
    public virtual Cwe Cwe { get; set; } = null!;

    /// <summary>
    /// When this mapping was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// User who created this mapping
    /// </summary>
    public string? UserId { get; set; }
}