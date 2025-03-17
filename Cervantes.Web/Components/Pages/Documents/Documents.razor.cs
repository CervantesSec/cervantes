using System.Net.Http.Json;
using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Components.Pages.Projects;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Documents;

public partial class Documents: ComponentBase
{
        private List<Document> model = new List<Document>();
        private List<CORE.Entities.Document> seleDocs = new List<CORE.Entities.Document>();

    private List<BreadcrumbItem> _items = new List<BreadcrumbItem>();
    private string searchString = "";
    [Inject] private DocumentController _documentController { get; set; }
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    private ClaimsPrincipal userAth;
    protected override async Task OnInitializedAsync()
    {
        userAth = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;

        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(@localizer["documents"], href: null, disabled: true,icon: Icons.Material.Filled.InsertDriveFile)
        };
        await Update();
    }
    
    private async Task Update()
    {
        model = _documentController.Get().ToList();
    }
    
    private Func<CORE.Entities.Document, bool> _quickFilter => element =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.User.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Description.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.CreatedDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };

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
    
    async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.Document> args)
    {
        var parameters = new DialogParameters { ["document"]=args.Item};
        IMudExDialogReference<DocumentDialog>? dlgReference = await Dialog.ShowEx<DocumentDialog>("Simple Dialog", parameters,maxWidthEx);

        var result = await dlgReference.Result;

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
                /*var records = model.Select(e => new IFR.Export.ProjectExport()
                {
                    Name = e.Name,
                    Description = e.Description,
                    Client = e.Client.Name,
                    CreatedUser = e.User.FullName,
                    StartDate = e.StartDate.ToShortDateString(),
                    EndDate = e.EndDate.ToShortDateString(),
                    Template = e.Template,
                    Status = e.Status.ToString(),
                    ProjectType = e.ProjectType.ToString(),
                    Language = e.Language.ToString(),
                    Score = e.Score.ToString(),
                    FindingsId = e.FindingsId.ToString(),
                    ExecutiveSummary = e.ExecutiveSummary


                }).ToList();
                var file = ExportToCsv.ExportProjects(records);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);*/
                break;
        }
    }
    
    private async Task OpenDialogCreate(DialogOptionsEx options)
    {
        IMudExDialogReference<DocumentCreateDialog>? dlgReference = await Dialog.ShowEx<DocumentCreateDialog>("Simple Dialog", options);
        // wait modal to close
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
                var parameters = new DialogParameters { ["documents"]=seleDocs };

                var dialog =  await Dialog.ShowEx<DocumentDeleteBulkDialog>("Edit", parameters,mediumWidth);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<Document> items)
    {
        
        seleDocs = items.ToList();
    }
  
}