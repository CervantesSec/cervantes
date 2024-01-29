using System.Collections.Generic;
using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;

namespace Cervantes.CORE.ViewModels;

public class ClientDetailsViewModel
{
    public CORE.Entities.Client Client { get; set; }
    public List<Project> Projects { get; set; }
    public List<Vuln> Vulns { get; set; }
    public List<CORE.Entities.Task> Tasks { get; set; }
}