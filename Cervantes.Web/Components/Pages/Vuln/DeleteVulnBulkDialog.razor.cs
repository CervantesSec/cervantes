using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class DeleteVulnBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<CORE.Entities.Vuln> vulns { get; set; } = new List<CORE.Entities.Vuln>();
    [Inject] private VulnController _vulnController { get; set; }

    private async Task Submit()
    {
        await form.Validate();
        if (form.IsValid)
        {
        var anyDeleted = false;
        foreach (var vuln in vulns.ToList())
        {
            var response = await _vulnController.DeleteVuln(vuln.Id);
            if (response is NoContentResult)
            {
                Snackbar.Add(@localizer["deletedVuln"], Severity.Success);
                anyDeleted = true;
            }
            else if (response is ObjectResult { StatusCode: 403 })
            {
                Snackbar.Add(@localizer["noInProject"], Severity.Warning);
            }
            else
            {
                Snackbar.Add(@localizer["deletedVulnError"], Severity.Error);
            }
        }

        // Close once every delete has finished, so the caller reloads its grid with the final data.
        if (anyDeleted)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
        }
    }
}