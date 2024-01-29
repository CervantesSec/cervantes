using System;

namespace Cervantes.CORE.ViewModels;

public class ProjectCreateNoteViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid ProjectId { get; set; }
}