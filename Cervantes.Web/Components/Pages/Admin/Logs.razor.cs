using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.Export;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin;

public partial class Logs: ComponentBase
{
    private List<BreadcrumbItem> _items;
    private List<Log> model = new List<Log>();
    private string searchString = "";
    [Inject] LogController _logController { get; set; }
    [Inject ]private IExportToCsv ExportToCsv { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Admin", href: null,icon: Icons.Material.Filled.AdminPanelSettings),
            new BreadcrumbItem("Logs", href: null, disabled: true,icon: Icons.Material.Filled.ViewCompact)
        };
        await Update();

    }
    protected async Task Update()
    {

        model.RemoveAll(item => true);
        model = _logController.GetAll().ToList();
    }
    async Task RowClicked(DataGridRowClickEventArgs<Project> args)
    {
        /*var parameters = new DialogParameters { ["project"]=args.Item };

        var dialog =  DialogService.Show<ProjectDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }*/
    }
    
    private Func<Log, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Exception.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Message.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Level.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Logger.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Url.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.CreatedOn.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.StackTrace.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };
    
    private async Task Export(int id)
    {
        switch (id)
        {
            case 0:
                var file = ExportToCsv.ExportLogs(model);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);
                break;
        }
    }
    
    private async Task DeleteAll()
    {
        var response = await _logController.DeleteAll();
        if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
        {
            Snackbar.Add(@localizer["logsDeleted"], Severity.Success);
            await Update();
        }
        else
        {
            Snackbar.Add(@localizer["logsDeletedError"], Severity.Error);
        }
    }
    
    private Func<Log, int, string> _rowStyleFunc => (x, i) =>
    {
        if (x.Level.Contains("Error"))
            return "background-color:#ff3f5f"; // Red for Error
        if (x.Level.Contains("Info"))
            return "background-color:#4a86ff"; // Blue for Info
        if (x.Level.Contains("Warning"))
            return "background-color:#ffb545";
        if (x.Level.Contains("Trace"))
            return "background-color:#7e6fff";
        return "";
    };
}