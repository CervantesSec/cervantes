namespace Cervantes.IFR.Export;

public class ProjectExport
{
    /// <summary>
    /// Project Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Project Description
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Project Description
    /// </summary>
    public string Client { get; set; }
    
    /// <summary>
    /// Project Description
    /// </summary>
    public string CreatedUser { get; set; }

    /// <summary>
    /// Project Start Date
    /// </summary>
    public string StartDate { get; set; }

    /// <summary>
    /// Project End Date
    /// </summary>
    public string EndDate { get; set; }

    /// <summary>
    /// Is project a template
    /// </summary>
    public bool Template { get; set; }

    /// <summary>
    /// Project Status
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Project Type
    /// </summary>
    public string ProjectType { get; set; }
    
    /// <summary>
    /// Project Language
    /// </summary>
    public string Language { get; set; }
    
    /// <summary>
    /// Project Scoreing type
    /// </summary>
    public string Score { get; set; }
    
    public string FindingsId { get; set; }
    
    public string ExecutiveSummary { get; set; }
}