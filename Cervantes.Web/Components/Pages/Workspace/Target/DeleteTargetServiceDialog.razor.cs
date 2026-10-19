using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.AspNetCore.Mvc;

namespace Cervantes.Web.Components.Pages.Workspace.Target;

public partial class DeleteTargetServiceDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public CORE.Entities.TargetServices service { get; set; }
    [Inject] private TargetController _TargetController { get; set; }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _TargetController.DeleteService(service.Id);
            if (response is NoContentResult)
            {
                Snackbar.Add(@localizer["deletedService"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["deletedServiceError"], Severity.Error);
            }
            
        }
    }
}