using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.IFR.Export;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Logs;

public partial class AuditLogs : ComponentBase
{
    private List<BreadcrumbItem> _items;
    private string searchString = "";
    [Inject] LogController _logController { get; set; }
    [Inject ]private IExportToCsv ExportToCsv { get; set; }
    private ClaimsPrincipal userAth;
    [Inject] private AuditController _AuditController { get; set; }
    private List<CORE.Entities.Audit> model = new List<CORE.Entities.Audit>();

    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        model = new List<Audit>();
        model = _AuditController.Get().ToList();
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Admin", href: null,icon: Icons.Material.Filled.AdminPanelSettings),
            new BreadcrumbItem("Logs", href: null, disabled: true,icon: Icons.Material.Filled.ViewCompact)
        };

    }
    
    private Func<Audit, bool> _quickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (x.Type.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.DateTime.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.UserId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.TableName.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.AffectedColumns.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.Id.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.OldValues.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.NewValues.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.PrimaryKey.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
}