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

    // Filter state variables
    private string selectedTimeRange = "all";
    private string selectedProjectStatus = "all";
    private string selectedVulnRisk = "all";
    private string selectedTaskStatus = "all";
    private bool showOnlyActiveProjects = false;
    private bool showOnlyMyTasks = false;
    private bool loading = true;

    // Filtered data properties
    private List<Project> FilteredProjects => ApplyProjectFilters();
    private List<CORE.Entities.Vuln> FilteredVulns => ApplyVulnFilters();
    private List<CORE.Entities.Task> FilteredTasks => ApplyTaskFilters();

    protected override async Task OnInitializedAsync()
    {
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

        await LoadDashboardData();
    }

    private async Task LoadDashboardData()
    {
        loading = true;
        try
        {
            Projects = _projectController.Get().ToList();
            Clients = _ClientsController.Get().ToList();
            Vulns = _VulnController.GetVulns().ToList();
            Tasks = _TaskController.Get().ToList();
            
            // Simulate async operation for better UX
            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            // Handle errors gracefully - you might want to add proper error handling here
            Projects = new List<Project>();
            Clients = new List<Client>();
            Vulns = new List<CORE.Entities.Vuln>();
            Tasks = new List<CORE.Entities.Task>();
        }
        finally
        {
            loading = false;
            StateHasChanged();
        }
    }


    // Project Chart Methods
    private double[] GetProjectChartData()
    {
        var grouped = FilteredProjects.GroupBy(p => p.Status).ToList();
        return grouped.Select(g => (double)g.Count()).ToArray();
    }

    private string[] GetProjectChartLabels()
    {
        var grouped = FilteredProjects.GroupBy(p => p.Status).ToList();
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
            var count = FilteredVulns.Count(v => v.Risk == riskOrder[i]);
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
        return statusOrder.Select(status => (double)FilteredTasks.Count(t => t.Status == status)).ToArray();
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

    // Filter Methods
    private List<Project> ApplyProjectFilters()
    {
        var filtered = Projects.AsQueryable();

        // Time range filter
        if (selectedTimeRange != "all")
        {
            var cutoffDate = GetCutoffDate(selectedTimeRange);
            filtered = filtered.Where(p => p.StartDate >= cutoffDate);
        }

        // Project status filter
        if (selectedProjectStatus != "all")
        {
            if (Enum.TryParse<ProjectStatus>(selectedProjectStatus, out var status))
            {
                filtered = filtered.Where(p => p.Status == status);
            }
        }

        // Active projects only
        if (showOnlyActiveProjects)
        {
            filtered = filtered.Where(p => p.Status == ProjectStatus.Active);
        }

        return filtered.ToList();
    }

    private List<CORE.Entities.Vuln> ApplyVulnFilters()
    {
        var filtered = Vulns.AsQueryable();

        // Time range filter
        if (selectedTimeRange != "all")
        {
            var cutoffDate = GetCutoffDate(selectedTimeRange);
            filtered = filtered.Where(v => v.CreatedDate >= cutoffDate);
        }

        // Vulnerability risk filter
        if (selectedVulnRisk != "all")
        {
            if (Enum.TryParse<VulnRisk>(selectedVulnRisk, out var risk))
            {
                filtered = filtered.Where(v => v.Risk == risk);
            }
        }

        return filtered.ToList();
    }

    private List<CORE.Entities.Task> ApplyTaskFilters()
    {
        var filtered = Tasks.AsQueryable();

        // Time range filter
        if (selectedTimeRange != "all")
        {
            var cutoffDate = GetCutoffDate(selectedTimeRange);
            filtered = filtered.Where(t => t.StartDate >= cutoffDate);
        }

        // Task status filter
        if (selectedTaskStatus != "all")
        {
            if (Enum.TryParse<TaskStatus>(selectedTaskStatus, out var status))
            {
                filtered = filtered.Where(t => t.Status == status);
            }
        }

        // My tasks only filter
        if (showOnlyMyTasks && User != null)
        {
            filtered = filtered.Where(t => t.AsignedUserId == User.Id);
        }

        return filtered.ToList();
    }

    private DateTime GetCutoffDate(string timeRange)
    {
        return timeRange switch
        {
            "7d" => DateTime.Now.AddDays(-7),
            "30d" => DateTime.Now.AddDays(-30),
            "90d" => DateTime.Now.AddDays(-90),
            "1y" => DateTime.Now.AddYears(-1),
            _ => DateTime.MinValue
        };
    }

    // Event Handlers
    private async Task OnFiltersChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task RefreshDashboard()
    {
        await LoadDashboardData();
    }

    private async Task ResetFilters()
    {
        selectedTimeRange = "all";
        selectedProjectStatus = "all";
        selectedVulnRisk = "all";
        selectedTaskStatus = "all";
        showOnlyActiveProjects = false;
        showOnlyMyTasks = false;
        await InvokeAsync(StateHasChanged);
    }
    
}