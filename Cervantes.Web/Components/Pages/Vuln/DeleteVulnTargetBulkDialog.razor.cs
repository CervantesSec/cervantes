using Cervantes.CORE.Entities;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Vuln;

public partial class DeleteVulnTargetBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public List<VulnTargets> targets { get; set; }
    [Inject] private VulnController _VulnController { get; set; }

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var target in targets)
            {
                var response = await _VulnController.DeleteVulnTarget(target.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")  
                {
                    Snackbar.Add(@localizer["deletedTarget"], Severity.Success);
                }
                else
                {
                    Snackbar.Add(@localizer["deletedTargetError"], Severity.Error);
                }
            }
            MudDialog.Close(DialogResult.Ok(true));

            
        }
    }
}