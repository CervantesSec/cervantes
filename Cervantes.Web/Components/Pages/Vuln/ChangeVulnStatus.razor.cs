using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class ChangeVulnStatus : ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public VulnStatusUpdate vulns { get; set; }
    [Inject] private VulnController _VulnController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            
                var response = await _VulnController.VulnStatusUpdate(vulns);
                if (response is NoContentResult)            
                {
                    Snackbar.Add(@localizer["vulnsUpdated"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["vulnsUpdatedError"], Severity.Error);
                }
            
            
            MudDialog.Close(DialogResult.Ok(true));
            
        }
    }
}