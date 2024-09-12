using System.Security.Claims;
using ApexCharts;
using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;
using TaskStatus = Cervantes.CORE.Entities.TaskStatus;

namespace Cervantes.Web.Components.Pages;

public partial class Home: ComponentBase
{
    [Inject] ProjectController _projectController { get; set; }
    [Inject] ClientsController _ClientsController { get; set; }
    [Inject] VulnController _VulnController { get; set; }
    [Inject] TaskController _TaskController { get; set; }
    [Inject] UserController _UserController { get; set; }

    private List<BreadcrumbItem> _items;
    [CascadingParameter] bool _isDarkMode { get; set; }
    private List<Project> Projects { get; set; } = new List<Project>();
    private List<Client> Clients { get; set; } = new List<Client>();
    private List<CORE.Entities.Vuln> Vulns { get; set; } = new List<CORE.Entities.Vuln>();
    private List<CORE.Entities.Task> Tasks { get; set; } = new List<CORE.Entities.Task>();
    private ApplicationUser User { get; set; }

    private ApexChartOptions<Project> optionsProject;
    private ApexChartOptions<CORE.Entities.Vuln> optionsVulns;
    private ApexChartOptions<CORE.Entities.Task> optionsTask = new();

    private ApexChart<Project> chart;
    protected override async Task OnInitializedAsync()
    {
        Projects = _projectController.Get().ToList();
        Clients = _ClientsController.Get().ToList();
        Vulns = _VulnController.GetVulns().ToList();
        Tasks = _TaskController.Get().ToList();
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: null, disabled: true, icon: Icons.Material.Filled.Home)
        };

        if (_accessor.HttpContext.User == null)
        {
            NavigationManager.NavigateTo("Account/Login");
            return;
        }
        
        if (_accessor.HttpContext.User?.Identity?.IsAuthenticated == true)
        {
            User = _UserController.GetUser(_accessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier));
        }

    }

    protected override async Task OnParametersSetAsync()
    {
        if (_isDarkMode)
        {
            optionsProject = new ApexChartOptions<Project>
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
        }else
        {
            optionsProject = new ApexChartOptions<Project>
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
            optionsTask = new ApexChartOptions<CORE.Entities.Task>
            {
               
                Chart = new Chart
                {
                    Background = "0",
                    Height = 275,
                },
            };
            //optionsTask.PlotOptions = new PlotOptions { Pie = new PlotOptionsPie { StartAngle = -90, EndAngle = 90 } };

            
        }
        await base.OnParametersSetAsync();
    }

    private string GetPointColorProject(Project project)
    {
        switch (project.Status)
        {
            case ProjectStatus.Active:
                return "#3dcb6c";
            case ProjectStatus.Archived:
                return "#4a86ff";
            case ProjectStatus.Waiting:
                return "#ffb545";
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
    
}