using System.Net;
using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Workspace.Target;

public partial class DeleteTargetDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public CORE.Entities.Target target { get; set; }
[Inject] private TargetController _TargetController { get; set; }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await _TargetController.Delete(target.Id);
            if (response.ToString() == "Microsoft.AspNetCore.Mvc.NoContentResult")
            {
                Snackbar.Add(@localizer["deletedTarget"], Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                Snackbar.Add(@localizer["deletedTargetError"], Severity.Error);
            }
            
        }
    }
}