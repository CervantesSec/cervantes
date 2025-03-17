using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cervantes.Web.Components.Pages.Tasks;

public partial class DeleteTaskBulkDialog: ComponentBase
{
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; }
 
    void Cancel() => MudDialog.Cancel();
     
    [Inject] ISnackbar Snackbar { get; set; }

    MudForm form;


    [Parameter] public List<CORE.Entities.Task> tasks { get; set; }
    [Inject] TaskController _taskController { get; set; }

    private async System.Threading.Tasks.Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            foreach (var task in tasks)
            {
                var response = await _taskController.Delete(task.Id);
                if (response.ToString() == "Microsoft.AspNetCore.Mvc.OkResult")
                {
                    Snackbar.Add(@localizer["taskDeleted"], Severity.Success);
                
                }
                else
                {
                    Snackbar.Add(@localizer["taskDeletedError"], Severity.Error);
                }
            }
            
            MudDialog.Close(DialogResult.Ok(true));
        }
    }
}