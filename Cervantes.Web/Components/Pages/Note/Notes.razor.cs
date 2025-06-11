using System.Net.Http.Json;
using System.Security.Claims;
using Cervantes.CORE.ViewModels;
using Cervantes.IFR.Export;
using Cervantes.Web.Components.Pages.Clients;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;

namespace Cervantes.Web.Components.Pages.Note;

public partial class Notes: ComponentBase
{
    [Inject ]private IExportToCsv ExportToCsv { get; set; }
    private string searchString = "";
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    [Inject] private NoteController _NoteController { get; set; }
    private List<CORE.Entities.Note> model = new List<CORE.Entities.Note>();
    private List<CORE.Entities.Note> seleNotes = new List<CORE.Entities.Note>();
    private List<BreadcrumbItem> _items;
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
            new BreadcrumbItem(@localizer["notes"], href: null, disabled: true,icon: Icons.Material.Filled.Notes)
        };
        await Update();
        
    }
    
    protected async Task Update()
    {
        model = _NoteController.GetByUserId().ToList();
    }
    
    private async Task OpenDialogCreate(DialogOptionsEx options)
    {
        IMudExDialogReference<CreateNoteDialog>? dlgReference = await Dialog.ShowEx<CreateNoteDialog>("Simple Dialog", options);
        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    private Func<CORE.Entities.Note, bool> _quickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (x.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.CreatedDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };

    async Task RowClicked(DataGridRowClickEventArgs<CORE.Entities.Note> args)
    {
        var parameters = new DialogParameters { ["note"]=args.Item };
        IMudExDialogReference<NoteDialog>? dlgReference = await Dialog.ShowEx<NoteDialog>("Simple Dialog", parameters, maxWidthEx);

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
                /*var records = model.Select(e => new IFR.Export.ClientExport
                {
                    Name = e.Name,
                    Description = e.Description,
                    CreatedDate = e.CreatedDate,
                    ContactName = e.ContactName,
                    ContactPhone = e.ContactPhone,
                    ContactEmail = e.ContactEmail,
                    Url = e.Url
                }).ToList();
                var file = ExportToCsv.ExportClients(records);
                await JS.InvokeVoidAsync("downloadFile", file);
                Snackbar.Add(@localizer["exportSuccessfull"], Severity.Success);
                ExportToCsv.DeleteFile(file);*/
                break;
        }
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["notes"]=seleNotes };

                var dialog =  await Dialog.ShowEx<DeleteNoteBulkDialog>("Edit", parameters,middleWidthEx);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<CORE.Entities.Note> items)
    {
        
        seleNotes = items.ToList();
    }
 
}