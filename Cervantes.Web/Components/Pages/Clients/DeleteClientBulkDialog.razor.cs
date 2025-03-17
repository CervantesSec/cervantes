using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Clients;

public partial class DeleteClientBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;

    CreateClientDialog.ClientModelFluentValidator clientValidator = new CreateClientDialog.ClientModelFluentValidator();

    [Parameter] public List<CORE.Entities.Client> clients { get; set; }
    [Inject] private ClientsController _clientsController { get; set; }
    
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var client in clients)
            {
                var response = await _clientsController.Delete(client.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {
                    Snackbar.Add(@localizer["clientDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["clientDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));

            
            
        }
    }
}