using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class DeleteVulnDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public CORE.Entities.Vuln vuln { get; set; } = new CORE.Entities.Vuln();
    [Inject] private VulnController _vulnController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _vulnController.DeleteVuln(vuln.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
            {
                Snackbar.Add(@localizer["deletedVuln"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else if (response is BadRequestObjectResult badRequestResult)
            {
                var message = badRequestResult.Value;
                if (message.ToString() == "NotAllowed")
                {
                    Snackbar.Add(@localizer["noInProject"], Severity.Warning);
                }
			        
            }
            else
            {
                Snackbar.Add(@localizer["deletedVulnError"], Severity.Error);
            }
            
        }
    }

}