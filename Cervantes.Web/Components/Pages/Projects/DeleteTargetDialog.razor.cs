using System.Net;
using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Projects;

public partial class DeleteTargetDialog: ComponentBase
{
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;
    
    [Parameter] public Target target { get; set; } = new Target();

    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            var response = await Http.PostAsync("api/Target/Delete/"+target.Id, null);
            if (response.IsSuccessStatusCode)
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