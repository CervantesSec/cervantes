using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Reports;

public partial class Reports: ComponentBase
{
    private List<ReportTemplate> model = new List<ReportTemplate>();
    private List<ReportTemplate> seleReports = new List<ReportTemplate>();

    private List<BreadcrumbItem> _items;
    private string searchString = "";
    [Inject] ReportController reportController { get; set; } 

    protected override async Task OnInitializedAsync()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Admin", href: null,icon: Icons.Material.Filled.AdminPanelSettings),
            new BreadcrumbItem(@localizer["reportTemplates"], href: null, disabled: true,icon: Icons.Material.Filled.FileCopy)
        };
        await Update();
    }
    
    protected async Task Update()
    {
        model = reportController.Templates().ToList();
    }

    
    
    private Func<ReportTemplate, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Language.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };
    
    DialogOptions fullScreen = new DialogOptions() { FullScreen = true, CloseButton = true };
    private async Task OpenDialogCreate(DialogOptions options)
    {
        var dialog = Dialog.Show<CreateReport>("Custom Options Dialog", options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };

    async Task DeleteDialog(ReportTemplateViewModel report,DialogOptions options)
    {
        var parameters = new DialogParameters { ["report"]=report };

        var dialog =  Dialog.Show<DeleteReportDialog>("Delete", parameters,options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            //model.Remove(report);
            StateHasChanged();
        }
    }
    
    async Task EditDialog(ReportTemplateViewModel report,DialogOptions options)
    {
        var parameters = new DialogParameters { ["report"]=report };

        var dialog =  Dialog.Show<EditDialog>("Edit", parameters,options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }

    
    private async Task Export(int id)
    {
        switch (id)
        {
            case 0:
                /*var file = ExportToCsv.ExportLogs(model);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);*/
                break;
        }
    }
    
    async Task RowClicked(DataGridRowClickEventArgs<ReportTemplate> args)
    {
        var parameters = new DialogParameters { ["report"]=args.Item };

        var dialog =  Dialog.Show<ReportDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            model = reportController.Templates().ToList();
            StateHasChanged();
        }
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["reports"]=seleReports };

                var dialog =  Dialog.Show<DeleteReportBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<ReportTemplate> items)
    {
        
        seleReports = items.ToList();
    }
    
}