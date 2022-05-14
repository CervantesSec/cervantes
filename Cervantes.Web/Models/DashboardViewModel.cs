using Cervantes.CORE;
using System.Collections.Generic;

namespace Cervantes.Web.Models;

public class DashboardViewModel
{
    public int ProjectNumber { get; set; }
    public int VulnNumber { get; set; }
    public int TasksNumber { get; set; }
    public int ClientNumber { get; set; }
    public IEnumerable<Project> ActiveProjects { get; set; }
    public IEnumerable<Client> RecentClients { get; set; }
    public IEnumerable<Document> RecentDocuments { get; set; }
    public IEnumerable<Vuln> RecentVulns { get; set; }

    public int ProjectPercetagesActive { get; set; }
    public int ProjectPercetagesArchived { get; set; }
    public int ProjectPercetagesWaiting { get; set; }

    public int Open { get; set; }
    public int Accepted { get; set; }
    public int Confirmed { get; set; }
    public int Resolved { get; set; }
    public int OutOfScope { get; set; }
    public int Invalid { get; set; }
}