using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cervantes.Web.Models;

public class ClientViewModel
{
    /// <summary>
    /// Porject Note Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Note Name
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// Note Name
    /// </summary>
    [Required]
    public string Url { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    [Required]
    public string ContactName { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    [Required]
    public string ContactEmail { get; set; }

    /// <summary>
    /// Note description
    /// </summary>
    [Required]
    public string ContactPhone { get; set; }

    /// <summary>
    /// File Uploaded
    /// </summary>
    [Required]
    public IFormFile upload { get; set; }

    /// <summary>
    /// Client Image
    /// </summary>
    public string ImagePath { get; set; }

    /// <summary>
    /// Client created by
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Creation Date
    /// </summary>
    public DateTime CreatedDate { get; set; }
}