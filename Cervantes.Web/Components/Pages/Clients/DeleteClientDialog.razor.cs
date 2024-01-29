using System.Net.Http.Json;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Clients;

public partial class DeleteClientDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;

    CreateClientDialog.ClientModelFluentValidator clientValidator = new CreateClientDialog.ClientModelFluentValidator();

    [Parameter] public CORE.Entities.Client client { get; set; } = new CORE.Entities.Client();
    [Inject] private ClientsController _clientsController { get; set; }


    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _clientsController.Delete(client.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(@localizer["clientDeleted"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["clientDeletedError"], Severity.Error);
            }
            
        }
    }
}