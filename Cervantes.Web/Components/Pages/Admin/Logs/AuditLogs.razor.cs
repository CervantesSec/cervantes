using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.IFR.Export;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Logs;

public partial class AuditLogs : ComponentBase, IDisposable
{
    private List<BreadcrumbItem> _items;
    private string searchString = "";
    [Inject] LogController _logController { get; set; }
    [Inject] private IExportToCsv ExportToCsv { get; set; }
    private ClaimsPrincipal userAth;
    [Inject] private AuditController _AuditController { get; set; }
    private bool isLoading = false;
    private int currentPage = 0;
    private int pageSize = 50;
    private Timer? searchTimer;
    private MudDataGrid<Audit>? dataGrid;

    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Admin", href: null,icon: Icons.Material.Filled.AdminPanelSettings),
            new BreadcrumbItem("Logs", href: null, disabled: true,icon: Icons.Material.Filled.ViewCompact)
        };
        
        // Data will be loaded via ServerReload method
    }
    
    private async Task OnSearchChanged(string value)
    {
        searchString = value;
        currentPage = 0;
        
        searchTimer?.Dispose();
        searchTimer = new Timer(async _ => await InvokeAsync(async () => 
        {
            if (dataGrid != null)
                await dataGrid.ReloadServerData();
        }), null, TimeSpan.FromMilliseconds(500), Timeout.InfiniteTimeSpan);
    }
    
    private async Task<GridData<Audit>> ServerReload(GridState<Audit> state)
    {
        try
        {
            currentPage = state.Page;
            pageSize = state.PageSize;
            
            var query = _AuditController.GetPaged(currentPage, pageSize, string.IsNullOrEmpty(searchString) ? null : searchString);
            var items = query.ToList();
            
            var count = _AuditController.GetCount(string.IsNullOrEmpty(searchString) ? null : searchString);
            
            return new GridData<Audit>()
            {
                Items = items,
                TotalItems = count
            };
        }
        catch (Exception)
        {
            return new GridData<Audit>()
            {
                Items = new List<Audit>(),
                TotalItems = 0
            };
        }
    }
    
    private string[] ParseJsonArray(string? jsonString)
    {
        if (string.IsNullOrEmpty(jsonString)) return Array.Empty<string>();
        
        try
        {
            return jsonString.Trim('[', ']', '{', '}')
                .Split(',')
                .Select(s => s.Replace('"', ' ').Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }
    
    public void Dispose()
    {
        searchTimer?.Dispose();
    }
}