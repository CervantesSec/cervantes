using Cervantes.CORE.Entities;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.CORE.ViewModel;

public class SearchViewModel
{
    public IEnumerable<ApplicationUser> Users { get; set; }
    public IEnumerable<Client> Clients { get; set; }
    public IEnumerable<Project> Projects { get; set; }
    public IEnumerable<Document> Documents { get; set; }
    public IEnumerable<Report> Reports { get; set; }
    public IEnumerable<Target> Targets { get; set; }
    public IEnumerable<TargetServices> TargetServices { get; set; }
    public IEnumerable<CORE.Entities.Task> Tasks { get; set; }
    public IEnumerable<Vault> Vaults { get; set; }
    public IEnumerable<VulnCategory> VulnCategories { get; set; }
    public IEnumerable<Vuln> Vulns { get; set; }
}