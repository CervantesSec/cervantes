using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;
using TaskStatus = Cervantes.CORE.Entities.TaskStatus;

namespace Cervantes.Web.Components.Pages.Workspace;

public partial class WorkspaceIndex: ComponentBase
{
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] private ProjectController _ProjectController { get; set; }
    [Inject] private TaskController _TaskController { get; set; }
    [Inject] private VulnController _VulnController { get; set; }
    [Inject] private VaultController _VaultController { get; set; }
    [Inject] private TargetController _TargetController { get; set; }

    [Parameter] public Guid project { get; set; }
    [CascadingParameter] bool _isDarkMode { get; set; }

    private CORE.Entities.Project Project = new CORE.Entities.Project();
    private List<BreadcrumbItem> _items;
    private List<ProjectUser> Members = new List<ProjectUser>();
    private List<CORE.Entities.Task> Tasks = new List<CORE.Entities.Task>();
    private List<CORE.Entities.Vuln> Vulns = new List<CORE.Entities.Vuln>();
    private List<CORE.Entities.Vault> Vaults = new List<CORE.Entities.Vault>();
    private List<CORE.Entities.Target> Targets = new List<CORE.Entities.Target>();

    protected override async Task OnInitializedAsync()
    {
        var user = await _ProjectController.VerifyUser(project);
        if (user == false)
        {
            Snackbar.Add(@localizer["noRolePermission"], Severity.Warning);
            navigationManager.NavigateTo("workspaces");
        }
        
        Project = _ProjectController.GetById(project);
        Tasks = _TaskController.GetByUser(project).ToList();
        Vulns = _VulnController.GetByProject(project).ToList();
        Vaults = _VaultController.GetByProject(project).ToList();
        Targets = _TargetController.GetByProjectId(project).ToList();
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Workspace", href: "/workspaces",icon: Icons.Material.Filled.Workspaces),
            new BreadcrumbItem(Project.Name, href: "/workspace/"+project,icon: Icons.Material.Filled.Folder),
            new BreadcrumbItem("Dashboard", href: null, disabled: true, icon: Icons.Material.Filled.Dashboard)
        };
        await Update();

       

        await base.OnInitializedAsync();
    }


    protected async Task Update()
    {

        Members = _ProjectController.GetMembers(project).ToList();

    }
    
    // Task Chart Methods
    private double[] GetTaskChartData()
    {
        var statusOrder = new[] { TaskStatus.Backlog, TaskStatus.ToDo, TaskStatus.InProgress, TaskStatus.Blocked, TaskStatus.Done };
        return statusOrder.Select(status => (double)Tasks.Count(t => t.Status == status)).ToArray();
    }

    private string[] GetTaskChartLabels()
    {
        return new[] { "Backlog", "ToDo", "InProgress", "Blocked", "Done" };
    }

    private ChartOptions GetTaskChartOptions()
    {
        return new ChartOptions
        {
            // Colors matching MudBlazor theme: Backlog=Info, ToDo=Primary, InProgress=Warning, Blocked=Error, Done=Success
            ChartPalette = new[] { "#2196F3", "#594AE2", "#FF9800", "#F44336", "#4CAF50" }
        };
    }

    // Vault Chart Methods
    private double[] GetVaultChartData()
    {
        var grouped = Vaults.GroupBy(v => v.Type).ToList();
        return grouped.Select(g => (double)g.Count()).ToArray();
    }

    private string[] GetVaultChartLabels()
    {
        var grouped = Vaults.GroupBy(v => v.Type).ToList();
        return grouped.Select(g => g.Key.ToString()).ToArray();
    }

    private ChartOptions GetVaultChartOptions()
    {
        return new ChartOptions
        {
            ChartPalette = new[] { "#2196F3", "#4CAF50", "#FF9800", "#F44336", "#9C27B0" }
        };
    }

    // Vulnerability Chart Methods
    private List<ChartSeries> GetVulnChartSeries()
    {
        var riskOrder = new[] { VulnRisk.Critical, VulnRisk.High, VulnRisk.Medium, VulnRisk.Low, VulnRisk.Info };
        var riskNames = new[] { "Critical", "High", "Medium", "Low", "Info" };
        
        var seriesList = new List<ChartSeries>();
        
        for (int i = 0; i < riskOrder.Length; i++)
        {
            var count = Vulns.Count(v => v.Risk == riskOrder[i]);
            seriesList.Add(new ChartSeries 
            { 
                Name = riskNames[i], 
                Data = new double[] { count }
            });
        }
        
        return seriesList;
    }

    private string[] GetVulnChartLabels()
    {
        return new[] { "Vulnerabilities" };
    }

    private ChartOptions GetVulnChartOptions()
    {
        return new ChartOptions
        {
            ChartPalette = new[] { "#7C4DFF", "#F44336", "#FF9800", "#4CAF50", "#2196F3" }
        };
    }
    
    private AxisChartOptions GetVulnAxisChartOptions()
    {
        return new AxisChartOptions
        {
            MatchBoundsToSize = true
        };
    }
    
}