using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Workspace.Vault;

public partial class DeleteVaultBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<CORE.Entities.Vault> vaults { get; set; }
    [Parameter] public Guid project { get; set; }
    [Inject] private VaultController VaultController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var vault in vaults)
            {
                var response = await VaultController.Delete(vault.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
                {
                    Snackbar.Add(@localizer["vaultDeleted"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["vaultDeletedError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));

            
        }
    }
}