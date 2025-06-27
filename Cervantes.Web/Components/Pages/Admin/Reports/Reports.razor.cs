using System.Net.Http.Json;
using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Admin.Reports;

public partial class Reports: ComponentBase
{
    private List<ReportTemplate> model = new List<ReportTemplate>();
    private List<ReportTemplate> seleReports = new List<ReportTemplate>();

    private List<BreadcrumbItem> _items;
    private string searchString = "";
    [Inject] ReportController reportController { get; set; } 
    DialogOptions mediumWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };

    DialogOptionsEx maxWidthEx = new DialogOptionsEx() 
    {
        MaximizeButton = true,
        CloseButton = true,
        FullHeight = true,
        CloseOnEscapeKey = true,
        MaxWidth = MaxWidth.Medium,
        MaxHeight = MaxHeight.False,
        FullWidth = true,
        DragMode = MudDialogDragMode.Simple,
        Animations = new[] { AnimationType.SlideIn },
        Position = DialogPosition.CenterRight,
        DisableSizeMarginY = true,
        DisablePositionMargin = true,
        BackdropClick = false,
        Resizeable = true,
    };
    DialogOptionsEx middleWidthEx = new DialogOptionsEx() 
    {
        MaximizeButton = true,
        CloseButton = true,
        FullHeight = false,
        CloseOnEscapeKey = true,
        MaxWidth = MaxWidth.Medium,
        MaxHeight = MaxHeight.False,
        FullWidth = true,
        DragMode = MudDialogDragMode.Simple,
        Animations = new[] { AnimationType.SlideIn },
        Position = DialogPosition.Center,
        DisableSizeMarginY = true,
        DisablePositionMargin = true,
        BackdropClick = false,
        Resizeable = true,
    };
    private ClaimsPrincipal userAth;
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
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
    private async Task OpenDialogCreate(DialogOptionsEx options)
    {
        IMudExDialogReference<CreateReport>? dlgReference = await Dialog.ShowExAsync<CreateReport>("Simple Dialog", options);
        // wait modal to close
        var result = await dlgReference.Result;
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

        var dialog =  await Dialog.ShowEx<DeleteReportDialog>("Delete", parameters,options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            //model.Remove(report);
            StateHasChanged();
        }
    }
    
    
    async Task RowClicked(DataGridRowClickEventArgs<ReportTemplate> args)
    {
        var parameters = new DialogParameters { ["report"]=args.Item };
        IMudExDialogReference<ReportDialog>? dlgReference = await Dialog.ShowExAsync<ReportDialog>("Simple Dialog", parameters, maxWidthEx);
        var result = await dlgReference.Result;

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
                IMudExDialogReference<DeleteReportBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteReportBulkDialog>("Simple Dialog", parameters, middleWidthEx);

                var result = await dlgReference.Result;

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