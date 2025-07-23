using System.Security.Claims;
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


    // Project Chart Methods
    private double[] GetProjectChartData()
    {
        var grouped = Projects.GroupBy(p => p.Status).ToList();
        return grouped.Select(g => (double)g.Count()).ToArray();
    }

    private string[] GetProjectChartLabels()
    {
        var grouped = Projects.GroupBy(p => p.Status).ToList();
        return grouped.Select(g => g.Key.ToString()).ToArray();
    }

    private ChartOptions GetProjectChartOptions()
    {
        return new ChartOptions
        {
            ChartPalette = new[] { "#3dcb6c", "#4a86ff", "#ffb545", "#7e6fff" }
        };
    }

    // Vulnerability Chart Methods
    private List<ChartSeries> GetVulnChartSeries()
    {
        var riskOrder = new[] { VulnRisk.Critical, VulnRisk.High, VulnRisk.Medium, VulnRisk.Low, VulnRisk.Info };
        var riskNames = new[] { "Critical", "High", "Medium", "Low", "Info" };
        // Colors matching MudBlazor theme: Secondary, Error, Warning, Success, Info
        var colors = new[] { "#7C4DFF", "#F44336", "#FF9800", "#4CAF50", "#2196F3" };
        
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
    
}