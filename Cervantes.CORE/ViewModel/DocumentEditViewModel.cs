using System;
using System.ComponentModel.DataAnnotations;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace Cervantes.CORE.ViewModels;

public class DocumentEditViewModel
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
}