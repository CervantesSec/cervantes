using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;

namespace Cervantes.Web.Components.Pages.Admin.Reports;

public partial class ReportComponents: ComponentBase
{
    private List<CORE.Entities.ReportComponents> model = new List<CORE.Entities.ReportComponents>();
    private List<CORE.Entities.ReportComponents> seleComponents = new List<CORE.Entities.ReportComponents>();

    private List<BreadcrumbItem> _items;
    private string searchString = "";
    [Inject] ReportController reportController { get; set; } 
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };

    const string headerIcon =
        @"<svg version=""1.1"" id=""Capa_1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" x=""0px"" y=""0px""
	 width=""470.586px"" height=""470.586px"" viewBox=""0 0 470.586 470.586"" style=""enable-background:new 0 0 470.586 470.586;""
	 xml:space=""preserve"">
<g>
	<path d=""M327.081,0H90.234c-15.9,0-28.854,12.959-28.854,28.859v412.863c0,15.924,12.953,28.863,28.854,28.863H380.35
		c15.917,0,28.855-12.939,28.855-28.863V89.234L327.081,0z M333.891,43.184l35.996,39.121h-35.996V43.184z M384.972,441.723
		c0,2.542-2.081,4.629-4.634,4.629H90.234c-2.551,0-4.62-2.087-4.62-4.629V28.859c0-2.548,2.069-4.613,4.62-4.613h219.41v70.181
		c0,6.682,5.444,12.099,12.129,12.099h63.198V441.723z M292.415,111.713H111.593V63.292h180.898c0,0.267-0.076,0.485-0.076,0.757
		V111.713z""/>
</g>
</svg>";
    const string footerIcon = @"<svg fill=""#000000"" version=""1.1"" id=""Capa_1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" viewBox=""0 0 470.586 470.586"" xml:space=""preserve""><g id=""SVGRepo_bgCarrier"" stroke-width=""0""></g><g id=""SVGRepo_tracerCarrier"" stroke-linecap=""round"" stroke-linejoin=""round""></g><g id=""SVGRepo_iconCarrier""> <g> <path d=""M327.081,0H90.234c-15.9,0-28.853,12.959-28.853,28.859v412.863c0,15.924,12.953,28.863,28.853,28.863H380.35 c15.917,0,28.855-12.939,28.855-28.863V89.234L327.081,0z M333.891,43.184l35.996,39.121h-35.996V43.184z M384.972,441.723 c0,2.542-2.081,4.629-4.635,4.629H90.234c-2.547,0-4.619-2.087-4.619-4.629V28.859c0-2.548,2.072-4.613,4.619-4.613h219.411v70.181 c0,6.682,5.443,12.099,12.129,12.099h63.198V441.723z M111.593,359.871h236.052v48.421H111.593V359.871z""></path> </g> </g></svg>";
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
    protected override async Task OnInitializedAsync()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem("Admin", href: null,icon: Icons.Material.Filled.AdminPanelSettings),
            new BreadcrumbItem(@localizer["reports"], href: "/",icon: Icons.Custom.FileFormats.FilePdf),
            new BreadcrumbItem(@localizer["components"], href: null, disabled: true,icon: Icons.Material.Filled.SpaceDashboard)
        };
        await Update();
    }
    
    protected async Task Update()
    {
        model = reportController.Components().ToList();
    }
    
    private async Task OpenDialogCreate(DialogOptionsEx options)
    {
        IMudExDialogReference<CreateReportComponentDialog>? dlgReference = await DialogEx.ShowEx<CreateReportComponentDialog>("Simple Dialog", options);
        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    private Func<CORE.Entities.ReportComponents, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Language.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.ComponentType.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };
    
    async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.ReportComponents> args)
    {
        var parameters = new DialogParameters { ["component"]=args.Item };
        IMudExDialogReference<ReportComponentsDialog>? dlgReference = await DialogEx.ShowEx<ReportComponentsDialog>("Simple Dialog", parameters, maxWidthEx);
        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["components"]=seleComponents };

                var dialog =  Dialog.Show<DeleteReportComponentBulkDialog>("Edit", parameters,mediumWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<CORE.Entities.ReportComponents> items)
    {
        
        seleComponents = items.ToList();
    }
}