using Cervantes.CORE.Entities;
using Cervantes.IFR.Export;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Clients;

public partial class Clients: ComponentBase
{
    private List<CORE.Entities.Client> model = new List<CORE.Entities.Client>();
    private List<CORE.Entities.Client> seleClients = new List<CORE.Entities.Client>();

    private List<BreadcrumbItem> _items;

    private string searchString = "";
    DialogOptions maxWidth = new DialogOptions() { MaxWidth = MaxWidth.ExtraLarge, FullWidth = true };
    [Parameter] public Guid clientId { get; set; }

    [Inject] private ClientsController _clientsController { get; set; }
    [Inject ]private IExportToCsv ExportToCsv { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(@localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(@localizer["clients"], href: null, disabled: true,icon: Icons.Material.Filled.BusinessCenter)
        };
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
    private async Task OpenDialogCreate(DialogOptions options)
    {
        var dialog = Dialog.Show<CreateClientDialog>("Custom Options Dialog", options);
        // wait modal to close
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
        
    }
    
    
    async Task RowClicked(DataGridRowClickEventArgs<Client> args)
    {
        var parameters = new DialogParameters { ["client"]=args.Item };

        var dialog =  Dialog.Show<ClientDialog>("Edit", parameters, maxWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await Update();
            StateHasChanged();
        }
    }
    
    async Task OpenClientDialog(Client args)
    {
        var parameters = new DialogParameters { ["client"]=args };

        var dialog =  Dialog.Show<ClientDialog>("Edit", parameters, maxWidth);
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
    }
    
    private async Task BtnActions(int id)
    {
        switch (id)
        {
            case 0:
                var parameters = new DialogParameters { ["clients"]=seleClients };

                var dialog =  Dialog.Show<DeleteClientBulkDialog>("Edit", parameters,maxWidth);
                var result = await dialog.Result;

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