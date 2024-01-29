using System.Net.Http.Json;
using ApexCharts;
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
    private ApexChartOptions<CORE.Entities.Task> optionsTask = new();
    private ApexChartOptions<CORE.Entities.Vuln> optionsVulns;
    private ApexChartOptions<CORE.Entities.Vault> optionsVault;

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

    protected override async Task OnParametersSetAsync()
    {
        if (_isDarkMode)
        {
            optionsTask = new ApexChartOptions<CORE.Entities.Task>
            {
                Theme = new Theme
                {
                    Mode = Mode.Dark,
                },
                Chart = new Chart
                {
                    Background = "0",
                    Height = 275,
                },
            };
            optionsVulns = new ApexChartOptions<CORE.Entities.Vuln>
            {
                Theme = new Theme
                {
                    Mode = Mode.Dark,
                },
                Chart = new Chart
                {
                    Background = "0",
                    Height = 275,
                },
            };
            optionsVault = new ApexChartOptions<CORE.Entities.Vault>
            {
                Theme = new Theme
                {
                    Mode = Mode.Dark,
                },
                Chart = new Chart
                {
                    Background = "0",
                    Height = 275,
                },
            };
        }
        else
        {
            optionsTask = new ApexChartOptions<CORE.Entities.Task>
            {
               
                Chart = new Chart
                {
                    Background = "0",
                    Height = 275,
                },
            };
            optionsVulns = new ApexChartOptions<CORE.Entities.Vuln>
            {
                Chart = new Chart
                {
                    Background = "0",
                    Height = 275,
                },
            };
            optionsVault = new ApexChartOptions<CORE.Entities.Vault>
            {
                Chart = new Chart
                {
                    Background = "0",
                    Height = 275,
                },
            };
        }
        await base.OnParametersSetAsync();
    }

    protected async Task Update()
    {

        Members = _ProjectController.GetMembers(project).ToList();

    }
    
    private string GetPointColorTasks(CORE.Entities.Task task)
    {
        switch (task.Status)
        {
            case TaskStatus.InProgress:
                return "#ff9800";
            case TaskStatus.Backlog:
                return "#594ae2";
            case TaskStatus.Blocked:
                return "#f44336";
            case TaskStatus.ToDo:
                return "#ffb545";
            case TaskStatus.Done:
                return "#00c853";
            default:
                return "#7e6fff";
        }

    }
    
    private string GetPointColorVulns(CORE.Entities.Vuln vuln)
    {
        switch (vuln.Risk)
        {
            case VulnRisk.Critical:
                return "#ff1f69";
            case VulnRisk.High:
                return "#ff6699";
            case VulnRisk.Medium:
                return "#ffb545";
            case VulnRisk.Low:
                return "#1ec8a5";
            case VulnRisk.Info:
                return "#4a86ff";
            default:
                return "#7e6fff";
        }

    }
    
}