using System.Security.Claims;
using Cervantes.CORE.Entities;
using Cervantes.IFR.Export;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Clients;

public partial class Clients: ComponentBase
{
    private List<CORE.Entities.Client> model = new List<CORE.Entities.Client>();
    private List<CORE.Entities.Client> seleClients = new List<CORE.Entities.Client>();

    private List<BreadcrumbItem> _items;

    private string searchString = "";
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
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
    [Parameter] public Guid clientId { get; set; }

    [Inject] private ClientsController _clientsController { get; set; }
   // [Inject] private IExportToCsv ExportToCsv { get; set; }
   ClaimsPrincipal user;
    protected override async Task OnInitializedAsync()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(@localizer["clients"], href: null, disabled: true,icon: Icons.Material.Filled.BusinessCenter)
        };
        user = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        await Update();
    }
    
    protected async Task Update()
    {
        model = _clientsController.Get().ToList();
    }


    private Func<Client, bool> _quickFilter => x =>
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (x.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.Url.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.ContactName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.ContactEmail.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (x.ContactPhone.Contains(searchString))
            return true;
        return false;
    };
    private async Task OpenDialogCreate()
    {
        IMudExDialogReference<CreateClientDialog>? dlgReference = await Dialog.ShowExAsync<CreateClientDialog>("Simple Dialog", maxWidthEx);

        // wait modal to close
        var result = await dlgReference.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    
    async Task RowClicked(DataGridRowClickEventArgs<Client> args)
    {
        var parameters = new DialogParameters { ["client"]=args.Item };
        IMudExDialogReference<ClientDialog>? dlgReference = await Dialog.ShowExAsync<ClientDialog>("Simple Dialog",parameters, maxWidthEx);

        var result = await dlgReference.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }
    
    
    /*private async Task Export(int id)
    {
        switch (id)
        {
            case 0:
                var records = model.Select(e => new IFR.Export.ClientExport
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
                ExportToCsv.DeleteFile(file);
                break;
        }
    }*/
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["clients"]=seleClients };
                IMudExDialogReference<DeleteClientBulkDialog>? dlgReference = await Dialog.ShowExAsync<DeleteClientBulkDialog>("Simple Dialog", parameters, middleWidthEx);

                var result = await dlgReference.Result;

                if (!result.Canceled)
                {
                    await Update();
                    StateHasChanged();
                }
                break;
        }
    }
    
    void SelectedItemsChanged(HashSet<Client> items)
    {
        
        seleClients = items.ToList();
    }
 
}