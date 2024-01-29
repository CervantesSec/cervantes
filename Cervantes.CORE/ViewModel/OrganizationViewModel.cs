using System;
using Microsoft.AspNetCore.Components.Forms;

namespace Cervantes.CORE.ViewModel;

public class OrganizationViewModel
{
    /// <summary>
    /// Organization Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Organization Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Organization Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Organization Url
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Organization Contact Name
    /// </summary>
    public string ContactName { get; set; }

    /// <summary>
    /// Organization contact email
    /// </summary>
    public string ContactEmail { get; set; }

    /// <summary>
    /// Organization contact phone
    /// </summary>
    public string ContactPhone { get; set; }

    /// <summary>
    /// File Uploaded
    /// </summary>
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }

    /// <summary>
    /// Organization Image
    /// </summary>
    public string ImagePath { get; set; }
}