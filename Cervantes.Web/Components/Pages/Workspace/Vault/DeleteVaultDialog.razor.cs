using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Workspace.Vault;

public partial class DeleteVaultDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }
    [Inject] VaultController VaultController { get; set; }

    MudForm form;
    
    [Parameter] public CORE.Entities.Vault vault { get; set; }
    [Parameter] public Guid project { get; set; }


    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            
                var response = await VaultController.Delete(vault.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
                {
                    Snackbar.Add(@localizer["vaultDeleted"], Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));

                }
                else
                {
                    Snackbar.Add(@localizer["vaultDeletedError"], Severity.Error);
                }
            
        }
    }
}