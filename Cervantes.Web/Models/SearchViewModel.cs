using System.Collections.Generic;
using Cervantes.CORE;

namespace Cervantes.Web.Models;

public class SearchViewModel
{
    public string Search { get; set; }
    public IEnumerable<ApplicationUser> Users { get; set; }
    public IEnumerable<Client> Clients { get; set; }
    public IEnumerable<Project> Projects { get; set; }
    public IEnumerable<Document> Documents { get; set; }
    public IEnumerable<Report> Reports { get; set; }
    public IEnumerable<Target> Targets { get; set; }
    public IEnumerable<TargetServices> TargetServices { get; set; }
    public IEnumerable<Task> Tasks { get; set; }
    public IEnumerable<Vault> Vaults { get; set; }
    public IEnumerable<VulnCategory> VulnCategories { get; set; }
    public IEnumerable<Vuln> Vulns { get; set; }
}